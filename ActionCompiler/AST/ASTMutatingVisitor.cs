using System;
using System.Diagnostics;
using System.Linq;

namespace Action.AST
{
    public abstract class ASTMutatingVisitor : NodeVisitor<SymbolNode>
    {
#if DEBUG
        static ASTMutatingVisitor()
        {
            Type type = typeof(ASTMutatingVisitor);
            var methods = type.GetMethods()
                .Where(m => m.IsVirtual)
                .Where(m => m.DeclaringType != type)
                .Where(m => m.Name is not ("ToString" or "Equals" or "GetHashCode"));
            if (methods.Any())
            {
                string str = methods.Select(m => m.Name).Aggregate((l, r) => $"{l}, {r}");
                Debug.Assert(false, $"Not every member implemented on ASTMutatingVisitor, missing: {str}");
            }
        }
#endif

        public override SymbolNode Default => throw new InvalidOperationException();

        public override AdditiveExprNode VisitAdditiveExpr(AdditiveExprNode additiveExprNode)
        {
            ExprNode left = (ExprNode)Visit(additiveExprNode.left);
            ExprNode right = (ExprNode)Visit(additiveExprNode.right);
            return new AdditiveExprNode(left, right, additiveExprNode.oper);
        }

        public override ArrayNode VisitArray(ArrayNode arrayNode)
        {
            return new ArrayNode(
                arrayNode.values.Select(Visit).Cast<ExprNode>().ToList()
            );
        }

        public override ArrayAccessNode VisitArrayAccess(ArrayAccessNode arrayAccessNode)
        {
            ExprNode arrayExpr = (ExprNode)Visit(arrayAccessNode.arrayExpr);
            ExprNode expr = (ExprNode)Visit(arrayAccessNode.expr);
            return new ArrayAccessNode(arrayExpr, expr);
        }

        /*public override ArrayTypeNode VisitArrayType(ArrayTypeNode arrayType)
        {
            return base.VisitArrayType(arrayType);
        }

        public override AssignmentNode VisitAssignment(AssignmentNode assignmentNode)
        {
            return base.VisitAssignment(assignmentNode);
        }

        public override BlockNode VisitBlock(BlockNode blockNode)
        {
            return base.VisitBlock(blockNode);
        }

        public override BoolNode VisitBool(BoolNode boolNode)
        {
            return base.VisitBool(boolNode);
        }

        public override BoolExprNode VisitBooleanExpr(BoolExprNode boolExprNode)
        {
            return base.VisitBooleanExpr(boolExprNode);
        }

        public override BoolTypeNode VisitBoolType(BoolTypeNode boolTypeNode)
        {
            return base.VisitBoolType(boolTypeNode);
        }

        public override BoundIdentifierNode VisitBoundIdentifier(BoundIdentifierNode identifierNode)
        {
            return base.VisitBoundIdentifier(identifierNode);
        }

        public override ColourNode VisitColour(ColourNode colourNode)
        {
            return base.VisitColour(colourNode);
        }

        public override ComplexNode VisitComplex(ComplexNode complexNode)
        {
            return base.VisitComplex(complexNode);
        }

        public override CoordinateNode VisitCoordinate(CoordinateNode coordinateNode)
        {
            return base.VisitCoordinate(coordinateNode);
        }

        public override CoordTypeNode VisitCoordType(CoordTypeNode coordTypeNode)
        {
            return base.VisitCoordType(coordTypeNode);
        }

        public override DeclarationNode VisitDeclaration(DeclarationNode declarationNode)
        {
            return base.VisitDeclaration(declarationNode);
        }

        public override EntityNode VisitEntity(EntityNode entityNode)
        {
            return base.VisitEntity(entityNode);
        }

        public override EqualityExprNode VisitEqualityExpr(EqualityExprNode equalityExprNode)
        {
            return base.VisitEqualityExpr(equalityExprNode);
        }

        public override ExpressionStatementNode VisitExpressionStatement(ExpressionStatementNode expressionStatementNode)
        {
            return base.VisitExpressionStatement(expressionStatementNode);
        }

        public override FieldDecNode VisitFieldDeclaration(FieldDecNode fieldNode)
        {
            return base.VisitFieldDeclaration(fieldNode);
        }

        public override FileNode VisitFile(FileNode file)
        {
            return base.VisitFile(file);
        }

        public override FloatNode VisitFloat(FloatNode floatNode)
        {
            return base.VisitFloat(floatNode);
        }

        public override FloatTypeNode VisitFloatType(FloatTypeNode floatTypeNode)
        {
            return base.VisitFloatType(floatTypeNode);
        }

        public override ForeachStatementNode VisitForeachStatement(ForeachStatementNode foreachStatementNode)
        {
            return base.VisitForeachStatement(foreachStatementNode);
        }

        public override ForStatementNode VisitForStatement(ForStatementNode forStatementNode)
        {
            return base.VisitForStatement(forStatementNode);
        }

        public override FunctionNode VisitFunction(FunctionNode functionNode)
        {
            return base.VisitFunction(functionNode);
        }

        public override FunctionArgumentNode VisitFunctionArgument(FunctionArgumentNode functionArgumentNode)
        {
            return base.VisitFunctionArgument(functionArgumentNode);
        }

        public override FunctionCallExprNode VisitFunctionCallExpr(FunctionCallExprNode funcCallExprNode)
        {
            return base.VisitFunctionCallExpr(funcCallExprNode);
        }

        public override GameNode VisitGame(GameNode gameNode)
        {
            return base.VisitGame(gameNode);
        }

        public override IdentifierNode VisitIdentifier(IdentifierNode identifierNode)
        {
            return base.VisitIdentifier(identifierNode);
        }

        public override IfStatementNode VisitIfStatement(IfStatementNode ifStatementNode)
        {
            return base.VisitIfStatement(ifStatementNode);
        }

        public override IntNode VisitInt(IntNode intNode)
        {
            return base.VisitInt(intNode);
        }

        public override IntTypeNode VisitIntType(IntTypeNode intTypeNode)
        {
            return base.VisitIntType(intTypeNode);
        }

        public override IsNode VisitIs(IsNode isexpr)
        {
            return base.VisitIs(isexpr);
        }

        public override KeywordNode VisitKeyword(KeywordNode keywordNode)
        {
            return base.VisitKeyword(keywordNode);
        }

        public override MapNode VisitMap(MapNode mapNode)
        {
            return base.VisitMap(mapNode);
        }

        public override MemberAccessNode VisitMemberAccess(MemberAccessNode memberAccessNode)
        {
            return base.VisitMemberAccess(memberAccessNode);
        }

        public override MultiplicativeExprNode VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode)
        {
            return base.VisitMultiplicativeExprNode(multiplicativeExprNode);
        }

        public override NatNumNode VisitNatNum(NatNumNode natNumNode)
        {
            return base.VisitNatNum(natNumNode);
        }

        public override NewObjectNode VisitNewObject(NewObjectNode newObjectExprNode)
        {
            return base.VisitNewObject(newObjectExprNode);
        }

        public override PropertyNode VisitProperty(PropertyNode propertyNode)
        {
            return base.VisitProperty(propertyNode);
        }

        public override ReferenceNode VisitReference(ReferenceNode referenceNode)
        {
            return base.VisitReference(referenceNode);
        }

        public override RelationalExprNode VisitRelationalExpr(RelationalExprNode relationalExprNode)
        {
            return base.VisitRelationalExpr(relationalExprNode);
        }

        public override SectionNode VisitSection(SectionNode sectionNode)
        {
            return base.VisitSection(sectionNode);
        }

        public override SimpleTypeNode VisitSimpleType(SimpleTypeNode simpleTypeNode)
        {
            return base.VisitSimpleType(simpleTypeNode);
        }

        public override StringNode VisitString(StringNode stringNode)
        {
            return base.VisitString(stringNode);
        }

        public override StringTypeNode VisitStringType(StringTypeNode stringTypeNode)
        {
            return base.VisitStringType(stringTypeNode);
        }

        public override UnaryExprNode VisitUnaryExpr(UnaryExprNode unaryExprNode)
        {
            return base.VisitUnaryExpr(unaryExprNode);
        }

        public override WhileStatementNode VisitWhileStatement(WhileStatementNode whileStatementNode)
        {
            return base.VisitWhileStatement(whileStatementNode);
        }

        public override PostFixExprNode VisitPostFixExpr(PostFixExprNode postFixExprNode)
        {
            return base.VisitPostFixExpr(postFixExprNode);
        }*/
    }
}