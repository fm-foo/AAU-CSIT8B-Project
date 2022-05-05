using Action.AST;

namespace ActionCompiler.Compiler
{
    public record SectionSymbolEntry(SectionNode section, ComplexNode? scope);
}