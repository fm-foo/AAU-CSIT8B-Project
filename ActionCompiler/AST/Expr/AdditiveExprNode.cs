﻿namespace ActionCompiler.AST.Expr
{
    public record AdditiveExprNode(ExprNode left, ExprNode right, AdditiveOper oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitAdditiveExpr(this);
        }
    }

    public enum AdditiveOper
    {
        PLUS,
        MINUS
    }
}
