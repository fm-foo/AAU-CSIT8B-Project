using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST.Expr
{
    public record NewObjectNode(IdentifierNode identifier, IEnumerable<ExprNode> funcArgs) : ExprNode
    {

        public virtual bool Equals(NewObjectNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && funcArgs.SequenceEqual(other.funcArgs);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            hc.Add(identifier);
            foreach (var arg in funcArgs)
                hc.Add(arg);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitNewObject(this);
        }
    }
}
