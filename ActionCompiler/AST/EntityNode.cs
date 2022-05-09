using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST
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
            var hc = new HashCode();
            hc.Add(type);
            foreach (var field in fieldDecs)
                hc.Add(field);
            foreach (var func in funcDecs)
                hc.Add(func);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEntity(this);
        }
    }
}
