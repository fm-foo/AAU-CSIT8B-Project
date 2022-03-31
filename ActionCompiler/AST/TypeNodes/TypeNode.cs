namespace Action.AST
{
    public record TypeNode() : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitType(this);
        }
    }
}