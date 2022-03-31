namespace Action.AST
{

    public record FloatTypeNode() : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloatType(this);
        }
    }

    public record ArrayTypeNode(TypeNode type) : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArrayType(this);
        }
    }
}