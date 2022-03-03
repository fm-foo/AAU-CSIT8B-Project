using System.Collections.Generic;

namespace Action.AST
{
    public record MapNode(
        IdentifierNode identifier,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> sections) : ComplexNode(new MapKeywordNode(), properties, sections)
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMap(this);
        }
    }
}