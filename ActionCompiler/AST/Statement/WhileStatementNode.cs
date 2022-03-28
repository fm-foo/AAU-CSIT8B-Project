namespace Action.AST
{
    public record WhileStatementNode(ExprNode expr, StatementNode statement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }
}