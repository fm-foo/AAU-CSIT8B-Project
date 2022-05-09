using ActionCompiler.AST.Statement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST
{
    public record FunctionNode(IEnumerable<FunctionArgumentNode> args, BlockNode block) : ValueNode
    {
        public virtual bool Equals(FunctionNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return args.SequenceEqual(other.args) && block.Equals(other.block);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            foreach (var arg in args)
                hc.Add(arg);
            hc.Add(block);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunction(this);
        }
    }
}
