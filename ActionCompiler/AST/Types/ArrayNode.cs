using System.Collections.Generic;

namespace Action.AST
{
    public record ArrayNode(IEnumerable<ExprNode> values) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArray(this);
        }
    }
}