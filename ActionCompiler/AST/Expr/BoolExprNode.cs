using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST.Expr
{
    public record BoolExprNode(ExprNode left, ExprNode right, BooleanOperator oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBooleanExpr(this);
        }

        public virtual bool Equals(BoolExprNode? other)
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

    public enum BooleanOperator
    {
        AND,
        OR
    }
}
