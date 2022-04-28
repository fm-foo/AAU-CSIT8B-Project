﻿using Action.AST;
using Action.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.Compiler
{
    /// <summary>
    /// Convert <see cref="ForStatementNode"/> and <see cref="ForeachStatementNode"/> nodes to <see cref="WhileStatementNode"/> nodes
    /// </summary>
    public class ForAndForeachNodeConverterVisitor : AutomaticNodeVisitor<SymbolNode>
    {

        public override SymbolNode VisitEntity(EntityNode entityNode)
        {
            List<PropertyNode> updatedDeclarations = entityNode.funcDecs.Select(f => this.Visit(f)).Cast<PropertyNode>().ToList();

            return new EntityNode(entityNode.identifier, entityNode.fieldDecs, updatedDeclarations);
        }

        public override SymbolNode VisitProperty(PropertyNode propertyNode)
        {
            Debug.Assert(propertyNode.value is FunctionNode);
            FunctionNode? value = this.Visit(propertyNode.value) as FunctionNode;
            return new PropertyNode(propertyNode.identifier, value);
        }

        public override SymbolNode VisitFunction(FunctionNode functionNode)
        {
            return new FunctionNode(functionNode.args, (Visit(functionNode.block) as BlockNode)!);
        }

        public override SymbolNode VisitBlock(BlockNode blockNode)
        {
            List<StatementNode> statements = new List<StatementNode>();
            
            foreach (StatementNode statementNode in blockNode.statements)
            {
                switch (statementNode)
                {
                    case ForStatementNode forStatement:
                        IEnumerable<StatementNode> statementNodes = GetWhileStatement(forStatement);
                        statements.AddRange(statementNodes);
                        break;
                    case ForeachStatementNode foreachStatement:
                        // TODO: foreach node
                        break;
                    default:
                        statements.Add(statementNode);
                        break;
                }
            }

            return new BlockNode(statements);
        }

        private IEnumerable<StatementNode> GetWhileStatement(ForStatementNode forStatement)
        {
            StatementNode? initStatement = forStatement.initialization; 
            ExprNode? condition = forStatement.condition;
            ExprNode? control = forStatement.control;

            //BlockNode statements = forStatement.statement is BlockNode ? (forStatement.statement as BlockNode)! : new BlockNode(new StatementNode[] { forStatement.statement });

            // Initialization, condition and control nodes are all null - it is an infinite loop
            if (initStatement is null && condition is null && control is null)
            {
                return new StatementNode[] { new WhileStatementNode(new BoolNode(true), forStatement.statement) };
            }
            // Initilaization statement is null - assume initialization happened before
            else if (initStatement is null && condition is not null && control is not null)
            {
                BlockNode statements = GetBlockNode(forStatement.statement, control);
                return new StatementNode[] { new WhileStatementNode(condition, statements) };
            }
            // Condition statement is null - assume infinite loop
            else if (initStatement is not null && condition is null && control is not null)
            {
                BlockNode statements = GetBlockNode(forStatement.statement, control);
                return new StatementNode[] {initStatement, new WhileStatementNode(new BoolNode(true), statements) };
            }
            // Control statement is null - assume it is part of the statement(s)
            else if (initStatement is not null && condition is not null && control is null)
            {
                return new StatementNode[] { initStatement, new WhileStatementNode(condition, forStatement.statement) };
            }
            // Initialization and condition statements are null - assume intitialization happened before, and it is an infinite loop
            else if (initStatement is null && condition is null && control is not null)
            {
                BlockNode statements = GetBlockNode(forStatement.statement, control);
                return new StatementNode[] { new WhileStatementNode(new BoolNode(true), statements)};
            }
            // Initialization and control statements are null - assume initialization happened before and control is part of the statement(s)
            else if (initStatement is null && condition is not null && control is null)
            {
                return new WhileStatementNode[] {new WhileStatementNode(condition, forStatement.statement) };
            }
            // Condition and control are null - assume infinite loop
            else if (initStatement is not null && condition is null && control is null)
            {
                return new StatementNode[] {initStatement, new WhileStatementNode(new BoolNode(true), forStatement.statement) };
            }
            // None of the statements/expressions are null
            else
            {
                Debug.Assert(initStatement is not null);
                Debug.Assert(condition is not null);
                Debug.Assert(control is not null);
                BlockNode statements = GetBlockNode(forStatement.statement, control);
                return new StatementNode[] {initStatement!, new WhileStatementNode(condition, statements)};
            }
        }

        private BlockNode GetBlockNode(StatementNode statement, ExprNode control)
        {
            List<StatementNode> statementNodes = new List<StatementNode>() { };
            if (statement is BlockNode block)
            {
                statementNodes.AddRange(block.statements);
            }
            else
            {
                statementNodes.Add(statement);
            }
            statementNodes.Add(new ExpressionStatementNode(control));
            return new BlockNode(statementNodes);
        }

        public override SymbolNode MergeValues(SymbolNode oldValue, SymbolNode newValue)
        {
            throw new NotImplementedException();
        }        
    }
}