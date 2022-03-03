namespace Action.AST
{
    public record ReferenceNode(IdentifierNode referenceType, IdentifierNode reference, CoordinateNode coords) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitReference(this);
        }
    }
}