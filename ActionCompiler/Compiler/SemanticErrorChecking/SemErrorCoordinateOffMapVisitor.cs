using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorCoordinateOffMapVisitor : NodeVisitor<IEnumerable<DiagnosticResult>>
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
            (int x, int y) size = GetSize(mapNode);
            foreach (var sec in mapNode.sections.OfType<SectionNode>())
            {
                foreach (var property in sec.properties.Where(p => p.identifier is ShapeKeywordNode))
                {
                    ComplexNode val = (ComplexNode)property.value!;
                    if (val.type is CoordinatesKeywordNode)
                    {
                        foreach (var coord in val.values)
                        {
                            CoordinateNode coordinate = (CoordinateNode)coord;
                            if (coordinate.x.integer > size.x || coordinate.x.integer < 0)
                            {
                                yield return new DiagnosticResult(Severity.Error, "X coordinate is out of bounds!", Error.CoordinatesOffMap);
                            }
                            if (coordinate.y.integer > size.y || coordinate.y.integer < 0)
                            {
                                yield return new DiagnosticResult(Severity.Error, "Y coordinate is out of bounds!", Error.CoordinatesOffMap);
                            }
                        }
                    }
                }
            }
        }

        public (int, int) GetSize(ComplexNode node)
        {
            (int x, int y) size = (0,0);
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
                                size.x = res.integer;
                            }
                            if (prop.identifier is WidthKeywordNode)
                            {
                                IntNode res = (IntNode)prop.value;
                                size.y = res.integer;
                            }
                        }
                    }
                }
            }
            return size;
        }

    }
}