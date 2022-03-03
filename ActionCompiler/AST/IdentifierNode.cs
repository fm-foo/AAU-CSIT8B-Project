namespace Action.AST
{
    public record IdentifierNode(string identifier) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIdentifier(this);
        }
    }
}