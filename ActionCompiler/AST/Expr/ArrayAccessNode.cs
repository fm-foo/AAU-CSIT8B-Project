using System;

namespace ActionCompiler.AST.Expr
{
    public record ArrayAccessNode(ExprNode arrayExpr, ExprNode expr) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArrayAccess(this);
        }

        public virtual bool Equals(ArrayAccessNode? other)
        {
            return other is not null
                && arrayExpr == other.arrayExpr
                && expr == other.expr;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(arrayExpr, expr);
        }
    }

}
