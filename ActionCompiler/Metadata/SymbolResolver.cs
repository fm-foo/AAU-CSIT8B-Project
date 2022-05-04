using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Action.AST;

namespace Action.Metadata
{

    public class InternalSymbolResolver : ISymbolResolver
    {
        public FileNode Bind(FileNode node)
        {
            throw new System.NotImplementedException();
        }

        private class FunctionBinder : ASTMutatingVisitor
        {
            public override EntityNode VisitEntity(EntityNode entity)
            {
                var bindings = GetBindings(entity.fieldDecs).ToList();
                var functions = BindFunctions(entity.funcDecs, bindings);
                return entity with { funcDecs = functions };
            }

            public override GameNode VisitGame(GameNode game)
            {
                var bindings = GetBindings(game.fieldDecs);
                var functions = BindFunctions(game.funcDecs, bindings);
                return game with { funcDecs = functions };
            }

            private static IEnumerable<Binding> GetBindings(IEnumerable<FieldDecNode> fields)
            {
                throw new NotImplementedException();
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
                return (DeclarationNode)base.VisitDeclaration(node);
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
            throw new System.NotImplementedException();
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