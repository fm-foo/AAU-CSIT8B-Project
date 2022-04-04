using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class SemErrorOnlyOneShapeProperty : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            var combinedNodes = file.nodes.Where(n => n.GetType() == typeof(MapNode) || n.GetType() == typeof(SectionNode));

            foreach (var node in combinedNodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            return CheckProperties(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode node)
        {
            return CheckProperties(node);
        }

        private IEnumerable<DiagnosticResult> CheckProperties(ComplexNode node)
        {
            Debug.Assert(node is MapNode || node is SectionNode);

            int count = node.properties.Where(n => n.identifier.GetType() == typeof(ShapeKeywordNode)).Count();

            if (count > 1)
            {
                yield return new DiagnosticResult(Severity.Error, $"{node.GetType().Name} can only contain one shape property!");
            }
        }
    }
}
