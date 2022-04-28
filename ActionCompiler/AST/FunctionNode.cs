using System;
using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record FunctionNode(List<FunctionArgumentNode> args, BlockNode block) : ValueNode
    {
        public virtual bool Equals(FunctionNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return this.args.SequenceEqual(other.args) && this.block.Equals(other.block);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }
    }
}
