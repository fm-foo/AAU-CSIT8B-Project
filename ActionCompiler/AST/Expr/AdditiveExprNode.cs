using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record AdditiveExprNode(ExprNode expr, ExprNode? additiveExpr = null, AdditiveOper? oper = null) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitAddativeExpr(this);
        }
    }

    public enum AdditiveOper
    {
        PLUS,
        MINUS
    }
}
