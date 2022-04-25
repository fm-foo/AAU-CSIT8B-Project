using System.Collections.Generic;

namespace Action.AST
{
    public record BlockNode(IEnumerable<StatementNode> statements) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}