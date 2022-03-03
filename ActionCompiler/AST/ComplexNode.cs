using System.Collections.Generic;

namespace Action.AST
{
    public record ComplexNode(
        IdentifierNode type,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> values) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitComplex(this);
        }
    }
}