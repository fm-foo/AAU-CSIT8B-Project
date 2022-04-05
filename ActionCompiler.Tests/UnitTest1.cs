using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Action.Compiler;
using Xunit;

namespace ActionCompiler.Tests;

public class OnlyOnePropertyTests
{

    public static IEnumerable<object[]> TestDataFix => GetTestData().Select(td => new object[] { td });

    public static IEnumerable<TestData> GetTestData()
    {
        yield return new TestData(
            "map BasicMap {" +
            "    background: colour { hex: #222222; };" +
            "    shape: box {" +
            "            height: 10; width: 10;" +
            "    };" +
            "};"
        );

        yield return new TestData(
            "map BasicMap {" +
            "    background: colour { hex: #222222; };" +
            "    background: colour { hex: #222222; };" +
            "    shape: box {" +
            "            height: 10; width: 10;" +
            "    };" +
            "};",
            new Diagnostic(Severity.Error, Error.MultipleProperties)
        );
    }
    
    [Theory]
    [MemberData(nameof(TestDataFix))]
    public void Test(TestData data)
    {
        Utilities.PerformTest(data);
    }
}

public record TestData(string file, params Diagnostic[] diagnostics);

public record Diagnostic(Severity severity, Error error);

public static class Utilities
{
    private static Stream StringToStream(string str)
    {
        byte[] arr = Encoding.UTF8.GetBytes(str);
        return new MemoryStream(arr);
    }

    public static void PerformTest(TestData data)
    {
        Stream stream = StringToStream(data.file);
        var compiler = new Action.Compiler.ActionCompiler();
        var result = compiler.Compile(stream);
        if (data.diagnostics.Length == 0)
        {
            Assert.Empty(result.Diagnostics);
            Assert.True(result.Success);
        } else
        {
            // todo: rewrite this to properly match diagnostics with errors
            // also, todo: rewrite this for line numbers
            Assert.Equal(data.diagnostics.Length, result.Diagnostics.Count());
            foreach (var diagnostic in data.diagnostics)
            {
                var resultdiag = result.Diagnostics.FirstOrDefault(r => 
                    r.severity == diagnostic.severity 
                    && r.error == diagnostic.error
                );
                Assert.NotNull(resultdiag);
            }
        }
    }
}