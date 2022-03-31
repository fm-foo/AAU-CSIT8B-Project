﻿using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record RelationalExprNode(ExprNode expr, ExprNode? relationalExpr = null, RelationalOper? oper = null) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitRelationalExpr(this);
        }
    }

    public enum RelationalOper
    {
        LESSTHAN,
        GREATERTHAN,
        LESSTHANOREQUAL,
        GREATERTHANOREQUAL,
    }
}