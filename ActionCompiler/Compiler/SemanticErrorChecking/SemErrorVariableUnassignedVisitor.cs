using Action.AST;
using Action.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorVariableUnassignedVisitor : DiagnosticsVisitor
    {

        public override IEnumerable<DiagnosticResult> VisitMap(MapNode mapNode)
        {
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitSection(SectionNode sectionNode)
        {
            yield break;
        }

        public override IEnumerable<DiagnosticResult> VisitEntity(EntityNode entityNode)
        {
            return CheckDeclaredVariablesGE(entityNode);
        }
        public override IEnumerable<DiagnosticResult> VisitGame(GameNode gameNode)
        {
            return CheckDeclaredVariablesGE(gameNode);
        }

        public IEnumerable<DiagnosticResult> CheckDeclaredVariablesGE(ComplexNode node)
        {
            Dictionary<IdentifierNode, bool> variables = new Dictionary<IdentifierNode, bool>();

            foreach (PropertyNode prop in node.properties)
            {
                FunctionNode fun = (FunctionNode)prop.value;
                foreach (StatementNode stat in fun.block.statements)
                {
                    if (stat is DeclarationNode)
                    {
                        DeclarationNode dec = (DeclarationNode)stat;
                        if (dec.expr == null)
                        {
                            variables.Add(dec.identifier, false);
                        }
                        else
                        {
                            variables.Add(dec.identifier, true);
                        }
                    }
                    else if (stat is AssignmentNode)
                    {
                        AssignmentNode assign = (AssignmentNode)stat;
                        if (assign.rightSide is ExprNode)
                        {
                            if (CheckExpr(assign.rightSide, variables))
                            {
                                IdentifierNode nodeL = (IdentifierNode)assign.leftSide;
                                if (variables.ContainsKey(nodeL))
                                {
                                    variables[nodeL] = true;
                                }
                                else
                                {
                                    yield return new DiagnosticResult(Severity.Error, "variable assigned before having been declared");
                                }
                            }
                            else
                            {
                                yield return new DiagnosticResult(Severity.Error, "variable used before having been initialized");

                            }
                        }
                    }
                    else if (stat is ExpressionStatementNode)
                    {
                        ExpressionStatementNode expr = (ExpressionStatementNode)stat;
                        if (expr.expr is FunctionCallExprNode)
                        {
                            FunctionCallExprNode func = (FunctionCallExprNode)expr.expr;
                            if (func.funcArgs.Any())
                            {
                                foreach (IdentifierNode id in func.funcArgs)
                                {
                                    if (variables.ContainsKey(id))
                                    {
                                        if (!variables[id])
                                        {
                                            yield return new DiagnosticResult(Severity.Error, "variable used before having been initialized");
                                        }
                                    }
                                    else
                                    {
                                        yield return new DiagnosticResult(Severity.Error, "variable used before having been declared");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            yield break;
        }

        public bool CheckExpr(ExprNode myExpr, Dictionary<IdentifierNode, bool> variables)
        {
            bool res = true;
            switch (myExpr)
            {
                // Do I really need to do that for every kind of expression?
                case AdditiveExprNode add:
                    if (add.left is IdentifierNode)
                    {
                        IdentifierNode id = (IdentifierNode)add.left;
                        if (variables.ContainsKey(id))
                        {
                            if (!variables[id])
                            {
                                res = false;
                            }
                        }
                    }
                    else
                    {
                        res = CheckExpr(add.left, variables);
                    }
                    break;
                default:
                    break;
            }
            return res;
        }

        // TODO: add a method for all the if, while stuff, which take the node and 
        //the dictionnary as arguments
    }
}