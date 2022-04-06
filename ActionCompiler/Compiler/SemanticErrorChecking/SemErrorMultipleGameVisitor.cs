using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorMultipleGameVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            var gameNodes = nodes.nodes.Where(n => n.GetType() == typeof(GameNode));

            if(gameNodes.Count() > 1){
                yield return new DiagnosticResult(Severity.Error, "You can't have more than one game element");
            }
        }
    }
}