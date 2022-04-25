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
    public class EmptyBackgroundVisitorTests
    {
        private readonly SemErrorEmptyBackgroundVisitor _visitor;

        public EmptyBackgroundVisitorTests()
        {
            _visitor = new SemErrorEmptyBackgroundVisitor();
        }

        [Theory]
        [MemberData(nameof(EmptyBackgroundVisitorTestData.GetCorrectData), MemberType = typeof(EmptyBackgroundVisitorTestData))]
        public void CorrectInputNoDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformCorrectTests(data, _visitor);
        }

        [Theory]
        [MemberData(nameof(EmptyBackgroundVisitorTestData.GetIncorrectData), MemberType = typeof(EmptyBackgroundVisitorTestData))]
        public void IncorrectInputGetDiagnosticResults(TestData data)
        {
            Utility.Utilities.PerformIncorrectTests(data, _visitor);
        }
    }
}
