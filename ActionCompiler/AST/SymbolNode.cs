using System.Text;

namespace ActionCompiler.AST
{
    public abstract record SymbolNode
    {
        public SymbolNode? Parent { get; set; }
        public abstract T Accept<T>(NodeVisitor<T> visitor);

        public override abstract int GetHashCode();

        protected virtual bool PrintMembers(StringBuilder builder)
        {
            string parent = Parent is null ? "null" : $"{Parent.EqualityContract.Name} {{ ... }}";
            builder.Append($"Parent = {parent}");
            return true;
        }
    }
}