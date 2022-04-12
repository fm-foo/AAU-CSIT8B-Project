namespace Action.AST
{
    public record ArrayNode(ExprNode[] values) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArray(this);
        }
    }
}