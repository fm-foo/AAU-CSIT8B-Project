using Action.AST;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Action.Compiler
{
    public class ReferenceResolverVisitor : NodeVisitor<ComplexNode?>
    {
        public ReferenceResolverVisitor(ISet<SectionSymbolEntry> symbols, List<DiagnosticResult> diagnostics)
        {
            this.symbols = symbols;
            this.diagnostics = diagnostics;
            scopes = new Stack<ComplexNode?>();
            scopes.Push(null);
        }

        private readonly Stack<ComplexNode?> scopes;
        private readonly ISet<SectionSymbolEntry> symbols;
        private readonly List<DiagnosticResult> diagnostics;

        public override ComplexNode? VisitMap(MapNode mapNode)
        {
            var newsections = VisitMapOrSection(mapNode);
            if (newsections is null)
                return null;
            return mapNode with { sections = newsections };
        }

        public override ComplexNode? VisitSection(SectionNode sectionNode)
        {
            var newsections = VisitMapOrSection(sectionNode);
            if (newsections is null)
                return null;
            return sectionNode with { sections = newsections };
        }

        private List<ComplexNode>? VisitMapOrSection(ComplexNode node)
        {
            Debug.Assert(node is MapNode or SectionNode);
            scopes.Push(node);
            List<ComplexNode> newsections = new List<ComplexNode>();
            foreach (var section in node.values)
            {
                var newnode = Visit(section);
                if (newnode is null)
                    return null;
                newsections.Add(newnode);
            }
            scopes.Pop();
            return newsections;
        }

        public override ComplexNode? VisitReference(ReferenceNode referenceNode)
        {
            foreach (var scope in scopes)
            {
                var symbol = symbols.SingleOrDefault(s => s.scope == scope && s.section.identifier == referenceNode.reference);
                if (symbol is not null)
                    return symbol.section with { coords = referenceNode.coords };
            }
            diagnostics.Add(new DiagnosticResult(Severity.Error, $"Could not resolve reference {referenceNode.reference.identifier}"));
            return null;
        }

        public override ComplexNode VisitGame(GameNode gameNode)
        {
            return gameNode;
        }

        public override ComplexNode VisitEntity(EntityNode entityNode)
        {
            return entityNode;
        }
    }
}