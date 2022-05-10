using System;

namespace ActionCompiler.AST.Bindings
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

    public record BoundFieldDecNode(FieldDecNode field, Guid id) : FieldDecNode(field.identifier, field.type, field.expr)
    {
        public virtual bool Equals(BoundFieldDecNode? other)
        {
            return other is not null
                && field == other.field
                && id == other.id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id);
        }
    }
}