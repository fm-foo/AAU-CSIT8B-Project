namespace ActionCompiler.AST.TypeNodes
{
    public record FloatTypeNode : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloatType(this);
        }

        public override string GetTypeName()
        {
            return "double";
        }
    }
}