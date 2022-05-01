namespace Action.AST {
    public record MemberAccessNode(ExprNode expr, IdentifierNode identifier) : ExprNode {
        public override T Accept<T>(NodeVisitor<T> visitor) {
            return visitor.VisitMemberAccess(this);
        }
    }
}
