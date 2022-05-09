using System;

namespace ActionCompiler.AST.Types
{
    public record CoordinateNode(IntNode x, IntNode y) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitCoordinate(this);
        }

        public virtual bool Equals(CoordinateNode? other)
        {
            return other is not null
                && x == other.x
                && y == other.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}