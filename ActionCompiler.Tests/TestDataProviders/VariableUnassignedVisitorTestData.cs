using ActionCompiler.Compiler.SemanticErrorChecking;
using ActionCompiler.Tests.Tests;
using ActionCompiler.UnitTests.TestDataProviders;
using System.Collections.Generic;

namespace ActionCompiler.Tests.TestDataProviders
{
    /// <summary>
    /// Provides data for testing the <see cref="SemErrorCoordinateSectionVisitor"/>
    /// </summary>
    internal class VariableUnassignedVisitorTestData
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

                        entity SnakeBody {
                            create: function(int life) {
                                int a = 3;
                                int b;
                                a = b;
                            }

                            act: function() {

                            }

                            destroy: function() {
                                
                            }

                        };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.NotDefinitelyAssigned)
                )
           };
            yield return new object[]
            {
                new SemanticErrorVisitorTestData(
                    @"

                        entity SnakeBody {
                            create: function(int life) {
                                int a;
                                int i = 2;
                                if(i!=2){
                                    a = 4;
                                }else{

                                }
                                i = a;
                            }

                            act: function() {

                            }

                            destroy: function() {
                                
                            }

                        };
                    ",
                    new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.NotDefinitelyAssigned)
                )
           };
        }
    }
}
