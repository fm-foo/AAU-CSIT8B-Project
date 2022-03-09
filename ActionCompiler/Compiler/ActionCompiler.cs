using Antlr4.Runtime;
using Action.Parser;
using Action.AST;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using Antlr4.Runtime.Dfa;
using Antlr4.Runtime.Sharpen;
using Antlr4.Runtime.Atn;

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

            ast = ResolveReferences(ast, logger, diagnostics);
            if (ast is null)
                return CompilationResult.Failure(diagnostics);

            ast = TrimSections(ast, logger);
                
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
            var listener = new TestErrorListener();
            parser.RemoveErrorListeners();
            parser.AddErrorListener(listener);
            ActionParser.FileContext tree = parser.file();
            var visitor = new ASTGenerator();
            return visitor.VisitFile(tree);
        }

        // put semantics error checking here
        // recommendation: create a Visitor pattern class
        // maybe subclass it for different types of errors?
        private bool SemanticsErrorCheck(List<ComplexNode> ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            return true;
        }

        private List<ComplexNode>? ResolveReferences(List<ComplexNode> ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("reference");
            logger.LogInformation("Resolving references");
            var visitor = new SectionSymbolTableGenerator();
            var symboltable = visitor.Visit(ast).ToHashSet();
            Debug.Assert(visitor.Visit(ast).Count() == symboltable.Count);
            List<ComplexNode> newnodes = new List<ComplexNode>();
            bool valid = true;
            foreach (ComplexNode node in ast)
            {   
                // the stack scope stuff should balance itself
                // but we create a new one each time just to make sure
                // TODO: can we check if it's balanced using a disposable/finalizer check at the end? 
                ReferenceResolverVisitor? resolver = new ReferenceResolverVisitor(symboltable, diagnostics);
                ComplexNode? newnode = resolver.Visit(node);
                if (newnode is null)
                    valid = false;
                else
                    newnodes.Add(newnode);
            }
            return valid ? newnodes : null;
        }

        private List<ComplexNode> TrimSections(List<ComplexNode> ast, ILogger<ActionCompiler> logger)
        {
            using var scope = logger.BeginScope("trimming");
            logger.LogInformation("Section trimming");
            SectionTrimmerVisitor trimmer = new SectionTrimmerVisitor();
            List<ComplexNode> newnodes = new List<ComplexNode>();
            foreach (var node in ast)
            {
                var newnode = trimmer.Visit(node);
                if (newnode is not null)
                    newnodes.Add(newnode);
            }
            return newnodes;
        }
    }

    public class TestErrorListener : BaseErrorListener
    {
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, 
            int line, int charPositionInLine, string msg, RecognitionException e)
        {
            ;
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}