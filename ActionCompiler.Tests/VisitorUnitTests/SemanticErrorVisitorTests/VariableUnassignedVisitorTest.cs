using Action.AST;
using Action.Compiler;
using ActionCompiler.Tests.TestDataProviders;
using System.Collections.Generic;
using System.Linq;
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
        public void CorrectInputNoDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformCorrectTests(data, _visitor);
        }


        [Theory]
        [MemberData(nameof(VariableUnassignedVisitorTestData.GetIncorrectData), MemberType = typeof(VariableUnassignedVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(TestData data)
        {
            Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
