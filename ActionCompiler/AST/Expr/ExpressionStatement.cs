namespace Action.AST
{
    public record ExpressionStatementNode(ExprNode expr) : StatementNode //SymbolNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }
    }

}
