using ActionCompiler.AST;
using ActionCompiler.AST.Expr;
using ActionCompiler.AST.Statement;
using System.Collections.Generic;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    /// <summary>
    /// Only certain kind of expressions can be lone:
    /// "Cerrtain kinds of expressions are valid as lone statements.The following types of expressions are valid. 
    /// Any other expression produces an error at compile time.If the expression produces a value, it is discarded.
    /// * Function call expressions
    /// * Pre/post increment/decrement expressions
    /// * New object expressions”
    /// </summary>
    internal class SemErrorLoneExpressions : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> Default => Enumerable.Empty<DiagnosticResult>();

        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            IEnumerable<ValueNode> entityAndGameNodes = file.nodes.Where(n => n is GameNode || n is EntityNode);

            foreach (ComplexNode node in entityAndGameNodes)
            {
                foreach (DiagnosticResult diagnosticResult in Visit(node))
                {
                    yield return diagnosticResult;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitGame(GameNode gameNode)
        {
            return VisitComplex(gameNode);
        }

        public override IEnumerable<DiagnosticResult> VisitEntity(EntityNode entityNode)
        {
            return VisitComplex(entityNode);
        }

        public override IEnumerable<DiagnosticResult> VisitComplex(ComplexNode complexNode)
        {
            foreach (PropertyNode node in complexNode.properties.Where(n => n is not FieldDecNode))
            {
                foreach (DiagnosticResult diagnosticResult in Visit(node))
                {
                    yield return diagnosticResult;
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitProperty(PropertyNode propertyNode)
        {
            return Visit(propertyNode.value!);
        }

        public override IEnumerable<DiagnosticResult> VisitFunction(FunctionNode functionNode)
        {
            return Visit(functionNode.block);
        }

        public override IEnumerable<DiagnosticResult> VisitBlock(BlockNode blockNode)
        {
            foreach (StatementNode statementNode in blockNode.statements.Where(n => n is not DeclarationNode && n is not AssignmentNode))
            {
                if (statementNode is ExpressionStatementNode expressionStatement)
                {
                    switch(expressionStatement.expr)
                    {
                        case PostFixExprNode _:
                        case FunctionCallExprNode _:
                        case NewObjectNode _:
                            break;
                        default:
                            yield return new DiagnosticResult(Severity.Error, "Only function calls, postfix expressions and new object expressions are allowed as lone statements!", Error.InvalidLoneStatement);
                            break;
                    }
                } 
                else
                {
                    foreach (DiagnosticResult diagnosticResult in Visit(statementNode))
                    {
                        yield return diagnosticResult;
                    }
                }
            }
        }

        public override IEnumerable<DiagnosticResult> VisitIfStatement(IfStatementNode ifStatementNode)
        {
            IEnumerable<DiagnosticResult> resultPrimary = Visit(ifStatementNode.primaryStatement);
            IEnumerable<DiagnosticResult> resultElse = ifStatementNode.elseStatement is null? Enumerable.Empty<DiagnosticResult>() : Visit(ifStatementNode.elseStatement);

            return resultPrimary.Concat(resultElse);
        }

        public override IEnumerable<DiagnosticResult> VisitWhileStatement(WhileStatementNode whileStatementNode)
        {
            return Visit(whileStatementNode.statement);
        }

        public override IEnumerable<DiagnosticResult> VisitForeachStatement(ForeachStatementNode foreachStatementNode)
        {
            return Visit(foreachStatementNode.statement);
        }

        public override IEnumerable<DiagnosticResult> VisitForStatement(ForStatementNode forStatementNode)
        {
            return Visit(forStatementNode.statement);
        }
    }
}
