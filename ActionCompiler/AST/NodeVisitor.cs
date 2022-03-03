using System;

namespace Action.AST
{
    public class NodeVisitor<T>
    {
        public virtual T Default => default;

        public T Visit(ValueNode node)
        {
            return node.Accept(this);
        }

        public virtual T VisitComplex(ComplexNode complexNode)
        {
            return Default;
        }

        public virtual T VisitMap(MapNode mapNode)
        {
            return Default;
        }

        public virtual T VisitSection(SectionNode sectionNode)
        {
            return Default;
        }

        public virtual T VisitColour(ColourNode colourNode)
        {
            return Default;
        }

        public virtual T VisitCoordinate(CoordinateNode coordinateNode)
        {
            return Default;
        }

        public virtual T VisitIdentifier(IdentifierNode identifierNode)
        {
            return Default;
        }

        public virtual T VisitInt(IntNode intNode)
        {
            return Default;
        }

        public virtual T VisitNatNum(NatNumNode natNumNode)
        {
            return Default;
        }

        public virtual T VisitProperty(PropertyNode propertyNode)
        {
            return Default;
        }

        public virtual T VisitReference(ReferenceNode referenceNode)
        {
            return Default;
        }

        public virtual T VisitString(StringNode stringNode)
        {
            return Default;
        }
    }
}