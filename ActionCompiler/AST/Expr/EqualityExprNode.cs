using System;

namespace ActionCompiler.AST.Expr
{
    public record EqualityExprNode(ExprNode left, ExprNode right, EqualityOperator oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEqualityExpr(this);
        }

        public virtual bool Equals(EqualityExprNode? other)
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

    public enum EqualityOperator
    {
        EQUALS,
        NOTEQUALS
    }
}
