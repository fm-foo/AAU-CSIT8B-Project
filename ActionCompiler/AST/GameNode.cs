using System.Collections.Generic;
using System.Linq;

namespace Action.AST
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
    }
}
