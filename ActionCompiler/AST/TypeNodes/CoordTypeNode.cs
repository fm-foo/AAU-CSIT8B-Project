namespace Action.AST
{
    public record CoordTypeNode() : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitCoordType(this);
        }
    }

}