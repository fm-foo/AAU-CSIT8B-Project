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
        public static void Main()
        {
            using Stream stream = new FileStream(@"ExamplePrograms\entity.txt", FileMode.Open);
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
