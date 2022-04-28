using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record EntityNode(
        IdentifierNode identifier,
        IEnumerable<FieldDecNode> fieldDecs,
        IEnumerable<PropertyNode> funcDecs) 
        : ComplexNode(new EntityKeywordNode(), fieldDecs.Concat(funcDecs), Enumerable.Empty<ValueNode>())
    {
        public virtual bool Equals(EntityNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && fieldDecs.SequenceEqual(other.fieldDecs) && funcDecs.SequenceEqual(other.funcDecs);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEntity(this);
        }
    }
}
