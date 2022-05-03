using ActionCompiler.Compiler.SemanticErrorChecking;
using System.Collections.Generic;

namespace ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders
{
    /// <summary>
    /// Provides test data for testing the <see cref="SemErrorOnlyOnePropertyVisitor"/> visitor.
    /// </summary>
    internal class OnlyOnePropertyVisitorTestData
    {
        /// <summary>
        /// Test data that does not contain any relevant errors.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetCorrectData()
        {
            return BaseTestData.CorrectTestData();
        }

        /// <summary>
        /// Data that contains multiple of the same properties
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetIncorrectData()
        {
            yield return new object[]
            {
                new SemanticErrorVisitorTestData
                (
                    @"
                        map TestMap {
                            background: colour { hex: #123456; };
                            background: colour { hex: #123456; };
                            background: colour { hex: #123456; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                        };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MultipleProperties)
                )
            };
            yield return new object[]
           {
                new SemanticErrorVisitorTestData
                (
                    @"
                        map TestMap {
                            background: colour { hex: #123456; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            shape: box {
                                height: 10; width: 10; 
                            };
                        };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MultipleProperties)
                )
           };
        }
    }
}
