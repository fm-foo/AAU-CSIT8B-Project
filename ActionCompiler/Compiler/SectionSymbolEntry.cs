using ActionCompiler.AST;

namespace ActionCompiler.Compiler
{
    public record SectionSymbolEntry(SectionNode section, ComplexNode? scope);
}