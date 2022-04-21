using Action.AST;
using Action.Compiler;
using ActionCompiler.Compiler.SemanticErrorChecking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActionCompiler.Tests.Tests
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
        public void CorrectInputNoDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(CoordinateOffMapVisitorTestData.GetIncorrectData), MemberType = typeof(CoordinateOffMapVisitorTestData))]
        public void IncorrectInputGetDiagnosticResult(TestData data)
        {
            Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
