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
    public class OnlyOnePropertyVisitorTests
    {
        private readonly Compiler.SemanticErrorChecking.SemErrorOnlyOnePropertyVisitor _visitor;

        public OnlyOnePropertyVisitorTests()
        {
            _visitor = new Compiler.SemanticErrorChecking.SemErrorOnlyOnePropertyVisitor();
        }

        [Theory]
        [MemberData(nameof(OnlyOnePropertyVisitorTestData.GetCorrectData), MemberType = typeof(OnlyOnePropertyVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformCorrectTests(data, _visitor);
        }


        [Theory]
        [MemberData(nameof(OnlyOnePropertyVisitorTestData.GetIncorrectData), MemberType = typeof(OnlyOnePropertyVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(TestData data)
        {
            Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
