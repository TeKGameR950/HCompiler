using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using RoslynCSharp;
using Trivial.CodeSecurity;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace HCompiler
{
    public class RoslynCSharpSettings
    {
        // Should loaded or compiled code be security checked.
        public bool securityCheckCode = true;

        // Should external pinvoke calls be allowed, It is highly recommended that this options remains disabled to prevent external unverified code from running.
        public bool allowPInvoke = false;

        // The restrictions that are used to security check code.
        public CodeSecurityRestrictions securityRestrictions = new CodeSecurityRestrictions();

        // The log level used to determine which types of messages get logged to the console.
        public LogDetail logDetail = LogDetail.None;

        // Should the compiler allow unsafe code to be compiled.
        public bool allowUnsafeCode = true;

        // Should the compiler optimize the output.
        public bool allowOptimizeCode = false;

        // Should the compiler use multiple threads to compile the code.
        public bool allowConcurrentCompile = true;

        // Idk what that shit is
        public bool deterministic = false;

        // Should the compiler generate the output in memory or write it to the file system.
        public bool generateInMemory = true;

        // Should the debug symbols pdb file be generated.
        public bool generateSymbols = false;

        // The current compiler warning level.
        public int warningLevel = 4;

        // The target C# language version that should be supported.
        public LanguageVersion languageVersion = LanguageVersion.Latest;

        // The target platform architecture.
        public Platform targetPlatform = Platform.AnyCpu;

        // A list of assembly references used by the compiler.
        public List<string> references = new List<string>();

        // A list of define symbols used by the compiler.
        public List<string> defineSymbols = new List<string>();

        // Remove that shit ? (seems to be unityeditor only)
        public bool allowHotReloading = true;
        public bool hotReloadCopySerializedFields = true;
        public bool hotReloadCopyNonSerializedFields = true;
        public bool hotReloadDestroyOriginalScript = true;
        public bool hotReloadDisableOriginalScript = true;
        public bool hotReloadSecurityCheckCode = false;
        public bool hotReloadUseCSharpProjectReferences = true;

        public List<string> blacklistedReferences = new List<string>();

        public RoslynCSharpSettings()
        {
            securityRestrictions = new CodeSecurityRestrictions();
            securityCheckCode = true;
            securityRestrictions.NamespaceReferences.DefaultBehaviour = CodeSecurityRestrictions.CodeSecurityBehaviour.Deny;
            Rules.WhitelistedNamespaces.ToList().ForEach(x =>
            {
                securityRestrictions.NamespaceReferences.AddEntryName(x, CodeSecurityRestrictions.CodeSecurityBehaviour.Allow);
            });

            securityRestrictions.MemberReferences.DefaultBehaviour = CodeSecurityRestrictions.CodeSecurityBehaviour.Allow;
            Rules.BlacklistedMembers.ToList().ForEach(x =>
            {
                securityRestrictions.MemberReferences.AddEntryName(x, CodeSecurityRestrictions.CodeSecurityBehaviour.Deny);
            });

            securityRestrictions.TypeReferences.DefaultBehaviour = CodeSecurityRestrictions.CodeSecurityBehaviour.Allow;
            Rules.BlacklistedTypes.ToList().ForEach(x =>
            {
                securityRestrictions.TypeReferences.AddEntryName(x, CodeSecurityRestrictions.CodeSecurityBehaviour.Deny);
            });

            
        }

        public string GetAppdataFolder()
        {
            return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HModLoader"), "binaries");
        }

        public enum LogDetail
        {
            // Dont log any messages.
            None,
            // Only log errors.
            Errors,
            // Only log warnings and errors.
            Warnings,
            // Log all types of messages.
            Info,
        }
    }
}