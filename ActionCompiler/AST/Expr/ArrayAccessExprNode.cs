namespace Action.AST {
    public record ArrayAccessExprNode(ExprNode expr, ExprNode exprBody) : ExprNode {
        public override T Accept<T>(NodeVisitor<T> visitor) {
            return visitor.VisitArrayAccessExpr(this);
        }
    }
}
