using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorCoordinateOffMapVisitor : DiagnosticsVisitor
    {
        // TODO: This does not take into account sub-sub-sections - it only checks maps
        //      It needs to be extended to check every sub-section in any section
        // TODO: This does not take into account the relative coordinates of 
        // TODO: This does not check reference sections - does this happen after reference sections are resolved?
        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            var size = GetSize(mapNode);
            var result = mapNode.sections
                .OfType<SectionNode>()
                .Select(s => s.GetProperty<ShapeKeywordNode, ComplexNode>())
                .Where(s => s.type is CoordinatesKeywordNode)
                .SelectMany(s => s.values)
                .Cast<CoordinateNode>()
                .Where(c => c.x.integer > size.width || c.y.integer > size.height)
                .Select(c => new DiagnosticResult(Severity.Error, "The given coordinates are off the map"));
            return result;
        }

        public (int width, int height) GetSize(ComplexNode node)
        {
            ComplexNode shape = node.GetProperty<ShapeKeywordNode, ComplexNode>();
            int height = shape.GetProperty<HeightKeywordNode, IntNode>().integer;
            int width = shape.GetProperty<WidthKeywordNode, IntNode>().integer;
            return (width, height);
        }

    }
}