using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Action.TokenCompiler;

[Generator]
public class TokenEnumGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var files = context.AdditionalFiles.Where(f => f.Path.EndsWith(".tokens"));
        foreach (var file in files)
        {
            FileInfo fileinfo = new FileInfo(file.Path);
            using StreamReader reader = fileinfo.OpenText();
            Dictionary<int, string> values = new Dictionary<int, string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                int lastIndex = line.LastIndexOf('='); // Use this instead of line.Split('=') because we have the '=' character (e.g. '='=16)

                string[] splitLine = new string[] {line.Substring(0, lastIndex), line.Substring(lastIndex + 1)};

                string identifier = splitLine[0];
                int id = int.Parse(splitLine[1]);
                if (!values.ContainsKey(id))
                {
                    values[id] = identifier;
                }
            }
            EmitEnum(context, fileinfo.Name.Replace(".tokens", "Token"), values);
        }
    }

    private static void EmitEnum(GeneratorExecutionContext context, string name, Dictionary<int, string> values)
    {
        var enumdecl = EnumDeclaration(name);
        enumdecl = enumdecl.AddMembers(
            values.Select(v => 
                EnumMemberDeclaration(Identifier(v.Value))
                    .WithEqualsValue(
                        EqualsValueClause(
                            LiteralExpression(SyntaxKind.NumericLiteralExpression, 
                            Literal(v.Key)))))
            .ToArray()
        );

        var unit = CompilationUnit()
            .AddMembers(
                NamespaceDeclaration(IdentifierName("Action.Parser")).AddMembers(enumdecl)
            );

        SourceText text = SourceText.From(unit.NormalizeWhitespace().ToFullString(), Encoding.UTF8);
        context.AddSource($"{name}.cs", text);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        //Thread.Sleep(5000);
    }
}