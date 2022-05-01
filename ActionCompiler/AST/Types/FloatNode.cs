namespace Action.AST
{
    public record FloatNode(double f) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }
    }
}