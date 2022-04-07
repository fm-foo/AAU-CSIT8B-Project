﻿using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record UnaryExprNode(ExprNode primaryExpr, UnaryOper oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    public enum UnaryOper
    {
        PLUS,
        MINUS,
        NEGATE,
        INCREMENT,
        DECREMENT
    }
}
