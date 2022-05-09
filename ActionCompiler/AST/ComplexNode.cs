using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST
{
    public record ComplexNode(
        IdentifierNode type,
        IEnumerable<PropertyNode> properties,
        IEnumerable<ValueNode> values) : ValueNode
    {

        public virtual bool Equals(ComplexNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return type.Equals(other.type) && properties.SequenceEqual(other.properties) && values.SequenceEqual(other.values);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            hc.Add(type);
            foreach (var prop in properties)
                hc.Add(prop);
            foreach (var value in values)
                hc.Add(value);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitComplex(this);
        }
    }
}