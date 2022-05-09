using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionCompiler.AST
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
            var hc = new HashCode();
            foreach (var node in nodes)
                hc.Add(node);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFile(this);
        }
    }
}