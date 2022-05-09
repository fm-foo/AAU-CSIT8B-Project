using ActionCompiler.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

public record IntegrationTestData(string File, IEnumerable<DiagnosticResult> Diagnostics);


public static class Utility
{
    public static MemoryStream GetTextAsStream(string txt)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(txt));
    } 
}