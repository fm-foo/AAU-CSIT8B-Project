using System;

namespace ActionCompiler.AST.Expr
{
    public record UnaryExprNode(ExprNode primaryExpr, UnaryOper oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }

        public virtual bool Equals(UnaryExprNode? other)
        {
            return other is not null
                && primaryExpr == other.primaryExpr
                && oper == other.oper;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(primaryExpr, oper);
        }
    }

    public enum UnaryOper
    {
        PLUS,
        MINUS,
        NEGATE,
        INCREMENT,
        DECREMENT
    }
}
