using System;

namespace ActionCompiler.AST.Expr
{
    public record MultiplicativeExprNode(ExprNode left, ExprNode right, MultOper oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMultiplicativeExprNode(this);
        }

        public virtual bool Equals(MultiplicativeExprNode? other)
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

    public enum MultOper
    {
        TIMES,
        DIV
    }
}
