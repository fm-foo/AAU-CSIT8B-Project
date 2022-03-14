using Action.AST;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SemErrorLineOnlyOneCoordinate : NodeVisitor<IEnumerable<DiagnosticResult>>
    {

        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            foreach (var node in nodes.nodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }
        public override IEnumerable<DiagnosticResult> VisitComplex(ComplexNode complexNode)
        {
            if (complexNode.type is not LineKeywordNode)
                yield break;
            var numOfCoords = complexNode.values.Count();
            if (numOfCoords == 1)
                yield return new DiagnosticResult(Severity.Error, "cannot have a line node with only a single point");
        }
    }
}