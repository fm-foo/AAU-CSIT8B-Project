using Antlr4.Runtime;
using Action.Parser;
using Action.AST;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Collections.Generic;
using System;

namespace Action.Compiler
{
    public class ActionCompiler
    {
        public ActionCompiler()
        {
        }

        public CompilationResult Compile(Stream input, ILogger<ActionCompiler>? logger = null)
        {
            logger ??= NullLogger<ActionCompiler>.Instance;
            logger.BeginScope("compilation");
            logger.LogInformation("Beginning compilation");
            List<DiagnosticResult> diagnostics = new List<DiagnosticResult>();
            List<ComplexNode>? ast = Parse(input, logger, diagnostics);
            if (ast is null)
                return CompilationResult.Failure(diagnostics);

            bool valid = SemanticsErrorCheck(ast, logger, diagnostics);
            if (!valid)
                return CompilationResult.Failure(diagnostics);
            throw new NotImplementedException();
        }

        // add diagnostics to the list if there's warnings/errors
        // return null if parsing did not succeed
        private List<ComplexNode>? Parse(Stream input, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("parsing");
            logger.LogInformation("Beginning parse");
            ICharStream stream = new AntlrInputStream(input);
            ITokenSource lexer = new ActionLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            ActionParser parser = new ActionParser(tokens);
            parser.BuildParseTree = true;
            ActionParser.FileContext tree = parser.file();
            var visitor = new ASTVisitor();
            return visitor.VisitFile(tree);
        }

        // put semantics error checking here
        // recommendation: create a Visitor pattern class
        // maybe subclass it for different types of errors?
        private bool SemanticsErrorCheck(List<ComplexNode> ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {

            return true;
        }
    }

}