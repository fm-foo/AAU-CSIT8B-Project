namespace Action.AST
{
    public record FloatNode(float f) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }
    }
}