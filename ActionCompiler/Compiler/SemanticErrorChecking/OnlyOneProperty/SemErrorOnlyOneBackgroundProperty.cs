using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class SemErrorOnlyOneBackgroundProperty : NodeVisitor<IEnumerable<DiagnosticResult>>
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

            int count = node.properties.Where(n => n.identifier.GetType() == typeof(BackgroundKeywordNode)).Count();

            if (count > 1)
            {
                yield return new DiagnosticResult(Severity.Error, $"{node.GetType().Name} can only contain one background property!");
            }
        }
    }
}
