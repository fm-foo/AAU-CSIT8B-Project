using Antlr4.Runtime;
using Action.Parser;
using System;
using Action.AST;
using System.IO;
using Action.Compiler;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Action
{
    public class Program
    {
        private static readonly string mapExampleTxt = @"ExamplePrograms\map examples.txt";
        private static readonly string semanticErrorTxt = @"ExamplePrograms\SemanticErrors\semanticErrors.txt";
        private static readonly string linesTxt = @"ExamplePrograms\lines.txt";
        private static readonly string entity =  @"ExamplePrograms\entity.txt";


        public static void Main()
        {
            using Stream stream = new FileStream(semanticErrorTxt, FileMode.Open);
            using var factory = LoggerFactory.Create(builder => builder.AddConsole());
            var compiler = new Compiler.ActionCompiler();
            var result = compiler.Compile(stream, factory.CreateLogger<Compiler.ActionCompiler>());
            if (result.Success)
            {
                Debug.Assert(result.Success);
                foreach (var image in result.Images)
                {
                    FileInfo file = new FileInfo(image.filename);
                    using var fs = file.Create();
                    image.file.CopyTo(fs);
                }
            }
            foreach (var diagnostic in result.Diagnostics)
                Console.WriteLine(diagnostic);
        }
    }
}
