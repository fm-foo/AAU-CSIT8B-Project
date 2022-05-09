using System;

namespace ActionCompiler.AST
{
    public record BoundIdentifierNode(Guid id) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBoundIdentifier(this);
        }

        public virtual bool Equals(BoundIdentifierNode? other)
        {
            return other is not null
                && id == other.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id);
        }
    }
}