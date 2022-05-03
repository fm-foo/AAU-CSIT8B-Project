namespace Action.AST
{
    public record DeclarationNode(TypeNode type, IdentifierNode identifier, ExprNode? expr = null) : StatementNode
    {
        public virtual bool Equals(DeclarationNode? other)
        {
            if(other is null)
            {
                return false;
            }

            return type.Equals(other.type) && identifier.Equals(other.identifier) && (expr is null? other.expr is null : expr.Equals(other.expr));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitDeclaration(this);
        }
    }
}