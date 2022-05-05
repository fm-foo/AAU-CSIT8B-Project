using ActionCompiler.AST.Expr;
using ActionCompiler.AST.TypeNodes;

namespace ActionCompiler.AST.Statement
{
    public record ForeachStatementNode(TypeNode type, IdentifierNode identifier, ExprNode iterable, StatementNode statement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitForeachStatement(this);
        }
    }
}