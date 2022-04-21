using Antlr4.Runtime;
using Action.Parser;
using System;
using Action.AST;
using System.IO;
using Action.Compiler;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace Action
{
    public class Program
    {
        private static readonly string mapExampleTxt = @"ExamplePrograms\map examples.txt";
        private static readonly string semanticErrorTxt = @"ExamplePrograms\SemanticErrors\semanticErrors.txt";
        private static readonly string linesTxt = @"ExamplePrograms\lines.txt";
        private static readonly string entity =  @"ExamplePrograms\entity.txt";


        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: program [filename]");
                return 1;
            }
            FileInfo file = new FileInfo(args.First());
            if (!file.Exists)
            {
                Console.WriteLine($"file {file.FullName} not found");
                return 2;
            }
            using Stream stream = file.OpenRead();
            using var factory = LoggerFactory.Create(builder => builder.AddConsole());
            var compiler = new Compiler.ActionCompiler();
            var result = compiler.Compile(stream, factory.CreateLogger<Compiler.ActionCompiler>());
            if (result.Success)
            {
                Debug.Assert(result.Success);
                foreach (var image in result.Images)
                {
                    FileInfo output = new FileInfo(image.filename);
                    using var fs = output.Create();
                    image.file.CopyTo(fs);
                }
            }
            foreach (var diagnostic in result.Diagnostics)
                Console.WriteLine(diagnostic);
            return 0;
        }
    }
}
