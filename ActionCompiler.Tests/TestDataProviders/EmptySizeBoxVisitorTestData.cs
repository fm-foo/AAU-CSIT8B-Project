using ActionCompiler.Tests.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.Tests.TestDataProviders
{
    internal class EmptySizeBoxVisitorTestData
    {
        public static IEnumerable<object[]> GetCorrectData()
        {
            return BaseTestData.CorrectTestData();
        }

        public static IEnumerable<object[]> GetIncorrectData()
        {
            yield return new object[]
            {
                new TestData
                (
                     @"
                        map TestMap {
                                background: colour { hex: #123456; };
                                shape: box {
                                        height: 10; 
                                };
                        };
                    ",
                     new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MissingBoxWidthHeight)
                )
            };
            yield return new object[]
            {       
                new TestData
                (
                     @"
                        map TestMap {
                                background: colour { hex: #123456; };
                                shape: box {height: 10; };
                        };
                    ",
                     new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MissingBoxWidthHeight)
                )
            };
            yield return new object[]
            {
                new TestData
                (
                     @"
                        map TestMap {
                                background: colour { hex: #123456; };
                                shape: box {width: 10; };
                        };
                    ",
                     new Diagnostic(Action.Compiler.Severity.Error, Action.Compiler.Error.MissingBoxWidthHeight)
                )
            };
        }
    }
}
