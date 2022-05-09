using System;
using ActionCompiler.AST.Expr;

namespace ActionCompiler.AST.Statement
{
    public record AssignmentNode(ExprNode leftSide, ExprNode rightSide) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }

        public virtual bool Equals(AssignmentNode? other)
        {
            return other is not null
                && leftSide == other.leftSide
                && rightSide == other.rightSide;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(leftSide, rightSide);
        }
    }
}