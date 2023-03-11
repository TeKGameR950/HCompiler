using System.Collections.Generic;
using System.Linq;

namespace HCompiler
{
    public class RaftModFixer
    {
        public static string FixFile(string file, bool secured)
        {
            string newFile = file;

            if (newFile.Contains("Semih_Network"))
                newFile = "using Semih_Network = Raft_Network;\n" + newFile;
            if (!secured && !file.Contains("using RaftModLoader;"))
                newFile = "using RaftModLoader;\n" + newFile;
            if (!secured && !file.Contains("using HMLLibrary;"))
                newFile = "using HMLLibrary;\n" + newFile;
            return newFile;
        }

        public static Dictionary<string, string> FixFiles(Dictionary<string, string> files, bool secured)
        {
            files.ToList().ForEach(x =>
            {
                files[x.Key] = FixFile(x.Value, secured);
            });
            return files;
        }
    }
}