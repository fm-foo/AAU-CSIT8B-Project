using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record BlockNode(IEnumerable<StatementNode> statements) : StatementNode
    {
        public virtual bool Equals(BlockNode? other)
        {
            if(other is null)
            {
                return false;
            }
            return statements.SequenceEqual(other.statements);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitBlock(this);
        }
    }
}