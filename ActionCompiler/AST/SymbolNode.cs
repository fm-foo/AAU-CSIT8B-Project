namespace Action.AST
{
    public abstract record SymbolNode
    {
        public abstract T Accept<T>(NodeVisitor<T> visitor); 

    }
}