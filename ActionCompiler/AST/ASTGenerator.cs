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

        public override object VisitGame([NotNull] ActionParser.GameContext context)
        {
            IdentifierNode identifier = (IdentifierNode)this.Visit(context.IDENTIFIER());

            List<FieldDecNode> fieldNodes = context.field_dec().Select(this.Visit).Cast<FieldDecNode>().ToList();

            List<PropertyNode> funcDecs = context.func_def().Select(this.Visit).Cast<PropertyNode>().ToList();
            return new GameNode(identifier, fieldNodes, funcDecs);
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
            List<FunctionArgumentNode> args = new List<FunctionArgumentNode>();

            if (context.func_def_args() is not null)
            {
                args = (List<FunctionArgumentNode>)this.Visit(context.func_def_args());
            }

            BlockNode block = (BlockNode)this.Visit(context.block());

            FunctionNode function = new(args, block);
            
            return new PropertyNode(identifier, function);
        }

        public override object VisitBlock([NotNull] ActionParser.BlockContext context)
        {
            List<StatementNode> statements = context.statement().Select(this.Visit).Cast<StatementNode>().ToList();

            return new BlockNode(statements);
        }


        public override object VisitFunc_def_args([NotNull] ActionParser.Func_def_argsContext context)
        {
            List<FunctionArgumentNode> args = new List<FunctionArgumentNode>();

            GetArgumentList(args, context);

            return args;
        }

        private void GetArgumentList(List<FunctionArgumentNode> args, ActionParser.Func_def_argsContext context)
        {
            args.Add((FunctionArgumentNode)this.Visit(context.func_def_arg()));

            if (context.func_def_args() is not null)
            {
                GetArgumentList(args, context.func_def_args());
            }
        }

        public override object VisitFunc_def_arg([NotNull] ActionParser.Func_def_argContext context)
        {
            IdentifierNode identifierNode = (IdentifierNode)this.Visit(context.IDENTIFIER());
            TypeNode type = (TypeNode)this.Visit(context.type());

            return new FunctionArgumentNode(identifierNode, type);
        }

        #region Statements
        public override object VisitStatement([NotNull] ActionParser.StatementContext context)
        {
            if (context.block() != null)
            {
                return (StatementNode)this.Visit(context.block());
            }
            else if (context.@if() is not null)
            {
                return (StatementNode)this.Visit(context.@if());
            }
            else if (context.@while() is not null)
            {
                return (StatementNode)this.Visit(context.@while());
            }
            else if (context.@for() is not null)
            {
                return (StatementNode)this.Visit(context.@for());
            }
            else if (context.@foreach() is not null)
            {
                return (StatementNode)this.Visit(context.@foreach());
            }
            else if (context.semicolon_statement() is not null)
            {
                return (StatementNode)this.Visit(context.semicolon_statement());
            }
            else
            {
                throw new Exception("Unknown statement!");
            }
        }

        public override object VisitIf([NotNull] ActionParser.IfContext context)
        {
            ExprNode expr = (ExprNode)this.Visit(context.expr());

            StatementNode statement = (StatementNode)this.Visit(context.statement());

            if (context.ELSE() != null)
            {
                StatementNode elseStatement = (StatementNode)this.Visit(context.else_statement());
                return new IfStatementNode(expr, statement, elseStatement);
            }

            return new IfStatementNode(expr, statement);
            
        }

        public override object VisitWhile([NotNull] ActionParser.WhileContext context)
        {
            ExprNode expr = (ExprNode)this.Visit(context.expr());
            StatementNode statement = (StatementNode)this.Visit(context.statement());

            return new WhileStatementNode(expr, statement);
        }

        public override object VisitFor([NotNull] ActionParser.ForContext context)
        {
            StatementNode? initialization = null;
            if (context.initialization() is not null)
            {
                initialization = (StatementNode)this.Visit(context.initialization());
            }

            ExprNode? condition = null;
            if (context.cond_expr() is not null)
            {
                condition = (ExprNode?)this.Visit(context.cond_expr());
            }

            ExprNode? control = null;
            if (context.control_expr() is not null)
            {
                control = (ExprNode)this.Visit(context.control_expr());
            }

            StatementNode statement = (StatementNode)this.Visit(context.statement());

            return new ForStatementNode(statement, initialization, condition, control);    
        }

        public override object VisitForeach([NotNull] ActionParser.ForeachContext context)
        {
            TypeNode type = (TypeNode)this.Visit(context.type());
            IdentifierNode identifier = (IdentifierNode)this.Visit(context.IDENTIFIER());
            ExprNode expr = (ExprNode)this.Visit(context.expr());
            StatementNode statement = (StatementNode)this.Visit(context.statement());

            return new ForeachStatementNode(type, identifier, expr, statement);
        }

        public override object VisitDeclaration([NotNull] ActionParser.DeclarationContext context)
        {
            TypeNode type = (TypeNode)this.Visit(context.type());
            IdentifierNode identifier = (IdentifierNode)this.Visit(context.IDENTIFIER());

            if (context.expr() is not null)
            {
                ExprNode expr = (ExprNode)this.Visit(context.expr());
                return new DeclarationNode(type, identifier, expr);
            }

            return new DeclarationNode(type, identifier);
        }

        public override object VisitAssignment([NotNull] ActionParser.AssignmentContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.left_expr());
            ExprNode right = (ExprNode)this.Visit(context.right_expr());
            
            return new AssignmentNode(left, right);
        }

        public override object VisitSemicolon_statement([NotNull] ActionParser.Semicolon_statementContext context)
        {
            if (context.declaration() is not null)
            {
                return this.Visit(context.declaration());
            }
            else if (context.assignment() is not null)
            {
                return this.Visit(context.assignment());
            }
            else if (context.expr() is not null)
            {
                return new ExpressionStatementNode((ExprNode)this.Visit(context.expr()));
            }
            else
            {
                throw new Exception("Unknown SemicolonStatement!");
            }
        }

        public override object VisitInitialization([NotNull] ActionParser.InitializationContext context)
        {
            return context.assignment() is not null? this.Visit(context.assignment()) : this.Visit(context.declaration());
        }

        public override object VisitCond_expr([NotNull] ActionParser.Cond_exprContext context)
        {
            return (ExprNode)this.Visit(context.expr());
        }

        public override object VisitControl_expr([NotNull] ActionParser.Control_exprContext context)
        {
            return (ExprNode)this.Visit(context.expr());
        }
        #endregion

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
            return this.Visit(context.equality_expr());
        }

        public override object VisitAndand_expr([NotNull] ActionParser.Andand_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.bool_expr());
            ExprNode right = (ExprNode)this.Visit(context.equality_expr());

            return new BoolExprNode(left, right, BooleanOperator.AND);
        }

        public override object VisitOror_expr([NotNull] ActionParser.Oror_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.bool_expr());
            ExprNode right = (ExprNode)this.Visit(context.equality_expr());

            return new BoolExprNode(left, right, BooleanOperator.OR);
        }

        #endregion
        #region equality_expr

        public override object VisitRel_expr([NotNull] ActionParser.Rel_exprContext context)
        {
            return this.Visit(context.relational_expr());
        }

        public override object VisitEqualsequals_expr([NotNull] ActionParser.Equalsequals_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.equality_expr());
            ExprNode right = (ExprNode)this.Visit(context.relational_expr());

            return new EqualityExprNode(left, right, EqualityOperator.EQUALS);
        }

        public override object VisitNotequals_expr([NotNull] ActionParser.Notequals_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.equality_expr());
            ExprNode right = (ExprNode)this.Visit(context.relational_expr());

            return new EqualityExprNode(left, right, EqualityOperator.NOTEQUALS);
        }

        #endregion
        #region relational_expr

        public override object VisitAdd_expr([NotNull] ActionParser.Add_exprContext context)
        {
            return this.Visit(context.additive_expr());
        }

        public override object VisitLessthan_expr([NotNull] ActionParser.Lessthan_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.relational_expr());
            ExprNode right = (ExprNode)this.Visit(context.additive_expr());

            return new RelationalExprNode(left, right, RelationalOper.LESSTHAN);
        }

        public override object VisitGreaterthan_expr([NotNull] ActionParser.Greaterthan_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.relational_expr());
            ExprNode right = (ExprNode)this.Visit(context.additive_expr());

            return new RelationalExprNode(left, right, RelationalOper.GREATERTHAN);
        }

        public override object VisitLessthanequal_expr([NotNull] ActionParser.Lessthanequal_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.relational_expr());
            ExprNode right = (ExprNode)this.Visit(context.additive_expr());

            return new RelationalExprNode(left, right, RelationalOper.LESSTHANOREQUAL);
        }

        public override object VisitGreaterthanequal_expr([NotNull] ActionParser.Greaterthanequal_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.relational_expr());
            ExprNode right = (ExprNode)this.Visit(context.additive_expr());

            return new RelationalExprNode(left, right, RelationalOper.GREATERTHANOREQUAL);
        }

        public override object VisitIs_expr([NotNull] ActionParser.Is_exprContext context)
        {
            ExprNode relationalExpr = (ExprNode)this.Visit(context.relational_expr());
            TypeNode type = (TypeNode)this.Visit(context.type());

            return new IsNode(relationalExpr, type);
        }

        #endregion
        #region additive_expr

        public override object VisitMult_expr([NotNull] ActionParser.Mult_exprContext context)
        {
             return this.Visit(context.multiplicative_expr());

        }

        public override object VisitPlus_expr([NotNull] ActionParser.Plus_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.additive_expr());
            ExprNode right = (ExprNode)this.Visit(context.multiplicative_expr());

            return new AdditiveExprNode(left, right, AdditiveOper.PLUS);
        }

        public override object VisitMinus_expr([NotNull] ActionParser.Minus_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.additive_expr());
            ExprNode right = (ExprNode)this.Visit(context.multiplicative_expr());

            return new AdditiveExprNode(left, right, AdditiveOper.MINUS);
        }

        #endregion
        #region multiplicative_expr

        public override object VisitUn_expr([NotNull] ActionParser.Un_exprContext context)
        {
            return this.Visit(context.unary_expr());
        }

        public override object VisitTimes_expr([NotNull] ActionParser.Times_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.multiplicative_expr());
            ExprNode right = (ExprNode)this.Visit(context.unary_expr());

            return new MultiplicativeExprNode(left, right, MultOper.TIMES);
        }

        public override object VisitDivide_expr([NotNull] ActionParser.Divide_exprContext context)
        {
            ExprNode left = (ExprNode)this.Visit(context.multiplicative_expr());
            ExprNode right = (ExprNode)this.Visit(context.unary_expr());

            return new MultiplicativeExprNode(left, right, MultOper.DIV);
        }

        #endregion
        #region unary_expr

        public override object VisitPrim_expr([NotNull] ActionParser.Prim_exprContext context)
        {
            return this.Visit(context.primary_expr());
        }

        public override object VisitPlus_unary_expr([NotNull] ActionParser.Plus_unary_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.PLUS);
        }

        public override object VisitMinus_unary_expr([NotNull] ActionParser.Minus_unary_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.MINUS);
        }

        public override object VisitPlusplus_expr([NotNull] ActionParser.Plusplus_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.INCREMENT);
        }

        public override object VisitMinusminus_expr([NotNull] ActionParser.Minusminus_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.DECREMENT);
        }

        public override object VisitBang_expr([NotNull] ActionParser.Bang_exprContext context)
        {
            ExprNode unaryExpr = (ExprNode)this.Visit(context.unary_expr());

            return new UnaryExprNode(unaryExpr, UnaryOper.NEGATE);
        }

        #endregion
        #region primary_expr   

        // TODO: missing member_access, typeof_expr, new_object

        public override object VisitLit([NotNull] ActionParser.LitContext context)
        {
            //return this.Visit(context.literal());
            return (ValueNode)this.Visit(context.literal());
        }

        public override object VisitIdentifier([NotNull] ActionParser.IdentifierContext context)
        {
            // return this.Visit(context.IDENTIFIER());
            return (ValueNode)this.Visit(context.IDENTIFIER());
        }

        public override object VisitParens_expr([NotNull] ActionParser.Parens_exprContext context)
        {
            return this.Visit(context.expr());
        }

        public override object VisitArray_access([NotNull] ActionParser.Array_accessContext context)
        {
            ExprNode arrayExpr = (ExprNode)this.Visit(context.primary_expr());
            ExprNode expr = (ExprNode)this.Visit(context.expr());

            return new ArrayAccessNode(arrayExpr, expr);
        }

        public override object VisitArray_creation([NotNull] ActionParser.Array_creationContext context)
        {
            ExprNode[] arrayValues = ((List<ExprNode>)this.Visit(context.array_values())).ToArray();
            return new ArrayNode(arrayValues);
        }


        public override object VisitArray_values([NotNull] ActionParser.Array_valuesContext context)
        {
            if (context.array_values() is not null)
            {
                List<ExprNode> exprNodes = (List<ExprNode>)this.Visit(context.array_values());
                ExprNode expr = (ExprNode)this.Visit(context.expr());
                exprNodes.Insert(0, expr);

                return exprNodes;
            }

            return new List<ExprNode>() {(ExprNode)this.Visit(context.expr())};
        }

        public override object VisitPostfix_increment([NotNull] ActionParser.Postfix_incrementContext context) {
            ExprNode expr = (ExprNode) this.Visit(context.primary_expr());
            return new PostFixExprNode(expr, PostFixOperator.PLUSPLUS);
        }

        public override object VisitPostfix_decrement([NotNull] ActionParser.Postfix_decrementContext context) {
            ExprNode expr = (ExprNode)this.Visit(context.primary_expr());
            return new PostFixExprNode(expr, PostFixOperator.MINUSMINUS);
        }

        public override object VisitFunc_call([NotNull] ActionParser.Func_callContext context) {
            ExprNode expr = (ExprNode)this.Visit(context.primary_expr());
            List<ExprNode> exprArgs = new();

            if (context.func_args() is not null) {
                exprArgs = (List<ExprNode>) this.Visit(context.func_args());
            }

            return new FunctionCallExprNode(expr, exprArgs);
        }

        public override object VisitFunc_args([NotNull] ActionParser.Func_argsContext context) {
            List<ExprNode> exprArgs = new ();

            GetFunctionArgsList(exprArgs, context);

            return exprArgs;
        }

        private void GetFunctionArgsList(List<ExprNode> exprArgs, ActionParser.Func_argsContext context) {
            exprArgs.Add((ExprNode)this.Visit(context.expr()));
            if (context.func_args() is not null) {
                GetFunctionArgsList(exprArgs, context.func_args());
            }
        }


        #endregion
        #region Types

        public override object VisitInt_type([NotNull] ActionParser.Int_typeContext context)
        {
            return new IntTypeNode();
        }

        public override object VisitFloat_type([NotNull] ActionParser.Float_typeContext context)
        {
            return new FloatTypeNode();
        }

        public override object VisitBool_type([NotNull] ActionParser.Bool_typeContext context)
        {
            return new BoolTypeNode();
        }

        public override object VisitString_type([NotNull] ActionParser.String_typeContext context)
        {
            return new StringTypeNode();
        }

        public override object VisitCoord_type([NotNull] ActionParser.Coord_typeContext context)
        {
            return new CoordTypeNode();
        }

        public override object VisitSimple_type([NotNull] ActionParser.Simple_typeContext context)
        {
            return new SimpleTypeNode((IdentifierNode)this.Visit(context.IDENTIFIER()));
        }

        public override object VisitArray_type([NotNull] ActionParser.Array_typeContext context)
        {
            return new ArrayTypeNode((TypeNode)this.Visit(context.type()));
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
                ActionToken.BOOL_LIT => VisitBoolean(node),
                ActionToken.FLOAT_LIT => VisitFloat(node),
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

        private object VisitFloat(ITerminalNode node)
        {
            Debug.Assert((ActionToken)node.Symbol.Type == ActionToken.FLOAT_LIT);
            return new FloatNode(float.Parse(node.GetText()));
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