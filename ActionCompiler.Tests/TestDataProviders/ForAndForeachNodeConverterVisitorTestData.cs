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

        public static IEnumerable<object[]> ForEachTestData()
        {
            yield return new object[]
           {
                @"
                    entity TestEntity {
                        int[] intArray = [1, 2, 3];
                        testFunction: function() {
                            foreach(int x in intArray) {
                                DoMath(x);
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        int[] intArray = [1, 2, 3];
                        testFunction: function() {
                                int index = 0;
                                int length = intArray.Length();
                                while(index < length) {
                                    int x = intArray[index];
                                    DoMath(x);
                                    index++;
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
                        string[] stringArray = [""1"", ""2"", ""3""];
                        testFunction: function() {
                            foreach(string x in stringArray) {
                                Use(x);
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        string[] stringArray = [""1"", ""2"", ""3""];
                        testFunction: function() {
                                int index = 0;
                                int length = stringArray.Length();
                                while(index < length) {
                                    string x = stringArray[index];
                                    Use(x);
                                    index++;
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
                        MyOwnEntity[] entityArray = [new MyOwnEntity(1), new MyOwnEntity(2), new MyOwnEntity(3)];
                        testFunction: function() {
                            foreach(MyOwnEntity x in entityArray) {
                                Use(x);
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        MyOwnEntity[] entityArray = [new MyOwnEntity(1), new MyOwnEntity(2), new MyOwnEntity(3)];
                        testFunction: function() {
                                int index = 0;
                                int length = entityArray.Length();
                                while(index < length) {
                                    MyOwnEntity x = entityArray[index];
                                    Use(x);
                                    index++;
                                }
                            }
                        }   
                    };
                "
            };
        }

        public static IEnumerable<object[]> CombinedTestData()
        {
            yield return new object[]
            {
                @"
                    entity TestEntity {
                        int[] intArray = [1, 2, 3];
                        testFunction: function() {
                            foreach(int x in intArray) {
                                for (int i = 0; i < 10; i++) {
                                    DoStuff(x, i);
                                }
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        int[] intArray = [1, 2, 3];
                        testFunction: function() {
                                int index = 0;
                                int length = intArray.Length();
                                while(index < length) {
                                    int x = intArray[index];
                                    int i = 0;
                                    while(i < 10) {
                                        DoStuff(x, i);
                                        i++;
                                    }
                                    index++;
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
                        int[] intArray = [1, 2, 3];
                        string[] stringArray = [""1"", ""2"", ""3""];
                        testFunction: function() {
                            foreach(int x in intArray) {
                                for (int i = 0; i < 10; i++) {
                                    foreach(string s in stringArray){
                                        DoStuff(x, i, s);
                                    }
                                }
                            }
                        }   
                    };
                ",
                @"
                    entity TestEntity {
                        int[] intArray = [1, 2, 3];
                        string[] stringArray = [""1"", ""2"", ""3""];
                        testFunction: function() {
                                int index = 0;
                                int length = intArray.Length();
                                while(index < length) {
                                    int x = intArray[index];
                                    int i = 0;
                                    while(i < 10) {
                                        int index = 0;
                                        int length = stringArray.Length();
                                        while(index < length) {
                                            string s = stringArray[index];
                                            DoStuff(x, i, s);
                                            index++;
                                        }
                                        i++;
                                    }
                                    index++;
                                }
                            }
                        }   
                    };
                "
            };
        }
    }
}
