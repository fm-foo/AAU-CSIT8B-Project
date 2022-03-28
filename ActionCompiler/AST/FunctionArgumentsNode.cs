using System.Collections.Generic;

namespace Action.AST
{
    public record FunctionArgumentsNode(List<FunctionArgumentNode> args) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionArguments(this);
        }
    }
}
