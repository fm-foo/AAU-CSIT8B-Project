using Action.AST;
using ActionCompiler.Compiler;
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
    /// <summary>
    /// Tests the <see cref="ForAndForeachNodeConverterVisitor"/> visitor to ensure that it correctly converts <see cref="ForStatementNode"/> nodes and <see cref="ForeachStatementNode"/> into <see cref="WhileStatementNode"/> nodes
    /// </summary>
    public class ForAndForeachNodeConverterVisitorTests
    {
        [Theory]
        [MemberData(nameof(ForAndForeachNodeConverterVisitorTestData.ForTestData), MemberType = typeof(ForAndForeachNodeConverterVisitorTestData))]
        public void TestForNodeConversion(string forNodeInput, string whileNodeInput)
        {
            FileNode forAst = Utility.Utilities.Parse(forNodeInput);
            FileNode whileAst = Utility.Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode convertedAst = Utility.Utilities.Convert(forAst, visitor);

           Assert.Equal(whileAst, convertedAst);
        }
    }
}
