using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorEmptyBackgroundVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
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
            return CheckColour(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckColour(sectionNode);
        }

        public IEnumerable<DiagnosticResult> CheckColour(ComplexNode node)
        {

            foreach (var property in node.properties)
            {
                if (property.identifier is BackgroundKeywordNode)
                {
                    ComplexNode val = (ComplexNode)property.value;
                    if (val.type is ColourKeywordNode)
                    {
                        if (!val.properties.Any())
                        {
                            yield return new DiagnosticResult(Severity.Error, "Missing colour value!", Error.MissingBackgroundColorValue);
                        }
                    }
                    if (val.type is ImageKeywordNode)
                    {
                        if (!val.properties.Any())
                        {
                            yield return new DiagnosticResult(Severity.Error, "Missing image path!", Error.MissingBackgroundImagePathValue);
                        }
                    }

                }

            }
        }
    }
}