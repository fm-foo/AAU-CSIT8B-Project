using System;
using ActionCompiler.AST.TypeNodes;

namespace ActionCompiler.AST.Expr
{
    public record IsNode(ExprNode expr, TypeNode type) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIs(this);
        }

        public virtual bool Equals(IsNode? other)
        {
            return other is not null
                && expr == other.expr
                && type == other.type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(expr, type);
        }
    }
}
