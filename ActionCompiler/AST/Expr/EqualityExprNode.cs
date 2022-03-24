using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record EqualityExprNode(ExprNode expr, ExprNode? equaluityExpr = null, EqualityOperator? oper = null) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEqualityExpr(this);
        }
    }

    public enum EqualityOperator
    {
        EQUALS,
        NOTEQUALS
    }
}
