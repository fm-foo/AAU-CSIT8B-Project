namespace Action.AST
{
    public record IsNode(ExprNode expr, TypeNode type) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitIs(this);
        }
    }
}
