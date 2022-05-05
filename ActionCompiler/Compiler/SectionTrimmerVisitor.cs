using Action.AST;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.Compiler
{
    public class SectionTrimmerVisitor : NodeVisitor<ComplexNode?>
    {
        public override ComplexNode VisitMap(MapNode mapNode)
        {
            var sections = TrimSections(mapNode.sections);
            return new MapNode(mapNode.identifier, mapNode.properties, sections);
        }

        public override ComplexNode? VisitSection(SectionNode sectionNode)
        {
            if (sectionNode.coords is null)
                return null;
            var sections = TrimSections(sectionNode.sections);
            return new SectionNode(sectionNode.coords, sectionNode.identifier, sectionNode.properties, sections);
        }

        public List<ComplexNode> TrimSections(IEnumerable<ValueNode> sections)
        {
            List<ComplexNode> newsections = new List<ComplexNode>();
            foreach (SectionNode section in sections.Cast<SectionNode>())
            {
                ComplexNode? newsection = Visit(section);
                if (newsection is not null)
                    newsections.Add(newsection);
            }
            return newsections;
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