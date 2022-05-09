using System;
using ActionCompiler.AST.Expr;

namespace ActionCompiler.AST.Statement
{
    public record IfStatementNode(ExprNode test, StatementNode primaryStatement, StatementNode? elseStatement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }

        public virtual bool Equals(IfStatementNode? other)
        {
            return other is not null
                && test == other.test
                && primaryStatement == other.primaryStatement
                && elseStatement == other.elseStatement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(test, primaryStatement, elseStatement);
        }
    }
}