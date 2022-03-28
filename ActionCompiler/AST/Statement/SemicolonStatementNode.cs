namespace Action.AST
{
    public record SemicolonStatementNode() : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitSemicolonStatement(this);
        }
    }
}