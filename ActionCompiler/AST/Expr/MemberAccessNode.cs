using System;

namespace ActionCompiler.AST.Expr
{
    public record MemberAccessNode(ExprNode expr, IdentifierNode identifier) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMemberAccess(this);
        }

        public virtual bool Equals(MemberAccessNode? other)
        {
            return other is not null
                && expr == other.expr
                && identifier == other.identifier;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(expr, identifier);
        }
    }
}
