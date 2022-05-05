using ActionCompiler.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorFailedBinding : DiagnosticsVisitor
    {
        public override IEnumerable<DiagnosticResult> VisitIdentifier(IdentifierNode identifierNode)
        {
            yield return new DiagnosticResult(Severity.Error, $"Identifier '{identifierNode}' not bound", Error.FailedBinding);
        }
    }
}