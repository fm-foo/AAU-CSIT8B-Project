﻿using System;
using ActionCompiler.AST.Expr;

namespace ActionCompiler.AST.Statement
{
    public record WhileStatementNode(ExprNode expr, StatementNode statement) : StatementNode
    {
        public virtual bool Equals(WhileStatementNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return expr.Equals(other.expr) && statement.Equals(other.statement);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(expr, statement);
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }
}