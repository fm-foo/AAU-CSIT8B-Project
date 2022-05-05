namespace ActionCompiler.AST
{
    public abstract record SymbolNode
    {
        public SymbolNode? Parent { get; set; }
        public abstract T Accept<T>(NodeVisitor<T> visitor);
    }
}