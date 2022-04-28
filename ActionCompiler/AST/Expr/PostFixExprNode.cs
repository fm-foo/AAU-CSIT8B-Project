using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record PostFixExprNode(ExprNode expr, PostFixOperator oper) : ExprNode
    {
        public virtual bool Equals(PostFixExprNode? other)
        {
            if (other is null)
            {
                return false;
            }

            return expr.Equals(other.expr) && oper.Equals(other.oper);
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VistPostFixExpr(this);
        }
    }

    public enum PostFixOperator
    {
        PLUSPLUS,
        MINUSMINUS
    }
}
