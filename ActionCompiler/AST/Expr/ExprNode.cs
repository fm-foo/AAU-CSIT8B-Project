﻿namespace Action.AST
{
    public record ExprNode() : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitExpr(this);
        }
    }

}
