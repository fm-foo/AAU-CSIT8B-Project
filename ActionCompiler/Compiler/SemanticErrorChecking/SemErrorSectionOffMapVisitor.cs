using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorSectionOffMapVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode nodes)
        {
            IEnumerable<MapNode> query1 = nodes.nodes.OfType<MapNode>();
            foreach (var node in query1)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            var size = GetSize(mapNode);
            foreach (var sec in mapNode.sections)
            {
                if (sec is ReferenceNode valRef)
                {
                    if (valRef.coords.x.integer > size[0] && valRef.coords.y.integer > size[1])
                    {
                        yield return new DiagnosticResult(Severity.Error, "section is off the map");
                    }
                }
                if (sec is SectionNode valSec)
                {
                    if (valSec.coords != null)
                    {
                        if (valSec.coords.x.integer > size[0] && valSec.coords.y.integer > size[1])
                        {
                            yield return new DiagnosticResult(Severity.Error, "section is off the map");
                        }
                    }
                }
            }
        }

        public int[] GetSize(ComplexNode node)
        {
            int[] size = new int[] { 0, 0 };
            foreach (var property in node.properties)
            {
                if (property.identifier is ShapeKeywordNode)
                {
                    ComplexNode val = (ComplexNode)property.value;
                    if (val.type is BoxKeywordNode)
                    {
                        foreach (var prop in val.properties)
                        {
                            if (prop.identifier is HeightKeywordNode)
                            {
                                IntNode res = (IntNode)prop.value;
                                size[0] = res.integer;
                            }
                            if (prop.identifier is WidthKeywordNode)
                            {
                                IntNode res = (IntNode)prop.value;
                                size[1] = res.integer;
                            }
                        }
                    }
                }
            }
            return size;
        }

    }
}