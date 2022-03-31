namespace Action.AST
{
    public record ArrayAccessNode(ExprNode arrayExpr, ExprNode expr) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitArrayAccess(this);
        }
    }

}
