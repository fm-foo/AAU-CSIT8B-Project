using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record RelationalExprNode(ExprNode left, ExprNode right, RelationalOper oper) : ExprNode
    {
        public virtual bool Equals(RelationalExprNode? other)
        {
            if(other is null)
            {
                return false;
            }
            return left.Equals(other.left) && right.Equals(other.right) && oper.Equals(other.oper);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitRelationalExpr(this);
        }
    }

    public enum RelationalOper
    {
        LESSTHAN,
        GREATERTHAN,
        LESSTHANOREQUAL,
        GREATERTHANOREQUAL
    }
}
