namespace Action.AST
{
    public record IntNode(int integer) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitInt(this);
        }
    }

    public record FloatNode(float f) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }
    }
}