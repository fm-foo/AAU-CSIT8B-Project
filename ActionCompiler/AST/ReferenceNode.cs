using System;
using ActionCompiler.AST.Types;

namespace ActionCompiler.AST
{
    public record ReferenceNode(IdentifierNode referenceType, IdentifierNode reference, 
        CoordinateNode coords) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitReference(this);
        }

        public virtual bool Equals(ReferenceNode? other)
        {
            return other is not null
                && referenceType == other.referenceType
                && reference == other.reference
                && coords == other.coords;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(referenceType, reference, coords);
        }
    }
}