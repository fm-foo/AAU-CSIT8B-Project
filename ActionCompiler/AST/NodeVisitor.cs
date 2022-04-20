using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Action.Compiler;

namespace Action.AST
{
    public abstract class DiagnosticsVisitor : AutomaticNodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> Default => Array.Empty<DiagnosticResult>();
        public override IEnumerable<DiagnosticResult> MergeValues(IEnumerable<DiagnosticResult> oldValue, IEnumerable<DiagnosticResult> newValue)
        {
            return (oldValue, newValue) switch
            {
                // both are empty arrays (Default)
                (Array { Length: 0 }, Array { Length: 0 }) => Default,
                // oldValue is empty array 
                (Array { Length: 0 }, _) => newValue,
                // newValue is empty array
                (_, Array { Length: 0 }) => oldValue,
                // neither is empty array
                _ => Enumerable.Concat(oldValue, newValue) 
            };
        }
    }

    public abstract class AutomaticNodeVisitor<T> : NodeVisitor<T>
    {
#if DEBUG
        static AutomaticNodeVisitor()
        {
            Type type = typeof(AutomaticNodeVisitor<T>);
            Debug.Assert(type.GetMethods(BindingFlags.Instance)
                .Where(m => m.IsVirtual)
                .Where(m => m.DeclaringType != type)
                .Any() == false, "Not every member implemented on AutomaticNodeVisitor");  
        }
#endif

        public override T Default => default;
        public override T VisitFile(FileNode file)
        {
            bool valueSet = false;
            T value = Default;
            foreach (var node in file.nodes)
            {
                if (!valueSet)
                {
                    value = Visit(node);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(node));
                }
            }
            return value;
        }
        public override T VisitFunctionCallExpr(FunctionCallExprNode funcCallExprNode)
        {
            T value = Visit(funcCallExprNode.expr);
            foreach (var node in funcCallExprNode.funcArgs)
                value = MergeValues(value, Visit(node));
            return value;
        }
        public override T VistPostFixExpr(PostFixExprNode postFixExprNode) => Visit(postFixExprNode.expr);
        public override T VisitAdditiveExpr(AdditiveExprNode additiveExprNode) => MergeValues(Visit(additiveExprNode.left), Visit(additiveExprNode.right));
        public override T VisitComplex(ComplexNode complexNode)
        {
            T value = Visit(complexNode.type);
            foreach (var property in complexNode.properties)
                value = MergeValues(value, Visit(property));
            foreach (var nodeValue in complexNode.values)
                value = MergeValues(value, Visit(nodeValue));
            return value;
        }
        public override T VisitMap(MapNode mapNode)
        {
            T value = Visit(mapNode.identifier);
            foreach (var property in mapNode.properties)
                value = MergeValues(value, Visit(property));
            foreach (var nodeValue in mapNode.sections)
                value = MergeValues(value, Visit(nodeValue));
            return value;
        }
        public override T VisitSection(SectionNode sectionNode)
        {
            bool valueSet = false;
            T value = Default;
            if (sectionNode.coords is not null)
            {
                value = Visit(sectionNode.coords);
                valueSet = true;
            }
            if (sectionNode.identifier is not null)
            {
                if (!valueSet)
                {
                    value = Visit(sectionNode.identifier);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(sectionNode.identifier));
                }
            }
            foreach (var property in sectionNode.properties)
            {
                if (!valueSet)
                {
                    value = Visit(property);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(property));
                }
            }
            foreach (var nodeValue in sectionNode.sections)
            {
                if (!valueSet)
                {
                    value = Visit(nodeValue);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(nodeValue));
                }
            }
            return value;
        }
        public override T VisitColour(ColourNode colourNode) => Default;
        public override T VisitCoordinate(CoordinateNode coordinateNode) => MergeValues(Visit(coordinateNode.x), Visit(coordinateNode.y));
        public override T VisitIdentifier(IdentifierNode identifierNode) => Default;
        public override T VisitInt(IntNode intNode) => Default;
        public override T VisitFloat(FloatNode floatNode) => Default;
        public override T VisitNatNum(NatNumNode natNumNode) => Default;
        public override T VisitProperty(PropertyNode propertyNode)
        {
            T value = Visit(propertyNode.identifier);
            if (propertyNode.value is not null)
            {
                value = MergeValues(value, Visit(propertyNode.value));
            }
            return value;
        }
        public override T VisitReference(ReferenceNode referenceNode)
        {
            T value = Visit(referenceNode.referenceType);
            value = MergeValues(value, Visit(referenceNode.reference));
            value = MergeValues(value, Visit(referenceNode.coords));
            return value;
        }
        public override T VisitString(StringNode stringNode) => Default;
        public override T VisitEntity(EntityNode entityNode)
        {
            T value = Visit(entityNode.identifier);
            foreach (var field in entityNode.fieldDecs)
                value = MergeValues(value, Visit(field));
            foreach (var funcs in entityNode.funcDecs)
                value = MergeValues(value, Visit(funcs));
            return value;
        }
        public override T VisitGame(GameNode gameNode)
        {
            T value = Visit(gameNode.identifier);
            foreach (var field in gameNode.fieldDecs)
                value = MergeValues(value, Visit(field));
            foreach (var funcs in gameNode.funcDecs)
                value = MergeValues(value, Visit(funcs));
            return value;
        }
        public override T VisitFieldDeclaration(FieldDecNode fieldNode)
        {
            T value = Visit(fieldNode.identifier);
            value = MergeValues(value, Visit(fieldNode.type));
            if (fieldNode.expr is not null)
                value = MergeValues(value, Visit(fieldNode.expr));
            return value;
        }
        public override T VisitEqualityExpr(EqualityExprNode equalityExprNode) => MergeValues(Visit(equalityExprNode.left), Visit(equalityExprNode.right));
        public override T VisitRelationalExpr(RelationalExprNode relationalExprNode) => MergeValues(Visit(relationalExprNode.left), Visit(relationalExprNode.right));
        public override T VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode) => MergeValues(Visit(multiplicativeExprNode.left), Visit(multiplicativeExprNode.right));
        public override T VisitUnaryExpr(UnaryExprNode unaryExprNode) => Visit(unaryExprNode.primaryExpr);
        public override T VisitBooleanExpr(BoolExprNode boolExprNode) => MergeValues(Visit(boolExprNode.left), Visit(boolExprNode.right));
        // todo: expand when the last nodes get added
        public override T VisitArrayAccess(ArrayAccessNode arrayAccessNode) => MergeValues(Visit(arrayAccessNode.arrayExpr), Visit(arrayAccessNode.expr));
        public override T VisitBool(BoolNode boolNode) => Default;
        public override T VisitArray(ArrayNode arrayNode)
        {
            bool valueSet = false;
            T value = Default;
            foreach (var node in arrayNode.values)
            {
                if (!valueSet)
                {
                    value = Visit(node);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(node));
                }
            }
            return value;
        }
        public override T VisitIntType(IntTypeNode intTypeNode) => Default;
        public override T VisitFloatType(FloatTypeNode floatTypeNode) => Default;
        public override T VisitBoolType(BoolTypeNode boolTypeNode) => Default;
        public override T VisitCoordType(CoordTypeNode coordTypeNode) => Default;
        public override T VisitStringType(StringTypeNode stringTypeNode) => Default;
        public override T VisitSimpleType(SimpleTypeNode simpleTypeNode) => Visit(simpleTypeNode.identifier);
        public override T VisitArrayType(ArrayTypeNode arrayType) => Visit(arrayType.type);
        public override T VisitFunction(FunctionNode functionNode)
        {
            bool valueSet = false;
            T value = Default;
            foreach (var args in functionNode.args)
            {
                if (!valueSet)
                {
                    value = Visit(args);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(args));
                }
            }
            if (!valueSet)
            {
                value = Visit(functionNode.block);
                valueSet = true;
            }
            else
            {
                value = MergeValues(value, Visit(functionNode.block));
            }
            return value;
        }
        public override T VisitBlock(BlockNode blockNode)
        {
            bool valueSet = false;
            T value = Default;
            foreach (var statement in blockNode.statements)
            {
                if (!valueSet)
                {
                    value = Visit(statement);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(statement));
                }
            }
            return value;
        }
        public override T VisitIfStatement(IfStatementNode ifStatementNode)
        {
            T value = Visit(ifStatementNode.test);
            value = MergeValues(value, Visit(ifStatementNode.primaryStatement));
            if (ifStatementNode.elseStatement is not null)
                value = MergeValues(value, Visit(ifStatementNode.elseStatement));
            return value;
        }
        public override T VisitWhileStatement(WhileStatementNode whileStatementNode) => MergeValues(Visit(whileStatementNode.expr), Visit(whileStatementNode.statement));
        public override T VisitForStatement(ForStatementNode forStatementNode)
        {
            bool valueSet = false;
            T value = Default;
            if (forStatementNode.initialization is not null)
            {
                value = Visit(forStatementNode.initialization);
                valueSet = true;
            }
            if (forStatementNode.condition is not null)
            {
                if (!valueSet)
                {
                    value = Visit(forStatementNode.condition);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(forStatementNode.condition));
                }
            }
            if (forStatementNode.control is not null)
            {
                if (!valueSet)
                {
                    value = Visit(forStatementNode.control);
                    valueSet = true;
                }
                else
                {
                    value = MergeValues(value, Visit(forStatementNode.control));
                }
            }
            if (!valueSet)
            {
                value = Visit(forStatementNode.statement);
                valueSet = true;
            }
            else
            {
                value = MergeValues(value, Visit(forStatementNode.statement));
            }
            return value;
        }
        public override T VisitForeachStatement(ForeachStatementNode foreachStatementNode)
        {
            T value = Visit(foreachStatementNode.type);
            value = MergeValues(value, Visit(foreachStatementNode.identifier));
            value = MergeValues(value, Visit(foreachStatementNode.iterable));
            value = MergeValues(value, Visit(foreachStatementNode.statement));
            return value;
        }
        public override T VisitExpressionStatement(ExpressionStatementNode expressionStatementNode) => Visit(expressionStatementNode.expr);
        public override T VisitDeclaration(DeclarationNode declarationNode)
        {
            T value = Visit(declarationNode.type);
            value = MergeValues(value, Visit(declarationNode.identifier));
            if (declarationNode.expr is not null)
            {
                value = MergeValues(value, Visit(declarationNode.expr));
            }
            return value;
        }
        public override T VisitAssignment(AssignmentNode assignmentNode) => MergeValues(Visit(assignmentNode.leftSide), Visit(assignmentNode.rightSide));
        public override T VisitFunctionArgument(FunctionArgumentNode functionArgumentNode) => MergeValues(Visit(functionArgumentNode.identifier), Visit(functionArgumentNode.typeNode));
        public override T VisitIs(IsNode isexpr) => MergeValues(Visit(isexpr.expr), Visit(isexpr.type));
        public abstract T MergeValues(T oldValue, T newValue);
    }

    public class NodeVisitor<T>
    {
        public virtual T Default => throw new NotImplementedException();
        public T Visit(SymbolNode node) => node.Accept(this);
        public virtual T VisitFile(FileNode file) => Default;
        public virtual T VisitFunctionCallExpr(FunctionCallExprNode funcCallExprNode) => Default;
        public virtual T VistPostFixExpr(PostFixExprNode postFixExprNode) => Default;
        public virtual T VisitAdditiveExpr(AdditiveExprNode additiveExprNode) => Default;
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
        public virtual T VisitEqualityExpr(EqualityExprNode equalityExprNode) => Default;
        public virtual T VisitRelationalExpr(RelationalExprNode relationalExprNode) => Default;
        public virtual T VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode) => Default;
        public virtual T VisitUnaryExpr(UnaryExprNode unaryExprNode) => Default;
        public virtual T VisitBooleanExpr(BoolExprNode boolExprNode) => Default;
        public virtual T VisitArrayAccess(ArrayAccessNode arrayAccessNode) => Default;
        public virtual T VisitBool(BoolNode boolNode) => Default;
        public virtual T VisitArray(ArrayNode arrayNode) => Default;
        public virtual T VisitIntType(IntTypeNode intTypeNode) => Default;
        public virtual T VisitFloatType(FloatTypeNode floatTypeNode) => Default;
        public virtual T VisitBoolType(BoolTypeNode boolTypeNode) => Default;
        public virtual T VisitCoordType(CoordTypeNode coordTypeNode) => Default;
        public virtual T VisitStringType(StringTypeNode stringTypeNode) => Default;
        public virtual T VisitSimpleType(SimpleTypeNode simpleTypeNode) => Default;
        public virtual T VisitArrayType(ArrayTypeNode arrayType) => Default;
        public virtual T VisitFunction(FunctionNode functionNode) => Default;
        public virtual T VisitBlock(BlockNode blockNode) => Default;
        public virtual T VisitIfStatement(IfStatementNode ifStatementNode) => Default;
        public virtual T VisitWhileStatement(WhileStatementNode whileStatementNode) => Default;
        public virtual T VisitForStatement(ForStatementNode forStatementNode) => Default;
        public virtual T VisitForeachStatement(ForeachStatementNode foreachStatementNode) => Default;
        public virtual T VisitExpressionStatement(ExpressionStatementNode expressionStatementNode) => Default;
        public virtual T VisitDeclaration(DeclarationNode declarationNode) => Default;
        public virtual T VisitAssignment(AssignmentNode assignmentNode) => Default;
        public virtual T VisitFunctionArgument(FunctionArgumentNode functionArgumentNode) => Default;
        public virtual T VisitIs(IsNode isexpr) => Default;
    }
}