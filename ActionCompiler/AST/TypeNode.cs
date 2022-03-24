namespace Action.AST
{
    public record TypeNode(TypeEnum type) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitType(this);
        }
    }

    public enum TypeEnum
    {
        INT,
        FLOAT,
        STRING,
        BOOL,
        COORD,
        IDENTIFIER
    }
}