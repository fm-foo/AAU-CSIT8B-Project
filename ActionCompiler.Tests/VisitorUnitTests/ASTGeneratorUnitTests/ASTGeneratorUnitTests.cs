using Action.AST;
using Action.Compiler;
using ActionCompiler.UnitTests.TestDataProviders.ASTGeneratorTestDataProviders;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using Xunit;

namespace ActionCompiler.UnitTests.VisitorUnitTests.ASTGeneratorUnitTests
{
    /// <summary>
    /// Tests to ensure that the <see cref="ASTGenerator"/> class generates correct trees.
    /// </summary>
    public class ASTGeneratorUnitTests
    {

        [Theory]
        [MemberData(nameof(ASTGeneratorTestDataProvider.GetMapTestData), MemberType = typeof(ASTGeneratorTestDataProvider))]
        public void TestMapASTGeneration(ASTGeneratorTestData testData)
        {
            Action.Compiler.ActionCompiler compiler = new();
            FileNode? ast = compiler.Parse(testData.FileAsStream, NullLogger<Action.Compiler.ActionCompiler>.Instance, new List<DiagnosticResult>());

            Assert.NotNull(ast);
            Assert.Equal(testData.AST, ast);
        }

    }
}
