using System;

namespace ActionCompiler.AST.Types
{

    public record IntNode(int integer) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitInt(this);
        }

        public virtual bool Equals(IntNode? other)
        {
            return other is not null
                && integer == other.integer;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(integer);
        }
    }
}