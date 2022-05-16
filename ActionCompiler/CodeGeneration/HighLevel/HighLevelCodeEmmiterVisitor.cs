using ActionCompiler.AST;
using ActionCompiler.Compiler;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ActionCompiler.CodeGeneration.HighLevel
{
    internal class HighLevelCodeEmmiterVisitor : AutomaticNodeVisitor<object>
    {

        public override object VisitFile(FileNode file)
        {
            List<ClassDeclarationSyntax> declarations = new();

            foreach (var node in file.nodes)
            {
                foreach (var item in (IEnumerable<ClassDeclarationSyntax>)Visit(node))
                {
                    declarations.Add(item);
                }
            }

            return declarations;
        }

        public override object VisitMap(MapNode mapNode)
        {
            ClassDeclarationSyntax classDeclaration = ClassDeclaration(mapNode.identifier.identifier)
                                                        .AddModifiers(Token(SyntaxKind.PublicKeyword))
                                                        .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseMap")));

            List<MemberDeclarationSyntax> memberDeclarations = GetMemberDeclarations(mapNode.properties);
            classDeclaration.AddMembers(memberDeclarations.ToArray());
            return classDeclaration;
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
                    default:
                        break;
                        // throw new NotImplementedException();
                }
            }

            return memberDeclarations;
        }

        private MemberDeclarationSyntax GetBackgroundDeclaration(PropertyNode node)
        {
            ComplexNode complex = (ComplexNode)node.value!;
            ColourNode colour = complex.GetProperty<HexKeywordNode, ColourNode>();

            // private readonly IBackground = new Background(r, g, b);
            VariableDeclarationSyntax variableDeclaration = VariableDeclaration(ParseTypeName("IBackground")).AddVariables(VariableDeclarator("_background").WithInitializer(EqualsValueClause(ObjectCreationExpression(ParseTypeName("Background")))));
            FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(variableDeclaration).AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

            return fieldDeclaration;
        }

        public override object MergeValues(object oldValue, object newValue)
        {
            throw new NotImplementedException();
        }
    }
}
