using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorGameFunctionMissingVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            var combinedNodes = nodes.nodes.OfType<GameNode>();

            foreach (var node in combinedNodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitGame(GameNode game)
        {
            foreach (var fun in game.funcDecs)
            {
                if (fun.identifier.identifier.Equals("initialize"))
                {
                    yield break;
                }
            }
            yield return new DiagnosticResult(Severity.Error, "The initialize function is missing");
        }
    }
}