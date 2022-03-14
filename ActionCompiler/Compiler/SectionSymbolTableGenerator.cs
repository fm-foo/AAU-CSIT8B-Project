using Action.AST;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Action.Compiler
{
    public class SectionSymbolTableGenerator : NodeVisitor<IEnumerable<SectionSymbolEntry>>
    {
        public override IEnumerable<SectionSymbolEntry> VisitFile(FileNode nodes)
        {
            foreach (var node in nodes.nodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
                if (node is SectionNode { identifier: not null } section)
                {
                    yield return new SectionSymbolEntry(section, null);
                }
            }
        }

        public override IEnumerable<SectionSymbolEntry> VisitMap(MapNode mapNode)
        {
            return VisitMapOrSection(mapNode);
        }

        public override IEnumerable<SectionSymbolEntry> VisitSection(SectionNode sectionNode)
        {
            return VisitMapOrSection(sectionNode);
        }

        private IEnumerable<SectionSymbolEntry> VisitMapOrSection(ComplexNode node)
        {
            Debug.Assert(node is MapNode or SectionNode);
            foreach (var section in node.values.OfType<SectionNode>())
            {
                foreach (var symbol in Visit(section))
                    yield return symbol;
                if (section.identifier is not null)
                    yield return new SectionSymbolEntry(section, node);
            }
        }
    }
}