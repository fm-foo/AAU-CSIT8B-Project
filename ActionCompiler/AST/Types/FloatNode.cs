using System;

namespace ActionCompiler.AST.Types
{
    public record FloatNode(double f) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }

        public virtual bool Equals(FloatNode? other)
        {
            return other is not null
                && f == other.f;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(f);
        }
    }
}