using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record MultiplicativeExprNode(ExprNode expr, ExprNode? multExpr = null, MultOper? oper = null) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMultiplicativeExprNode(this);
        }
    }

    public enum MultOper
    {
        TIMES,
        DIV
    }
}
