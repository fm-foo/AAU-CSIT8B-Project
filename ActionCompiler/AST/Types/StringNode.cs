namespace Action.AST
{
    public record StringNode(string s) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitString(this);
        }
    }
}