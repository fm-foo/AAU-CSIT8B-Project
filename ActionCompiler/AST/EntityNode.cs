using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record EntityNode(IdentifierNode identifier, IEnumerable<FieldNode> fieldDecs, IEnumerable<PropertyNode> funcDecs) : ComplexNode(new EntityKeywordNode(), fieldDecs, Enumerable.Empty<ValueNode>())
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEntity(this);
        }
    }
}
