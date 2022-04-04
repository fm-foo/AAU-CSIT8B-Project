using Action.AST;
using Action.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class SemErrorOnlyOnePathProperty : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            var combinedNodes = file.nodes.Where(n => n is MapNode || n is SectionNode);
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

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckProperties(sectionNode);
        }

        private IEnumerable<DiagnosticResult> CheckProperties(ComplexNode node)
        {
            Debug.Assert(node is MapNode || node is SectionNode);

            var background = node.properties.Where(n => n.identifier.GetType() == typeof(BackgroundKeywordNode));

            Debug.Assert(background.Count() == 1);

            return this.VisitProperty(background.First());
        }

        public override IEnumerable<DiagnosticResult> VisitProperty(PropertyNode propertyNode)
        {
            ComplexNode? complexNode = propertyNode.value as ComplexNode;

            Debug.Assert(complexNode != null);

            int count = complexNode.properties.Where(n => n.identifier.GetType() == typeof(PathKeywordNode)).Count();

            if (count > 1)
            {
                yield return new DiagnosticResult(Severity.Error, $"Shape may only contan one path property!");
            }
        }

    }
}
