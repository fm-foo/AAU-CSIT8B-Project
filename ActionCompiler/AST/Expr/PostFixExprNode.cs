using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record PostFixExprNode(ExprNode expr, PostFixOperator oper) : ExprNode
    {
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
