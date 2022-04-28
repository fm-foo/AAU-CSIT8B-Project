using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Action.AST
{
    public record FileNode(IEnumerable<ValueNode> nodes) : SymbolNode
    {
        public virtual bool Equals(FileNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return nodes.SequenceEqual(other.nodes);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFile(this);
        }
    }
}