using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

public class Program
{
    public static void Main()
    {
        ICharStream stream = new AntlrFileStream("map examples.txt");
        ITokenSource lexer = new ActionLexer(stream);
        ITokenStream tokens = new CommonTokenStream(lexer);
        ActionParser parser = new ActionParser(tokens);
        parser.BuildParseTree = true;
        IParseTree tree = parser.token();
        var listener = new TestListener();
        ParseTreeWalker.Default.Walk(listener, tree);
    }
}


public class TestListener : ActionBaseListener
{
    public override void EnterToken([NotNull] ActionParser.TokenContext context)
    {
        Console.WriteLine(context.GetText());
    }
}