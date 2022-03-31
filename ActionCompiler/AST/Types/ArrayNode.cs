namespace Action.AST
{
    public record ArrayNode(ExprNode[] array) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArray(this);
        }
    }
}