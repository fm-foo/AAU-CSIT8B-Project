using System.Linq;
using ActionCompiler.AST;

namespace ActionCompiler.Compiler
{
    public static class Utilities
    {
        public static U GetProperty<T, U>(this ComplexNode node)
            where T : KeywordNode
            where U : ValueNode
        {
            return node.properties
                .Where(n => n.identifier is T)
                .Select(n => n.value)
                .Cast<U>()
                .Single();
        }
    }
}