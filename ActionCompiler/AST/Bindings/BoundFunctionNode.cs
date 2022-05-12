using System;
using System.Collections.Generic;
using System.Linq;
using ActionCompiler.AST.TypeNodes;

namespace ActionCompiler.AST.Bindings
{
    public record BoundFunctionNode(FunctionNode node, IEnumerable<Binding> bindings) : FunctionNode(node)
    {
        public virtual bool Equals(BoundFunctionNode? other)
        {
            return other is not null
                && node == other.node
                && bindings.SequenceEqual(other.bindings);
        }

        public override int GetHashCode()
        {
            var hc = new HashCode();
            hc.Add(node);
            foreach (var binding in bindings)
                hc.Add(binding);
            return hc.ToHashCode();
        }
    }

    public record Binding(IdentifierNode identifier, TypeNode type, Guid id)
    {
        public Binding(IdentifierNode identifier, TypeNode type) : this(identifier, type, Guid.NewGuid())
        { }
    }
}
