namespace Action.AST
{
    public record SimpleTypeNode(IdentifierNode identifier) : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitSimpleType(this);
        }
    }

}