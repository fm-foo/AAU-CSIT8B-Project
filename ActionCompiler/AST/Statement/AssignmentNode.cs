namespace Action.AST
{
    public record AssignmentNode(ExprNode leftSide, ExprNode rightSide) : StatementNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitAssignment(this);
        }
    }
}