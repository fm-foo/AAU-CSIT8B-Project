using ActionCompiler.Compiler.SemanticErrorChecking;
using ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders;
using ActionCompiler.UnitTests.Utility;
using Xunit;

namespace ActionCompiler.UnitTests.VisitorUnitTests.SemanticErrorVisitorTests
{
    public class EmptyBackgroundVisitorTests
    {
        private readonly SemErrorEmptyBackgroundVisitor _visitor;

        public EmptyBackgroundVisitorTests()
        {
            _visitor = new SemErrorEmptyBackgroundVisitor();
        }

        [Theory]
        [MemberData(nameof(EmptyBackgroundVisitorTestData.GetCorrectData), MemberType = typeof(EmptyBackgroundVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(EmptyBackgroundVisitorTestData.GetIncorrectData), MemberType = typeof(EmptyBackgroundVisitorTestData))]
        public void IncorrectInputGetDiagnosticResults(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
