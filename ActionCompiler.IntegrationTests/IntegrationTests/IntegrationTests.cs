using ActionCompiler.IntegrationTests.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActionCompiler.IntegrationTests.IntegrationTests
{
    public class IntegrationTests
    {
        [Theory]
        [MemberData(nameof(TestDataProvider.GetIncorrectData), MemberType = typeof(TestDataProvider))]
        public void IntegrationTest_IncorrectFile_ExpectDiagnosticResults(IntegrationTestData testData)
        {
        }
    }
}
