using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler.SemanticErrorChecking
{
    public class SemErrorMultipleIdentifiers : NodeVisitor<IEnumerable<DiagnosticResult>>
    {
        public override IEnumerable<DiagnosticResult> VisitFile(FileNode file)
        {
            var blocks = file.nodes
                .OfType<ComplexNode>()
                .SelectMany(n => n.properties)
                .Select(n => n.value)
                .OfType<FunctionNode>()
                .Select(n => n.block);

            foreach (var block in blocks)
            {
                foreach (var result in Visit(block))
                    yield return result;
            }
        }

        public override IEnumerable<DiagnosticResult> VisitBlock(BlockNode blockNode)
        {
            HashSet<IdentifierNode> identifiers = new HashSet<IdentifierNode>();
            return VisitBlockInternal(blockNode, identifiers);
        }

        public IEnumerable<DiagnosticResult> VisitBlockInternal(BlockNode blockNode, HashSet<IdentifierNode> identifiers)
        {
            foreach (var statement in blockNode.statements)
            {
                switch (statement)
                {
                    case DeclarationNode dn:
                        bool added = identifiers.Add(dn.identifier);
                        if (!added)
                            yield return new DiagnosticResult(Severity.Error, $"Variable {dn.identifier.identifier} declared multiple times", Error.MultipleDeclaration);
                        break;
                    case BlockNode block:
                        foreach (var result in Visit(block))
                            yield return result;
                        break;
                    case ForStatementNode forn:
                        if (forn.statement is BlockNode forblock)
                        {
                            var innerIdentifiers = new HashSet<IdentifierNode>();
                            if (forn.initialization is DeclarationNode dec)
                                innerIdentifiers.Add(dec.identifier);
                            foreach (var result in VisitBlockInternal(forblock, innerIdentifiers))
                                yield return result;
                        }
                        break;
                    case ForeachStatementNode foreachn:
                        if (foreachn.statement is BlockNode feblock)
                        {
                            var innerIdentifiers = new HashSet<IdentifierNode>();
                            innerIdentifiers.Add(foreachn.identifier);
                            foreach (var result in VisitBlockInternal(feblock, innerIdentifiers))
                                yield return result;
                        }
                        break;
                    case WhileStatementNode whilen:
                        if (whilen.statement is BlockNode whileblock)
                        {
                            foreach (var result in Visit(whileblock))
                                yield return result;
                        }
                        break;
                    case IfStatementNode ifn:
                        if (ifn.primaryStatement is BlockNode ifblock)
                        {
                            foreach (var result in Visit(ifblock))
                                yield return result;
                        } // also checks null
                        if (ifn.elseStatement is BlockNode elseblock)
                        {
                            foreach (var result in Visit(elseblock))
                                yield return result;
                        }
                        break;
                    // expressions and assignments are irrelevant, can never (currently) produce identifiers
                    case ExpressionStatementNode:
                    case AssignmentNode:
                    default:
                        break;
                }
            }
        }
    }
}