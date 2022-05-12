using System;

namespace ActionCompiler.AST.Bindings
{
    public record BoundIdentifierNode(Binding binding) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBoundIdentifier(this);
        }

        public virtual bool Equals(BoundIdentifierNode? other)
        {
            return other is not null
                && binding == other.binding;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(binding);
        }
    }

    public record BoundFieldDecNode(FieldDecNode field, Binding binding) : FieldDecNode(field.identifier, field.type, field.expr)
    {
        public virtual bool Equals(BoundFieldDecNode? other)
        {
            return other is not null
                && field == other.field
                && binding == other.binding;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(field, binding);
        }
    }
}