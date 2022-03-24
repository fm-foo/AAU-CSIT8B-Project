namespace Action.AST
{
    public record BoolNode(bool val) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBool(this);
        }
    }
}