using ActionCompiler.AST.Expr;

namespace ActionCompiler.AST.Statement
{
    public record ForStatementNode(StatementNode? initialization, ExprNode? condition, ExprNode? control, StatementNode statement) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitForStatement(this);
        }
    }
}