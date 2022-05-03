using ActionCompiler.Compiler.SemanticErrorChecking;
using System.Collections.Generic;

namespace ActionCompiler.UnitTests.TestDataProviders.SemanticErrorTestDataProviders
{
    /// <summary>
    /// Provides data for testing the <see cref="SemErrorCoordinateSectionVisitor"/>
    /// </summary>
    internal class CoordinateSectionVisitorTestData
    {
        public static IEnumerable<object[]> GetCorrectData()
        {
            return BaseTestData.CorrectTestData();
        }

        public static IEnumerable<object[]> GetIncorrectData()
        {
            yield return new object[]
            {
                new SemanticErrorVisitorTestData(
                    @"
                        section TestSection (0, 0) {
                            background: color { hex: #222222; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            section (3, 3) {
                                background: image { path: ""./stone.png""; };
                                shape: box {
                                    height: 3;
                                    width: 3;
                                };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.StandaloneSectionWithCoordinates)
                )
           };
            yield return new object[]
            {
                new SemanticErrorVisitorTestData(
                    @"
                        section TestSection (-1, -1) {
                            background: color { hex: #222222; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            section (3, 3) {
                                background: image { path: ""./stone.png""; };
                                shape: box {
                                    height: 3;
                                    width: 3;
                                };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.StandaloneSectionWithCoordinates)
                )
           };
            yield return new object[]
            {
                new SemanticErrorVisitorTestData(
                    @"
                        section TestSection (10, 10) {
                            background: color { hex: #222222; };
                            shape: box {
                                height: 10; width: 10; 
                            };
                            section (3, 3) {
                                background: image { path: ""./stone.png""; };
                                shape: box {
                                    height: 3;
                                    width: 3;
                                };
                        };
                    };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.StandaloneSectionWithCoordinates)
                )
           };
        }
    }
}
