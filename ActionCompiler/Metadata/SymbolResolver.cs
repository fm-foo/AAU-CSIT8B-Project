using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ActionCompiler.AST;
using ActionCompiler.AST.Bindings;
using ActionCompiler.AST.Expr;
using ActionCompiler.AST.Statement;
using ActionCompiler.AST.TypeNodes;
using ActionCompiler.AST.Types;
using ActionCompiler.Compiler;

namespace ActionCompiler.Metadata
{

    public class InternalSymbolResolver : ISymbolResolver
    {
        public FileNode Bind(FileNode node)
        {
            FunctionBinder binder = new FunctionBinder();
            node = (FileNode)binder.Visit(node);
            IdentifierBinder id = new IdentifierBinder();
            node = (FileNode)id.Visit(node);
            return node;
        }

        private class FunctionBinder : ASTMutatingVisitor
        {
            public override EntityNode VisitEntity(EntityNode entity)
            {
                var bindings = GetBindings(entity.fieldDecs).ToList();
                var functions = BindFunctions(entity.funcDecs, bindings).ToList();
                var fields = BindFields(entity.fieldDecs, bindings);
                return entity with { funcDecs = functions, fieldDecs = fields };
            }

            public override GameNode VisitGame(GameNode game)
            {
                var bindings = GetBindings(game.fieldDecs).ToList();
                var functions = BindFunctions(game.funcDecs, bindings).ToList();
                var fields = BindFields(game.fieldDecs, bindings);
                return game with { funcDecs = functions, fieldDecs = fields };
            }

            private static IEnumerable<Binding> GetBindings(IEnumerable<FieldDecNode> fields)
            {
                return fields.Select(f => new Binding(f.identifier, f.type));
            }

            private static IEnumerable<PropertyNode> BindFunctions(IEnumerable<PropertyNode> nodes, IEnumerable<Binding> bindings)
            {
                foreach (var node in nodes)
                {
                    Debug.Assert(node.value is not null);
                    Debug.Assert(node.value.GetType() == typeof(FunctionNode));
                    var func = (FunctionNode)node.value;
                    yield return node with { value = new BoundFunctionNode(func, bindings) };
                }
            }

            private static IEnumerable<FieldDecNode> BindFields(IEnumerable<FieldDecNode> nodes, IEnumerable<Binding> bindings)
            {
                foreach (var node in nodes)
                {
                    var bind = bindings.Single(b => b.identifier == node.identifier);
                    yield return new BoundFieldDecNode(node, bind);
                }
            }
        }

        private class IdentifierBinder : ASTMutatingVisitor
        {
            private Stack<HashSet<Binding>> bindings = new Stack<HashSet<Binding>>();
            private IEnumerable<Binding> AllBindings => bindings.SelectMany(b => b);
            public override FunctionNode VisitFunction(FunctionNode node)
            {
                var bound = (BoundFunctionNode)node;
                var set = new HashSet<Binding>(bound.bindings);
                bindings.Push(set);
                var result = (FunctionNode)base.VisitFunction(node);
                bindings.Pop();
                Debug.Assert(!bindings.Any());
                return result;
            }

            public override BlockNode VisitBlock(BlockNode node)
            {
                var set = new HashSet<Binding>();
                bindings.Push(set);
                var result = (BlockNode)base.VisitBlock(node);
                bindings.Pop();
                return result;
            }

            public override DeclarationNode VisitDeclaration(DeclarationNode node)
            {
                var binding = new Binding(node.identifier, node.type);
                var set = bindings.Peek();
                set.Add(binding);
                if (node.expr is not null)
                {
                    var expr = (ExprNode)base.Visit(node.expr);
                    node = node with { expr = expr };
                }
                return new BoundDeclarationNode(node, binding);
            }

            public override MemberAccessNode VisitMemberAccess(MemberAccessNode memberAccessNode)
            {
                // don't visit the identifier - it can't be bound in this step
                ExprNode expr = (ExprNode)Visit(memberAccessNode.expr);
                return memberAccessNode with { expr = expr };
            }

            public override ValueNode VisitIdentifier(IdentifierNode node)
            {
                if (node.Parent is not ExprNode)
                {
                    return node;
                }
                var binding = AllBindings.SingleOrDefault(b => b.identifier == node);
                return binding is null
                    ? node
                    : new BoundIdentifierNode(binding);
            }
        }
    }

    public class MetadataSymbolResolver : ISymbolResolver
    {
        // todo: this
        public FileNode Bind(FileNode node)
        {
            return node;
        }
    }

    public class CombinedMetadataResolver : ISymbolResolver
    {
        private readonly List<ISymbolResolver> resolvers;
        public CombinedMetadataResolver(params ISymbolResolver[] resolvers)
        {
            this.resolvers = resolvers.ToList();
        }

        private CombinedMetadataResolver(List<ISymbolResolver> resolvers, ISymbolResolver resolver)
        {
            this.resolvers = new List<ISymbolResolver>(resolvers)
            {
                resolver
            };
        }

        public FileNode Bind(FileNode node)
        {
            foreach (var resolver in resolvers)
                node = resolver.Bind(node);
            return node;
        }

        public ISymbolResolver Add(ISymbolResolver resolver) => new CombinedMetadataResolver(resolvers, resolver);
    }

    public static class MetadataUtilities
    {
        public static ISymbolResolver Chain(this ISymbolResolver left, ISymbolResolver right)
        {
            if (left is CombinedMetadataResolver cmbl)
            {
                return cmbl.Add(right);
            }
            if (right is CombinedMetadataResolver cmbr)
            { 
                return cmbr.Add(left);
            }
            return new CombinedMetadataResolver(left, right);
        }
    }

    public interface ISymbolResolver
    {
        FileNode Bind(FileNode node);
    }

    public class TypeBinder : DiagnosticsVisitor
    {
        public override IEnumerable<DiagnosticResult> VisitAdditiveExpr(AdditiveExprNode additiveExprNode)
        {
            //TODO: wrong type errors
            base.VisitAdditiveExpr(additiveExprNode);
            var type = (additiveExprNode.left.Type, additiveExprNode.right.Type) switch
            {
                (IntTypeNode, IntTypeNode) => new IntTypeNode(),
                (IntTypeNode, FloatTypeNode) => new FloatTypeNode(),
                (FloatTypeNode, IntTypeNode) => new FloatTypeNode(),
                (FloatTypeNode, FloatTypeNode) => new FloatTypeNode(),
                (_, _) => null
            };
            if (type is not null)
            {
                additiveExprNode.Type = type;
            }
            else
            {
                if (additiveExprNode.left.Type == additiveExprNode.right.Type)
                {
                    yield return new DiagnosticResult(Severity.Error,
                        $"Type {additiveExprNode.left.Type} is invalid for operation {additiveExprNode.oper}",
                        Error.InvalidType);
                }
                else
                {
                    yield return new DiagnosticResult(Severity.Error, 
                        $"Cannot operate on types {additiveExprNode.left.Type} and {additiveExprNode.right.Type} with operation {additiveExprNode.oper}", 
                        Error.MismatchedTypes);
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitArray(ArrayNode arrayNode)
        {
            // TODO: ensure all elements are the same type
            base.VisitArray(arrayNode);
            TypeNode type;
            if (arrayNode.Count() == 0)
            {
                // special-case - we need to target-type it, that is pull the type from what we expect
                type = GetTargetType(arrayNode);
            }
            else
            {
                var types = arrayNode.values.Select(v => v.Type).Where(t => t is not null).Distinct();
                if (types.Count() == 1)
                {
                    type = types.Single()!;
                }
                else
                {
                    yield return new DiagnosticResult(Severity.Error, 
                        $"Array has mismatched types", 
                        Error.MismatchedTypes);
                    yield break;
                }
            }
            arrayNode.Type = type;
        }

        private static TypeNode GetTargetType(SymbolNode node)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DiagnosticResult> VisitArrayAccess(ArrayAccessNode arrayAccessNode)
        {
            // TODO: ensure array type
            base.VisitArrayAccess(arrayAccessNode);
            if (arrayAccessNode.arrayExpr.Type is not ArrayTypeNode arrayType)
            {
                yield return new DiagnosticResult(Severity.Error,
                    $"Not an array type",
                    Error.InvalidType);
                yield break;
            }
            arrayAccessNode.Type = arrayType.type;
        }

        public override IEnumerable<DiagnosticResult> VisitAssignment(AssignmentNode assignmentNode)
        {
            // TODO: ensure correct types both sides
            base.VisitAssignment(assignmentNode);
            if (assignmentNode.leftSide.Type != assignmentNode.rightSide.Type)
            {
                yield return new DiagnosticResult(Severity.Error,
                    $"Assigned type is not equal to assigning type",
                    Error.MismatchedTypes);
                yield break;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitBool(BoolNode boolNode)
        {
            boolNode.Type = new BoolTypeNode();
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitBooleanExpr(BoolExprNode boolExprNode)
        {
            // TODO: wrong type
            base.VisitBooleanExpr(boolExprNode);
            if (boolExprNode.left.Type is not BoolTypeNode || boolExprNode.right.Type is not BoolTypeNode)
            {
                yield return new DiagnosticResult(Severity.Error,
                    $"Cannot operate on types {boolExprNode.left.Type} and {boolExprNode.right.Type} with operation {boolExprNode.oper}", 
                    Error.InvalidType);
                yield break;
            }
            boolExprNode.Type = new BoolTypeNode();
        }

        public override IEnumerable<DiagnosticResult> VisitBoundIdentifier(BoundIdentifierNode identifierNode)
        {
            identifierNode.Type = identifierNode.binding.type;
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitColour(ColourNode colourNode)
        {
            //TODO: should we make colours a valid type/literal? maybe
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitCoordinate(CoordinateNode coordinateNode)
        {
            coordinateNode.Type = new CoordTypeNode();
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitDeclaration(DeclarationNode declarationNode)
        {
            // TODO: ensure correct types both sides
            base.VisitDeclaration(declarationNode);
            if (declarationNode.expr is not null)
            {
                if (declarationNode)
            }
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitEqualityExpr(EqualityExprNode equalityExprNode)
        {
            // TODO: wrong types
            equalityExprNode.Type = new BoolTypeNode();
            return base.VisitEqualityExpr(equalityExprNode);
        }

        public override IEnumerable<DiagnosticResult> VisitFloat(FloatNode floatNode)
        {
            floatNode.Type = new FloatTypeNode();
            return base.VisitFloat(floatNode);
        }

        public override IEnumerable<DiagnosticResult> VisitForeachStatement(ForeachStatementNode foreachStatementNode)
        {
            // TODO: check type if it's an iterable
            return base.VisitForeachStatement(foreachStatementNode);
        }

        public override IEnumerable<DiagnosticResult> VisitFunctionArgument(FunctionArgumentNode functionArgumentNode)
        {
            // TODO: need to bind function args - probably not here, though
            return base.VisitFunctionArgument(functionArgumentNode);
        }

        public override IEnumerable<DiagnosticResult> VisitFunctionCallExpr(FunctionCallExprNode funcCallExprNode)
        {
            // TODO: expr type resolves to the return type of the function
            // need to figure out how THAT works out
            return base.VisitFunctionCallExpr(funcCallExprNode);
        }

        public override IEnumerable<DiagnosticResult> VisitIdentifier(IdentifierNode identifierNode)
        {
            // TODO: failed binding
            return base.VisitIdentifier(identifierNode);
        }

        public override IEnumerable<DiagnosticResult> VisitIfStatement(IfStatementNode ifStatementNode)
        {
            // TODO: check if statement type?
            return base.VisitIfStatement(ifStatementNode);
        }

        public override IEnumerable<DiagnosticResult> VisitInt(IntNode intNode)
        {
            intNode.Type = new IntTypeNode();
            return base.VisitInt(intNode);
        }

        public override IEnumerable<DiagnosticResult> VisitIs(IsNode isexpr)
        {
            isexpr.Type = new BoolTypeNode();
            return base.VisitIs(isexpr);
        }

        public override IEnumerable<DiagnosticResult> VisitMemberAccess(MemberAccessNode memberAccessNode)
        {
            // TODO: needs to be bound first
            return base.VisitMemberAccess(memberAccessNode);
        }

        public override IEnumerable<DiagnosticResult> VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode)
        {
            //TODO: wrong type errors
            base.VisitMultiplicativeExprNode(multiplicativeExprNode);
            if (multiplicativeExprNode.left.Type == multiplicativeExprNode.right.Type)
            {
                multiplicativeExprNode.Type = multiplicativeExprNode.left.Type;
            }
            else
            {
                //TODO: mismatched type error
            }
            return multiplicativeExprNode;
        }

        public override IEnumerable<DiagnosticResult> VisitNatNum(NatNumNode natNumNode)
        {
            //TODO: is this ever hit? we don't have a NatNum type
            Debug.Assert(false);
            return base.VisitNatNum(natNumNode);
        }

        public override IEnumerable<DiagnosticResult> VisitNewObject(NewObjectNode newObjectExprNode)
        {
            //TODO: failed to bind new obj. error
            return base.VisitNewObject(newObjectExprNode);
        }

        public override IEnumerable<DiagnosticResult> VisitPostFixExpr(PostFixExprNode postFixExprNode)
        {
            //TODO: wrong type errors
            base.VisitPostFixExpr(postFixExprNode);
            postFixExprNode.Type = postFixExprNode.expr.Type;
            return postFixExprNode;
        }

        public override IEnumerable<DiagnosticResult> VisitRelationalExpr(RelationalExprNode relationalExprNode)
        {
            //TODO: wrong type errors
            relationalExprNode.Type = new BoolTypeNode();
            return base.VisitRelationalExpr(relationalExprNode);
        }

        public override IEnumerable<DiagnosticResult> VisitString(StringNode stringNode)
        {
            stringNode.Type = new StringTypeNode();
            return base.VisitString(stringNode);
        }

        public override IEnumerable<DiagnosticResult> VisitUnaryExpr(UnaryExprNode unaryExprNode)
        {
            //TODO: wrong type errors
            base.VisitUnaryExpr(unaryExprNode);
            unaryExprNode.Type = unaryExprNode.primaryExpr.Type;
            return unaryExprNode; 
        }
    }
}