using System;
using System.Collections.Generic;
using System.Text;

namespace Action.AST
{
    public record FileNode(IEnumerable<ValueNode> nodes) : SymbolNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFile(this);
        }
    }
}