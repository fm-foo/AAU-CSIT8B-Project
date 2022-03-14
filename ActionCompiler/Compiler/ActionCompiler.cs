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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;

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
            FileNode? ast = Parse(input, logger, diagnostics);
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
        private FileNode? Parse(Stream input, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("parsing");
            logger.LogInformation("Beginning parse");
            ICharStream stream = new AntlrInputStream(input);
            ITokenSource lexer = new ActionLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            ActionParser parser = new ActionParser(tokens);
            parser.BuildParseTree = true;
            var listener = new TestErrorListener(diagnostics);
            //parser.RemoveErrorListeners();
            parser.AddErrorListener(listener);
            ActionParser.FileContext tree = parser.file();
            var visitor = new ASTGenerator();
            return visitor.VisitFile(tree);
        }

        // put semantics error checking here
        // recommendation: create a Visitor pattern class
        // maybe subclass it for different types of errors?
        private bool SemanticsErrorCheck(FileNode ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            var visitors = new NodeVisitor<IEnumerable<DiagnosticResult>>[]
            {
                new SemErrorNoIdentifierSectionVisitor(),
                new SemErrorEmptyBackgroundVisitor(),
                new SemErrorCoordinateSectionVisitor(),
                new SemErrorEmptySizeBoxVisitor(),
                new SemErrorMapWithoutBSVisitor(),
                new SemErrorSectionOffMapVisitor(),
            };
            foreach (var visitor in visitors)
            {
                var result = visitor.Visit(ast);
                diagnostics.AddRange(result);
            }
            return !diagnostics.Any(error => error.severity == Severity.Error);
        }

        private FileNode? ResolveReferences(FileNode ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("reference");
            logger.LogInformation("Resolving references");
            var visitor = new SectionSymbolTableGenerator();
            var symboltable = visitor.Visit(ast).ToHashSet();
            Debug.Assert(visitor.Visit(ast).Count() == symboltable.Count);
            List<ComplexNode> newnodes = new List<ComplexNode>();
            bool valid = true;
            foreach (ComplexNode node in ast.nodes)
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
            return valid ? new FileNode(newnodes) : null;
        }

        private FileNode TrimSections(FileNode ast, ILogger<ActionCompiler> logger)
        {
            using var scope = logger.BeginScope("trimming");
            logger.LogInformation("Section trimming");
            SectionTrimmerVisitor trimmer = new SectionTrimmerVisitor();
            List<ComplexNode> newnodes = new List<ComplexNode>();
            foreach (var node in ast.nodes)
            {
                var newnode = trimmer.Visit(node);
                if (newnode is not null)
                    newnodes.Add(newnode);
            }
            return new FileNode(newnodes);
        }

        private CompilationResult CompileToImages(FileNode ast, ILogger<ActionCompiler> logger, List<DiagnosticResult> diagnostics)
        {
            using var scope = logger.BeginScope("compile");
            logger.LogInformation("Compiling to image");
            List<ImageFile> images = new List<ImageFile>();
            bool success = true;
            foreach (MapNode map in ast.nodes.Cast<MapNode>())
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
        private Image<Argb32>? CopyToImage(Section tiles, List<DiagnosticResult> diagnostics)
        {
            var width = tiles.Width * TileSize;
            var height = tiles.Height * TileSize;
            Image<Argb32>? image = new Image<Argb32>(width, height, new Argb32(0, 0, 0, 0));
            tiles.ForEach((x, y, tile) =>
            {
                if (image is null)
                    return; //Break out of ForEach

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
                    // few notes
                    // the image should be opened and an error should be added if it doesn't exist
                    // return null if there's an error
                    // the image should be resized to 50x50, and a warning diagnostic should be added if it's not 50x50
                    try
                    {
                        using Image inputImage = Image.Load(i.file);

                        if (inputImage.Width != TileSize || inputImage.Height != TileSize)
                        {
                            diagnostics.Add(new DiagnosticResult(Severity.Warning, $"Input image \"{i.file}\" has wrong dimensions! Width: {inputImage.Width}px, Height: {inputImage.Height}px, Required: Width: {TileSize}px, Height: {TileSize}px!"));
                            inputImage.Mutate(i => i.Resize(new Size(TileSize, TileSize)));
                        }

                        Point location = new Point(x * TileSize, y * TileSize);   

                        image.Mutate(i =>
                            i.DrawImage(inputImage, location, 1f)
                        );

                    }
                    catch (FileNotFoundException exception)
                    {
                        diagnostics.Add(new DiagnosticResult(Severity.Error, exception.Message));
                        image.Dispose();
                        image = null; // Cannot directly return value 
                    }
                }
                else
                    throw new InvalidOperationException();
            });
            return image;
        }

        private Section CompileSection(ComplexNode node)
        {
            Debug.Assert(node is SectionNode or MapNode);
            // index as map[x, y]
            Section map = GetDimensions(node);
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

        private static void FillShape(Section map, ComplexNode node, Tile tile)
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
                Debug.Assert(linecoords.All(c => c is CoordinateNode));

                map.Fill(new EmptyTile());
                List<(CoordinateNode, CoordinateNode)> coordinates = linecoords
                    .Cast<CoordinateNode>()
                    .Zip(linecoords.Cast<CoordinateNode>().Skip(1), (a, b) => (a, b))
                    .ToList(); 
                    // Get a list of coordinate pairs: (a, b, c) -> ((a, b), (b, c))

                foreach ((CoordinateNode p1, CoordinateNode p2) in coordinates)
                {
                    List<CoordinateNode> points = GetLinePoints(p1, p2);
                    foreach (CoordinateNode p in points)
                    {
                        map[p.x.integer, p.y.integer] = tile;
                    }
                }

               // throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the points that are interescted by the line drawn between <paramref name="p0"/> and <paramref name="p1"/>
        /// Based on: https://www.redblobgames.com/grids/line-drawing.html
        /// </summary>
        /// <param name="p0">First point.</param>
        /// <param name="p1">Second point.</param>
        /// <returns></returns>
        private static List<CoordinateNode> GetLinePoints(CoordinateNode p0, CoordinateNode p1)
        {
            List<CoordinateNode> points = new();

            int dx = p1.x.integer - p0.x.integer;
            int dy = p1.y.integer - p0.y.integer;
            int nx = Math.Abs(dx);
            int ny = Math.Abs(dy);
            int sign_x = dx > 0 ? 1 : -1;
            int sign_y = dy > 0 ? 1 : -1;

            CoordinateNode p = new CoordinateNode(new IntNode(p0.x.integer), new IntNode(p0.y.integer));
            points.Add(p);

            for ((int ix, int iy) = (0, 0); ix < nx || iy < ny;)
            {
                int decision = (1 + 2 * ix) * ny - (1 + 2 * iy) * nx;
                if (decision == 1)
                {
                    p = new CoordinateNode(new IntNode(p.x.integer + sign_x), new IntNode(p.y.integer + sign_y));
                    ix++;
                    iy++;
                }
                else if (decision < 0)
                {
                    p = new CoordinateNode(new IntNode(p.x.integer + sign_x), new IntNode(p.y.integer));
                    ix++;
                }
                else
                {
                    p = new CoordinateNode(new IntNode(p.x.integer), new IntNode(p.y.integer + sign_y));
                    iy++;
                }
                points.Add(p);
            }

            points.Add(p1);

            return points;
        }

        private static Section GetDimensions(ComplexNode node)
        {
            var shape = node.properties
                .Where(n => n.identifier is ShapeKeywordNode)
                .Select(n => n.value)
                .Single();
            if (shape is PointKeywordNode)
                return new Section(0, 0, 1, 1);
            var cshape = (ComplexNode)shape;
            return cshape switch
            {
                { type: BoxKeywordNode } => GetDimensionsBox(cshape),
                { type: LineKeywordNode } => GetDimensionsCoords(cshape),
                { type: CoordinatesKeywordNode } => GetDimensionsCoords(cshape),
                _ => throw new NotImplementedException(cshape.type.ToString())
            };
        }

        private static Section GetDimensionsBox(ComplexNode shape)
        {
            Debug.Assert(shape.type is BoxKeywordNode);
            IntNode height = shape.GetProperty<HeightKeywordNode, IntNode>();
            IntNode width = shape.GetProperty<WidthKeywordNode, IntNode>();
            return new Section(0, 0, width.integer, height.integer);
        }

        private static Section GetDimensionsCoords(ComplexNode shape)
        {
            Debug.Assert(shape.type is CoordinatesKeywordNode or LineKeywordNode);
            Debug.Assert(shape.values.All(c => c is CoordinateNode));
            // For now, get the size of a bounding box that encompasses all lines
            List<CoordinateNode> coordinates = shape.values.Cast<CoordinateNode>().ToList();

            int heightMin = coordinates.Min(c => c.y.integer);
            int heightMax = coordinates.Max(c => c.y.integer);

            int height = heightMax - heightMin;

            int widthMin = coordinates.Min(c => c.x.integer);
            int widthMax = coordinates.Max(c => c.x.integer);

            int width = widthMax - widthMin;

            return new Section(widthMin, heightMin, width + 1, height + 1);
        }
    }

    public class Section
    {

        private readonly Tile[,] tiles;

        public int Width { get; }
        public int Height { get; }
        public int WidthMin { get; }
        public int HeightMin { get; }

        public Section(int xmin, int ymin, int xsize, int ysize)
        {
            tiles = new Tile[xsize, ysize];
            WidthMin = xmin;
            HeightMin = ymin;
            Width = xsize;
            Height = ysize;
        }

        public void Fill(Tile tile)
        {
            ForEach((x, y, _) => this[x, y] = tile);
        }

        public void ForEach(Action<int, int, Tile> action)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                    action(x + WidthMin, y + HeightMin, tiles[x, y]);
        }

        public void CopyTo(Section dest, int x, int y)
        {
            for (int source_x = 0; source_x < tiles.GetLength(0); source_x++)
            {
                for (int source_y = 0; source_y < tiles.GetLength(1); source_y++)
                {
                    int dest_x = source_x + x + WidthMin;
                    int dest_y = source_y + y + HeightMin;
                    if (dest_x >= 0
                        && dest_y >= 0 
                        && dest_x < dest.Width
                        && dest_y < dest.Height)
                    {
                        Tile srctile = tiles[source_x, source_y];
                        Tile desttile = dest[dest_x, dest_y];
                        if (srctile is not EmptyTile
                            && desttile is not EmptyTile)
                        {
                            dest[dest_x, dest_y] = srctile; 
                        }
                        
                    }
                }
            }
        }

        public Tile this[int x, int y]
        {
            get => tiles[x - WidthMin, y - HeightMin];
            set => tiles[x - WidthMin, y - HeightMin] = value;
        }
    }

    public abstract record Tile;
    public record EmptyTile : Tile;
    public record ColourTile(byte r, byte g, byte b) : Tile;
    public record ImageTile(string file) : Tile;


    public record ImageFile(string filename, Stream file);
}