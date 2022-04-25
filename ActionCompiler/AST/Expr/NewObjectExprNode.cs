namespace Action.AST {
    using System.Collections.Generic;

    public record NewObjectExprNode(IdentifierNode identifier, List<ExprNode> funcArgs) : ExprNode {
        public override T Accept<T>(NodeVisitor<T> visitor) {
            return visitor.VisitNewObjectExpr(this);
        }
    }
}
