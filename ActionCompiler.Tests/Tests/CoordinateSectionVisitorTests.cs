using ActionCompiler.Compiler.SemanticErrorChecking;
using ActionCompiler.Tests.TestDataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActionCompiler.Tests.Tests
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
            Utility.Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(CoordinateSectionVisitorTestData.GetIncorrectData), MemberType = typeof(CoordinateSectionVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(TestData data)
        {
            Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
