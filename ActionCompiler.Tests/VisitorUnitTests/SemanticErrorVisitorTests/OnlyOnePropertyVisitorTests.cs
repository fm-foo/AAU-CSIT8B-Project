using Xunit;
using ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders;
using ActionCompiler.UnitTests.Utility;

namespace ActionCompiler.UnitTests.VisitorUnitTests.SemanticErrorVisitorTests
{
    /// <summary>
    /// Tests the <see cref="Compiler.SemanticErrorChecking.SemErrorOnlyOnePropertyVisitor"/> visitor
    /// </summary>
    public class OnlyOnePropertyVisitorTests
    {
        private readonly Compiler.SemanticErrorChecking.SemErrorOnlyOnePropertyVisitor _visitor;

        public OnlyOnePropertyVisitorTests()
        {
            _visitor = new Compiler.SemanticErrorChecking.SemErrorOnlyOnePropertyVisitor();
        }

        [Theory]
        [MemberData(nameof(OnlyOnePropertyVisitorTestData.GetCorrectData), MemberType = typeof(OnlyOnePropertyVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformCorrectTests(data, _visitor);
        }


        [Theory]
        [MemberData(nameof(OnlyOnePropertyVisitorTestData.GetIncorrectData), MemberType = typeof(OnlyOnePropertyVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
