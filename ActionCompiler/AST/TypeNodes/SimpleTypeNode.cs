namespace Action.AST
{
    public record SimpleTypeNode() : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitSimpleType(this);
        }
    }

}