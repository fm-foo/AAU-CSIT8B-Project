using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionCompiler.Tests.TestDataProviders
{
    internal static class ForAndForeachNodeConverterVisitorTestData
    {
        public static IEnumerable<object[]> ForTestData()
        {
            // Initialization, condition and control statements are null
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (;;){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            while(true) {
                                DoNothing();
                            }
                        }   
                    };
                "
            };

            // Condition and control are null
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (int x = 0;;){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(true) {
                                DoNothing();
                            }
                        }   
                    };
                "
            };

            // Initialization and control are null
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            for (;x < 10;){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(x < 10) {
                                DoNothing();
                            }
                        }   
                    };
                "
            };

            // Initialization and control are null

            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            for (;;x++){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(true) {
                                DoNothing();
                                x++;
                            }
                        }   
                    };
                "
            };

            // Control is null
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (int x = 0;x < 10;){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(x < 10) {
                                DoNothing();
                            }
                        }   
                    };
                "
            };

            // Condition is null
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (int x = 0;;x++){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(true) {
                                DoNothing();
                                x++;
                            }
                        }   
                    };
                "
            };

            // Initialization is null
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (;x < 10;x++){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            while(x < 10) {
                                DoNothing();
                                x++;
                            }
                        }   
                    };
                "
            };

            // All statements/expressions present
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (int x = 0;x < 10;x++){
                                DoNothing();
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(x < 10) {
                                DoNothing();
                                x++;
                            }
                        }   
                    };
                "
            };

            // Nested for statements
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (int x = 0;x < 10;x++){
                                for(int y = 0; y < 10; y++) {
                                    DoNothing(x, y);
                                }
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            int x = 0;
                            while(x < 10) {
                                int y = 0;
                                while(y < 10) {
                                    DoNothing(x, y);
                                    y++;
                                }
                                x++;    
                            }
                        }   
                    };
                "
            };

            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (;;){
                                for(;;) {
                                    DoNothing();
                                }
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            while(true) {
                                while(true) {
                                    DoNothing();
                                }
                            }
                        }   
                    };
                "
            };

            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (;;){
                                for(int x = 0;;x++) {
                                    DoNothing(x);
                                }
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            while(true) {
                                int x = 0;
                                while(true) {
                                    DoNothing(x);
                                    x++;            
                                }
                            }
                        }   
                    };
                "
            };
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        testFunction: function() {
                            for (;;){
                                for(int x = 1;x < 10;) {
                                    DoNothing(x);
                                }
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        testFunction: function() {
                            while(true) {
                                int x = 1;
                                while(x < 10) {
                                    DoNothing(x);
                                }
                            }
                        }   
                    };
                "
            };
        }
    }
}
