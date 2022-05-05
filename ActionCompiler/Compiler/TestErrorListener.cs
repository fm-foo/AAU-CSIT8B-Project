using Antlr4.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActionCompiler.Parser;

namespace ActionCompiler.Compiler
{
    public class TestErrorListener : BaseErrorListener
    {
        private readonly List<DiagnosticResult> diagnostics;
        public TestErrorListener(List<DiagnosticResult> diags)
        {
            diagnostics = diags;
        }
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol,
            int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string error = e switch
            {
                InputMismatchException m => $"Unexpected input. Got '{offendingSymbol.Text}' ({(ActionToken)offendingSymbol.Type}) but expected {GetExpectedTokens(m)}.",
                _ => "Unexpected error",
            };
            string linestr = $"({line}, {charPositionInLine})";
            diagnostics.Add(new DiagnosticResult(Severity.Error, $"{linestr}: {error}"));
        }

        private static string GetExpectedTokens(InputMismatchException ex)
        {
            var expected = ex.GetExpectedTokens().ToList().Cast<ActionToken>();
            return string.Join(" or ", expected.Select(s => s.ToString()));
        }
    }
}