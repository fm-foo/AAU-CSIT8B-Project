using System;

namespace Action.AST
{
    public record ExprNode() : SymbolNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitExpr(this);
        }
    }
}
