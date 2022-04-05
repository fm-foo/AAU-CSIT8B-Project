using System;
using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record FunctionNode(List<FunctionArgumentNode> args, BlockNode block) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }
    }
}
