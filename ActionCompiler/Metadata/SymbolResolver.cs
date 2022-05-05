using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ActionCompiler.AST;
using ActionCompiler.AST.Bindings;
using ActionCompiler.AST.Expr;
using ActionCompiler.AST.Statement;

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
                return entity with { funcDecs = functions };
            }

            public override GameNode VisitGame(GameNode game)
            {
                var bindings = GetBindings(game.fieldDecs).ToList();
                var functions = BindFunctions(game.funcDecs, bindings).ToList();
                return game with { funcDecs = functions };
            }

            private static IEnumerable<Binding> GetBindings(IEnumerable<FieldDecNode> fields)
            {
                return fields.Select(f => new Binding(f.identifier));
            }

            private static IEnumerable<PropertyNode> BindFunctions(IEnumerable<PropertyNode> nodes, IEnumerable<Binding> bindings)
            {
                foreach (var node in nodes)
                {
                    Console.WriteLine(node);
                    Debug.Assert(node.value is not null);
                    Debug.Assert(node.value.GetType() == typeof(FunctionNode));
                    var func = (FunctionNode)node.value;
                    yield return node with { value = new BoundFunctionNode(func, bindings) };
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
                var binding = new Binding(node.identifier);
                var set = bindings.Peek();
                set.Add(binding);
                if (node.expr is not null)
                {
                    var expr = (ExprNode)base.Visit(node.expr);
                    return node with { expr = expr };
                }
                return node;
            }

            public override MemberAccessNode VisitMemberAccess(MemberAccessNode memberAccessNode)
            {
                // don't visit the identifier - it can't be bound in this step
                ExprNode expr = (ExprNode)Visit(memberAccessNode.expr);
                return memberAccessNode with { expr = expr };
            }

            public override ValueNode VisitIdentifier(IdentifierNode node)
            {
                var binding = AllBindings.SingleOrDefault(b => b.identifier == node);
                return binding is null
                    ? node
                    : new BoundIdentifierNode(binding.id);
            }
        }
    }

    public class MetadataSymbolResolver : ISymbolResolver
    {
        // todo: this
        public FileNode Bind(FileNode node)
        {
            throw new NotImplementedException();
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
}