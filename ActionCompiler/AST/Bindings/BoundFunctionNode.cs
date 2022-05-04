using System;
using System.Collections.Generic;

namespace Action.AST
{
    public record BoundFunctionNode(FunctionNode node, IEnumerable<Binding> bindings) : FunctionNode(node.args, node.block);

    public record Binding(IdentifierNode identifier, Guid id)
    {
        public Binding(IdentifierNode identifier) : this(identifier, Guid.NewGuid())
        { }
    }
}
