namespace ActionCompiler.AST.Types
{
    public record FloatNode(double f) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }
    }
}