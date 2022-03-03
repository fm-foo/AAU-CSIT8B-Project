using Action.AST;

namespace Action.Compiler
{
    public record SectionSymbolEntry(SectionNode section, ComplexNode? scope);
}