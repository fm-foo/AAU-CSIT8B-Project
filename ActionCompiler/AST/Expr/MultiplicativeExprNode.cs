namespace ActionCompiler.AST.Expr
{
    public record MultiplicativeExprNode(ExprNode left, ExprNode right, MultOper oper) : ExprNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitMultiplicativeExprNode(this);
        }
    }

    public enum MultOper
    {
        TIMES,
        DIV
    }
}
