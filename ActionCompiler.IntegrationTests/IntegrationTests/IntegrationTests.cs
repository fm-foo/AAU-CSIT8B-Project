using ActionCompiler.Compiler;
using ActionCompiler.IntegrationTests.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActionCompiler.IntegrationTests.IntegrationTests
{
    public class IntegrationTests
    {
        [Theory]
        [MemberData(nameof(TestDataProvider.GetIncorrectData), MemberType = typeof(TestDataProvider))]
        public void IntegrationTest_IncorrectFile_ExpectDiagnosticResults(IntegrationTestData testData)
        {
            Compiler.ActionCompiler compiler = new Compiler.ActionCompiler();

            CompilationResult result = compiler.Compile(Utility.GetTextAsStream(testData.File));

            Assert.False(result.Success);
            Assert.NotEmpty(result.Diagnostics);

            bool equal = result.Diagnostics.OrderBy(x => x.message).SequenceEqual(testData.Diagnostics.OrderBy(x => x.message));
            Assert.True(equal);
        }

        [Theory]
        [MemberData(nameof(TestDataProvider.GetCorrectData), MemberType = typeof(TestDataProvider))]
        public void IntegrationTest_CorrectFile_ExpectCompilationSuccess(IntegrationTestData testData)
        {
            Compiler.ActionCompiler compiler = new Compiler.ActionCompiler();

            CompilationResult result = compiler.Compile(Utility.GetTextAsStream(testData.File));

            Assert.True(result.Success);
            Assert.Empty(result.Diagnostics);
        }
    }
}
