using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.AST.Expr
{
    public record NewObjectNode(IdentifierNode identifier, IEnumerable<ExprNode> funcArgs) : ExprNode
    {

        public virtual bool Equals(NewObjectNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return identifier.Equals(other.identifier) && funcArgs.SequenceEqual(other.funcArgs);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitNewObject(this);
        }
    }
}
