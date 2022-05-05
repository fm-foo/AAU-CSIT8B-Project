using System.Collections.Generic;

namespace ActionCompiler.UnitTests.TestDataProviders
{
    /// <summary>
    /// Contain some base test data that is always correct
    /// </summary>
    internal static class BaseTestData
    {
        public static IEnumerable<object[]> CorrectTestData()
        {
            yield return new object[]
            {
                new SemanticErrorVisitorTestData(
                    @"
                        map TestMap {
                                background: colour { hex: #123456; };
                                shape: line {
                                                (0, 0);
                                                (4, -4);
                                };
                        };
                    "
                )
            };

            yield return new object[]
            {
                new SemanticErrorVisitorTestData
                (
                    @"
                        section TestSection {
                            background: colour { hex: #222222; };
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
                    "
                )
            };
            yield return new object[]
            {
                new TestData
                (
                    @"

                        entity SnakeBody {
                            create: function(int life) {
                                int a = 3;
                                int i = 2;
                                if(i!=2){
                                    a = 4;
                                }else{
                                    a = 3;
                                }
                            }

                            act: function() {

                            }

                            destroy: function() {
                                
                            }

                        }
                    "
                )
            };
        }
    }
}