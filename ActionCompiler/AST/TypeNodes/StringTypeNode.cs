namespace Action.AST
{
    public record StringTypeNode() : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitStringType(this);
        }
    }

}