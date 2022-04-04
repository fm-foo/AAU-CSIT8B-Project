namespace Action.AST
{
    //TODO: replace integer with a string to enable error checking and create a property that can be used to convert it to int
    public record IntNode(int integer) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitInt(this);
        }
    }
}