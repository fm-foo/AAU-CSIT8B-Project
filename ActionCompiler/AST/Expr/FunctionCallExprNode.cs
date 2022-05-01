using System.Collections.Generic;
using System.Linq;

namespace Action.AST {
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
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor) 
        {
            return visitor.VisitFunctionCallExpr(this);
        }
    }
}
