namespace Action.AST
{
    public record FunctionArgumentNode(IdentifierNode identifier, TypeNode typeNode) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionArgument(this);
        }
    }
}