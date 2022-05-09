using System;
using ActionCompiler.AST.Expr;
using ActionCompiler.AST.TypeNodes;

namespace ActionCompiler.AST.Statement
{
    public record ForeachStatementNode(TypeNode type, IdentifierNode identifier, 
        ExprNode iterable, StatementNode statement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitForeachStatement(this);
        }

        public virtual bool Equals(ForeachStatementNode? other)
        {
            return other is not null
                && type == other.type
                && identifier == other.identifier
                && iterable == other.iterable
                && statement == other.statement;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(type, identifier, iterable, statement);
        }
    }
}