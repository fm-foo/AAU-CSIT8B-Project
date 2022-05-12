using System;
using System.Collections.Generic;
using System.Linq;
using ActionCompiler.AST.Statement;

namespace ActionCompiler.AST.Bindings
{
    public record BoundDeclarationNode(DeclarationNode dec, Binding binding) : DeclarationNode(dec.type, dec.identifier, dec.expr)
    {
        public virtual bool Equals(BoundDeclarationNode? other)
        {
            return other is not null
                && dec == other.dec
                && binding == other.binding;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(dec, binding);
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBoundDeclaration(this);
        }
    }
}
