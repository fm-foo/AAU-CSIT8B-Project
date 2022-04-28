namespace Action.AST
{
    public record ExpressionStatementNode(ExprNode expr) : StatementNode
    {
        public virtual bool Equals(ExpressionStatementNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return expr.Equals(other.expr);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }
    }

}
