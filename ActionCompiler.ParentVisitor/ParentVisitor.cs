using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ActionCompiler.TokenCompiler;

[Generator]
public class AstMutatingVisitorGenerator : ISourceGenerator
{
    private static readonly DiagnosticDescriptor FailedForSomeReason = new DiagnosticDescriptor(id: "ACTION0001",
                                                                                            title: "Failed to generate",
                                                                                            messageFormat: "Failed to generate",
                                                                                            category: "Action",
                                                                                            DiagnosticSeverity.Warning,
                                                                                            isEnabledByDefault: true);


    public void Execute(GeneratorExecutionContext context)
    {
        var basetype = context.Compilation.GetTypeByMetadataName("ActionCompiler.AST.NodeVisitor`1")!;
        Debug.Assert(basetype is not null);
        var symbolnode = context.Compilation.GetTypeByMetadataName("ActionCompiler.AST.SymbolNode")!;
        Debug.Assert(symbolnode is not null);
        var constructed = basetype!.Construct(symbolnode);
        var typesyntax = GenericName(Identifier(basetype.Name))
            .AddTypeArgumentListArguments(ParseTypeName(symbolnode.Name));
        var klass = ClassDeclaration("ParentResolverVisitor")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword))
            .AddBaseListTypes(SimpleBaseType(typesyntax));
        var members = constructed.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.IsVirtual)
            .Where(m => m.Name is not ("Default" or "get_Default"))
            .Select(m => CreateMethod(context, m))
            .Where(m => m is not null);
        klass = klass.AddMembers(members.ToArray());
        CompilationUnitSyntax comp = CompilationUnit()
            .AddUsings(UsingDirective(IdentifierName("System.Linq")),
                    UsingDirective(IdentifierName("System")),
                    UsingDirective(IdentifierName("System.Collections.Generic")),
                    UsingDirective(IdentifierName("ActionCompiler.AST")),
                    UsingDirective(IdentifierName("ActionCompiler.AST.Expr")),
                    UsingDirective(IdentifierName("ActionCompiler.AST.Statement")),
                    UsingDirective(IdentifierName("ActionCompiler.AST.TypeNodes")),
                    UsingDirective(IdentifierName("ActionCompiler.AST.Types")),
                    UsingDirective(IdentifierName("ActionCompiler.AST.Bindings")),
                    UsingDirective(IdentifierName("ActionCompiler.Parser")))
            .AddMembers(NamespaceDeclaration(IdentifierName("ActionCompiler.AST"))
                .AddMembers(klass));
        var st = SourceText.From(comp.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
        context.AddSource("ParentVisitor.cs", st);
    }

    private static MethodDeclarationSyntax? CreateMethod(GeneratorExecutionContext ctx, IMethodSymbol m)
    {
        Debug.Assert(m.Parameters.Count() == 1);
        var param = m.Parameters.Single();
        var paramType = ParseTypeName(param.Type.Name);
        var paramIdentifier = Identifier(param.Name);
        var identifier = Identifier(m.Name);
        var symbolnode = ctx.Compilation.GetTypeByMetadataName("ActionCompiler.AST.SymbolNode");
        BlockSyntax block = GenerateMethodBody(ctx, Identifier(param.Name), param.Type);
        if (block is null)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(
                FailedForSomeReason, Location.None
            ));
            return null;
        }
        return MethodDeclaration(ParseTypeName(symbolnode.Name), identifier)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword))
            .AddParameterListParameters(Parameter(paramIdentifier).WithType(paramType))
            .WithBody(block);
    }

    private static BlockSyntax GenerateKeywordBody(GeneratorExecutionContext ctx, SyntaxToken identifier, ITypeSymbol type)
    {
        return Block(
            new[] {
                ReturnStatement(IdentifierName(identifier))
            }
        );
    }

    private static BlockSyntax? GenerateMethodBody(GeneratorExecutionContext ctx, SyntaxToken identifier, ITypeSymbol type)
    {
        if (type is IErrorTypeSymbol)
            return null;
        Debug.Assert(type.IsRecord);
        if (type.IsAbstract)
        {
            return GenerateKeywordBody(ctx, identifier, type);
        }
        List<StatementSyntax> statements = new List<StatementSyntax>();
        var constructor = type.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.Name is ".ctor")
            .Where(m => m.DeclaredAccessibility == Accessibility.Public)
            .Single();
        var objectReturnIdentifier = Identifier("objReturnValue");
        var deferred = new List<StatementSyntax>();
        foreach (var param in constructor.Parameters)
        {
            var paramid = Identifier(param.Name);
            StatementSyntax statement;
            StatementSyntax? deferredStatement = null;
            TypeSyntax paramtype;
            ExpressionSyntax expr;
            var member = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(identifier), IdentifierName(paramid));
            // four possibilities:
            // node type
            if (IsSymbolNode(param.Type))
            {
                paramtype = ParseTypeName(param.Type.Name);
                // Type name = (Type)Visit(main.name);
                expr = CastExpression(paramtype, InvocationExpression(IdentifierName("Visit"))
                    .AddArgumentListArguments(Argument(member)));

                // name.Parent = objectReturnIdentifier;
                deferredStatement = ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName(paramid),
                                                IdentifierName("Parent")),
                                            IdentifierName(objectReturnIdentifier)));
            }
            // Ienumerable node type
            else if (param.Type is INamedTypeSymbol { Name: "IEnumerable", Arity: 1 } nt && IsSymbolNode(nt.TypeArguments[0]))
            {
                // IEnumerable<Type> name = main.name.Select(Visit).Cast<Type>().ToList();
                var internaltype = ParseTypeName(nt.TypeArguments[0].Name);
                paramtype = GenericName("IEnumerable")
                    .AddTypeArgumentListArguments(internaltype);
                // main.name
                expr = member;
                // main.name.Select
                expr = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, IdentifierName("Select"));
                // main.name.Select(Visit)
                expr = InvocationExpression(expr).AddArgumentListArguments(Argument(IdentifierName("Visit")));
                // main.name.Select(Visit).Cast<Type>
                expr = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, GenericName("Cast")
                    .AddTypeArgumentListArguments(internaltype));
                // main.name.Select(Visit).Cast<Type>()
                expr = InvocationExpression(expr);
                // main.name.Select(Visit).Cast<Type>().ToList
                expr = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, IdentifierName("ToList"));
                // main.Name.Select(Visit).Cast<Type>().ToList()
                expr = InvocationExpression(expr);

                deferredStatement = ForEachStatement(
                                    internaltype,
                                    Identifier("node"),
                                    IdentifierName(paramid),
                                    ExpressionStatement(
                                        AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("node"),
                                                IdentifierName("Parent")),
                                            IdentifierName(objectReturnIdentifier))));
            } 
            // integral type (int/string/etc)
            else if (IsIntegralType(param.Type))
            {
                //Type name = main.name
                paramtype = GetIntegralTypeName(param.Type.Name);
                expr = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(identifier), IdentifierName(paramid));
                // Type name = (Type)Visit(main.name);
            }
            // other
            else
            {
                // todo: diagnostic
                throw new InvalidOperationException();
            }
            if (!param.Type.IsValueType)
            {
                expr = ConditionalExpression(
                    IsPatternExpression(member, ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                    PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    expr
                );
            }
            statement = LocalDeclarationStatement(VariableDeclaration(paramtype)
                .AddVariables(VariableDeclarator(paramid)
                    .WithInitializer(EqualsValueClause(expr))));
            
            statements.Add(statement);
            if (deferredStatement is not null)
            {
                deferredStatement = IfStatement(
                    IsPatternExpression(IdentifierName(paramid), UnaryPattern(ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression)))),
                    deferredStatement
                );
                deferred.Add(deferredStatement);
            }

        }
        var identifiers = constructor.Parameters
            .Select(p => p.Name)
            .Select(p => IdentifierName(p))
            .Select(p => Argument(p));
        var typename = ParseTypeName(type.Name);
        var newstatement = ObjectCreationExpression(typename)
            .AddArgumentListArguments(identifiers.ToArray());
        var decl = VariableDeclaration(typename)
            .AddVariables(VariableDeclarator(objectReturnIdentifier).WithInitializer(EqualsValueClause(newstatement)));
        statements.Add(LocalDeclarationStatement(decl));
        statements.AddRange(deferred);
        statements.Add(ReturnStatement(IdentifierName(objectReturnIdentifier)));
        return Block(statements.ToArray());
    }

    private static bool IsSymbolNode(ITypeSymbol type)
    {
        if (type.Name == "SymbolNode")
            return true;
        if (type.BaseType is null)
            return false;
        return IsSymbolNode(type.BaseType);
    }

    private static bool IsIntegralType(ITypeSymbol type)
    {
        return type.Name switch
        {
            "String" => true,
            "Int32" => true,
            "Double" => true,
            "UInt32"=> true,
            "Byte" => true,
            "Guid" => true,
            "Boolean" => true,
            _ => IsEnumType(type)
        };
    }

    private static bool IsEnumType(ITypeSymbol type)
    {
        if (type.Name == "Enum")
            return true;
        if (type.BaseType is null)
            return false;
        return IsEnumType(type.BaseType);
    }

    private static TypeSyntax GetIntegralTypeName(string name)
    {
        return name switch
        {
            "String" => PredefinedType(Token(SyntaxKind.StringKeyword)),
            "Int32" => PredefinedType(Token(SyntaxKind.IntKeyword)),
            "Double" => PredefinedType(Token(SyntaxKind.DoubleKeyword)),
            "UInt32"=> PredefinedType(Token(SyntaxKind.UIntKeyword)),
            "Byte" => PredefinedType(Token(SyntaxKind.ByteKeyword)),
            "Guid" => IdentifierName("Guid"),
            "Boolean" => PredefinedType(Token(SyntaxKind.BoolKeyword)),
            _ => IdentifierName(name)
        };
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}