using ActionCompiler.AST;
using ActionCompiler.AST.Expr;
using ActionCompiler.AST.Statement;
using ActionCompiler.AST.TypeNodes;
using ActionCompiler.AST.Types;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ActionCompiler.Compiler
{
    /// <summary>
    /// Convert <see cref="ForStatementNode"/> and <see cref="ForeachStatementNode"/> nodes to <see cref="WhileStatementNode"/> nodes
    /// </summary>
    public class ForAndForeachNodeConverterVisitor : ASTMutatingVisitor
    {
        public override SymbolNode VisitFile(FileNode file)
        {
            IEnumerable<ValueNode> nodes = file.nodes.Where(n => n is (EntityNode or GameNode));

            List<ValueNode> newNodes = new();
            newNodes.AddRange(file.nodes.Where(n => !nodes.Contains(n)));

            foreach (ValueNode node in nodes)
            {
                ValueNode newNode = (ValueNode)Visit(node);
                newNodes.Add(newNode);
            }

            return new FileNode(newNodes);
        }
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
                        statements.AddRange(GetWhileStatement(forStatement));
                        break;
                    case ForeachStatementNode foreachStatement:
                        statements.AddRange(GetWhileStatement(foreachStatement));
                        break;
                    default:
                        statements.Add(statementNode);
                        break;
                }
            }

            return new BlockNode(statements);
        }

        /// <summary>
        /// Get a list of statement nodes that are syntactically equivalent to <paramref name="foreachStatement"/>.
        /// Assume that:
        ///     1. Only arrays can be enumerated.
        ///     2. Arrays have a built-in .Length() method.
        /// </summary>
        /// <param name="forStatement"></param>
        /// <returns></returns>
        private IEnumerable<StatementNode> GetWhileStatement(ForeachStatementNode foreachStatement)
        {
            TypeNode type = foreachStatement.type; // The type of the variable, e.g. foreach(int ...)
            IdentifierNode identifier = foreachStatement.identifier; // The identifier of the variable, e.g foreach(int x ...)
            ExprNode iterable = foreachStatement.iterable; // The expression (identifier of the array that will be enumerated), e.g. foreach (int x in array) ...

            Debug.Assert(iterable is IdentifierNode);

            // TODO: make sure that there are no naming conflicts
            // This does not take into account that variables/fields with the names 'index' or 'length' may have been declared by the programmer
            IdentifierNode indexIdentifierNode = new IdentifierNode("index");
            IdentifierNode lengthIdentifierNode = new IdentifierNode("length");

            DeclarationNode indexDeclarationNode = new DeclarationNode(new IntTypeNode(), indexIdentifierNode, new IntNode(0));
            DeclarationNode lengthDeclarationNode = new DeclarationNode(new IntTypeNode(), lengthIdentifierNode, new FunctionCallExprNode(new MemberAccessNode((IdentifierNode)iterable, new IdentifierNode("Length")), Enumerable.Empty<ExprNode>()));

            DeclarationNode arrayAccessDeclaration = new DeclarationNode(type, identifier, new ArrayAccessNode(iterable, indexIdentifierNode));
            PostFixExprNode increment = new PostFixExprNode(indexIdentifierNode, PostFixOperator.PLUSPLUS);

            List<StatementNode> statements = new List<StatementNode>();
            statements.Add(arrayAccessDeclaration);

            if (foreachStatement.statement is BlockNode block)
            {
                statements.AddRange(((BlockNode)Visit(block)).statements);
            }
            else
            {
                statements.Add(foreachStatement.statement);
            }
            statements.Add(new ExpressionStatementNode(increment));

            BlockNode blockNode = new BlockNode(statements);

            WhileStatementNode whileStatementNode = new WhileStatementNode(new RelationalExprNode(indexIdentifierNode, lengthIdentifierNode, RelationalOper.LESSTHAN), blockNode);

            return new StatementNode[] {indexDeclarationNode, lengthDeclarationNode, whileStatementNode };
        }

       
        /// <summary>
        /// Get a list of statement nodes that are syntactically equivalent to <paramref name="forStatement"/>.
        /// </summary>
        /// <param name="forStatement"></param>
        /// <returns></returns>
        private IEnumerable<StatementNode> GetWhileStatement(ForStatementNode forStatement)
        {
            StatementNode? initStatement = forStatement.initialization; 
            ExprNode? condition = forStatement.condition;
            ExprNode? control = forStatement.control;

            BlockNode blockNode = GetBlockNode(forStatement.statement, control);

            // Initialization, condition and control nodes are all null - it is an infinite loop
            if (initStatement is null && condition is null && control is null)
            {
                return new StatementNode[] { new WhileStatementNode(new BoolNode(true), blockNode) };
            }
            // Initialization statement is null - assume initialization happened before
            else if (initStatement is null && condition is not null && control is not null)
            {
                return new StatementNode[] { new WhileStatementNode(condition, blockNode) };
            }
            // Condition statement is null - assume infinite loop
            else if (initStatement is not null && condition is null && control is not null)
            {
                return new StatementNode[] {initStatement, new WhileStatementNode(new BoolNode(true), blockNode) };
            }
            // Control statement is null - assume it is part of the statement(s)
            else if (initStatement is not null && condition is not null && control is null)
            {
                return new StatementNode[] { initStatement, new WhileStatementNode(condition, blockNode) };
            }
            // Initialization and condition statements are null - assume initialization happened before, and it is an infinite loop
            else if (initStatement is null && condition is null && control is not null)
            {
                return new StatementNode[] { new WhileStatementNode(new BoolNode(true), blockNode) };
            }
            // Initialization and control statements are null - assume initialization happened before and control is part of the statement(s)
            else if (initStatement is null && condition is not null && control is null)
            {
                return new WhileStatementNode[] {new WhileStatementNode(condition, blockNode) };
            }
            // Condition and control are null - assume infinite loop
            else if (initStatement is not null && condition is null && control is null)
            {
                return new StatementNode[] {initStatement, new WhileStatementNode(new BoolNode(true), blockNode) };
            }
            // None of the statements/expressions are null
            else
            {
                Debug.Assert(initStatement is not null);
                Debug.Assert(condition is not null);
                Debug.Assert(control is not null);
                return new StatementNode[] {initStatement!, new WhileStatementNode(condition, blockNode) };
            }
        }

        private BlockNode GetBlockNode(StatementNode statement, ExprNode? control = null)
        {
            List<StatementNode> statementNodes = new List<StatementNode>() { };
            if (statement is BlockNode block)
            {
                statementNodes.AddRange(((BlockNode)Visit(block)).statements);
            }
            else
            {
                statementNodes.Add(statement);
            }

            if (control is not null)
            {
                statementNodes.Add(new ExpressionStatementNode(control));
            }
            return new BlockNode(statementNodes);
        }
    }
}
