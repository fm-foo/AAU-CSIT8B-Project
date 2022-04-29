using System;

namespace Action.AST
{
    public record BoundIdentifierNode(Guid id) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBoundIdentifier(this);
        }
    }
}