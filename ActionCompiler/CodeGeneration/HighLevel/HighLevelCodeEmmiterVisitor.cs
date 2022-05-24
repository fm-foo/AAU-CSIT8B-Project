﻿using ActionCompiler.AST;
using ActionCompiler.AST.Expr;
using ActionCompiler.AST.Statement;
using ActionCompiler.AST.Types;
using ActionCompiler.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ActionCompiler.CodeGeneration.HighLevel
{
    public class HighLevelCodeEmmiterVisitor : AutomaticNodeVisitor<object>
    {
        private static readonly string[] gameFunctions = { "initialize" };
        private static readonly string[] entityFunctions = { "initialize", "act", "destroy" };

        public override object VisitFile(FileNode file)
        {
            List<(string name, CompilationUnitSyntax)> declarations = new();

            foreach (var node in file.nodes)
            {
                (string name, ClassDeclarationSyntax declarationSyntax) item = ((string name, ClassDeclarationSyntax declarationSyntax))Visit(node);

                CompilationUnitSyntax syntax = CompilationUnit().AddMembers(item.declarationSyntax);
                declarations.Add((item.name,syntax));
            }

            return declarations;
        }

        public override object VisitMap(MapNode mapNode)
        {
            ClassDeclarationSyntax classDeclaration = ClassDeclaration(mapNode.identifier.identifier)
                                                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                                        .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseMap")));

            List<MemberDeclarationSyntax> memberDeclarations = GetMemberDeclarations(mapNode.properties);
            classDeclaration = classDeclaration.AddMembers(memberDeclarations.ToArray());
            return (mapNode.identifier.identifier, classDeclaration);
        }

        public override object VisitGame(GameNode gameNode)
        {
            ClassDeclarationSyntax classDeclaration = ClassDeclaration(gameNode.identifier.identifier)
                                                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                                    .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseGame")));

            List<MemberDeclarationSyntax> memberDeclarations = GetFunctionDeclarations(gameNode.funcDecs);
            //memberDeclarations.AddRange(GetFieldDeclarations(gameNode.fieldDecs));

            classDeclaration = classDeclaration.AddMembers(memberDeclarations.ToArray());
            
            return (gameNode.identifier.identifier, classDeclaration);
        }

        private List<MemberDeclarationSyntax> GetFieldDeclarations(IEnumerable<FieldDecNode> fieldDecs)
        {
            throw new NotImplementedException();
        }

        private List<MemberDeclarationSyntax> GetFunctionDeclarations(IEnumerable<PropertyNode> funcDecs)
        {
            List<MemberDeclarationSyntax> lst = new();

            foreach (var dec in funcDecs)
            {
                FunctionNode function = (FunctionNode)dec.value!;

                SyntaxToken identifier = Identifier(dec.identifier.identifier);
                BlockSyntax block = (BlockSyntax)Visit(function.block);

                IEnumerable<ParameterSyntax>? args = function.args.Select(a => Identifier(a.identifier.identifier)).Select(p => Parameter(p).WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword))));

                MethodDeclarationSyntax? functionDeclaration = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), identifier)
                                            .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                            .AddParameterListParameters(args.ToArray())
                                            .WithBody(block);

                functionDeclaration = ShouldOverride(dec.identifier.identifier) ? functionDeclaration.AddModifiers(Token(SyntaxKind.OverrideKeyword)) : functionDeclaration;

                lst.Add(functionDeclaration);
            }

            return lst;
        }

        private bool ShouldOverride(string identifier)
        { 
            return gameFunctions.Contains(identifier, StringComparer.InvariantCultureIgnoreCase) || entityFunctions.Contains(identifier, StringComparer.InvariantCultureIgnoreCase);
        }

        public override object VisitBlock(BlockNode blockNode)
        {
            List<StatementSyntax> statements = new();

            foreach (StatementNode node in blockNode.statements)
            {
                StatementSyntax statementSyntax = (StatementSyntax)Visit(node);
                statements.Add(statementSyntax);
            }

            return Block(statements.ToArray());
        }

        public override object VisitDeclaration(DeclarationNode declarationNode)
        {
            VariableDeclaratorSyntax variableDeclarationSyntax = VariableDeclarator(declarationNode.identifier.identifier);

            if (declarationNode.expr is not null)
            {
                variableDeclarationSyntax = variableDeclarationSyntax.WithInitializer(EqualsValueClause((ExpressionSyntax)Visit(declarationNode.expr)));
            }

            LocalDeclarationStatementSyntax variableDeclaration = LocalDeclarationStatement(VariableDeclaration(ParseTypeName(declarationNode.type.GetTypeName())).AddVariables(variableDeclarationSyntax));
            return variableDeclaration;
        }

        public override object VisitInt(IntNode intNode)
        {
            return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(intNode.integer));
        }

        public override object VisitFloat(FloatNode floatNode)
        {
            return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(floatNode.f));
        }

        public override object VisitString(StringNode stringNode)
        {
            return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(stringNode.s));
        }

        public override object VisitBool(BoolNode boolNode)
        {
            return boolNode.val ?
                LiteralExpression(SyntaxKind.TrueLiteralExpression, Token(SyntaxKind.TrueKeyword)) :
                LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword));
        }


        private List<MemberDeclarationSyntax> GetMemberDeclarations(IEnumerable<PropertyNode> properties)
        {
            List<MemberDeclarationSyntax> memberDeclarations = new();

            foreach (PropertyNode node in properties)
            {
                IdentifierNode identifier = node.identifier;
                switch (identifier)
                {
                    case BackgroundKeywordNode:
                        memberDeclarations.Add(GetBackgroundDeclaration(node));
                        break;
                    case ShapeKeywordNode:
                        memberDeclarations.Add(GetShapeDeclaration(node));
                        break;
                    default:
                        break;
                        // throw new NotImplementedException();
                }
            }

            return memberDeclarations;
        }

        private MemberDeclarationSyntax GetShapeDeclaration(PropertyNode node)
        {
            ComplexNode complex = (ComplexNode)node.value!;
            IdentifierNode shapeType = complex.type;

            switch (shapeType)
            {
                case BoxKeywordNode:
                    return BoxShapeDeclaration(complex);
                default:
                    break;
            }


            return null;
        }

        private MemberDeclarationSyntax BoxShapeDeclaration(ComplexNode complex)
        {
            int height = complex.GetProperty<HeightKeywordNode, IntNode>().integer;
            int width = complex.GetProperty<WidthKeywordNode, IntNode>().integer;

            VariableDeclarationSyntax variableDeclaration = VariableDeclaration(ParseTypeName("IShape")).AddVariables(VariableDeclarator("_shape").WithInitializer(EqualsValueClause(ObjectCreationExpression(ParseTypeName("BoxShape")).AddArgumentListArguments(Argument(IdentifierName(height.ToString())), Argument(IdentifierName(width.ToString()))))));
            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(variableDeclaration).AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

            return fieldDeclaration;
        }

        private MemberDeclarationSyntax GetBackgroundDeclaration(PropertyNode node)
        {
            ComplexNode complex = (ComplexNode)node.value!;
            ColourNode colour = complex.GetProperty<HexKeywordNode, ColourNode>();

            List<ArgumentSyntax> args = new();
            args.Add(Argument(IdentifierName(colour.r.ToString())));
            args.Add(Argument(IdentifierName(colour.g.ToString())));
            args.Add(Argument(IdentifierName(colour.b.ToString())));

            // private readonly IBackground = new Background(r, g, b);
            VariableDeclarationSyntax variableDeclaration = VariableDeclaration(ParseTypeName("IBackground")).AddVariables(VariableDeclarator("_background").WithInitializer(EqualsValueClause(ObjectCreationExpression(ParseTypeName("Background")).AddArgumentListArguments(args.ToArray()))));
           // VariableDeclarationSyntax variableDeclaration = VariableDeclaration(ParseTypeName("IBackground")).AddVariables(VariableDeclarator("_background"));
            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(variableDeclaration).AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

            return fieldDeclaration;
        }

        public override object MergeValues(object oldValue, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}