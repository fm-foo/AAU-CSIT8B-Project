using ActionCompiler.Compiler;
using Xunit;
using ActionCompiler.UnitTests.TestDataProviders.ASTMutationVisitorTestDataProviders;
using ActionCompiler.UnitTests.Utility;
using ActionCompiler.AST.Statement;
using ActionCompiler.AST;

namespace ActionCompiler.UnitTests.VisitorUnitTests.ASTMutationVisitorTests
{
    /// <summary>
    /// Tests the <see cref="ForAndForeachNodeConverterVisitor"/> visitor to ensure that it correctly converts <see cref="ForStatementNode"/> nodes and <see cref="ForeachStatementNode"/> into <see cref="WhileStatementNode"/> nodes
    /// </summary>
    public class ForAndForeachNodeConverterVisitorTests
    {
        [Theory]
        [MemberData(nameof(ForAndForeachNodeConverterVisitorTestDataProvider.ForTestData), MemberType = typeof(ForAndForeachNodeConverterVisitorTestDataProvider))]
        public void TestForNodeConversion(string forNodeInput, string whileNodeInput)
        {
            FileNode forAst = Utility.Utilities.Parse(forNodeInput);
            FileNode whileAst = Utility.Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode convertedAst = (FileNode)visitor.Visit(forAst);

            Assert.Equal(whileAst, convertedAst);
        }

        [Theory]
        [MemberData(nameof(ForAndForeachNodeConverterVisitorTestDataProvider.ForEachTestData), MemberType = typeof(ForAndForeachNodeConverterVisitorTestDataProvider))]
        public void TestForEachNodeConversion(string forEachNodeInput, string whileNodeInput)
        {
            FileNode foreachAst = Utility.Utilities.Parse(forEachNodeInput);
            FileNode whileAst = Utility.Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode convertedAst = (FileNode)visitor.Visit(foreachAst);

            Assert.Equal(whileAst, convertedAst);
        }

        [Theory]
        [MemberData(nameof(ForAndForeachNodeConverterVisitorTestDataProvider.CombinedTestData), MemberType = typeof(ForAndForeachNodeConverterVisitorTestDataProvider))]
        public void TestForAndForeachCombinedConversion(string combinedInput, string whileNodeInput)
        {
            FileNode combinedAst = Utility.Utilities.Parse(combinedInput);
            FileNode whileAst = Utility.Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode combinedConvertedAst = (FileNode)visitor.Visit(combinedAst);

            Assert.Equal(whileAst, combinedConvertedAst);
        }
    }
}
