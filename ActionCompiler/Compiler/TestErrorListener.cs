using Antlr4.Runtime;
using System.Collections.Generic;
using System.IO;

namespace Action.Compiler
{
    public class TestErrorListener : BaseErrorListener
    {
        private readonly List<DiagnosticResult> diagnostics;
        public TestErrorListener(List<DiagnosticResult> diags){
            this.diagnostics = diags;
        }
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, 
            int line, int charPositionInLine, string msg, RecognitionException e)
        {
            diagnostics.Add(new DiagnosticResult(Severity.Error, $"In position { line } : {charPositionInLine} {recognizer} is expected but {offendingSymbol.Text} is given"));
            //base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}