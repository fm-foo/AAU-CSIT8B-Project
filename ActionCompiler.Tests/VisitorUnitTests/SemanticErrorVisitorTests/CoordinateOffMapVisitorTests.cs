using ActionCompiler.Compiler.SemanticErrorChecking;
using Xunit;
using ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders;
using ActionCompiler.UnitTests.Utility;

namespace ActionCompiler.UnitTests.VisitorUnitTests.SemanticErrorVisitorTests
{
    public class CoordinateOffMapVisitorTests
    {
        private readonly SemErrorCoordinateOffMapVisitor _visitor;

        public CoordinateOffMapVisitorTests()
        {
            _visitor = new SemErrorCoordinateOffMapVisitor();
        }

        [Theory]
        [MemberData(nameof(CoordinateOffMapVisitorTestData.GetCorrectData), MemberType = typeof(CoordinateOffMapVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(CoordinateOffMapVisitorTestData.GetIncorrectData), MemberType = typeof(CoordinateOffMapVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
