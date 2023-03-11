using Microsoft.CodeAnalysis;

namespace RoslynCSharp.Compiler
{
    public sealed class CompilationError
    {
        // Private
        public Diagnostic diagnostic = null;
        public string code = null;
        public string message = null;
        public Location location = Location.None;
        public DiagnosticSeverity severity = DiagnosticSeverity.Info;
        public bool isWarningAsError = false;
        public bool isSupressed = false;

        // Properties
        public string Code
        {
            get { return code; }
        }

        public string Message
        {
            get { return message; }
        }

        public string SourceFile
        {
            get { return location.SourceTree.FilePath; }
        }

        public int SourceLine
        {
            get { return location.GetLineSpan().StartLinePosition.Line; }
        }

        public int SourceColumn
        {
            get { return location.GetLineSpan().StartLinePosition.Character; }
        }

        public bool IsInfo
        {
            get { return severity == DiagnosticSeverity.Info; }
        }

        public bool IsWarning
        {
            get { return severity == DiagnosticSeverity.Warning; }
        }

        public bool IsError
        {
            get { return severity == DiagnosticSeverity.Error; }
        }

        public bool IsWarningAsError
        {
            get { return isWarningAsError; }
        }

        public bool IsSuppressed
        {
            get { return isSupressed; }
        }

        // Internal
        internal CompilationError(Diagnostic diagnostic)
        {
            this.diagnostic = diagnostic;
            this.code = diagnostic.Id;
            this.message = diagnostic.GetMessage();
            this.location = diagnostic.Location;
            this.severity = diagnostic.Severity;
            this.isWarningAsError = diagnostic.IsWarningAsError;
            this.isSupressed = diagnostic.IsSuppressed;
        }

        // Methods
        public override string ToString()
        {
            return diagnostic.ToString();
        }
    }
}
