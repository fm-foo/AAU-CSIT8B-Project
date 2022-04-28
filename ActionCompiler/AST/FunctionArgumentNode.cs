namespace Action.AST
{
    public record FunctionArgumentNode(IdentifierNode identifier, TypeNode typeNode) : ValueNode
    {
        public virtual bool Equals(FunctionArgumentNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return this.identifier.Equals(other.identifier) && this.typeNode.Equals(other.typeNode);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionArgument(this);
        }
    }
}