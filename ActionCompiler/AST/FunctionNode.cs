using System;
using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    // TODO: add type node
    public record FunctionNode() : ValueNode
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            throw new NotImplementedException();
           // return visitor.VisitField(this);
        }
    }
}
