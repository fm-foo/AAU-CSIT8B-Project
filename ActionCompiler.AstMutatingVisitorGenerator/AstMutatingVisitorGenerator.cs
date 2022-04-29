using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Action.TokenCompiler;

[Generator]
public class AstMutatingVisitorGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var basetype = context.Compilation.GetTypeByMetadataName("Action.AST.NodeVisitor`1");
        Debug.Assert(basetype is not null);
        var symbolnode = context.Compilation.GetTypeByMetadataName("Action.AST.SymbolNode");
        Debug.Assert(symbolnode is not null);
        var constructed = basetype.Construct(symbolnode);
        var typesyntax = GenericName(Identifier(basetype.Name))
            .AddTypeArgumentListArguments(ParseTypeName(symbolnode.Name));
        var klass = ClassDeclaration("ASTMutatingVisitor")
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBaseListTypes(SimpleBaseType(typesyntax));
        var members = constructed.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.IsVirtual)
            .Where(m => m.Name is not ("Default" or "get_Default"))
            .Select(m => CreateMethod(context, m));
        klass = klass.AddMembers(members.ToArray());

        Console.Error.WriteLine(klass.NormalizeWhitespace().ToFullString());
    }

    private static MethodDeclarationSyntax CreateMethod(GeneratorExecutionContext ctx, IMethodSymbol m)
    {
        Debug.Assert(m.Parameters.Count() == 1);
        var param = m.Parameters.Single();
        var paramType = ParseTypeName(param.Type.Name);
        var paramIdentifier = Identifier(param.Name);
        var identifier = Identifier(m.Name);
        BlockSyntax block = GenerateMethodBody(ctx, Identifier(param.Name), param.Type);
        return MethodDeclaration(paramType, identifier)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(Parameter(paramIdentifier).WithType(paramType))
            .WithBody(block);
    }

    private static BlockSyntax GenerateMethodBody(GeneratorExecutionContext ctx, SyntaxToken identifier, ITypeSymbol type)
    {
        Debug.Assert(type.IsRecord);
        var constructor = type.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.Name is ".ctor")
            .Where(m => m.DeclaredAccessibility == Accessibility.Public)
            .Single();
        ;
        throw new NotImplementedException();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        while (!Debugger.IsAttached)
        {
            Console.Error.WriteLine("Waiting for debugger...");
            Thread.Sleep(3000);
        }
    }
}