﻿using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class SemErrorOnlyOneWidthProperty : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
             var combinedNodes = file.nodes.Where(n => n is MapNode || n is SectionNode);
            foreach (var node in combinedNodes)
            {
                foreach (var symbol in Visit(node))
                    yield return symbol;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            return CheckProperties(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            return CheckProperties(sectionNode);
        }

        private IEnumerable<DiagnosticResult> CheckProperties(ComplexNode node)
        {
            Debug.Assert(node is MapNode || node is SectionNode);

            var shape = node.properties.Where(n => n.identifier.GetType() == typeof(ShapeKeywordNode));

            Debug.Assert(shape.Count() == 1);

            return this.VisitProperty(shape.First());
        }

        public override IEnumerable<DiagnosticResult> VisitProperty(PropertyNode propertyNode)
        {
            ComplexNode? complexNode = propertyNode.value as ComplexNode;

            Debug.Assert(complexNode != null);

            int count = complexNode.properties.Where(n => n.identifier.GetType() == typeof(WidthKeywordNode)).Count();

            if (count > 1)
            {
                yield return new DiagnosticResult(Severity.Error, $"Shape may only contan one width property!");
            }
        }
        
    }
}
