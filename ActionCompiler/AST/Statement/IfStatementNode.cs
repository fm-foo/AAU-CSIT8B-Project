namespace Action.AST
{
    public record IfStatementNode(ExprNode expr, StatementNode primaryStatement, StatementNode? elseStatement = null) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }
    }
}