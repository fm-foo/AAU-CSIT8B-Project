using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST.Expr
{
    public record FunctionCallExprNode(ExprNode expr, IEnumerable<ExprNode> funcArgs) : ExprNode
    {

        public virtual bool Equals(FunctionCallExprNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return expr.Equals(other.expr) && funcArgs.SequenceEqual(other.funcArgs);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            hc.Add(expr);
            foreach (var arg in funcArgs)
                hc.Add(arg);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFunctionCallExpr(this);
        }
    }
}
