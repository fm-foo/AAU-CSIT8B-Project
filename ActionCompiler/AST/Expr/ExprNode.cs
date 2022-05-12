using System.Text;
using ActionCompiler.AST.TypeNodes;

namespace ActionCompiler.AST.Expr
{
    public abstract record ExprNode : SymbolNode
    {
        public TypeNode? Type { get; set; }

        protected override bool PrintMembers(StringBuilder builder)
        {
            builder.Append($"Type = {(Type is null ? "<indeterminite>" : Type)}");
            return true;
        }
    }
}
