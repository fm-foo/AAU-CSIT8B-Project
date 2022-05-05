namespace ActionCompiler.AST.Types
{
    public record CoordinateNode(IntNode x, IntNode y) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitCoordinate(this);
        }
    }
}