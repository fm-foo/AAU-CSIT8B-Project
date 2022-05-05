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
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitComplex(this);
        }
    }
}