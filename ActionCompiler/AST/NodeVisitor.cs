using System;
using System.Collections.Generic;

namespace Action.AST
{
    public class NodeVisitor<T>
    {
        public virtual T Default => default;
        public T Visit(SymbolNode node) => node.Accept(this);
        public virtual T VisitFile(FileNode file) => Default;
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
        public virtual T VisitEntity(EntityNode entityNode) => Default;
        public virtual T VisitField(FieldNode fieldNode) => Default;
        public virtual T VisitExpr(ExprNode exprNode) => Default;
        public virtual T VisitEqualityExpr(EqualityExprNode equalityExprNode) => Default;
        public virtual T VisitRelationalExpr(RelationalExprNode relationalExprNode) => Default;
        public virtual T VisitAddativeExpr(AdditiveExprNode addativeExprNode) => Default;
        public virtual T VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode) => Default;
        public virtual T VisitUnaryExpr(UnaryExprNode unaryExprNode) => Default;
        public virtual T VisitBooleanExpr(BoolExprNode boolExprNode) => Default;
        public virtual T VisitPrimaryExpr(PrimaryExprNode primaryExprNode) => Default;
        public virtual T VisitBool(BoolNode boolNode) => Default;
        public virtual T VisitType(TypeNode typeNode) => Default;
    }
}