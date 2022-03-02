using Antlr4.Runtime;
using ActionCompiler.Parser;
using System;
using ActionCompiler.AST;

namespace ActionCompiler
{
    public class Program
    {
        public static void Main()
        {
            ICharStream stream = new AntlrFileStream("map examples.txt");
            ITokenSource lexer = new ActionLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            ActionParser parser = new ActionParser(tokens);
            parser.BuildParseTree = true;
            ActionParser.FileContext tree = parser.file();
            var visitor = new ASTVisitor();
            var nodes = visitor.VisitFile(tree);
            foreach (var node in nodes)
            {
                Console.WriteLine(node);
            }
        }
    }
}