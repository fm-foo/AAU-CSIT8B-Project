using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST
{
    public record GameNode(
        IdentifierNode identifier,
        IEnumerable<FieldDecNode> fieldDecs,
        IEnumerable<PropertyNode> funcDecs)
        : ComplexNode(new GameKeywordNode(), fieldDecs.Concat(funcDecs), Enumerable.Empty<ValueNode>())
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitGame(this);
        }

        public virtual bool Equals(GameNode? other)
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
    }
}
