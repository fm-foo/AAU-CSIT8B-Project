using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.Compiler
{
    public class CompilationResult
    {
        private CompilationResult(IEnumerable<DiagnosticResult> diagnostics)
        {
            Success = false;
            Images = Enumerable.Empty<ImageFile>();
            Diagnostics = diagnostics;
        }

        public CompilationResult(IEnumerable<ImageFile> images, IEnumerable<DiagnosticResult> diagnostics)
        {
            Success = true;
            Images = images;
            Diagnostics = diagnostics;
        }

        public static CompilationResult Failure(IEnumerable<DiagnosticResult> diagnostics)
        {
            return new CompilationResult(diagnostics);
        }

        public bool Success { get; }
        public IEnumerable<ImageFile> Images { get; }
        public IEnumerable<DiagnosticResult> Diagnostics { get; }

        internal static CompilationResult Succeed(IEnumerable<ImageFile> images, IEnumerable<DiagnosticResult> diagnostics)
        {
            return new CompilationResult(images, diagnostics);
        }
    }

}