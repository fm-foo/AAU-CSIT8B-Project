namespace ActionCompiler.AST.TypeNodes
{
    public record StringTypeNode : TypeNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitStringType(this);
        }
    }

}