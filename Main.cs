using Microsoft.CodeAnalysis;
using RoslynCSharp;
using RoslynCSharp.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trivial.CodeSecurity;
using UnityEngine;

namespace HCompiler
{
    public class Main
    {
        public static ScriptDomain compilerDomain;
        //public static Component MainThreadDispatcher;
        public static string tempFolder = Path.GetTempPath();
        public static string hmlTempFolder = Path.Combine(tempFolder, "HCompiler");
        public static bool isRDS;

        public static async void Initialize()
        {
            Directory.CreateDirectory(hmlTempFolder);
            compilerDomain = ScriptDomain.CreateDomain("HCompilerDomain", true, true);
            await CompileCode("preload", new Dictionary<string, string>() { { "preload.cs", "public class test { }" } }, new List<byte[]>(), true);
            compilerDomain.Dispose();
        }

        public static string GetSecurityReport(CodeSecurityReport r)
        {
            string result = "";
            List<string> illegalAssemblies = r.IllegalAssemblyReferences.Select(x => x.ReferencedAssemebly.Name).ToList();
            List<string> illegalNamespaces = r.IllegalNamespaceReferences.Select(x => x.ReferencedNamespace).ToList();
            List<string> illegalTypes = r.IllegalTypeReferences.Select(x => x.ReferencedType.FullName).ToList();
            List<string> illegalMembers = r.IllegalMemberReferences.Select(x => x.ReferencedType.Name + "." + x.ReferencedMember.Name).ToList();
            List<string> illegalPInvoke = r.IllegalPInvokes.Select(x => x.PInvokeTargetMethod).ToList();

            if (illegalAssemblies.Count > 0)
                result += "Illegal Assemblies References (" + illegalAssemblies.Count + ") : (" + string.Join(",", illegalAssemblies.Take(5)).TrimEnd(',') + ")\n";
            if (illegalNamespaces.Count > 0)
                result += "Illegal Namespaces References (" + illegalNamespaces.Count + ") : (" + string.Join(",", illegalNamespaces.Take(5)).TrimEnd(',') + ")\n";
            if (illegalTypes.Count > 0)
                result += "Illegal Types References (" + illegalTypes.Count + ") : (" + string.Join(",", illegalTypes.Take(5)).TrimEnd(',') + ")\n";
            if (illegalMembers.Count > 0)
                result += "Illegal Members References (" + illegalMembers.Count + ") : (" + string.Join(",", illegalMembers.Take(5)).TrimEnd(',') + ")\n";
            if (illegalPInvoke.Count > 0)
                result += "Illegal PInvoke References (" + illegalPInvoke.Count + ") : (" + string.Join(",", illegalPInvoke.Take(5)).TrimEnd(',') + ")\n";

            return result;
        }

        public static async Task<CompilationResult> CompileCode(string jobName, Dictionary<string, string> files, List<byte[]> dlls, bool runSecurityVerifications = true)
        {
#if GAME_RAFT
            if (Application.productName == "Raft" && !isRDS)
                files = RaftModFixer.FixFiles(files, runSecurityVerifications);
#endif
            using (ScriptDomain domain = ScriptDomain.CreateDomain("CompilerDomain-" + DateTime.Now.Ticks, true, true, AppDomain.CurrentDomain))
            {
                List<IMetadataReferenceProvider> references = new List<IMetadataReferenceProvider>();
                if (!runSecurityVerifications)
                {
                    dlls.ForEach(dll =>
                    {
                        references.Add(AssemblyReference.FromImage(dll));
                    });
                }
                ScriptSecurityMode mode = runSecurityVerifications ? ScriptSecurityMode.EnsureSecurity : ScriptSecurityMode.EnsureLoad;
                if (mode == ScriptSecurityMode.EnsureLoad)
                    Debug.LogWarning("[HCompiler] Job \"" + jobName + "\" > This job will be ran without any security verification !");
                try
                {
                    List<string> filesPath = new List<string>();
                    foreach (var file in files)
                    {
                        string tempFilePath = Path.Combine(hmlTempFolder, DateTime.Now.Ticks + "-" + file.Key.Replace("\\", "_").Replace("/", "_"));
                        File.WriteAllText(tempFilePath, file.Value);
                        filesPath.Add(tempFilePath);
                    }
                    AsyncCompileOperation op = domain.CompileAndLoadFilesAsync(filesPath.ToArray(), mode, references.ToArray());
                    while (op.keepWaiting)
                        await Task.Delay(1);

                    foreach (var file in filesPath)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch { }
                    }
                    CompilationResult result = op.CompileDomain.CompileResult;

                    if (result == null)
                    {
                        Debug.LogError("[HCompiler] \"" + jobName + "\" > The compilation failed with no output !");
                        return new CompilationResult(false, new List<Diagnostic>());
                    }
                    if (result != null && result.Success == false)
                    {
                        if (op?.CompileDomain?.SecurityResult?.IsSecurityVerified == false)
                            Debug.LogError("[HCompiler] \"" + jobName + "\" > The job failed the security verification !\n" + GetSecurityReport(op.CompileDomain.SecurityResult));
                        else
                            Debug.LogError("[HCompiler] \"" + jobName + "\" > The compilation failed ! Errors : \n" + string.Join("\n", result.Errors.Select(x => ParseCompilationError(x)).Take(10).ToList()));
                        return new CompilationResult(false, new List<Diagnostic>());
                    }
                    if (result.Success && result.OutputAssembly != null)
                    {
                        if (mode == ScriptSecurityMode.EnsureLoad)
                            return result;
                        if (mode == ScriptSecurityMode.EnsureSecurity)
                        {
                            CodeSecurityReport sres = op?.CompileDomain?.SecurityResult;
                            if (sres != null)
                            {
                                if (sres.IsSecurityVerified)
                                    return result;
                                else
                                    Debug.LogError("[HCompiler] \"" + jobName + "\" > The job failed the security verification !\n" + GetSecurityReport(sres));
                            }
                        }
                    }
                    // i guess ?
                    return new CompilationResult(false, new List<Diagnostic>());
                }
                catch (Exception ex)
                {
                    Debug.Log("[HCompiler] \"" + jobName + "\" > A fatal error occured ! Error : " + ex.Message + "\n" + ex.StackTrace);
                    return new CompilationResult(false, new List<Diagnostic>());
                }
            }
        }

        public static string ParseCompilationError(CompilationError error)
        {
            string err = error.ToString();
            if (error?.location?.SourceTree?.FilePath != null)
            {
                string filePath = error.location.SourceTree.FilePath;
                string[] name = filePath.Split('-');
                name[0] = null;
                err = error.ToString().Replace(filePath, string.Join("", name));
            }
            return err;
        }
    }
}