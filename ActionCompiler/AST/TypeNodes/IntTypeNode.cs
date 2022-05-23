namespace ActionCompiler.AST.TypeNodes
{
    public record IntTypeNode : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIntType(this);
        }

        public override string GetTypeName()
        {
            return "int";
        }
    }

}