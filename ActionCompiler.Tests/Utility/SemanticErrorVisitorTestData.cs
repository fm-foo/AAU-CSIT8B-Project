using Action.AST;
using ActionCompiler.Compiler;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

public record ASTGeneratorTestData(string File, FileNode AST)
{
    public MemoryStream FileAsStream
    {
        get
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(File));
        }
    }

 }
public record SemanticErrorVisitorTestData(string File, Diagnostic? Diagnostics = null);
public record Diagnostic(Severity Severity, Error Error);
