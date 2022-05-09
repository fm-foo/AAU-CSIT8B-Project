using ActionCompiler.AST.Expr;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST.Types
{
    public record ArrayNode(IEnumerable<ExprNode> values) : ValueNode
    {

        public virtual bool Equals(ArrayNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return values.SequenceEqual(other.values);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            foreach (var value in values)
                hc.Add(value);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArray(this);
        }
    }
}