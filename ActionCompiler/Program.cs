using Antlr4.Runtime;
using Action.Parser;
using System;
using Action.AST;
using System.IO;
using Action.Compiler;
using Microsoft.Extensions.Logging;

namespace Action
{
    public class Program
    {
        public static void Main()
        {
            using Stream stream = new FileStream("map examples semantic errors.txt", FileMode.Open);
            using var factory = LoggerFactory.Create(builder => builder.AddConsole());
            var compiler = new ActionCompiler();
            var result = compiler.Compile(stream, factory.CreateLogger<ActionCompiler>());
            foreach (var diagnostic in result.Diagnostics)
            Console.WriteLine(diagnostic);
        }
    }
}