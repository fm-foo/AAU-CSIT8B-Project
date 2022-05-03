namespace Action.AST
{
    public record PropertyNode(IdentifierNode identifier, ExprNode? value) : SymbolNode
    {
        public virtual bool Equals(PropertyNode? other)
        {
            if(other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && value is null? other.value is null : value!.Equals(other.value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitProperty(this);
        }
    }
}