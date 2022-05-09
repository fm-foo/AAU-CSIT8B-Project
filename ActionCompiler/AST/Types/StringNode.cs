using System;

namespace ActionCompiler.AST.Types
{
    public record StringNode(string s) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitString(this);
        }

        public virtual bool Equals(StringNode? other)
        {
            return other is not null
                && s == other.s;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(s);
        }
    }
}