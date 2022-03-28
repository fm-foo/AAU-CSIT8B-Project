namespace Action.AST
{
    public record DeclarationNode(TypeNode type, IdentifierNode identifier, ExprNode? expr = null) : SemicolonStatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitDeclaration(this);
        }
    }
}