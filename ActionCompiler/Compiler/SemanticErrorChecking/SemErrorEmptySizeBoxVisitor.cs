using Action.AST;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorEmptySizeBoxVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            var combinedNodes = nodes.nodes.Where(n => n is MapNode || n is SectionNode);
            foreach (var node in combinedNodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            return CheckSize(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckSize(sectionNode);
        }

        public IEnumerable<DiagnosticResult> CheckSize(ComplexNode node)
        {

            foreach (var property in node.properties)
            {
                if (property.identifier is ShapeKeywordNode)
                {
                    ComplexNode val = (ComplexNode)property.value!;
                    if (val.type is BoxKeywordNode)
                    {
                        if (!(val.properties.Count() == 2) && !(val.properties.OfType<WidthKeywordNode>().Count() == 1 && val.properties.OfType<HeightKeywordNode>().Count() == 1))
                        {
                            yield return new DiagnosticResult(Severity.Error, "Missing height and/or width of the box", Error.MissingBoxWidthHeight);
                        }
                    }
                }
            }
        }
    }
}