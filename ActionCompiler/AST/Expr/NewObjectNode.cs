namespace Action.AST {
    using System.Collections.Generic;

    public record NewObjectNode(IdentifierNode identifier, IEnumerable<ExprNode> funcArgs) : ExprNode {
        public override T Accept<T>(NodeVisitor<T> visitor) {
            return visitor.VisitNewObject(this);
        }
    }
}
