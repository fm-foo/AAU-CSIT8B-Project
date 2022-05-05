using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST
{
    public record MapNode(
        IdentifierNode identifier,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> sections) : ComplexNode(new MapKeywordNode(), properties, sections)
    {

        public virtual bool Equals(MapNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && properties.SequenceEqual(other.properties) && sections.SequenceEqual(other.sections);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMap(this);
        }
    }
}