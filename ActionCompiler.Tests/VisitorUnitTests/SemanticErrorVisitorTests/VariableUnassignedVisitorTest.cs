using ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders;
using ActionCompiler.UnitTests.Utility;
using Xunit;


namespace ActionCompiler.UnitTests.VisitorUnitTests.SemanticErrorVisitorTests
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
            Utilities.PerformCorrectTests(data, _visitor);
        }


        [Theory]
        [MemberData(nameof(VariableUnassignedVisitorTestData.GetIncorrectData), MemberType = typeof(VariableUnassignedVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(SemanticErrorVisitorTestData data)
        {
            Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
