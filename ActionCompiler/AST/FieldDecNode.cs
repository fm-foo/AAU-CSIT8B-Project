using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record FieldDecNode(IdentifierNode identifier, TypeNode type, ExprNode? expr) : PropertyNode(identifier, expr)
    {

        public virtual bool Equals(FieldDecNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && type.Equals(other.type) && expr is null? other.expr is null : expr!.Equals(other.expr);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFieldDeclaration(this);
        }
    }
}
