namespace Action.AST
{
    public record FunctionArgumentNode(IdentifierNode identifier, TypeNode type) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionArgument(this);
        }
    }
}