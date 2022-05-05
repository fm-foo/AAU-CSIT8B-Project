using ActionCompiler.AST.TypeNodes;

namespace ActionCompiler.AST
{
    public record FunctionArgumentNode(IdentifierNode identifier, TypeNode typeNode) : ValueNode
    {
        public virtual bool Equals(FunctionArgumentNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && typeNode.Equals(other.typeNode);
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