namespace ActionCompiler.AST.Types
{
    public record StringNode(string s) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitString(this);
        }
    }
}