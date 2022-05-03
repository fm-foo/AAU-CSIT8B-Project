using ActionCompiler.Compiler.SemanticErrorChecking;
using System.Collections.Generic;

namespace ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders
{
    /// <summary>
    /// Provide test data for the <see cref="SemErrorEmptyBackgroundVisitor"/> visitor
    /// </summary>
    internal class EmptyBackgroundVisitorTestData
    {
        public static IEnumerable<object[]> GetCorrectData()
        {
            return BaseTestData.CorrectTestData();
        }

        public static IEnumerable<object[]> GetIncorrectData()
        {
            yield return new object[]
            {
                new TestData(
                    @"
                        map TestMap {
                            background: colour { };
                            shape: box {
                                height: 10; width: 10; 
                            };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MissingBackgroundColorValue)
                )
            };

            yield return new object[]
            {
                new TestData
                (
                    @"
                        section TestSection {
                            background: image { };
                            shape: box {
                                height: 10; width: 10; 
                            };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MissingBackgroundImagePathValue)
                )
            };
        }
    }
}
