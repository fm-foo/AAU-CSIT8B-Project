using ActionCompiler.AST;
using ActionCompiler.AST.Types;
using ActionCompiler.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ActionCompiler.CodeGeneration.HighLevel
{
    public class HighLevelCodeEmmiterVisitor : AutomaticNodeVisitor<object>
    {

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
