namespace Action.AST
{
    // TODO: is this correct (is an expression also a statement?)
    public record ExprNode() : SemicolonStatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitExpr(this);
        }
    }

}
