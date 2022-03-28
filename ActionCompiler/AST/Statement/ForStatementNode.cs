namespace Action.AST
{
    public record ForStatementNode(StatementNode statement, StatementNode? initialization = null, ExprNode? condition = null, ExprNode? control = null) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitForStatement(this);
        }
    }
}