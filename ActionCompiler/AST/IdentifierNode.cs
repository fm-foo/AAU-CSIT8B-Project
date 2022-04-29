using System;

namespace Action.AST
{
    public record IdentifierNode(string identifier) : ValueNode
    {
        public virtual bool Equals(IdentifierNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return String.Equals(identifier, other.identifier);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIdentifier(this);
        }
    }
}