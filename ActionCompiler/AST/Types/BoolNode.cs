using System;

namespace ActionCompiler.AST.Types
{
    public record BoolNode(bool val) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBool(this);
        }

        public virtual bool Equals(BoolNode? other)
        {
            return other is not null
                && val == other.val;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(val);
        }
    }
}