using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Trivial.CodeSecurity;
using HCompiler;
using System.IO;
using static HCompiler.RoslynCSharpSettings;
using UnityEngine;

namespace RoslynCSharp
{
    public sealed class RoslynCSharp
    {
        public static RoslynCSharpSettings settings = new RoslynCSharpSettings();

        public static void Log(string format, params object[] args)
        {
            if (settings.logDetail >= LogDetail.Info)
            {
                if (args.Length == 0)
                {
                    Debug.Log(format);
                }
                else
                {
                    Debug.Log(string.Format(format, args));
                }
            }
        }

        public static void LogWarning(string format, params object[] args)
        {
            if (settings.logDetail >= LogDetail.Warnings)
            {
                if(args.Length == 0)
                {
                    Debug.LogWarning(format);
                }
                else
                {
                    Debug.LogWarning(string.Format(format, args));
                }
            }
        }

        public static void LogError(string format, params object[] args)
        {
            if (settings.logDetail >= LogDetail.Errors)
            {
                if(args.Length == 0)
                {
                    Debug.LogError(format);
                }
                else
                {
                    Debug.LogError(string.Format(format, args));
                }
            }
        }
    }
}
