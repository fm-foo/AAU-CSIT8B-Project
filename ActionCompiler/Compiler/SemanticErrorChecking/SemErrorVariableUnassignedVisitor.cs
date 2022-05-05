using ActionCompiler.AST;
using ActionCompiler.AST.Statement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorVariableUnassignedVisitor : DiagnosticsVisitor
    {
        public override IEnumerable<DiagnosticResult> VisitFunction(FunctionNode functionNode)
        {
            var visitor = new VariableAssignmentLevelVisitor();
            return visitor.Visit(functionNode.block);
        }

        private class VariableAssignmentLevelVisitor : DiagnosticsVisitor
        {
            // todo: this may need to be updated if we get any new kinds of statements

            public VariableAssignmentLevelVisitor()
            {
                AssignedIdentifiers = new Dictionary<IdentifierNode, bool>();
            }

            public VariableAssignmentLevelVisitor(Dictionary<IdentifierNode, bool> alreadySet)
            {
                AssignedIdentifiers = new Dictionary<IdentifierNode, bool>(alreadySet);
            }
            public readonly Dictionary<IdentifierNode, bool> AssignedIdentifiers;

            public override IEnumerable<DiagnosticResult> VisitDeclaration(DeclarationNode declarationNode)
            {
                IEnumerable<DiagnosticResult> result = declarationNode.expr is not null 
                    ? Visit(declarationNode.expr) 
                    : Enumerable.Empty<DiagnosticResult>();
                AssignedIdentifiers[declarationNode.identifier] = declarationNode.expr is not null;
                return result;
            }

            public override IEnumerable<DiagnosticResult> VisitIdentifier(IdentifierNode identifierNode)
            {
                bool exists = AssignedIdentifiers.TryGetValue(identifierNode, out bool assigned);
                if (!exists)
                {
                    // TODO: log this, it'll be useful for diagnosing bugs
                    yield break;
                }
                if (!assigned)
                {
                    yield return new DiagnosticResult(Severity.Error, $"{identifierNode} has not been definitely assigned", Error.NotDefinitelyAssigned);
                    yield break;
                }
                Debug.Assert(exists && assigned); // just in case we mess with the code later
                yield break;
            }

            public override IEnumerable<DiagnosticResult> VisitAssignment(AssignmentNode assignmentNode)
            {
                if (assignmentNode.leftSide is IdentifierNode identifier)
                {
                    IEnumerable<DiagnosticResult> result = Visit(assignmentNode.rightSide);
                    AssignedIdentifiers[identifier] = true;
                    return result;
                }
                else
                {
                    return base.VisitAssignment(assignmentNode);
                }
            }

            public override IEnumerable<DiagnosticResult> VisitBlock(BlockNode blockNode)
            {
                var nextLevelVisitor = new VariableAssignmentLevelVisitor(this.AssignedIdentifiers);
                IEnumerable<DiagnosticResult> result = Enumerable.Empty<DiagnosticResult>();
                foreach (var statement in blockNode.statements)
                {
                    result = result.Concat(nextLevelVisitor.Visit(statement));
                }
                foreach (var kvp in nextLevelVisitor.AssignedIdentifiers)
                {
                    if (AssignedIdentifiers.ContainsKey(kvp.Key) && kvp.Value)
                        AssignedIdentifiers[kvp.Key] = true;
                }
                return result;
            }

            public override IEnumerable<DiagnosticResult> VisitForStatement(ForStatementNode forStatementNode)
            {
                var nextLevelVisitor = new VariableAssignmentLevelVisitor(this.AssignedIdentifiers);
                var result = Enumerable.Empty<DiagnosticResult>();
                if (forStatementNode.initialization is not null)
                    result = result.Concat(nextLevelVisitor.Visit(forStatementNode.initialization));
                if (forStatementNode.condition is not null)
                    result = result.Concat(nextLevelVisitor.Visit(forStatementNode.condition));
                if (forStatementNode.control is not null)
                    result = result.Concat(nextLevelVisitor.Visit(forStatementNode.control));
                result = result.Concat(nextLevelVisitor.Visit(forStatementNode.statement));
                return result;
            }

            public override IEnumerable<DiagnosticResult> VisitForeachStatement(ForeachStatementNode foreachStatementNode)
            {
                var nextLevelVisitor = new VariableAssignmentLevelVisitor(this.AssignedIdentifiers);
                nextLevelVisitor.AssignedIdentifiers[foreachStatementNode.identifier] = true;
                return base.VisitForeachStatement(foreachStatementNode);
            }

            public override IEnumerable<DiagnosticResult> VisitIfStatement(IfStatementNode ifStatementNode)
            {
                var result = Visit(ifStatementNode.test);
                if (ifStatementNode.elseStatement is null)
                {
                    var nextLevelVisitor = new VariableAssignmentLevelVisitor(this.AssignedIdentifiers);
                    result = result.Concat(nextLevelVisitor.Visit(ifStatementNode.primaryStatement));
                }
                else
                {
                    var trueVisitor = new VariableAssignmentLevelVisitor(this.AssignedIdentifiers);
                    var falseVisitor = new VariableAssignmentLevelVisitor(this.AssignedIdentifiers);
                    result = result.Concat(trueVisitor.Visit(ifStatementNode.primaryStatement));
                    result = result.Concat(falseVisitor.Visit(ifStatementNode.elseStatement));
                    foreach (var kvp in AssignedIdentifiers.Where(a => !a.Value))
                    {
                        bool trueHas = trueVisitor.AssignedIdentifiers.TryGetValue(kvp.Key, out bool trueValue);
                        bool falseHas = falseVisitor.AssignedIdentifiers.TryGetValue(kvp.Key, out bool falseValue);
                        if (trueHas && trueValue && falseHas && falseValue)
                            AssignedIdentifiers[kvp.Key] = true;
                    }
                }
                return result;
            }
        }
    }
}