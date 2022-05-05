using ActionCompiler.AST.Types;
using System.Collections.Generic;

namespace ActionCompiler.AST
{
    public record SectionNode(
        CoordinateNode? coords,
        IdentifierNode? identifier,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> sections) : ComplexNode(new SectionKeywordNode(), properties, sections)
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitSection(this);
        }
    }
}