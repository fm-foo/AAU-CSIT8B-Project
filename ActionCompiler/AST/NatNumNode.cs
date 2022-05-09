using System;

namespace ActionCompiler.AST
{
    public record NatNumNode(uint i) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitNatNum(this);
        }

        public virtual bool Equals(NatNumNode? other)
        {
            return other is not null
                && i == other.i;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(i);
        }
    }
}