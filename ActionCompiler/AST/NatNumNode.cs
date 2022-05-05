namespace ActionCompiler.AST
{
    public record NatNumNode(uint i) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitNatNum(this);
        }
    }
}