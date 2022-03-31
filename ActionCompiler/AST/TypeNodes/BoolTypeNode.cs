namespace Action.AST
{
    public record BoolTypeNode() : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBoolType(this);
        }
    }

}