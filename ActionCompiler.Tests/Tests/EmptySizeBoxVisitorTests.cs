using ActionCompiler.Compiler.SemanticErrorChecking;
using ActionCompiler.Tests.TestDataProviders;
using Xunit;

namespace ActionCompiler.Tests.Tests
{
    public class EmptySizeBoxVisitorTests
    {
        private readonly SemErrorEmptySizeBoxVisitor _visitor;

        public EmptySizeBoxVisitorTests()
        {
            _visitor = new SemErrorEmptySizeBoxVisitor();
        }

        [Theory]
        [MemberData(nameof(EmptySizeBoxVisitorTestData.GetCorrectData), MemberType = typeof(EmptySizeBoxVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(EmptySizeBoxVisitorTestData.GetIncorrectData), MemberType = typeof(EmptySizeBoxVisitorTestData))]
        public void IncorrectInputGetDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
