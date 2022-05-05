namespace ActionCompiler.AST.Expr
{
    public record EqualityExprNode(ExprNode left, ExprNode right, EqualityOperator oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitEqualityExpr(this);
        }
    }

    public enum EqualityOperator
    {
        EQUALS,
        NOTEQUALS
    }
}
