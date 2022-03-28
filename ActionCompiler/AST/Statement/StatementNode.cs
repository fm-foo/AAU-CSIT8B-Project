namespace Action.AST
{
    public record StatementNode() : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitStatement(this);
        }
    }
}