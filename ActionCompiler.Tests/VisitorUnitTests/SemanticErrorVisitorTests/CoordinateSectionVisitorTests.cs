using ActionCompiler.Compiler.SemanticErrorChecking;
using ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders;
using ActionCompiler.UnitTests.Utility;
using Xunit;

namespace ActionCompiler.UnitTests.VisitorUnitTests.SemanticErrorVisitorTests
{
    public class CoordinateSectionVisitorTests
    {
        private readonly SemErrorCoordinateSectionVisitor _visitor;

        public CoordinateSectionVisitorTests()
        {
            _visitor = new SemErrorCoordinateSectionVisitor();
        }

        [Theory]
        [MemberData(nameof(CoordinateSectionVisitorTestData.GetCorrectData), MemberType = typeof(CoordinateSectionVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(TestData data)
        {
            Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(CoordinateSectionVisitorTestData.GetIncorrectData), MemberType = typeof(CoordinateSectionVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(TestData data)
        {
            Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
