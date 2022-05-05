using System;

namespace ActionCompiler.AST
{
    public record BoundIdentifierNode(Guid id) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBoundIdentifier(this);
        }
    }
}