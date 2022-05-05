using ActionCompiler.Tests.TestDataProviders;
using Xunit;

namespace ActionCompiler.Tests.Tests
{
    /// <summary>
    /// Tests the <see cref="Compiler.SemanticErrorChecking.SemErrorOnlyOnePropertyVisitor"/> visitor
    /// </summary>
    public class VariableUnassignedVisitorTests
    {
        private readonly Compiler.SemanticErrorChecking.SemErrorVariableUnassignedVisitor _visitor;

        public VariableUnassignedVisitorTests()
        {
            _visitor = new Compiler.SemanticErrorChecking.SemErrorVariableUnassignedVisitor();
        }

        [Theory]
        [MemberData(nameof(VariableUnassignedVisitorTestData.GetCorrectData), MemberType = typeof(VariableUnassignedVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(SemanticErrorVisitorTestData data)
        {
            UnitTests.Utility.Utilities.PerformCorrectTests(data, _visitor);
        }


        [Theory]
        [MemberData(nameof(VariableUnassignedVisitorTestData.GetIncorrectData), MemberType = typeof(VariableUnassignedVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(SemanticErrorVisitorTestData data)
        {
            UnitTests.Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
