using System;
using System.Diagnostics;
using Antlr4.Build.Tasks;

namespace Action.AST
{
    public abstract record SymbolNode
    {
        public SymbolNode? Parent { get; set; }
        public abstract T Accept<T>(NodeVisitor<T> visitor);
    }
}