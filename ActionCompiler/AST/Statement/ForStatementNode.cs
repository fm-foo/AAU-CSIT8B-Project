namespace Action.AST
{
    public record ForStatementNode(StatementNode? initialization, ExprNode? condition, StatementNode? control, StatementNode statement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitForStatement(this);
        }
    }
}