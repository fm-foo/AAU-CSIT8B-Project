using ActionCompiler.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.IntegrationTests.DataProviders
{
    internal class TestDataProvider
    {
        public static IEnumerable<object[]> GetIncorrectData()
        {
            // Two entities with the same name
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        create: function() {}
                        act: function() {}
                        destroy: function() {}
                    };
                    entity TestEntity{
                          
                        create: function() {}
                        act: function() {}
                        destroy: function() {}
                    };
                    ",
                    new DiagnosticResult[] { new DiagnosticResult(Severity.Error, "The identifier TestEntity has already been defined!", Error.IdentifierAlreadyDefined) }
                )
            };
            // Entity missing 'create' function
            yield return new object[] 
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        act: function() {}
                        destroy: function() {}
                    };
                    ",
                    new DiagnosticResult[] {new DiagnosticResult(Severity.Error, "The 'create' function is missing", Error.EntityMissingCreateFunction)}
                )
            };
            // Entity missing 'act' function
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        create: function() {}
                        destroy: function() {}
                    };
                    ",
                    new DiagnosticResult[] {new DiagnosticResult(Severity.Error, "The 'act' function is missing", Error.EntityMissingActFunction)}
                )
            };
            // Entity missing 'destroy' function
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        create: function() {}
                        act: function() {}
                    };
                    ",
                    new DiagnosticResult[] {new DiagnosticResult(Severity.Error, "The 'destroy' function is missing", Error.EntityMissingDestroyFunction)}
                )
            };
            // Entity missing 'create' and 'act' functions
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        destroy: function() {}
                    };
                    ",
                    new DiagnosticResult[] 
                    {
                        new DiagnosticResult(Severity.Error, "The 'create' function is missing", Error.EntityMissingCreateFunction),
                        new DiagnosticResult(Severity.Error, "The 'act' function is missing", Error.EntityMissingActFunction)
                    }
                )
            };
            // Entity missing 'create' and 'destroy' functions
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        act: function() {}
                    };
                    ",
                    new DiagnosticResult[]
                    {
                        new DiagnosticResult(Severity.Error, "The 'create' function is missing", Error.EntityMissingCreateFunction),
                        new DiagnosticResult(Severity.Error, "The 'destroy' function is missing", Error.EntityMissingDestroyFunction)
                    }
                )
            };
            // Entity missing 'act' and 'destroy' functions
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{
                          
                        create: function() {}
                    };
                    ",
                    new DiagnosticResult[]
                    {
                        new DiagnosticResult(Severity.Error, "The 'act' function is missing", Error.EntityMissingActFunction),
                        new DiagnosticResult(Severity.Error, "The 'destroy' function is missing", Error.EntityMissingDestroyFunction)
                    }
                )
            };
        }

        public static IEnumerable<object[]> GetCorrectData()
        {
            yield return new object[]
            {
                new IntegrationTestData
                (
                    @"
                    entity TestEntity{

                        create: function() {}
                        act: function() {}
                        destroy: function() {}
                    };
                    ",
                    Enumerable.Empty<DiagnosticResult>()
                )
            };
        }
    }
}
