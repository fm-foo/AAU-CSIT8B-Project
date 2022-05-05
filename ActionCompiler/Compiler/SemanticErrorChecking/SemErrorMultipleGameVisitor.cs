using ActionCompiler.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorMultipleGameVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            var gameNodes = nodes.nodes.OfType<GameNode>();

            if (gameNodes.Count() > 1)
            {
                yield return new DiagnosticResult(Severity.Error, "You can't have more than one game element");
            }
        }
    }
}