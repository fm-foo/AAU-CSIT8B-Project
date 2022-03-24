using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record PrimaryExprNode(ValueNode value) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitPrimaryExpr(this);
        }
    }

    public enum PrimaryExprOper
    {
        POSTFIX_INCREMENT,
        POSTFIX_DECREMENT,

    }
}
