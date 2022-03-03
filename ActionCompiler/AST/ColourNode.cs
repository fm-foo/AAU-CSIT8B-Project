namespace Action.AST
{
    public record ColourNode(byte r, byte g, byte b) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitColour(this);
        }
    }
}