using Action.AST;
using Action.Compiler;
using Action.Parser;
using ActionCompiler.Compiler;
using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace ActionCompiler.Tests.Utility;

public static class Utilities
{
    internal static Stream StringToStream(string str)
    {
        byte[] arr = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(arr);
    }

    internal static FileNode Parse(string inputFile)
    {
        Stream input = StringToStream(inputFile);
        ICharStream stream = new AntlrInputStream(input);
        ITokenSource lexer = new ActionLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        ActionParser parser = new ActionParser(tokens);
        parser.BuildParseTree = true;
        ActionParser.FileContext tree = parser.file();
        var visitor = new ASTGenerator();

        return visitor.VisitFile(tree);
    }

    public static void PerformCorrectTests(TestData data, NodeVisitor<IEnumerable<DiagnosticResult>> visitor)
    {
        FileNode ast = Parse(data.File);

        Assert.Empty(visitor.Visit(ast));
    }

    public static void PerformIncorrectTests(TestData data, NodeVisitor<IEnumerable<DiagnosticResult>> visitor)
    {
        FileNode ast = Parse(data.File);

        IEnumerable<DiagnosticResult> results = visitor.Visit(ast);

        Assert.NotEmpty(results);

        foreach (DiagnosticResult result in results)
        {
            Assert.Equal(result.severity, data.Diagnostics!.Severity);
            Assert.Equal(result.error, data.Diagnostics.Error);
        }
    }

    internal static FileNode Convert(FileNode forAst, ForAndForeachNodeConverterVisitor visitor)
    {
        List<ComplexNode> newNodes = new List<ComplexNode>();
        foreach (var node in forAst.nodes)
        {
            if (node is not (EntityNode or GameNode))
            {
                newNodes.Add(node as ComplexNode);
            }
            else
            {
                newNodes.Add(visitor.Visit(node) as ComplexNode);
            }
        }
        FileNode fileNode = new FileNode(newNodes);
        return fileNode;
    }
}