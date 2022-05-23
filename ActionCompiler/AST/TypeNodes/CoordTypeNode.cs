namespace ActionCompiler.AST.TypeNodes
{
    public record CoordTypeNode : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitCoordType(this);
        }

        public override string GetTypeName()
        {
            throw new System.NotImplementedException();
        }
    }

}