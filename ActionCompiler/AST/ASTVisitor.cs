using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Action.Parser;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Action.AST
{
    public class ASTVisitor : ActionBaseVisitor<object>
    {

        public override List<ComplexNode> VisitFile([NotNull] ActionParser.FileContext context)
        {
            return context.map_or_section()
                .Select(this.Visit)
                .Cast<ComplexNode>()
                .ToList();
        }


        /*
         *  Complex objects
         */
        public override object VisitMap([NotNull] ActionParser.MapContext context)
        {
            IdentifierNode identifier = (IdentifierNode)this.Visit(context.IDENTIFIER());
            List<PropertyNode> properties = context.section_properties()
                .Select(this.Visit)
                .Cast<PropertyNode>()
                .ToList();
            List<ValueNode> sections = context.section_statements()
                .Select(this.Visit)
                .Cast<ValueNode>()
                .ToList();
            return new MapNode(identifier, properties, sections);
        }

        public override SectionNode VisitSection([NotNull] ActionParser.SectionContext context)
        {
            CoordinateNode? coords = context.POINT_LIT() is null
                ? null
                : (CoordinateNode)this.Visit(context.POINT_LIT());
            IdentifierNode? identifier = context.IDENTIFIER() is null
                ? null
                : (IdentifierNode)this.Visit(context.IDENTIFIER());
            List<PropertyNode> properties = context.section_properties()
                .Select(this.Visit)
                .Cast<PropertyNode>()
                .ToList();
            List<ValueNode> sections = context.section_statements()
                .Select(this.Visit)
                .Cast<ValueNode>()
                .ToList();
            return new SectionNode(coords, identifier, properties, sections);
        }

        public override ComplexNode VisitColour([NotNull] ActionParser.ColourContext context)
        {
            return ComplexNode<ColourKeywordNode>(context.colour_properties, Empty);
        }

        public override object VisitImage([NotNull] ActionParser.ImageContext context)
        {
            return ComplexNode<ImageKeywordNode>(context.image_properties, Empty);
        }

        public override object VisitBox([NotNull] ActionParser.BoxContext context)
        {
            return ComplexNode<BoxKeywordNode>(context.box_properties, Empty);
        }

        public override object VisitLine([NotNull] ActionParser.LineContext context)
        {
            return ComplexNode<LineKeywordNode>(Empty, context.point_statements);
        }

        public override object VisitCoordinates([NotNull] ActionParser.CoordinatesContext context)
        {
            return ComplexNode<CoordinatesKeywordNode>(Empty, context.point_statements);
        }

        public override object VisitPoint_shape([NotNull] ActionParser.Point_shapeContext context)
        {
            return new PointKeywordNode();
        }

        public override object VisitReference_section([NotNull] ActionParser.Reference_sectionContext context)
        {
            return new ReferenceNode(
                new SectionKeywordNode(),
                (IdentifierNode)this.Visit(context.IDENTIFIER()),
                (CoordinateNode)this.Visit(context.POINT_LIT())
            );
        }

        private static IParseTree[] Empty() => Array.Empty<IParseTree>();

        private ComplexNode ComplexNode<T>(Func<IParseTree[]> properties, Func<IParseTree[]> statements)
            where T : IdentifierNode, new()
        {
            return new ComplexNode(
                new T(),
                properties()
                    .Select(this.Visit)
                    .Cast<PropertyNode>()
                    .ToList(),
                statements()
                    .Select(this.Visit)
                    .Cast<ValueNode>()
                    .ToList()
            );
        }

        /*
         *  Properties
         */
        #region properties
        public override PropertyNode VisitHex_property([NotNull] ActionParser.Hex_propertyContext context)
        {
            return new PropertyNode(
                new HexKeywordNode(),
                (ColourNode)this.Visit(context.COLOUR_LIT())
            );
        }

        public override object VisitHeight_property([NotNull] ActionParser.Height_propertyContext context)
        {
            return new PropertyNode(
                new HeightKeywordNode(),
                (ColourNode)this.Visit(context.HEIGHT())
            );
        }

        public override object VisitWidth_property([NotNull] ActionParser.Width_propertyContext context)
        {
            return new PropertyNode(
                new WidthKeywordNode(),
                (ColourNode)this.Visit(context.WIDTH())
            );
        }

        public override object VisitShape_property([NotNull] ActionParser.Shape_propertyContext context)
        {
            return new PropertyNode(
                new ShapeKeywordNode(),
                (ValueNode)this.Visit(context.shape_values())
            );
        }

        public override PropertyNode VisitBackground_property([NotNull] ActionParser.Background_propertyContext context)
        {
            return new PropertyNode(
                new BackgroundKeywordNode(),
                (ValueNode)this.Visit(context.background_values())
            );
        }

        public override PropertyNode VisitPath_property([NotNull] ActionParser.Path_propertyContext context)
        {
            return new PropertyNode(
                new PathKeywordNode(),
                (ValueNode)this.Visit(context.STRING())
            );
        }
        #endregion

        /*
         *  Terminals
         */
        #region terminals
        public override object VisitTerminal(ITerminalNode node)
        {
            ActionToken token = (ActionToken)node.Symbol.Type;
            return token switch
            {
                ActionToken.STRING => VisitString(node),
                ActionToken.POINT_LIT => VisitPoint(node),
                ActionToken.IDENTIFIER => VisitIdentifier(node),
                ActionToken.INTEGER => VisitInteger(node),
                ActionToken.COLOUR_LIT => VisitColour(node),
                _ => base.VisitTerminal(node),
            };
        }
        private static readonly Regex hexColourRegex = new Regex(
            @"#([a-f0-9]{2})([a-f0-9]{2})([a-f0-9]{2})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static ColourNode VisitColour(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.COLOUR_LIT);
            Match result = hexColourRegex.Match(node.GetText());
            Debug.Assert(result.Success);
            return new ColourNode(
                byte.Parse(result.Groups[1].ValueSpan, NumberStyles.HexNumber),
                byte.Parse(result.Groups[2].ValueSpan, NumberStyles.HexNumber),
                byte.Parse(result.Groups[3].ValueSpan, NumberStyles.HexNumber)
            );
        }
        private static readonly Regex stringRegex = new Regex(
            @"""([^""]*)""|'([^']*)'",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static StringNode VisitString(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.STRING);
            Match result = stringRegex.Match(node.GetText());
            Debug.Assert(result.Success);
            return new StringNode(result.Groups[1].Value);
        }

        private static readonly Regex pointRegex = new Regex(
            @"(-?[0-9]+)\s*,\s*(-?[0-9]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static CoordinateNode VisitPoint(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.POINT_LIT);
            Match result = pointRegex.Match(node.GetText());
            Debug.Assert(result.Success);
            return new CoordinateNode(
                new IntNode(int.Parse(result.Groups[1].Value)),
                new IntNode(int.Parse(result.Groups[2].Value))
            );
        }

        private static IntNode VisitInteger(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.INTEGER);
            return new IntNode(int.Parse(node.GetText()));
        }

        private static IdentifierNode VisitIdentifier(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.IDENTIFIER);
            return new IdentifierNode(node.GetText());
        }
        #endregion

        // So, the way the visitor works by default: it'll visit every child rule in order
        // then return the last result.
        // For example, for this rule:
        //      colour : COLOUR OPEN_BRACE (colour_properties)* CLOSE_BRACE;
        // the default impl. of VisitColour will see these children:
        // COLOUR -> VisitTerminal
        // OPEN_BRACE -> VisitTerminal
        // colour_properties -> VisitColour_properties (for each colour_properties)
        // CLOSE_BRACE -> VisitTerminal
        //
        // and it will return the result of the last one.
        //
        // This is not ideal, for rules like this:
        //      colour_properties : hex_property SEMICOLON;
        //
        // Since (assumedly) we override hex_property, to return something interesting
        // BUT VisitTerminal on SEMICOLON does not return anything interesting.
        //
        // We are given two ways to influence this: overriding ShouldVisitNextChild
        // and overriding AggregateResult.
        //
        // We override aggregate result to, in essence, discard null values.
        // If we're ever given two non-null values, we're not overriding something
        // like VisitColour, which we should.
        // So we fail.
        protected override object? AggregateResult(object aggregate, object nextResult)
        {
            return (aggregate, nextResult) switch
            {
                (null, null) => null,
                (_, null) => aggregate,
                (null, _) => nextResult,
                (_, _) => throw new InvalidOperationException("Aggregating two non-null objects - did you forget to override a visitor method?")
            };
        }
    }
}