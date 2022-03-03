using System.Collections.Generic;

namespace Action.Compiler
{
    public class CompilationResult
    {
        private CompilationResult(IEnumerable<DiagnosticResult> diagnostics)
        {
            Diagnostics = diagnostics;
        }
        public static CompilationResult Failure(IEnumerable<DiagnosticResult> diagnostics)
        {
            return new CompilationResult(diagnostics);
        }

        public bool Success => false;
        public IEnumerable<DiagnosticResult> Diagnostics { get; }
    }

}