namespace Action.AST
{
    public record PropertyNode(IdentifierNode identifier, ValueNode value) : SymbolNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitProperty(this);
        }
    }
}