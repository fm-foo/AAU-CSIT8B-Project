namespace Action.AST
{
    public record SimpleTypeNode(IdentifierNode identifier) : TypeNode
    {

        public virtual bool Equals(IdentifierNode? other)
        {
            if(other is null) 
            {
                return false;
            }
            return identifier.Equals(other.identifier);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitSimpleType(this);
        }
    }

}