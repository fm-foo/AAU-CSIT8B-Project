using ActionCompiler.AST.Expr;

namespace ActionCompiler.AST.Statement
{
    public record IfStatementNode(ExprNode test, StatementNode primaryStatement, StatementNode? elseStatement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIfStatement(this);
        }
    }
}