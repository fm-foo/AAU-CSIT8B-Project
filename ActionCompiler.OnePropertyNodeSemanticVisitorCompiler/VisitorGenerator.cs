using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using Action.AST;

namespace VisitorGenerator
{
    [Generator]
    public class SemanticVisitorGenerator : ISourceGenerator
    {
        private readonly List<Type> types = new List<Type>() {  };

        ComplexNode complexNode

        public void Execute(GeneratorExecutionContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}