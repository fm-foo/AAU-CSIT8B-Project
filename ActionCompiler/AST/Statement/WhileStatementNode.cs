namespace Action.AST
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
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitWhileStatement(this);
        }
    }
}