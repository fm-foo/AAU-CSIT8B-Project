using System;
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
            var hc = new HashCode();
            hc.Add(identifier);
            foreach (var property in properties)
                hc.Add(property);
            foreach (var section in sections)
                hc.Add(section);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMap(this);
        }
    }
}