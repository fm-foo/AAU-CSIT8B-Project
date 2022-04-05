using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class OnlyOneProperty : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            foreach (var node in file.nodes.OfType<ComplexNode>())
            {
                foreach (var diagnostic in Visit(node))
                    yield return diagnostic;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            return VisitComplex(mapNode);
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode node)
        {
            return VisitComplex(node);
        }

        public override IEnumerable<DiagnosticResult> VisitEntity(EntityNode entity)
        {
            return VisitComplex(entity);
        }   

        public override IEnumerable<DiagnosticResult> VisitComplex(ComplexNode node)
        {
            HashSet<IdentifierNode> foundProperties = new HashSet<IdentifierNode>();

            foreach (var property in node.properties)
            {
                bool added = foundProperties.Add(property.identifier);

                if (!added)
                {
                    // it was already in the set
                    yield return new DiagnosticResult(Severity.Error, $"Property {property.identifier} on {node} exists multiple times", Error.MultipleProperties);
                }

                if (property.value is ComplexNode value)
                {
                    foreach (var diagnostic in Visit(value))
                        yield return diagnostic;
                }
            }

            foreach (var value in node.values.OfType<ComplexNode>())
            {
                foreach (var diagnostic in Visit(value))
                    yield return diagnostic;
            }

        }
    }
}
