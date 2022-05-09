using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST.Statement
{
    public record BlockNode(IEnumerable<StatementNode> statements) : StatementNode
    {
        public virtual bool Equals(BlockNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return statements.SequenceEqual(other.statements);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            foreach (var statement in statements)
                hc.Add(statement);
            return hc.ToHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}