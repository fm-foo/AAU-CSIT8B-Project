using System;
using System.Linq;

namespace Action.AST
{
    public record FunctionNode(FunctionArgumentsNode args, BlockNode block) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }
    }
}
