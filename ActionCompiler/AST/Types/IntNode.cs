namespace ActionCompiler.AST.Types
{

    public record IntNode(int integer) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitInt(this);
        }
    }
}