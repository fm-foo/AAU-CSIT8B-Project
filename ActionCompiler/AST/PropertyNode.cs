namespace Action.AST
{
    public record PropertyNode(IdentifierNode identifier, ExprNode? value) : SymbolNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitProperty(this);
        }
    }
}