using Action.AST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    /// <summary>
    /// 4. “It is an error for the left side expression to not be assignable (variable or field).”
    /// </summary>
    internal class SemErrorValidAssignment : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            foreach (var node in file.nodes.Where(n => n is EntityNode || n is GameNode))
            {
                foreach (var diagnostic in Visit(node))
                {
                    yield return diagnostic;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitEntity(EntityNode entityNode)
        {
            return VisitComplex(entityNode);
        }

        public override IEnumerable<DiagnosticResult> VisitGame(GameNode gameNode)
        {
            return VisitComplex(gameNode);
        }

        public override IEnumerable<DiagnosticResult> VisitComplex(ComplexNode complexNode)
        {
            foreach (var funcDeclaration in complexNode.properties.OfType<PropertyNode>().Where(p => p.value is FunctionNode))
            {
                foreach (var diagnostic in Visit(funcDeclaration.value!))
                {
                    yield return diagnostic;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitFunction(FunctionNode functionNode)
        {
            foreach (var assignment in functionNode.block.statements.OfType<AssignmentNode>())
            {
                foreach (var diagnostic in Visit(assignment))
                {
                    yield return diagnostic;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitAssignment(AssignmentNode assignmentNode)
        {
            // Left side must be variable (identifier) or field

            if (assignmentNode.leftSide is not (IdentifierNode or MemberAccessNode)) 
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, "Left side of an assignment expression must either be a variable or a field!", Error.InvalidAssignment) };
            }

            return Enumerable.Empty<DiagnosticResult>();
        }
    }
}
