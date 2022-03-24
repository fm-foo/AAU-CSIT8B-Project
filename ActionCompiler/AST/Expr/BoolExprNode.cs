using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record BoolExprNode(ExprNode expr, ExprNode? boolExpr = null, BooleanOperator? oper = null) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBooleanExpr(this);
        }
    }

    public enum BooleanOperator
    {
        AND,
        OR
    }
}
