using System.Collections.Generic;

namespace Action.AST
{
    public record BlockNode(List<StatementNode> statements) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}