using System;

namespace Action.AST
{
    public class NodeVisitor<T>
    {
        public virtual T Default => default;
        public T Visit(ValueNode node) => node.Accept(this);
        public virtual T VisitComplex(ComplexNode complexNode) => Default;
        public virtual T VisitMap(MapNode mapNode) => Default;
        public virtual T VisitSection(SectionNode sectionNode) => Default;
        public virtual T VisitColour(ColourNode colourNode) => Default;
        public virtual T VisitCoordinate(CoordinateNode coordinateNode) => Default;
        public virtual T VisitIdentifier(IdentifierNode identifierNode) => Default;
        public virtual T VisitInt(IntNode intNode) => Default;
        public virtual T VisitNatNum(NatNumNode natNumNode) => Default;
        public virtual T VisitProperty(PropertyNode propertyNode) => Default;
        public virtual T VisitReference(ReferenceNode referenceNode) => Default;
        public virtual T VisitString(StringNode stringNode) => Default;
    }
}