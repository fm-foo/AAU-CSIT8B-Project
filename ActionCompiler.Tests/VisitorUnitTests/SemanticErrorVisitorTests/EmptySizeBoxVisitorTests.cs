using ActionCompiler.Compiler.SemanticErrorChecking;
using ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders;
using ActionCompiler.UnitTests.Utility;
using Xunit;

namespace ActionCompiler.UnitTests.VisitorUnitTests.SemanticErrorVisitorTests
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
            Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(EmptySizeBoxVisitorTestData.GetIncorrectData), MemberType = typeof(EmptySizeBoxVisitorTestData))]
        public void IncorrectInputGetDiagnosticResults(TestData data)
        {
            Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
