using System;
using System.Collections.Generic;

namespace Action.AST
{
    public class NodeVisitor<T>
    {
        public virtual T Default => throw new NotImplementedException();
        public T Visit(SymbolNode node) => node.Accept(this);
        public virtual T VisitFile(FileNode file) => Default;
        public virtual T VisitFunctionCallExpr(FunctionCallExprNode funcCallExprNode) => Default;
        public virtual T VistPostFixExpr(PostFixExprNode postFixExprNode) => Default;
        public virtual T VisitAddativeExpr(AdditiveExprNode additiveExprNode) => Default;
        public virtual T VisitComplex(ComplexNode complexNode) => Default;
        public virtual T VisitMap(MapNode mapNode) => Default;
        public virtual T VisitSection(SectionNode sectionNode) => Default;
        public virtual T VisitColour(ColourNode colourNode) => Default;
        public virtual T VisitCoordinate(CoordinateNode coordinateNode) => Default;
        public virtual T VisitIdentifier(IdentifierNode identifierNode) => Default;
        public virtual T VisitInt(IntNode intNode) => Default;
        public virtual T VisitFloat(FloatNode floatNode) => Default;
        public virtual T VisitNatNum(NatNumNode natNumNode) => Default;
        public virtual T VisitProperty(PropertyNode propertyNode) => Default;
        public virtual T VisitReference(ReferenceNode referenceNode) => Default;
        public virtual T VisitString(StringNode stringNode) => Default;
        public virtual T VisitEntity(EntityNode entityNode) => Default;
        public virtual T VisitGame(GameNode gameNode) => Default;
        public virtual T VisitFieldDeclaration(FieldDecNode fieldNode) => Default;
        public virtual T VisitExpr(ExprNode exprNode) => Default;
        public virtual T VisitEqualityExpr(EqualityExprNode equalityExprNode) => Default;
        public virtual T VisitRelationalExpr(RelationalExprNode relationalExprNode) => Default;
        public virtual T VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode) => Default;
        public virtual T VisitUnaryExpr(UnaryExprNode unaryExprNode) => Default;
        public virtual T VisitBooleanExpr(BoolExprNode boolExprNode) => Default;
        public virtual T VisitPrimaryExpr(PrimaryExprNode primaryExprNode) => Default;
        public virtual T VisitArrayAccess(ArrayAccessNode arrayAccessNode) => Default;
        public virtual T VisitBool(BoolNode boolNode) => Default;
        public virtual T VisitArray(ArrayNode arrayNode) => Default;
        public virtual T VisitType(TypeNode typeNode) => Default;
        public virtual T VisitIntType(IntTypeNode intTypeNode) => Default;
        public virtual T VisitFloatType(FloatTypeNode floatTypeNode) => Default;
        public virtual T VisitBoolType(BoolTypeNode boolTypeNode) => Default;
        public virtual T VisitCoordType(CoordTypeNode coordTypeNode) => Default;
        public virtual T VisitStringType(StringTypeNode stringTypeNode) => Default;
        public virtual T VisitSimpleType(SimpleTypeNode simpleTypeNode) => Default;
        public virtual T VisitArrayType(ArrayTypeNode arrayType) => Default;
        public virtual T VisitFunction(FunctionNode functionNode) => Default;
        public virtual T VisitBlock(BlockNode blockNode) => Default;
        public virtual T VisitStatement(StatementNode statementNode) => Default;
        public virtual T VisitIfStatement(IfStatementNode ifStatementNode) => Default;
        public virtual T VisitWhileStatement(WhileStatementNode whileStatementNode) => Default;
        public virtual T VisitForStatement(ForStatementNode forStatementNode) => Default;
        public virtual T VisitForeachStatement(ForeachStatementNode foreachStatementNode) => Default;
        public virtual T VisitExpressionStatement(ExpressionStatementNode expressionStatementNode) => Default;
        public virtual T VisitDeclaration(DeclarationNode declarationNode) => Default;
        public virtual T VisitAssignment(AssignmentNode assignmentNode) => Default;
        public virtual T VisitFunctionArguments(FunctionArgumentsNode functionArgumentsNode) => Default;
        public virtual T VisitFunctionArgument(FunctionArgumentNode functionArgumentNode) => Default;
        public virtual T VisitIs(IsNode isexpr) => Default;
    }
}