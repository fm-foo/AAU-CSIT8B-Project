using System;
using ActionCompiler.AST.Expr;

namespace ActionCompiler.AST.Statement
{
    public record ForStatementNode(StatementNode? initialization, ExprNode? condition, ExprNode? control, StatementNode statement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitForStatement(this);
        }

        public virtual bool Equals(ForStatementNode? other)
        {
            return other is not null
                && initialization == other.initialization
                && condition == other.condition
                && control == other.control
                && statement == other.statement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(initialization, condition, control, statement);
        }
    }
}