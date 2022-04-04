namespace Action.AST
{

    // TODO: save value as a stirng (to enable error checking) and have a property to convert to float
    public record FloatNode(float f) : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }
    }
}