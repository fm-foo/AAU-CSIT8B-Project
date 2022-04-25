using Action.AST;
using Action.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    internal class SemErrorVariableDeclarationsAsInternalStatement : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> Default => Enumerable.Empty<DiagnosticResult>();

        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            foreach (ValueNode node in file.nodes.Where(n => n is EntityNode || n is GameNode))
            {
                foreach (DiagnosticResult diagnosticResult in Visit(node))
                {
                    yield return diagnosticResult;
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
            IEnumerable<PropertyNode> funcDefinitions = complexNode.properties.Where(p => p.value is FunctionNode);

            foreach (PropertyNode function in funcDefinitions)
            {
                foreach (DiagnosticResult diagnosticResult in Visit(function.value!))
                {
                    yield return diagnosticResult;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitFunction(FunctionNode functionNode)
        {
            return VisitBlock(functionNode.block);
        }

        public override IEnumerable<DiagnosticResult> VisitBlock(BlockNode blockNode)
        {
            IEnumerable<StatementNode> statements = blockNode.statements.Where(n => n is ForStatementNode || n is ForeachStatementNode || n is IfStatementNode || n is WhileStatementNode);

            foreach (StatementNode statement in statements)
            {
                foreach (DiagnosticResult diagnosticResult in Visit(statement))
                {
                    yield return diagnosticResult;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitIfStatement(IfStatementNode ifStatementNode)
        {
            List<DiagnosticResult> results = new List<DiagnosticResult>();

            if (ifStatementNode.primaryStatement is BlockNode)
            {
                results.AddRange(Visit(ifStatementNode.primaryStatement));
            }
            else if (ifStatementNode.primaryStatement is DeclarationNode)
            {
                results.Add(new DiagnosticResult(Severity.Error, $"Embedded statement in an 'if' statement cannot be variable declaration!", Error.InvalidDeclarationInEmbeddedStatement));
            }

            if (ifStatementNode.elseStatement is not null)
            {
                if (ifStatementNode.elseStatement is BlockNode)
                {
                    results.AddRange(Visit(ifStatementNode.elseStatement));
                }
                else if (ifStatementNode.elseStatement is DeclarationNode)
                {
                    results.Add(new DiagnosticResult(Severity.Error, $"Embedded statement in an 'if' statement cannot be variable declaration!", Error.InvalidDeclarationInEmbeddedStatement));
                }
            }

            return results;
        }

        public override IEnumerable<DiagnosticResult> VisitForStatement(ForStatementNode forStatementNode)
        {
            if (forStatementNode.statement is DeclarationNode)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Embedded statement in an 'for' statement cannot be variable declaration!", Error.InvalidDeclarationInEmbeddedStatement) };
            }

            return Visit(forStatementNode.statement);
        }

        public override IEnumerable<DiagnosticResult> VisitForeachStatement(ForeachStatementNode foreachStatementNode)
        {
            if (foreachStatementNode.statement is DeclarationNode)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Embedded statement in an 'foreach' statement cannot be variable declaration!", Error.InvalidDeclarationInEmbeddedStatement) };
            }

            return Visit(foreachStatementNode.statement);
        }

        public override IEnumerable<DiagnosticResult> VisitWhileStatement(WhileStatementNode whileStatementNode)
        {
            if (whileStatementNode.statement is DeclarationNode)
            {
                return new DiagnosticResult[] { new DiagnosticResult(Severity.Error, $"Embedded statement in an 'while' statement cannot be variable declaration!", Error.InvalidDeclarationInEmbeddedStatement) };
            }
            
            return Visit(whileStatementNode.statement);
        }
    }
}
