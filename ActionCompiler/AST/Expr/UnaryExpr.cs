﻿using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record UnaryExprNode(ExprNode primaryExpr, UnaryOper? oper = null) : ExprNode
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
        BANG,
        PLUSPLUS,
        MINUSMINUS
    }

    public enum PrimaryOper
    {
        PLUSPLUS,
        MINUSMINUS
    }
}
