namespace Action.AST {
    public record MemberAccessNode(ExprNode expr, IdentifierNode Identifier) : ExprNode {
        public override T Accept<T>(NodeVisitor<T> visitor) {
            return visitor.VisitMemberAccess(this);
        }
    }
}
