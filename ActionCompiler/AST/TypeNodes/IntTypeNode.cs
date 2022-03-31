namespace Action.AST
{
    public record IntTypeNode() : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIntType(this);
        }
    }

}