namespace Action.AST
{
    public record ColourNode(byte r, byte g, byte b) : ValueNode
    {
        public virtual bool Equals(ColourNode? other)
        {
            if (other is null)
            {
                return false;
            }
            return r == other.r && g == other.g && b == other.b;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitColour(this);
        }
    }
}