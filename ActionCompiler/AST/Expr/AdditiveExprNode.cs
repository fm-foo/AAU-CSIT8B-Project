using System;

namespace ActionCompiler.AST.Expr
{
    public record AdditiveExprNode(ExprNode left, ExprNode right, AdditiveOper oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitAdditiveExpr(this);
        }

        public virtual bool Equals(AdditiveExprNode? other)
        {
            return other is not null
                && left == other.left
                && right == other.right
                && oper == other.oper;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(left, right, oper);
        }
    }

    public enum AdditiveOper
    {
        PLUS,
        MINUS
    }
}
