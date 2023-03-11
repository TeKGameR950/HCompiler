using System.Collections.Generic;

namespace HCompiler
{
    public class Rules
    {
        // Empty namespace = ""
        // Constructors = "..ctor"

        public static List<string> WhitelistedNamespaces = new List<string>()
        {
            "System",
            "System.Collections",
            "UnityEngine",
            "UltimateWater",
            "UnityEngine.UI",
            "UnityEngine.Events",
            "TMPro",
            "RaftModLoader.ClientPlugin",
            ""
        };

        public static List<string> BlacklistedMembers = new List<string>()
        {
            
        };

        public static List<string> BlacklistedTypes = new List<string>()
        {
            
        };
    }
}
