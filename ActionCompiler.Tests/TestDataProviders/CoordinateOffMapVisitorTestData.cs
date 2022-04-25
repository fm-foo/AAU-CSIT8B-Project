using ActionCompiler.Compiler.SemanticErrorChecking;
using System.Collections.Generic;

namespace ActionCompiler.Tests.Tests
{
    /// <summary>
    /// Provides test data for testing the <see cref="SemErrorCoordinateOffMapVisitor"/> visitor.
    /// </summary>
    internal class CoordinateOffMapVisitorTestData
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
        /// Test data that contains sections that are outside of the defined map.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetIncorrectData()
        {
            yield return new object[]
            {
                new TestData
                (
                    @"
                        map TestMap {
                            background: colour { hex: #222222; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            section (5, 5) {
                                background: image { path: ""./stone.png""; };
                                shape: coordinates {
                                    (-10, 2);
                                };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.CoordinatesOffMap)
                )
            };
            yield return new object[]
            {
                new TestData
                (
                    @"
                        map TestMap {
                            background: colour { hex: #222222; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            section (5, 5) {
                                background: image { path: ""./stone.png""; };
                                shape: coordinates {
                                    (10, -10);
                                };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.CoordinatesOffMap)
                )
            };
            yield return new object[]
            {
                new TestData
                (
                    @"
                        map TestMap {
                            background: colour { hex: #222222; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            section (5, 5) {
                                background: image { path: ""./stone.png""; };
                                shape: coordinates {
                                    (-10, -10);
                                };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.CoordinatesOffMap)
                )
            };
        }
    }
}
