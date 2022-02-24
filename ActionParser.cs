//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Action.g4 by ANTLR 4.9.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.3")]
[System.CLSCompliant(false)]
public partial class ActionParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		WHITESPACE=1, SINGLELINE_COMMENT=2, MULTILINE_COMMENT=3, TOKEN=4, OPEN_BRACE=5, 
		CLOSE_BRACE=6, SEMICOLON=7, COLON=8, KEYWORD=9, STRING=10, DOUBLE_QUOTED_STRING=11, 
		SINGLE_QUOTED_STRING=12, COORDINATES=13, IDENTIFIER=14, INTEGER=15, NATURAL_NUMBER=16, 
		COLOUR=17;
	public const int
		RULE_token = 0;
	public static readonly string[] ruleNames = {
		"token"
	};

	private static readonly string[] _LiteralNames = {
		null, null, null, null, null, "'{'", "'}'", "';'", "':'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "WHITESPACE", "SINGLELINE_COMMENT", "MULTILINE_COMMENT", "TOKEN", 
		"OPEN_BRACE", "CLOSE_BRACE", "SEMICOLON", "COLON", "KEYWORD", "STRING", 
		"DOUBLE_QUOTED_STRING", "SINGLE_QUOTED_STRING", "COORDINATES", "IDENTIFIER", 
		"INTEGER", "NATURAL_NUMBER", "COLOUR"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Action.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string SerializedAtn { get { return new string(_serializedATN); } }

	static ActionParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public ActionParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public ActionParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class TokenContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] TOKEN() { return GetTokens(ActionParser.TOKEN); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TOKEN(int i) {
			return GetToken(ActionParser.TOKEN, i);
		}
		public TokenContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_token; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IActionListener typedListener = listener as IActionListener;
			if (typedListener != null) typedListener.EnterToken(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IActionListener typedListener = listener as IActionListener;
			if (typedListener != null) typedListener.ExitToken(this);
		}
	}

	[RuleVersion(0)]
	public TokenContext token() {
		TokenContext _localctx = new TokenContext(Context, State);
		EnterRule(_localctx, 0, RULE_token);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 3;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			do {
				{
				{
				State = 2;
				Match(TOKEN);
				}
				}
				State = 5;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			} while ( _la==TOKEN );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static char[] _serializedATN = {
		'\x3', '\x608B', '\xA72A', '\x8133', '\xB9ED', '\x417C', '\x3BE7', '\x7786', 
		'\x5964', '\x3', '\x13', '\n', '\x4', '\x2', '\t', '\x2', '\x3', '\x2', 
		'\x6', '\x2', '\x6', '\n', '\x2', '\r', '\x2', '\xE', '\x2', '\a', '\x3', 
		'\x2', '\x2', '\x2', '\x3', '\x2', '\x2', '\x2', '\x2', '\t', '\x2', '\x5', 
		'\x3', '\x2', '\x2', '\x2', '\x4', '\x6', '\a', '\x6', '\x2', '\x2', '\x5', 
		'\x4', '\x3', '\x2', '\x2', '\x2', '\x6', '\a', '\x3', '\x2', '\x2', '\x2', 
		'\a', '\x5', '\x3', '\x2', '\x2', '\x2', '\a', '\b', '\x3', '\x2', '\x2', 
		'\x2', '\b', '\x3', '\x3', '\x2', '\x2', '\x2', '\x3', '\a',
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
