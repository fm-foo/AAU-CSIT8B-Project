using ActionCompiler.AST.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST
{
    public record SectionNode(
        CoordinateNode? coords,
        IdentifierNode? identifier,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> sections) : ComplexNode(new SectionKeywordNode(), properties, sections)
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitSection(this);
        }

        public virtual bool Equals(SectionNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier == other.identifier 
                && properties.SequenceEqual(other.properties) 
                && sections.SequenceEqual(other.sections);
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
    }
}