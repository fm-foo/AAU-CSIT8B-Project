using Action.AST;
using ActionCompiler.Compiler;
using Xunit;
using ActionCompiler.UnitTests.TestDataProviders.ASTMutationVisitorTestDataProviders;
using ActionCompiler.UnitTests.Utility;

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
            FileNode forAst = Utilities.Parse(forNodeInput);
            FileNode whileAst = Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode convertedAst = (FileNode)visitor.Visit(forAst);

            Assert.Equal(whileAst, convertedAst);
        }

        [Theory]
        [MemberData(nameof(ForAndForeachNodeConverterVisitorTestDataProvider.ForEachTestData), MemberType = typeof(ForAndForeachNodeConverterVisitorTestDataProvider))]
        public void TestForEachNodeConversion(string forEachNodeInput, string whileNodeInput)
        {
            FileNode foreachAst = Utilities.Parse(forEachNodeInput);
            FileNode whileAst = Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode convertedAst = (FileNode)visitor.Visit(foreachAst);

            Assert.Equal(whileAst, convertedAst);
        }

        [Theory]
        [MemberData(nameof(ForAndForeachNodeConverterVisitorTestDataProvider.CombinedTestData), MemberType = typeof(ForAndForeachNodeConverterVisitorTestDataProvider))]
        public void TestForAndForeachCombinedConversion(string combinedInput, string whileNodeInput)
        {
            FileNode combinedAst = Utilities.Parse(combinedInput);
            FileNode whileAst = Utilities.Parse(whileNodeInput);

            ForAndForeachNodeConverterVisitor visitor = new ForAndForeachNodeConverterVisitor();
            FileNode combinedConvertedAst = (FileNode)visitor.Visit(combinedAst);

            Assert.Equal(whileAst, combinedConvertedAst);
        }
    }
}
