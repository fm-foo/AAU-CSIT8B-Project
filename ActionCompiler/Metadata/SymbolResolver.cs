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

    public class TypeBinder : AutomaticNodeVisitor<SymbolNode>
    {
        public override SymbolNode VisitAdditiveExpr(AdditiveExprNode additiveExprNode)
        {
            //TODO: 
            return base.VisitAdditiveExpr(additiveExprNode);
        }

        public override SymbolNode VisitArray(ArrayNode arrayNode)
        {
            // TODO: ensure all elements are the same type
            base.VisitArray(arrayNode);

            
        }

        public override SymbolNode VisitArrayAccess(ArrayAccessNode arrayAccessNode)
        {
            // TODO: ensure array type
            base.VisitArrayAccess(arrayAccessNode);
            var arrayType = arrayAccessNode.arrayExpr.Type as ArrayTypeNode;
            Debug.Assert(arrayType is not null);
            arrayAccessNode.Type = arrayType.type;
            return arrayAccessNode;
        }

        public override SymbolNode VisitAssignment(AssignmentNode assignmentNode)
        {
            // TODO: ensure correct types both sides
            return base.VisitAssignment(assignmentNode);
        }

        public override SymbolNode VisitBool(BoolNode boolNode)
        {
            boolNode.Type = new BoolTypeNode();
            return base.VisitBool(boolNode);
        }

        public override SymbolNode VisitBooleanExpr(BoolExprNode boolExprNode)
        {
            // TODO: wrong type
            boolExprNode.Type = new BoolTypeNode();
            return base.VisitBooleanExpr(boolExprNode);
        }

        public override SymbolNode VisitBoundIdentifier(BoundIdentifierNode identifierNode)
        {
            identifierNode.Type = identifierNode.binding.type;
            return base.VisitBoundIdentifier(identifierNode);
        }

        public override SymbolNode VisitColour(ColourNode colourNode)
        {
            //TODO: should we make colours a valid type/literal? maybe
            return base.VisitColour(colourNode);
        }

        public override SymbolNode VisitCoordinate(CoordinateNode coordinateNode)
        {
            coordinateNode.Type = new CoordTypeNode();
            return base.VisitCoordinate(coordinateNode);
        }

        public override SymbolNode VisitDeclaration(DeclarationNode declarationNode)
        {
            // TODO: ensure correct types both sides
            return base.VisitDeclaration(declarationNode);
        }

        public override SymbolNode VisitEqualityExpr(EqualityExprNode equalityExprNode)
        {
            // TODO: wrong types
            equalityExprNode.Type = new BoolTypeNode();
            return base.VisitEqualityExpr(equalityExprNode);
        }

        public override SymbolNode VisitFloat(FloatNode floatNode)
        {
            floatNode.Type = new FloatTypeNode();
            return base.VisitFloat(floatNode);
        }

        public override SymbolNode VisitForeachStatement(ForeachStatementNode foreachStatementNode)
        {
            // TODO: check type if it's an iterable
            return base.VisitForeachStatement(foreachStatementNode);
        }

        public override SymbolNode VisitFunctionArgument(FunctionArgumentNode functionArgumentNode)
        {
            // TODO: need to bind function args - probably not here, though
            return base.VisitFunctionArgument(functionArgumentNode);
        }

        public override SymbolNode VisitFunctionCallExpr(FunctionCallExprNode funcCallExprNode)
        {
            // TODO: expr type resolves to the return type of the function
            // need to figure out how THAT works out
            return base.VisitFunctionCallExpr(funcCallExprNode);
        }

        public override SymbolNode VisitIdentifier(IdentifierNode identifierNode)
        {
            // TODO: failed binding
            return base.VisitIdentifier(identifierNode);
        }

        public override SymbolNode VisitIfStatement(IfStatementNode ifStatementNode)
        {
            // TODO: check if statement type?
            return base.VisitIfStatement(ifStatementNode);
        }

        public override SymbolNode VisitInt(IntNode intNode)
        {
            intNode.Type = new IntTypeNode();
            return base.VisitInt(intNode);
        }

        public override SymbolNode VisitIs(IsNode isexpr)
        {
            isexpr.Type = new BoolTypeNode();
            return base.VisitIs(isexpr);
        }

        public override SymbolNode VisitMemberAccess(MemberAccessNode memberAccessNode)
        {
            // TODO: needs to be bound first
            return base.VisitMemberAccess(memberAccessNode);
        }

        public override SymbolNode VisitMultiplicativeExprNode(MultiplicativeExprNode multiplicativeExprNode)
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

        public override SymbolNode VisitNatNum(NatNumNode natNumNode)
        {
            //TODO: is this ever hit? we don't have a NatNum type
            Debug.Assert(false);
            return base.VisitNatNum(natNumNode);
        }

        public override SymbolNode VisitNewObject(NewObjectNode newObjectExprNode)
        {
            //TODO: failed to bind new obj. error
            return base.VisitNewObject(newObjectExprNode);
        }

        public override SymbolNode VisitPostFixExpr(PostFixExprNode postFixExprNode)
        {
            //TODO: wrong type errors
            base.VisitPostFixExpr(postFixExprNode);
            postFixExprNode.Type = postFixExprNode.expr.Type;
            return postFixExprNode;
        }

        public override SymbolNode VisitRelationalExpr(RelationalExprNode relationalExprNode)
        {
            //TODO: wrong type errors
            relationalExprNode.Type = new BoolTypeNode();
            return base.VisitRelationalExpr(relationalExprNode);
        }

        public override SymbolNode VisitString(StringNode stringNode)
        {
            stringNode.Type = new StringTypeNode();
            return base.VisitString(stringNode);
        }

        public override SymbolNode VisitUnaryExpr(UnaryExprNode unaryExprNode)
        {
            //TODO: wrong type errors
            base.VisitUnaryExpr(unaryExprNode);
            unaryExprNode.Type = unaryExprNode.primaryExpr.Type;
            return unaryExprNode; 
        }
    }
}