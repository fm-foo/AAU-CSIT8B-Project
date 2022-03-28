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
    public class ASTGenerator : ActionBaseVisitor<object>
    {

        public override FileNode VisitFile([NotNull] ActionParser.FileContext context)
        {
            return new FileNode(context.map_or_section()
                .Select(this.Visit)
                .Cast<ComplexNode>()
                .ToList());
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

        public override object VisitEntity([NotNull] ActionParser.EntityContext context)
        {
            IdentifierNode identifier = (IdentifierNode)this.Visit(context.IDENTIFIER());

            List<FieldDecNode> fieldNodes = context.field_dec().Select(this.Visit).Cast<FieldDecNode>().ToList();

            List<PropertyNode> funcDecs = context.func_def().Select(this.Visit).Cast<PropertyNode>().ToList();
            return new EntityNode(identifier, fieldNodes, funcDecs);
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
            return ComplexNode<LineKeywordNode>(Empty, context.coord_statements);
        }

        public override object VisitCoordinates([NotNull] ActionParser.CoordinatesContext context)
        {
            return ComplexNode<CoordinatesKeywordNode>(Empty, context.coord_statements);
        }

        public override object VisitUnit_shape([NotNull] ActionParser.Unit_shapeContext context)
        {
            throw new NotImplementedException();
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

        #region func_def

        public override object VisitFunc_def([NotNull] ActionParser.Func_defContext context)
        {
            IdentifierNode identifier = (IdentifierNode)this.Visit(context.IDENTIFIER());
            FunctionArgumentsNode arguments = (FunctionArgumentsNode)this.Visit(context.func_def_args());
            BlockNode block = (BlockNode)this.Visit(context.block());

            FunctionNode function = new(arguments, block);
            
            return new PropertyNode(identifier, function);
        }

        public override object VisitBlock([NotNull] ActionParser.BlockContext context)
        {
            List<StatementNode> statements = context.statement().Select(this.Visit).Cast<StatementNode>().ToList();

            return new BlockNode(statements);
        }


        public override object VisitFunc_def_args([NotNull] ActionParser.Func_def_argsContext context)
        {
            FunctionArgumentNode functionArgument = (FunctionArgumentNode)this.Visit(context.func_def_arg());

            if (context.func_def_args() is not null)
            {
                FunctionArgumentsNode otherArguments = (FunctionArgumentsNode)this.Visit(context.func_def_args());
                List<FunctionArgumentNode> args = otherArguments.args;
                args.Add(functionArgument);
                return new FunctionArgumentsNode(args);
            }

            return new FunctionArgumentsNode(new List<FunctionArgumentNode>() { functionArgument});
        }

        public override object VisitFunc_def_arg([NotNull] ActionParser.Func_def_argContext context)
        {
            IdentifierNode identifierNode = (IdentifierNode)this.Visit(context.IDENTIFIER());
            TypeNode type = (TypeNode)this.Visit(context.type());


            return new FunctionArgumentNode(identifierNode, type);
        }

        public override object VisitStatement([NotNull] ActionParser.StatementContext context)
        {
            // TODO: StatementNode
            return new StatementNode();
        }

        #endregion

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
                (IntNode)this.Visit(context.INTEGER())
            );
        }

        public override object VisitWidth_property([NotNull] ActionParser.Width_propertyContext context)
        {
            return new PropertyNode(
                new WidthKeywordNode(),
                (IntNode)this.Visit(context.INTEGER())
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

        public override object VisitField_dec([NotNull] ActionParser.Field_decContext context)
        {
            IdentifierNode identifierNode = (IdentifierNode)this.Visit(context.IDENTIFIER());
            TypeNode type = (TypeNode)this.Visit(context.type());

            if (context.expr() is not null)
            {
                return new FieldDecNode(identifierNode, type, (ExprNode)this.Visit(context.expr()));
            }
            else
            {
                return new FieldDecNode(identifierNode, type);
            }
        }
        #endregion

        public override object VisitExpr([NotNull] ActionParser.ExprContext context)
        {
            return this.Visit(context.bool_expr());
        }

        #region bool_expr
        public override object VisitEq_expr([NotNull] ActionParser.Eq_exprContext context)
        {
            //return new BoolExprNode((EqualityExprNode)this.Visit(context.equality_expr()));
            return this.Visit(context.equality_expr());
        }

        public override object VisitAndand_expr([NotNull] ActionParser.Andand_exprContext context)
        {
            //EqualityExprNode equalityExprNode = (EqualityExprNode)this.Visit(context.equality_expr());
            ExprNode expr = (ExprNode)this.Visit(context.equality_expr());
            //BoolExprNode boolExprNode = (BoolExprNode)this.Visit(context.bool_expr());
            ExprNode boolExprNode = (ExprNode)this.Visit(context.bool_expr());

            return new BoolExprNode(expr, boolExprNode, BooleanOperator.AND);
        }

        public override object VisitOror_expr([NotNull] ActionParser.Oror_exprContext context)
        {
            //EqualityExprNode equalityExprNode = (EqualityExprNode)this.Visit(context.equality_expr());
            ExprNode expr = (ExprNode)this.Visit(context.equality_expr());
            //BoolExprNode boolExprNode = (BoolExprNode)this.Visit(context.bool_expr());
            ExprNode boolExprNode = (ExprNode)this.Visit(context.bool_expr());

            return new BoolExprNode(expr, boolExprNode, BooleanOperator.OR);
        }

        #endregion
        #region equality_expr

        public override object VisitRel_expr([NotNull] ActionParser.Rel_exprContext context)
        {
            //return new EqualityExprNode((RelationalExprNode)this.Visit(context.relational_expr()));
            return this.Visit(context.relational_expr());
        }

        public override object VisitEqualsequals_expr([NotNull] ActionParser.Equalsequals_exprContext context)
        {
            //RelationalExprNode relationalExpr = (RelationalExprNode)this.Visit(context.relational_expr());
            ExprNode expr = (ExprNode)this.Visit(context.relational_expr());
            //EqualityExprNode equalityExpr = (EqualityExprNode)this.Visit(context.equality_expr());
            ExprNode equalityExpr = (ExprNode)this.Visit(context.equality_expr());

            return new EqualityExprNode(expr, equalityExpr, EqualityOperator.EQUALS);
        }

        public override object VisitNotequals_expr([NotNull] ActionParser.Notequals_exprContext context)
        {
            //RelationalExprNode relationalExpr = (RelationalExprNode)this.Visit(context.relational_expr());
            ExprNode expr = (ExprNode)this.Visit(context.relational_expr());
            //EqualityExprNode equalityExpr = (EqualityExprNode)this.Visit(context.equality_expr());
            ExprNode equalityExpr = (ExprNode)this.Visit(context.equality_expr());

            return new EqualityExprNode(expr, equalityExpr, EqualityOperator.NOTEQUALS);
        }

        #endregion
        #region relational_expr

        public override object VisitAdd_expr([NotNull] ActionParser.Add_exprContext context)
        {
            //return new RelationalExprNode((AdditiveExprNode)this.Visit(context.additive_expr()));
            return this.Visit(context.additive_expr());

        }

        public override object VisitLessthan_expr([NotNull] ActionParser.Lessthan_exprContext context)
        {
           // AdditiveExprNode additiveExpr = (AdditiveExprNode)this.Visit(context.additive_expr());
            ExprNode expr = (ExprNode)this.Visit(context.additive_expr());
            // RelationalExprNode relationalExpr = (RelationalExprNode)this.Visit(context.relational_expr());
            ExprNode relationalExpr = (ExprNode)this.Visit(context.relational_expr());

            return new RelationalExprNode(expr, relationalExpr, RelationalOper.LESSTHAN);
        }

        public override object VisitGreaterthan_expr([NotNull] ActionParser.Greaterthan_exprContext context)
        {
            // AdditiveExprNode additiveExpr = (AdditiveExprNode)this.Visit(context.additive_expr());
            ExprNode expr = (ExprNode)this.Visit(context.additive_expr());
            // RelationalExprNode relationalExpr = (RelationalExprNode)this.Visit(context.relational_expr());
            ExprNode relationalExpr = (ExprNode)this.Visit(context.relational_expr());

            return new RelationalExprNode(expr, relationalExpr, RelationalOper.GREATERTHAN);
        }

        public override object VisitLessthanequal_expr([NotNull] ActionParser.Lessthanequal_exprContext context)
        {
            // AdditiveExprNode additiveExpr = (AdditiveExprNode)this.Visit(context.additive_expr());
            ExprNode expr = (ExprNode)this.Visit(context.additive_expr());
            // RelationalExprNode relationalExpr = (RelationalExprNode)this.Visit(context.relational_expr());
            ExprNode relationalExpr = (ExprNode)this.Visit(context.relational_expr());

            return new RelationalExprNode(expr, relationalExpr, RelationalOper.LESSTHANOREQUAL);
        }

        public override object VisitGreaterthanequal_expr([NotNull] ActionParser.Greaterthanequal_exprContext context)
        {
            // AdditiveExprNode additiveExpr = (AdditiveExprNode)this.Visit(context.additive_expr());
            ExprNode expr = (ExprNode)this.Visit(context.additive_expr());
            // RelationalExprNode relationalExpr = (RelationalExprNode)this.Visit(context.relational_expr());
            ExprNode relationalExpr = (ExprNode)this.Visit(context.relational_expr());

            return new RelationalExprNode(expr, relationalExpr, RelationalOper.GREATERTHANOREQUAL);
        }

        #endregion
        #region additive_expr

        public override object VisitMult_expr([NotNull] ActionParser.Mult_exprContext context)
        {
             //return new AdditiveExprNode((MultiplicativeExprNode)this.Visit(context.multiplicative_expr()));
             return this.Visit(context.multiplicative_expr());

        }

        public override object VisitPlus_expr([NotNull] ActionParser.Plus_exprContext context)
        {
            // MultiplicativeExprNode multiplicativeExpr = (MultiplicativeExprNode)this.Visit(context.multiplicative_expr());
            ExprNode expr = (ExprNode)this.Visit(context.multiplicative_expr());
            //AdditiveExprNode additiveExpr = (AdditiveExprNode)this.Visit(context.additive_expr());
            ExprNode additiveExpr = (ExprNode)this.Visit(context.additive_expr());

            return new AdditiveExprNode(expr, additiveExpr, AdditiveOper.PLUS);
        }

        public override object VisitMinus_expr([NotNull] ActionParser.Minus_exprContext context)
        {
            // MultiplicativeExprNode multiplicativeExpr = (MultiplicativeExprNode)this.Visit(context.multiplicative_expr());
            ExprNode expr = (ExprNode)this.Visit(context.multiplicative_expr());
            //AdditiveExprNode additiveExpr = (AdditiveExprNode)this.Visit(context.additive_expr());
            ExprNode additiveExpr = (ExprNode)this.Visit(context.additive_expr());

            return new AdditiveExprNode(expr, additiveExpr, AdditiveOper.MINUS);
        }

        #endregion
        #region multiplicative_expr

        public override object VisitUn_expr([NotNull] ActionParser.Un_exprContext context)
        {
            //return new MultiplicativeExprNode((UnaryExprNode)this.Visit(context.unary_expr()));
            return this.Visit(context.unary_expr());
        }

        public override object VisitTimes_expr([NotNull] ActionParser.Times_exprContext context)
        {
            //UnaryExprNode unaryExpr = (UnaryExprNode)this.Visit(context.unary_expr());
            ExprNode expr = (ExprNode)this.Visit(context.unary_expr());
            //MultiplicativeExprNode multiplicativeExpr = (MultiplicativeExprNode)(this.Visit(context.multiplicative_expr()));
            ExprNode multiplicativeExpr = (ExprNode)this.Visit(context.multiplicative_expr());

            return new MultiplicativeExprNode(expr, multiplicativeExpr, MultOper.TIMES);
        }

        public override object VisitDivide_expr([NotNull] ActionParser.Divide_exprContext context)
        {
            //UnaryExprNode unaryExpr = (UnaryExprNode)this.Visit(context.unary_expr());
            ExprNode expr = (ExprNode)this.Visit(context.unary_expr());
            //MultiplicativeExprNode multiplicativeExpr = (MultiplicativeExprNode)(this.Visit(context.multiplicative_expr()));
            ExprNode multiplicativeExpr = (ExprNode)this.Visit(context.multiplicative_expr());

            return new MultiplicativeExprNode(expr, multiplicativeExpr, MultOper.DIV);
        }

        #endregion
        #region unary_expr

        public override object VisitPrim_expr([NotNull] ActionParser.Prim_exprContext context)
        {
            //return new UnaryExprNode((ValueNode)this.Visit(context.primary_expr()));
            return this.Visit(context.primary_expr());
        }

        public override object VisitPlus_unary_expr([NotNull] ActionParser.Plus_unary_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)(this.Visit(context.unary_expr()));

            return new UnaryExprNode(unaryExpr, UnaryOper.PLUS);
        }

        public override object VisitMinus_unary_expr([NotNull] ActionParser.Minus_unary_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)(this.Visit(context.unary_expr()));

            return new UnaryExprNode(unaryExpr, UnaryOper.MINUS);
        }

        public override object VisitPlusplus_expr([NotNull] ActionParser.Plusplus_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.PLUSPLUS);
        }

        public override object VisitMinusminus_expr([NotNull] ActionParser.Minusminus_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.MINUSMINUS);
        }

        public override object VisitBang_expr([NotNull] ActionParser.Bang_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.BANG);
        }

        #endregion
        #region primary_expr   

        // TODO: missing postfix_increment, postfix_decrement, func_call, member_access, typeof_expr, new_object

        public override object VisitLit([NotNull] ActionParser.LitContext context)
        {
            //return this.Visit(context.literal());
            return new PrimaryExprNode((ValueNode)this.Visit(context.literal()));
        }

        public override object VisitIdentifier([NotNull] ActionParser.IdentifierContext context)
        {
            // return this.Visit(context.IDENTIFIER());
            return new PrimaryExprNode((ValueNode)this.Visit(context.IDENTIFIER()));
        }

        public override object VisitParens_expr([NotNull] ActionParser.Parens_exprContext context)
        {
            return this.Visit(context.expr());
        }

        #endregion

        // Could also label the different types in Action.g4 (like expressions)
        public override object VisitType([NotNull] ActionParser.TypeContext context)
        {
            // This should always work, but it is ugly

            //if (context.FLOAT() is not null)
            //{
            //    return new TypeNode(TypeEnum.FLOAT);
            //}
            //else if (context.INT() is not null)
            //{
            //    return new TypeNode(TypeEnum.INT);
            //}
            //else if (context.STRING_KW() is not null)
            //{
            //    return new TypeNode(TypeEnum.STRING);
            //}
            //else if (context.BOOL() is not null)
            //{
            //    return new TypeNode(TypeEnum.BOOL);
            //}
            //else if (context.COORD() is not null)
            //{
            //    return new TypeNode(TypeEnum.COORD);
            //}
            //else if (context.IDENTIFIER() is not null)
            //{
            //    return new TypeNode(TypeEnum.IDENTIFIER);
            //}
            //else
            //{
            //    throw new Exception($"Unknown type!");
            //}

            // This is nicer, but could break if the grammar changes
            //switch (context.GetText())
            //{
            //    case "float":
            //        return new TypeNode(TypeEnum.FLOAT);
            //    case "int":
            //        return new TypeNode(TypeEnum.INT);
            //    case "string":
            //        return new TypeNode(TypeEnum.STRING);
            //    case "bool":
            //        return new TypeNode(TypeEnum.BOOL);
            //    case "coord":
            //        return new TypeNode(TypeEnum.COORD);
            //    default:
            //        return new TypeNode(TypeEnum.IDENTIFIER);
            //}


            // My understanding is if we are in this method, there will only be one child node of type ITerminalNode. If that is correct, this should work, otherwise it will probably break.
            List<IParseTree> children = context.children.ToList();
            Debug.Assert(children.Count == 1);
            
            IParseTree node = children[0];
            Debug.Assert(node is ITerminalNode);

            ITerminalNode terminalNode = (ITerminalNode)node;
            ActionToken token = (ActionToken)terminalNode.Symbol.Type;

            return token switch
            {
                ActionToken.FLOAT => new TypeNode(TypeEnum.FLOAT),
                ActionToken.INT => new TypeNode(TypeEnum.INT),
                ActionToken.STRING_KW => new TypeNode(TypeEnum.STRING),
                ActionToken.BOOL => new TypeNode(TypeEnum.BOOL),
                ActionToken.COORD => new TypeNode(TypeEnum.COORD),
                ActionToken.IDENTIFIER => new TypeNode(TypeEnum.IDENTIFIER),
                _ => throw new Exception($"Invalid token: {token}")
            };

        }

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
                ActionToken.BOOL_LIT => VisitBoolean(node),
                _ => base.VisitTerminal(node),
            };
        }
        private static readonly Regex hexColourRegex = new(
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
        private static readonly Regex stringRegex = new(
            @"""([^""]*)""|'([^']*)'",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static StringNode VisitString(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.STRING);
            Match result = stringRegex.Match(node.GetText());
            Debug.Assert(result.Success);
            return new StringNode(result.Groups[1].Value);
        }

        private static readonly Regex pointRegex = new(
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

        private static BoolNode VisitBoolean(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.BOOL_LIT);
            return new BoolNode(bool.Parse(node.GetText()));

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