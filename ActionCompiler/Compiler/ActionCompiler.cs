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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;

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

            return CompileToImages(ast, logger, diagnostics);
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

        private CompilationResult CompileToImages(List<ComplexNode> ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("compile");
            logger.LogInformation("Compiling to image");
            List<ImageFile> images = new List<ImageFile>();
            bool success = true;
            foreach (MapNode map in ast.Cast<MapNode>())
            {
                var imagefile = CompileMap(map, logger, diagnostics);
                if (imagefile is not null)
                    images.Add(imagefile);
                else
                    success = false;
            }
            if (success)
                return CompilationResult.Succeed(images, diagnostics);
            else
                return CompilationResult.Failure(diagnostics);
        }

        private ImageFile? CompileMap(MapNode map, 
            ILogger<ActionCompiler> logger, 
            List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("mapping");
            logger.LogInformation("Compiling map: {}", map.identifier);
            var tilemap = CompileSection(map);
            using Image<Argb32>? image = CopyToImage(tilemap, diagnostics);
            if (image is null)
                return null;
            Stream stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return new ImageFile(map.identifier.identifier + ".png", stream);
        }

        private const int TileSize = 50;
        private Image<Argb32>? CopyToImage(Tile[,] tiles, List<DiagnosticResult> diagnostics)
        {
            var width = tiles.GetLength(0) * TileSize;
            var height = tiles.GetLength(1) * TileSize;
            Image<Argb32> image = new Image<Argb32>(width, height, new Argb32(0, 0, 0, 0));
            tiles.ForEach((x, y, tile) =>
            {
                if (tile is EmptyTile)
                {
                    // do nothing
                } else if (tile is ColourTile c)
                {
                    IPath path = new RectangularPolygon(
                        x*TileSize, y*TileSize, TileSize, TileSize
                    );
                    Color color = new Color(new Rgb24(c.r, c.g, c.b));
                    image.Mutate(i => i.Fill(color, path));
                }
                else if (tile is ImageTile i)
                {
                    // do nothing - for now
                    // few notes
                    // the image should be opened and an error should be added if it doesn't exist
                    // return null if there's an error
                    // the image should be resized to 50x50, and a warning diagnostic should be added if it's not 50x50
                }
                else
                    throw new InvalidOperationException();
            });
            return image;
        }

        private Tile[,] CompileSection(ComplexNode node)
        {
            Debug.Assert(node is SectionNode or MapNode);
            (int height, int width) = GetDimensions(node);
            // index as map[x, y]
            Tile[,] map = new Tile[width, height];
            Tile backgroundTile = GetBackgroundTile(node);
            FillShape(map, node, backgroundTile);
            foreach (var section in node.values.Cast<SectionNode>())
            {
                var submap = CompileSection(section);
                var coords = section.coords;
                Debug.Assert(coords is not null);
                submap.CopyTo(map, coords.x.integer, coords.y.integer);
            }
            return map;
        }

        private static Tile GetBackgroundTile(ComplexNode node)
        {
            Debug.Assert(node is SectionNode or MapNode);
            var background = node.GetProperty<BackgroundKeywordNode, ComplexNode>();
            if (background is { type: ColourKeywordNode })
            {
                var colour = background.GetProperty<HexKeywordNode, ColourNode>();
                return new ColourTile(colour.r, colour.g, colour.b);
            }
            else if (background is { type: ImageKeywordNode })
            {
                var file = background.GetProperty<PathKeywordNode, StringNode>();
                return new ImageTile(file.s);
            }
            else
            {
                throw new NotImplementedException(background.type.ToString());
            }
        }

        private static void FillShape(Tile[,] map, ComplexNode node, Tile tile)
        {
            var shape = node.GetProperty<ShapeKeywordNode, ValueNode>();
            if (shape is PointKeywordNode or ComplexNode { type: BoxKeywordNode })
            {
                map.Fill(tile);
            }
            else if (shape is ComplexNode { type: CoordinatesKeywordNode, values: var coords})
            {
                map.Fill(new EmptyTile());
                foreach (CoordinateNode point in coords.Cast<CoordinateNode>())
                {
                    map[point.x.integer, point.y.integer] = tile;
                }
            }
            else if (shape is ComplexNode { type: LineKeywordNode, values: var linecoords})
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        private static (int height, int width) GetDimensions(ComplexNode node)
        {
            var shape = node.properties
                .Where(n => n.identifier is ShapeKeywordNode)
                .Select(n => n.value)
                .Single();
            if (shape is PointKeywordNode)
                return (1, 1);
            var cshape = (ComplexNode)shape;
            return cshape switch
            {
                { type: BoxKeywordNode } => GetDimensionsBox(cshape),
                { type: LineKeywordNode } => GetDimensionsCoords(cshape),
                { type: CoordinatesKeywordNode } => GetDimensionsCoords(cshape),
                _ => throw new NotImplementedException(cshape.type.ToString())
            };
        }

        private static (int height, int width) GetDimensionsBox(ComplexNode shape)
        {
            Debug.Assert(shape.type is BoxKeywordNode);
            IntNode height = shape.GetProperty<HeightKeywordNode, IntNode>();
            IntNode width = shape.GetProperty<WidthKeywordNode, IntNode>();
            return (height.integer, width.integer);
        }

        private static (int height, int width) GetDimensionsCoords(ComplexNode shape)
        {
            Debug.Assert(shape.type is CoordinatesKeywordNode or LineKeywordNode);
            throw new InvalidOperationException();
        }
    }

    public abstract record Tile;
    public record EmptyTile : Tile;
    public record ColourTile(byte r, byte g, byte b) : Tile;
    public record ImageTile(string file) : Tile;


    public record ImageFile(string filename, Stream file);
}