﻿using System.Collections.Generic;
using System.Linq;

namespace Action.AST
{
    public record FieldDecNode(IdentifierNode identifier, TypeNode type, ExprNode? expr) : PropertyNode(identifier, expr)
    {
        public override T Accept<T>(NodeVisitor<T> visitor)
        {
            return visitor.VisitFieldDeclaration(this);
        }
    }
}
