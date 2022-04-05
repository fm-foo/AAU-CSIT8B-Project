using System.Collections.Generic;

namespace Action.AST {
    public record FunctionCallExprNode(ExprNode expr, List<ExprNode> funcArgs) : ExprNode {

        public override T Accept<T>(NodeVisitor<T> visitor) {
            return visitor.VisitFunctionCallExpr(this);
        }


    }
}
