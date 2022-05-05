using System;
using System.Collections.Generic;

namespace Action.AST
{
    public record BoundFunctionNode(FunctionNode node, IEnumerable<Binding> bindings) : FunctionNode(node);

    public record Binding(IdentifierNode identifier, Guid id)
    {
        public Binding(IdentifierNode identifier) : this(identifier, Guid.NewGuid())
        { }
    }
}
