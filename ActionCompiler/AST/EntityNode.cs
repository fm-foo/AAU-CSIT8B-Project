using System.Collections.Generic;

namespace Action.AST
{
    public record EntityNode(IdentifierNode identifier, IEnumerable<PropertyNode> funcDefs, IEnumerable<ValueNode> fieldDecs) : ComplexNode(new EntityKeywordNode(), funcDefs, fieldDecs)
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEntity(this);
        }
    }
}
