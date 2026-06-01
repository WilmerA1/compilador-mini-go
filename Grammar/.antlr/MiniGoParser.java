// Generated from c:/Users/Wil/OneDrive - Estudiantes ITCR/Semestre I 2026/Compi/proyecto/MiniGoCompiler/Grammar/MiniGo.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue"})
public class MiniGoParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		T__31=32, T__32=33, T__33=34, T__34=35, T__35=36, T__36=37, T__37=38, 
		T__38=39, T__39=40, T__40=41, T__41=42, T__42=43, T__43=44, T__44=45, 
		T__45=46, T__46=47, T__47=48, T__48=49, T__49=50, T__50=51, T__51=52, 
		T__52=53, T__53=54, T__54=55, T__55=56, T__56=57, T__57=58, T__58=59, 
		T__59=60, T__60=61, T__61=62, T__62=63, T__63=64, IDENTIFIER=65, FLOATLITERAL=66, 
		INTLITERAL=67, RUNELITERAL=68, RAWSTRINGLITERAL=69, INTERPRETEDSTRINGLITERAL=70, 
		LINE_COMMENT=71, BLOCK_COMMENT=72, WS=73, ERROR_CHAR=74;
	public static final int
		RULE_root = 0, RULE_topDeclarationList = 1, RULE_variableDecl = 2, RULE_innerVarDecls = 3, 
		RULE_singleVarDecl = 4, RULE_singleVarDeclNoExps = 5, RULE_typeDecl = 6, 
		RULE_innerTypeDecls = 7, RULE_singleTypeDecl = 8, RULE_funcDecl = 9, RULE_funcFrontDecl = 10, 
		RULE_funcArgDecls = 11, RULE_declType = 12, RULE_sliceDeclType = 13, RULE_arrayDeclType = 14, 
		RULE_structDeclType = 15, RULE_structMemDecls = 16, RULE_identifierList = 17, 
		RULE_expression = 18, RULE_expressionList = 19, RULE_primaryExpression = 20, 
		RULE_operand = 21, RULE_literal = 22, RULE_index = 23, RULE_arguments = 24, 
		RULE_selector = 25, RULE_appendExpression = 26, RULE_lengthExpression = 27, 
		RULE_capExpression = 28, RULE_statementList = 29, RULE_block = 30, RULE_statement = 31, 
		RULE_simpleStatement = 32, RULE_assignmentStatement = 33, RULE_ifStatement = 34, 
		RULE_elseClause = 35, RULE_loopStatement = 36, RULE_switchStatement = 37, 
		RULE_expressionCaseClauseList = 38, RULE_expressionCaseClause = 39, RULE_expressionSwitchCase = 40;
	private static String[] makeRuleNames() {
		return new String[] {
			"root", "topDeclarationList", "variableDecl", "innerVarDecls", "singleVarDecl", 
			"singleVarDeclNoExps", "typeDecl", "innerTypeDecls", "singleTypeDecl", 
			"funcDecl", "funcFrontDecl", "funcArgDecls", "declType", "sliceDeclType", 
			"arrayDeclType", "structDeclType", "structMemDecls", "identifierList", 
			"expression", "expressionList", "primaryExpression", "operand", "literal", 
			"index", "arguments", "selector", "appendExpression", "lengthExpression", 
			"capExpression", "statementList", "block", "statement", "simpleStatement", 
			"assignmentStatement", "ifStatement", "elseClause", "loopStatement", 
			"switchStatement", "expressionCaseClauseList", "expressionCaseClause", 
			"expressionSwitchCase"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'package'", "';'", "'var'", "'('", "')'", "'='", "'type'", "'func'", 
			"','", "'['", "']'", "'struct'", "'{'", "'}'", "'+'", "'-'", "'!'", "'^'", 
			"'*'", "'/'", "'%'", "'<<'", "'>>'", "'&'", "'&^'", "'|'", "'=='", "'!='", 
			"'<'", "'<='", "'>'", "'>='", "'&&'", "'||'", "'.'", "'append'", "'len'", 
			"'cap'", "'print'", "'println'", "'return'", "'break'", "'continue'", 
			"':='", "'++'", "'--'", "'+='", "'-='", "'*='", "'/='", "'%='", "'&='", 
			"'|='", "'^='", "'<<='", "'>>='", "'&^='", "'if'", "'else'", "'for'", 
			"'switch'", "':'", "'case'", "'default'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, null, null, null, null, null, null, null, 
			null, null, null, null, null, "IDENTIFIER", "FLOATLITERAL", "INTLITERAL", 
			"RUNELITERAL", "RAWSTRINGLITERAL", "INTERPRETEDSTRINGLITERAL", "LINE_COMMENT", 
			"BLOCK_COMMENT", "WS", "ERROR_CHAR"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "MiniGo.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public MiniGoParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@SuppressWarnings("CheckReturnValue")
	public static class RootContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(MiniGoParser.IDENTIFIER, 0); }
		public TopDeclarationListContext topDeclarationList() {
			return getRuleContext(TopDeclarationListContext.class,0);
		}
		public TerminalNode EOF() { return getToken(MiniGoParser.EOF, 0); }
		public RootContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_root; }
	}

	public final RootContext root() throws RecognitionException {
		RootContext _localctx = new RootContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_root);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(82);
			match(T__0);
			setState(83);
			match(IDENTIFIER);
			setState(84);
			match(T__1);
			setState(85);
			topDeclarationList();
			setState(86);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TopDeclarationListContext extends ParserRuleContext {
		public List<VariableDeclContext> variableDecl() {
			return getRuleContexts(VariableDeclContext.class);
		}
		public VariableDeclContext variableDecl(int i) {
			return getRuleContext(VariableDeclContext.class,i);
		}
		public List<TypeDeclContext> typeDecl() {
			return getRuleContexts(TypeDeclContext.class);
		}
		public TypeDeclContext typeDecl(int i) {
			return getRuleContext(TypeDeclContext.class,i);
		}
		public List<FuncDeclContext> funcDecl() {
			return getRuleContexts(FuncDeclContext.class);
		}
		public FuncDeclContext funcDecl(int i) {
			return getRuleContext(FuncDeclContext.class,i);
		}
		public TopDeclarationListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_topDeclarationList; }
	}

	public final TopDeclarationListContext topDeclarationList() throws RecognitionException {
		TopDeclarationListContext _localctx = new TopDeclarationListContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_topDeclarationList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(93);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 392L) != 0)) {
				{
				setState(91);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case T__2:
					{
					setState(88);
					variableDecl();
					}
					break;
				case T__6:
					{
					setState(89);
					typeDecl();
					}
					break;
				case T__7:
					{
					setState(90);
					funcDecl();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				setState(95);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class VariableDeclContext extends ParserRuleContext {
		public SingleVarDeclContext singleVarDecl() {
			return getRuleContext(SingleVarDeclContext.class,0);
		}
		public InnerVarDeclsContext innerVarDecls() {
			return getRuleContext(InnerVarDeclsContext.class,0);
		}
		public VariableDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_variableDecl; }
	}

	public final VariableDeclContext variableDecl() throws RecognitionException {
		VariableDeclContext _localctx = new VariableDeclContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_variableDecl);
		int _la;
		try {
			setState(107);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,3,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(96);
				match(T__2);
				setState(97);
				singleVarDecl();
				setState(98);
				match(T__1);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(100);
				match(T__2);
				setState(101);
				match(T__3);
				setState(103);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==IDENTIFIER) {
					{
					setState(102);
					innerVarDecls();
					}
				}

				setState(105);
				match(T__4);
				setState(106);
				match(T__1);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class InnerVarDeclsContext extends ParserRuleContext {
		public List<SingleVarDeclContext> singleVarDecl() {
			return getRuleContexts(SingleVarDeclContext.class);
		}
		public SingleVarDeclContext singleVarDecl(int i) {
			return getRuleContext(SingleVarDeclContext.class,i);
		}
		public InnerVarDeclsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_innerVarDecls; }
	}

	public final InnerVarDeclsContext innerVarDecls() throws RecognitionException {
		InnerVarDeclsContext _localctx = new InnerVarDeclsContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_innerVarDecls);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(109);
			singleVarDecl();
			setState(110);
			match(T__1);
			setState(116);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==IDENTIFIER) {
				{
				{
				setState(111);
				singleVarDecl();
				setState(112);
				match(T__1);
				}
				}
				setState(118);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SingleVarDeclContext extends ParserRuleContext {
		public SingleVarDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_singleVarDecl; }
	 
		public SingleVarDeclContext() { }
		public void copyFrom(SingleVarDeclContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class VarDeclWithTypeAndInitContext extends SingleVarDeclContext {
		public IdentifierListContext identifierList() {
			return getRuleContext(IdentifierListContext.class,0);
		}
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public ExpressionListContext expressionList() {
			return getRuleContext(ExpressionListContext.class,0);
		}
		public VarDeclWithTypeAndInitContext(SingleVarDeclContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class VarDeclWithInitOnlyContext extends SingleVarDeclContext {
		public IdentifierListContext identifierList() {
			return getRuleContext(IdentifierListContext.class,0);
		}
		public ExpressionListContext expressionList() {
			return getRuleContext(ExpressionListContext.class,0);
		}
		public VarDeclWithInitOnlyContext(SingleVarDeclContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class VarDeclNoInitContext extends SingleVarDeclContext {
		public SingleVarDeclNoExpsContext singleVarDeclNoExps() {
			return getRuleContext(SingleVarDeclNoExpsContext.class,0);
		}
		public VarDeclNoInitContext(SingleVarDeclContext ctx) { copyFrom(ctx); }
	}

	public final SingleVarDeclContext singleVarDecl() throws RecognitionException {
		SingleVarDeclContext _localctx = new SingleVarDeclContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_singleVarDecl);
		try {
			setState(129);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,5,_ctx) ) {
			case 1:
				_localctx = new VarDeclWithTypeAndInitContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(119);
				identifierList();
				setState(120);
				declType();
				setState(121);
				match(T__5);
				setState(122);
				expressionList();
				}
				break;
			case 2:
				_localctx = new VarDeclWithInitOnlyContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(124);
				identifierList();
				setState(125);
				match(T__5);
				setState(126);
				expressionList();
				}
				break;
			case 3:
				_localctx = new VarDeclNoInitContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(128);
				singleVarDeclNoExps();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SingleVarDeclNoExpsContext extends ParserRuleContext {
		public IdentifierListContext identifierList() {
			return getRuleContext(IdentifierListContext.class,0);
		}
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public SingleVarDeclNoExpsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_singleVarDeclNoExps; }
	}

	public final SingleVarDeclNoExpsContext singleVarDeclNoExps() throws RecognitionException {
		SingleVarDeclNoExpsContext _localctx = new SingleVarDeclNoExpsContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_singleVarDeclNoExps);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(131);
			identifierList();
			setState(132);
			declType();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TypeDeclContext extends ParserRuleContext {
		public SingleTypeDeclContext singleTypeDecl() {
			return getRuleContext(SingleTypeDeclContext.class,0);
		}
		public InnerTypeDeclsContext innerTypeDecls() {
			return getRuleContext(InnerTypeDeclsContext.class,0);
		}
		public TypeDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_typeDecl; }
	}

	public final TypeDeclContext typeDecl() throws RecognitionException {
		TypeDeclContext _localctx = new TypeDeclContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_typeDecl);
		int _la;
		try {
			setState(145);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,7,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(134);
				match(T__6);
				setState(135);
				singleTypeDecl();
				setState(136);
				match(T__1);
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(138);
				match(T__6);
				setState(139);
				match(T__3);
				setState(141);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if (_la==IDENTIFIER) {
					{
					setState(140);
					innerTypeDecls();
					}
				}

				setState(143);
				match(T__4);
				setState(144);
				match(T__1);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class InnerTypeDeclsContext extends ParserRuleContext {
		public List<SingleTypeDeclContext> singleTypeDecl() {
			return getRuleContexts(SingleTypeDeclContext.class);
		}
		public SingleTypeDeclContext singleTypeDecl(int i) {
			return getRuleContext(SingleTypeDeclContext.class,i);
		}
		public InnerTypeDeclsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_innerTypeDecls; }
	}

	public final InnerTypeDeclsContext innerTypeDecls() throws RecognitionException {
		InnerTypeDeclsContext _localctx = new InnerTypeDeclsContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_innerTypeDecls);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(147);
			singleTypeDecl();
			setState(148);
			match(T__1);
			setState(154);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==IDENTIFIER) {
				{
				{
				setState(149);
				singleTypeDecl();
				setState(150);
				match(T__1);
				}
				}
				setState(156);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SingleTypeDeclContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(MiniGoParser.IDENTIFIER, 0); }
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public SingleTypeDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_singleTypeDecl; }
	}

	public final SingleTypeDeclContext singleTypeDecl() throws RecognitionException {
		SingleTypeDeclContext _localctx = new SingleTypeDeclContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_singleTypeDecl);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(157);
			match(IDENTIFIER);
			setState(158);
			declType();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class FuncDeclContext extends ParserRuleContext {
		public FuncFrontDeclContext funcFrontDecl() {
			return getRuleContext(FuncFrontDeclContext.class,0);
		}
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public FuncDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcDecl; }
	}

	public final FuncDeclContext funcDecl() throws RecognitionException {
		FuncDeclContext _localctx = new FuncDeclContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_funcDecl);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(160);
			funcFrontDecl();
			setState(161);
			block();
			setState(162);
			match(T__1);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class FuncFrontDeclContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(MiniGoParser.IDENTIFIER, 0); }
		public FuncArgDeclsContext funcArgDecls() {
			return getRuleContext(FuncArgDeclsContext.class,0);
		}
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public FuncFrontDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcFrontDecl; }
	}

	public final FuncFrontDeclContext funcFrontDecl() throws RecognitionException {
		FuncFrontDeclContext _localctx = new FuncFrontDeclContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_funcFrontDecl);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(164);
			match(T__7);
			setState(165);
			match(IDENTIFIER);
			setState(166);
			match(T__3);
			setState(168);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==IDENTIFIER) {
				{
				setState(167);
				funcArgDecls();
				}
			}

			setState(170);
			match(T__4);
			setState(172);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (((((_la - 4)) & ~0x3f) == 0 && ((1L << (_la - 4)) & 2305843009213694273L) != 0)) {
				{
				setState(171);
				declType();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class FuncArgDeclsContext extends ParserRuleContext {
		public List<SingleVarDeclNoExpsContext> singleVarDeclNoExps() {
			return getRuleContexts(SingleVarDeclNoExpsContext.class);
		}
		public SingleVarDeclNoExpsContext singleVarDeclNoExps(int i) {
			return getRuleContext(SingleVarDeclNoExpsContext.class,i);
		}
		public FuncArgDeclsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcArgDecls; }
	}

	public final FuncArgDeclsContext funcArgDecls() throws RecognitionException {
		FuncArgDeclsContext _localctx = new FuncArgDeclsContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_funcArgDecls);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(174);
			singleVarDeclNoExps();
			setState(179);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__8) {
				{
				{
				setState(175);
				match(T__8);
				setState(176);
				singleVarDeclNoExps();
				}
				}
				setState(181);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class DeclTypeContext extends ParserRuleContext {
		public DeclTypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_declType; }
	 
		public DeclTypeContext() { }
		public void copyFrom(DeclTypeContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class TypeSliceContext extends DeclTypeContext {
		public SliceDeclTypeContext sliceDeclType() {
			return getRuleContext(SliceDeclTypeContext.class,0);
		}
		public TypeSliceContext(DeclTypeContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class TypePrimitiveOrCustomContext extends DeclTypeContext {
		public TerminalNode IDENTIFIER() { return getToken(MiniGoParser.IDENTIFIER, 0); }
		public TypePrimitiveOrCustomContext(DeclTypeContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class TypeArrayContext extends DeclTypeContext {
		public ArrayDeclTypeContext arrayDeclType() {
			return getRuleContext(ArrayDeclTypeContext.class,0);
		}
		public TypeArrayContext(DeclTypeContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class TypeParenContext extends DeclTypeContext {
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public TypeParenContext(DeclTypeContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class TypeStructContext extends DeclTypeContext {
		public StructDeclTypeContext structDeclType() {
			return getRuleContext(StructDeclTypeContext.class,0);
		}
		public TypeStructContext(DeclTypeContext ctx) { copyFrom(ctx); }
	}

	public final DeclTypeContext declType() throws RecognitionException {
		DeclTypeContext _localctx = new DeclTypeContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_declType);
		try {
			setState(190);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,12,_ctx) ) {
			case 1:
				_localctx = new TypeParenContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(182);
				match(T__3);
				setState(183);
				declType();
				setState(184);
				match(T__4);
				}
				break;
			case 2:
				_localctx = new TypePrimitiveOrCustomContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(186);
				match(IDENTIFIER);
				}
				break;
			case 3:
				_localctx = new TypeSliceContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(187);
				sliceDeclType();
				}
				break;
			case 4:
				_localctx = new TypeArrayContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(188);
				arrayDeclType();
				}
				break;
			case 5:
				_localctx = new TypeStructContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(189);
				structDeclType();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SliceDeclTypeContext extends ParserRuleContext {
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public SliceDeclTypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_sliceDeclType; }
	}

	public final SliceDeclTypeContext sliceDeclType() throws RecognitionException {
		SliceDeclTypeContext _localctx = new SliceDeclTypeContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_sliceDeclType);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(192);
			match(T__9);
			setState(193);
			match(T__10);
			setState(194);
			declType();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ArrayDeclTypeContext extends ParserRuleContext {
		public TerminalNode INTLITERAL() { return getToken(MiniGoParser.INTLITERAL, 0); }
		public DeclTypeContext declType() {
			return getRuleContext(DeclTypeContext.class,0);
		}
		public ArrayDeclTypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arrayDeclType; }
	}

	public final ArrayDeclTypeContext arrayDeclType() throws RecognitionException {
		ArrayDeclTypeContext _localctx = new ArrayDeclTypeContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_arrayDeclType);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(196);
			match(T__9);
			setState(197);
			match(INTLITERAL);
			setState(198);
			match(T__10);
			setState(199);
			declType();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class StructDeclTypeContext extends ParserRuleContext {
		public StructMemDeclsContext structMemDecls() {
			return getRuleContext(StructMemDeclsContext.class,0);
		}
		public StructDeclTypeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_structDeclType; }
	}

	public final StructDeclTypeContext structDeclType() throws RecognitionException {
		StructDeclTypeContext _localctx = new StructDeclTypeContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_structDeclType);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(201);
			match(T__11);
			setState(202);
			match(T__12);
			setState(204);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==IDENTIFIER) {
				{
				setState(203);
				structMemDecls();
				}
			}

			setState(206);
			match(T__13);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class StructMemDeclsContext extends ParserRuleContext {
		public List<SingleVarDeclNoExpsContext> singleVarDeclNoExps() {
			return getRuleContexts(SingleVarDeclNoExpsContext.class);
		}
		public SingleVarDeclNoExpsContext singleVarDeclNoExps(int i) {
			return getRuleContext(SingleVarDeclNoExpsContext.class,i);
		}
		public StructMemDeclsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_structMemDecls; }
	}

	public final StructMemDeclsContext structMemDecls() throws RecognitionException {
		StructMemDeclsContext _localctx = new StructMemDeclsContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_structMemDecls);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(208);
			singleVarDeclNoExps();
			setState(209);
			match(T__1);
			setState(215);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==IDENTIFIER) {
				{
				{
				setState(210);
				singleVarDeclNoExps();
				setState(211);
				match(T__1);
				}
				}
				setState(217);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class IdentifierListContext extends ParserRuleContext {
		public List<TerminalNode> IDENTIFIER() { return getTokens(MiniGoParser.IDENTIFIER); }
		public TerminalNode IDENTIFIER(int i) {
			return getToken(MiniGoParser.IDENTIFIER, i);
		}
		public IdentifierListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_identifierList; }
	}

	public final IdentifierListContext identifierList() throws RecognitionException {
		IdentifierListContext _localctx = new IdentifierListContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_identifierList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(218);
			match(IDENTIFIER);
			setState(223);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__8) {
				{
				{
				setState(219);
				match(T__8);
				setState(220);
				match(IDENTIFIER);
				}
				}
				setState(225);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionContext extends ParserRuleContext {
		public ExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression; }
	 
		public ExpressionContext() { }
		public void copyFrom(ExpressionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class MulExprContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public MulExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class AndExprContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public AndExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class RelExprContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public RelExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class PrimaryExprContext extends ExpressionContext {
		public PrimaryExpressionContext primaryExpression() {
			return getRuleContext(PrimaryExpressionContext.class,0);
		}
		public PrimaryExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class AddExprContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public AddExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class UnaryExprContext extends ExpressionContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public UnaryExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class OrExprContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public OrExprContext(ExpressionContext ctx) { copyFrom(ctx); }
	}

	public final ExpressionContext expression() throws RecognitionException {
		return expression(0);
	}

	private ExpressionContext expression(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		ExpressionContext _localctx = new ExpressionContext(_ctx, _parentState);
		ExpressionContext _prevctx = _localctx;
		int _startState = 36;
		enterRecursionRule(_localctx, 36, RULE_expression, _p);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(230);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__3:
			case T__35:
			case T__36:
			case T__37:
			case IDENTIFIER:
			case FLOATLITERAL:
			case INTLITERAL:
			case RUNELITERAL:
			case RAWSTRINGLITERAL:
			case INTERPRETEDSTRINGLITERAL:
				{
				_localctx = new PrimaryExprContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(227);
				primaryExpression(0);
				}
				break;
			case T__14:
			case T__15:
			case T__16:
			case T__17:
				{
				_localctx = new UnaryExprContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(228);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 491520L) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(229);
				expression(6);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(249);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,18,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(247);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,17,_ctx) ) {
					case 1:
						{
						_localctx = new MulExprContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(232);
						if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
						setState(233);
						_la = _input.LA(1);
						if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 66584576L) != 0)) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(234);
						expression(6);
						}
						break;
					case 2:
						{
						_localctx = new AddExprContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(235);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(236);
						_la = _input.LA(1);
						if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 67469312L) != 0)) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(237);
						expression(5);
						}
						break;
					case 3:
						{
						_localctx = new RelExprContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(238);
						if (!(precpred(_ctx, 3))) throw new FailedPredicateException(this, "precpred(_ctx, 3)");
						setState(239);
						_la = _input.LA(1);
						if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 8455716864L) != 0)) ) {
						_errHandler.recoverInline(this);
						}
						else {
							if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
							_errHandler.reportMatch(this);
							consume();
						}
						setState(240);
						expression(4);
						}
						break;
					case 4:
						{
						_localctx = new AndExprContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(241);
						if (!(precpred(_ctx, 2))) throw new FailedPredicateException(this, "precpred(_ctx, 2)");
						setState(242);
						match(T__32);
						setState(243);
						expression(3);
						}
						break;
					case 5:
						{
						_localctx = new OrExprContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(244);
						if (!(precpred(_ctx, 1))) throw new FailedPredicateException(this, "precpred(_ctx, 1)");
						setState(245);
						match(T__33);
						setState(246);
						expression(2);
						}
						break;
					}
					} 
				}
				setState(251);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,18,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionListContext extends ParserRuleContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public ExpressionListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expressionList; }
	}

	public final ExpressionListContext expressionList() throws RecognitionException {
		ExpressionListContext _localctx = new ExpressionListContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_expressionList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(252);
			expression(0);
			setState(257);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__8) {
				{
				{
				setState(253);
				match(T__8);
				setState(254);
				expression(0);
				}
				}
				setState(259);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class PrimaryExpressionContext extends ParserRuleContext {
		public OperandContext operand() {
			return getRuleContext(OperandContext.class,0);
		}
		public AppendExpressionContext appendExpression() {
			return getRuleContext(AppendExpressionContext.class,0);
		}
		public LengthExpressionContext lengthExpression() {
			return getRuleContext(LengthExpressionContext.class,0);
		}
		public CapExpressionContext capExpression() {
			return getRuleContext(CapExpressionContext.class,0);
		}
		public PrimaryExpressionContext primaryExpression() {
			return getRuleContext(PrimaryExpressionContext.class,0);
		}
		public SelectorContext selector() {
			return getRuleContext(SelectorContext.class,0);
		}
		public IndexContext index() {
			return getRuleContext(IndexContext.class,0);
		}
		public ArgumentsContext arguments() {
			return getRuleContext(ArgumentsContext.class,0);
		}
		public PrimaryExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_primaryExpression; }
	}

	public final PrimaryExpressionContext primaryExpression() throws RecognitionException {
		return primaryExpression(0);
	}

	private PrimaryExpressionContext primaryExpression(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		PrimaryExpressionContext _localctx = new PrimaryExpressionContext(_ctx, _parentState);
		PrimaryExpressionContext _prevctx = _localctx;
		int _startState = 40;
		enterRecursionRule(_localctx, 40, RULE_primaryExpression, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(265);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__3:
			case IDENTIFIER:
			case FLOATLITERAL:
			case INTLITERAL:
			case RUNELITERAL:
			case RAWSTRINGLITERAL:
			case INTERPRETEDSTRINGLITERAL:
				{
				setState(261);
				operand();
				}
				break;
			case T__35:
				{
				setState(262);
				appendExpression();
				}
				break;
			case T__36:
				{
				setState(263);
				lengthExpression();
				}
				break;
			case T__37:
				{
				setState(264);
				capExpression();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(275);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,22,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(273);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,21,_ctx) ) {
					case 1:
						{
						_localctx = new PrimaryExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_primaryExpression);
						setState(267);
						if (!(precpred(_ctx, 6))) throw new FailedPredicateException(this, "precpred(_ctx, 6)");
						setState(268);
						selector();
						}
						break;
					case 2:
						{
						_localctx = new PrimaryExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_primaryExpression);
						setState(269);
						if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
						setState(270);
						index();
						}
						break;
					case 3:
						{
						_localctx = new PrimaryExpressionContext(_parentctx, _parentState);
						pushNewRecursionContext(_localctx, _startState, RULE_primaryExpression);
						setState(271);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(272);
						arguments();
						}
						break;
					}
					} 
				}
				setState(277);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,22,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class OperandContext extends ParserRuleContext {
		public LiteralContext literal() {
			return getRuleContext(LiteralContext.class,0);
		}
		public TerminalNode IDENTIFIER() { return getToken(MiniGoParser.IDENTIFIER, 0); }
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public OperandContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_operand; }
	}

	public final OperandContext operand() throws RecognitionException {
		OperandContext _localctx = new OperandContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_operand);
		try {
			setState(284);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case FLOATLITERAL:
			case INTLITERAL:
			case RUNELITERAL:
			case RAWSTRINGLITERAL:
			case INTERPRETEDSTRINGLITERAL:
				enterOuterAlt(_localctx, 1);
				{
				setState(278);
				literal();
				}
				break;
			case IDENTIFIER:
				enterOuterAlt(_localctx, 2);
				{
				setState(279);
				match(IDENTIFIER);
				}
				break;
			case T__3:
				enterOuterAlt(_localctx, 3);
				{
				setState(280);
				match(T__3);
				setState(281);
				expression(0);
				setState(282);
				match(T__4);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class LiteralContext extends ParserRuleContext {
		public TerminalNode INTLITERAL() { return getToken(MiniGoParser.INTLITERAL, 0); }
		public TerminalNode FLOATLITERAL() { return getToken(MiniGoParser.FLOATLITERAL, 0); }
		public TerminalNode RUNELITERAL() { return getToken(MiniGoParser.RUNELITERAL, 0); }
		public TerminalNode RAWSTRINGLITERAL() { return getToken(MiniGoParser.RAWSTRINGLITERAL, 0); }
		public TerminalNode INTERPRETEDSTRINGLITERAL() { return getToken(MiniGoParser.INTERPRETEDSTRINGLITERAL, 0); }
		public LiteralContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_literal; }
	}

	public final LiteralContext literal() throws RecognitionException {
		LiteralContext _localctx = new LiteralContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_literal);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(286);
			_la = _input.LA(1);
			if ( !(((((_la - 66)) & ~0x3f) == 0 && ((1L << (_la - 66)) & 31L) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class IndexContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public IndexContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_index; }
	}

	public final IndexContext index() throws RecognitionException {
		IndexContext _localctx = new IndexContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_index);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(288);
			match(T__9);
			setState(289);
			expression(0);
			setState(290);
			match(T__10);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ArgumentsContext extends ParserRuleContext {
		public ExpressionListContext expressionList() {
			return getRuleContext(ExpressionListContext.class,0);
		}
		public ArgumentsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arguments; }
	}

	public final ArgumentsContext arguments() throws RecognitionException {
		ArgumentsContext _localctx = new ArgumentsContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_arguments);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(292);
			match(T__3);
			setState(294);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 481036828688L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
				{
				setState(293);
				expressionList();
				}
			}

			setState(296);
			match(T__4);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SelectorContext extends ParserRuleContext {
		public TerminalNode IDENTIFIER() { return getToken(MiniGoParser.IDENTIFIER, 0); }
		public SelectorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_selector; }
	}

	public final SelectorContext selector() throws RecognitionException {
		SelectorContext _localctx = new SelectorContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_selector);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(298);
			match(T__34);
			setState(299);
			match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AppendExpressionContext extends ParserRuleContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public AppendExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_appendExpression; }
	}

	public final AppendExpressionContext appendExpression() throws RecognitionException {
		AppendExpressionContext _localctx = new AppendExpressionContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_appendExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(301);
			match(T__35);
			setState(302);
			match(T__3);
			setState(303);
			expression(0);
			setState(304);
			match(T__8);
			setState(305);
			expression(0);
			setState(306);
			match(T__4);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class LengthExpressionContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public LengthExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_lengthExpression; }
	}

	public final LengthExpressionContext lengthExpression() throws RecognitionException {
		LengthExpressionContext _localctx = new LengthExpressionContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_lengthExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(308);
			match(T__36);
			setState(309);
			match(T__3);
			setState(310);
			expression(0);
			setState(311);
			match(T__4);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class CapExpressionContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public CapExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_capExpression; }
	}

	public final CapExpressionContext capExpression() throws RecognitionException {
		CapExpressionContext _localctx = new CapExpressionContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_capExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(313);
			match(T__37);
			setState(314);
			match(T__3);
			setState(315);
			expression(0);
			setState(316);
			match(T__4);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class StatementListContext extends ParserRuleContext {
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public StatementListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statementList; }
	}

	public final StatementListContext statementList() throws RecognitionException {
		StatementListContext _localctx = new StatementListContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_statementList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(321);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 3747012413439320220L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
				{
				{
				setState(318);
				statement();
				}
				}
				setState(323);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BlockContext extends ParserRuleContext {
		public StatementListContext statementList() {
			return getRuleContext(StatementListContext.class,0);
		}
		public BlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_block; }
	}

	public final BlockContext block() throws RecognitionException {
		BlockContext _localctx = new BlockContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_block);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(324);
			match(T__12);
			setState(325);
			statementList();
			setState(326);
			match(T__13);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class StatementContext extends ParserRuleContext {
		public StatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement; }
	 
		public StatementContext() { }
		public void copyFrom(StatementContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class PrintStmtContext extends StatementContext {
		public ExpressionListContext expressionList() {
			return getRuleContext(ExpressionListContext.class,0);
		}
		public PrintStmtContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class TypeDeclWrapperContext extends StatementContext {
		public TypeDeclContext typeDecl() {
			return getRuleContext(TypeDeclContext.class,0);
		}
		public TypeDeclWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class SimpleStmtWrapperContext extends StatementContext {
		public SimpleStatementContext simpleStatement() {
			return getRuleContext(SimpleStatementContext.class,0);
		}
		public SimpleStmtWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class BlockStmtWrapperContext extends StatementContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public BlockStmtWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class BreakStmtContext extends StatementContext {
		public BreakStmtContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class LoopStmtWrapperContext extends StatementContext {
		public LoopStatementContext loopStatement() {
			return getRuleContext(LoopStatementContext.class,0);
		}
		public LoopStmtWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ReturnStmtContext extends StatementContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ReturnStmtContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class SwitchStmtWrapperContext extends StatementContext {
		public SwitchStatementContext switchStatement() {
			return getRuleContext(SwitchStatementContext.class,0);
		}
		public SwitchStmtWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ContinueStmtContext extends StatementContext {
		public ContinueStmtContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class PrintlnStmtContext extends StatementContext {
		public ExpressionListContext expressionList() {
			return getRuleContext(ExpressionListContext.class,0);
		}
		public PrintlnStmtContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class IfStmtWrapperContext extends StatementContext {
		public IfStatementContext ifStatement() {
			return getRuleContext(IfStatementContext.class,0);
		}
		public IfStmtWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class VarDeclWrapperContext extends StatementContext {
		public VariableDeclContext variableDecl() {
			return getRuleContext(VariableDeclContext.class,0);
		}
		public VarDeclWrapperContext(StatementContext ctx) { copyFrom(ctx); }
	}

	public final StatementContext statement() throws RecognitionException {
		StatementContext _localctx = new StatementContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_statement);
		int _la;
		try {
			setState(368);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__38:
				_localctx = new PrintStmtContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(328);
				match(T__38);
				setState(329);
				match(T__3);
				setState(331);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 481036828688L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
					{
					setState(330);
					expressionList();
					}
				}

				setState(333);
				match(T__4);
				setState(334);
				match(T__1);
				}
				break;
			case T__39:
				_localctx = new PrintlnStmtContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(335);
				match(T__39);
				setState(336);
				match(T__3);
				setState(338);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 481036828688L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
					{
					setState(337);
					expressionList();
					}
				}

				setState(340);
				match(T__4);
				setState(341);
				match(T__1);
				}
				break;
			case T__40:
				_localctx = new ReturnStmtContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(342);
				match(T__40);
				setState(344);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 481036828688L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
					{
					setState(343);
					expression(0);
					}
				}

				setState(346);
				match(T__1);
				}
				break;
			case T__41:
				_localctx = new BreakStmtContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(347);
				match(T__41);
				setState(348);
				match(T__1);
				}
				break;
			case T__42:
				_localctx = new ContinueStmtContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(349);
				match(T__42);
				setState(350);
				match(T__1);
				}
				break;
			case T__1:
			case T__3:
			case T__14:
			case T__15:
			case T__16:
			case T__17:
			case T__35:
			case T__36:
			case T__37:
			case IDENTIFIER:
			case FLOATLITERAL:
			case INTLITERAL:
			case RUNELITERAL:
			case RAWSTRINGLITERAL:
			case INTERPRETEDSTRINGLITERAL:
				_localctx = new SimpleStmtWrapperContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(351);
				simpleStatement();
				setState(352);
				match(T__1);
				}
				break;
			case T__12:
				_localctx = new BlockStmtWrapperContext(_localctx);
				enterOuterAlt(_localctx, 7);
				{
				setState(354);
				block();
				setState(355);
				match(T__1);
				}
				break;
			case T__60:
				_localctx = new SwitchStmtWrapperContext(_localctx);
				enterOuterAlt(_localctx, 8);
				{
				setState(357);
				switchStatement();
				setState(358);
				match(T__1);
				}
				break;
			case T__57:
				_localctx = new IfStmtWrapperContext(_localctx);
				enterOuterAlt(_localctx, 9);
				{
				setState(360);
				ifStatement();
				setState(361);
				match(T__1);
				}
				break;
			case T__59:
				_localctx = new LoopStmtWrapperContext(_localctx);
				enterOuterAlt(_localctx, 10);
				{
				setState(363);
				loopStatement();
				setState(364);
				match(T__1);
				}
				break;
			case T__6:
				_localctx = new TypeDeclWrapperContext(_localctx);
				enterOuterAlt(_localctx, 11);
				{
				setState(366);
				typeDecl();
				}
				break;
			case T__2:
				_localctx = new VarDeclWrapperContext(_localctx);
				enterOuterAlt(_localctx, 12);
				{
				setState(367);
				variableDecl();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SimpleStatementContext extends ParserRuleContext {
		public SimpleStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_simpleStatement; }
	 
		public SimpleStatementContext() { }
		public void copyFrom(SimpleStatementContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ExprStmtContext extends SimpleStatementContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ExprStmtContext(SimpleStatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ShortVarDeclStmtContext extends SimpleStatementContext {
		public List<ExpressionListContext> expressionList() {
			return getRuleContexts(ExpressionListContext.class);
		}
		public ExpressionListContext expressionList(int i) {
			return getRuleContext(ExpressionListContext.class,i);
		}
		public ShortVarDeclStmtContext(SimpleStatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class AssignStmtContext extends SimpleStatementContext {
		public AssignmentStatementContext assignmentStatement() {
			return getRuleContext(AssignmentStatementContext.class,0);
		}
		public AssignStmtContext(SimpleStatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class EmptyStmtContext extends SimpleStatementContext {
		public EmptyStmtContext(SimpleStatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class IncDecStmtContext extends SimpleStatementContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public IncDecStmtContext(SimpleStatementContext ctx) { copyFrom(ctx); }
	}

	public final SimpleStatementContext simpleStatement() throws RecognitionException {
		SimpleStatementContext _localctx = new SimpleStatementContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_simpleStatement);
		int _la;
		try {
			setState(380);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,30,_ctx) ) {
			case 1:
				_localctx = new EmptyStmtContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				}
				break;
			case 2:
				_localctx = new ShortVarDeclStmtContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(371);
				expressionList();
				setState(372);
				match(T__43);
				setState(373);
				expressionList();
				}
				break;
			case 3:
				_localctx = new AssignStmtContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(375);
				assignmentStatement();
				}
				break;
			case 4:
				_localctx = new IncDecStmtContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(376);
				expression(0);
				setState(377);
				_la = _input.LA(1);
				if ( !(_la==T__44 || _la==T__45) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
				break;
			case 5:
				_localctx = new ExprStmtContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(379);
				expression(0);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AssignmentStatementContext extends ParserRuleContext {
		public AssignmentStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignmentStatement; }
	 
		public AssignmentStatementContext() { }
		public void copyFrom(AssignmentStatementContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class AssignSimpleContext extends AssignmentStatementContext {
		public List<ExpressionListContext> expressionList() {
			return getRuleContexts(ExpressionListContext.class);
		}
		public ExpressionListContext expressionList(int i) {
			return getRuleContext(ExpressionListContext.class,i);
		}
		public AssignSimpleContext(AssignmentStatementContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class AssignCompoundContext extends AssignmentStatementContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public AssignCompoundContext(AssignmentStatementContext ctx) { copyFrom(ctx); }
	}

	public final AssignmentStatementContext assignmentStatement() throws RecognitionException {
		AssignmentStatementContext _localctx = new AssignmentStatementContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_assignmentStatement);
		int _la;
		try {
			setState(390);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,31,_ctx) ) {
			case 1:
				_localctx = new AssignSimpleContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(382);
				expressionList();
				setState(383);
				match(T__5);
				setState(384);
				expressionList();
				}
				break;
			case 2:
				_localctx = new AssignCompoundContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(386);
				expression(0);
				setState(387);
				_la = _input.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 288089638663356416L) != 0)) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				setState(388);
				expression(0);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class IfStatementContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public SimpleStatementContext simpleStatement() {
			return getRuleContext(SimpleStatementContext.class,0);
		}
		public ElseClauseContext elseClause() {
			return getRuleContext(ElseClauseContext.class,0);
		}
		public IfStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ifStatement; }
	}

	public final IfStatementContext ifStatement() throws RecognitionException {
		IfStatementContext _localctx = new IfStatementContext(_ctx, getState());
		enterRule(_localctx, 68, RULE_ifStatement);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(392);
			match(T__57);
			setState(396);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,32,_ctx) ) {
			case 1:
				{
				setState(393);
				simpleStatement();
				setState(394);
				match(T__1);
				}
				break;
			}
			setState(398);
			expression(0);
			setState(399);
			block();
			setState(401);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__58) {
				{
				setState(400);
				elseClause();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ElseClauseContext extends ParserRuleContext {
		public IfStatementContext ifStatement() {
			return getRuleContext(IfStatementContext.class,0);
		}
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public ElseClauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_elseClause; }
	}

	public final ElseClauseContext elseClause() throws RecognitionException {
		ElseClauseContext _localctx = new ElseClauseContext(_ctx, getState());
		enterRule(_localctx, 70, RULE_elseClause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(403);
			match(T__58);
			setState(406);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__57:
				{
				setState(404);
				ifStatement();
				}
				break;
			case T__12:
				{
				setState(405);
				block();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class LoopStatementContext extends ParserRuleContext {
		public BlockContext block() {
			return getRuleContext(BlockContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public List<SimpleStatementContext> simpleStatement() {
			return getRuleContexts(SimpleStatementContext.class);
		}
		public SimpleStatementContext simpleStatement(int i) {
			return getRuleContext(SimpleStatementContext.class,i);
		}
		public LoopStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_loopStatement; }
	}

	public final LoopStatementContext loopStatement() throws RecognitionException {
		LoopStatementContext _localctx = new LoopStatementContext(_ctx, getState());
		enterRule(_localctx, 72, RULE_loopStatement);
		int _la;
		try {
			setState(424);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,36,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(408);
				match(T__59);
				setState(409);
				block();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(410);
				match(T__59);
				setState(411);
				expression(0);
				setState(412);
				block();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(414);
				match(T__59);
				setState(415);
				simpleStatement();
				setState(416);
				match(T__1);
				setState(418);
				_errHandler.sync(this);
				_la = _input.LA(1);
				if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 481036828688L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
					{
					setState(417);
					expression(0);
					}
				}

				setState(420);
				match(T__1);
				setState(421);
				simpleStatement();
				setState(422);
				block();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class SwitchStatementContext extends ParserRuleContext {
		public ExpressionCaseClauseListContext expressionCaseClauseList() {
			return getRuleContext(ExpressionCaseClauseListContext.class,0);
		}
		public SimpleStatementContext simpleStatement() {
			return getRuleContext(SimpleStatementContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public SwitchStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_switchStatement; }
	}

	public final SwitchStatementContext switchStatement() throws RecognitionException {
		SwitchStatementContext _localctx = new SwitchStatementContext(_ctx, getState());
		enterRule(_localctx, 74, RULE_switchStatement);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(426);
			match(T__60);
			setState(430);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,37,_ctx) ) {
			case 1:
				{
				setState(427);
				simpleStatement();
				setState(428);
				match(T__1);
				}
				break;
			}
			setState(433);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 481036828688L) != 0) || ((((_la - 65)) & ~0x3f) == 0 && ((1L << (_la - 65)) & 63L) != 0)) {
				{
				setState(432);
				expression(0);
				}
			}

			setState(435);
			match(T__12);
			setState(436);
			expressionCaseClauseList();
			setState(437);
			match(T__13);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionCaseClauseListContext extends ParserRuleContext {
		public List<ExpressionCaseClauseContext> expressionCaseClause() {
			return getRuleContexts(ExpressionCaseClauseContext.class);
		}
		public ExpressionCaseClauseContext expressionCaseClause(int i) {
			return getRuleContext(ExpressionCaseClauseContext.class,i);
		}
		public ExpressionCaseClauseListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expressionCaseClauseList; }
	}

	public final ExpressionCaseClauseListContext expressionCaseClauseList() throws RecognitionException {
		ExpressionCaseClauseListContext _localctx = new ExpressionCaseClauseListContext(_ctx, getState());
		enterRule(_localctx, 76, RULE_expressionCaseClauseList);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(442);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__62 || _la==T__63) {
				{
				{
				setState(439);
				expressionCaseClause();
				}
				}
				setState(444);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionCaseClauseContext extends ParserRuleContext {
		public ExpressionSwitchCaseContext expressionSwitchCase() {
			return getRuleContext(ExpressionSwitchCaseContext.class,0);
		}
		public StatementListContext statementList() {
			return getRuleContext(StatementListContext.class,0);
		}
		public ExpressionCaseClauseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expressionCaseClause; }
	}

	public final ExpressionCaseClauseContext expressionCaseClause() throws RecognitionException {
		ExpressionCaseClauseContext _localctx = new ExpressionCaseClauseContext(_ctx, getState());
		enterRule(_localctx, 78, RULE_expressionCaseClause);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(445);
			expressionSwitchCase();
			setState(446);
			match(T__61);
			setState(447);
			statementList();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ExpressionSwitchCaseContext extends ParserRuleContext {
		public ExpressionSwitchCaseContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expressionSwitchCase; }
	 
		public ExpressionSwitchCaseContext() { }
		public void copyFrom(ExpressionSwitchCaseContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class SwitchCaseDefaultContext extends ExpressionSwitchCaseContext {
		public SwitchCaseDefaultContext(ExpressionSwitchCaseContext ctx) { copyFrom(ctx); }
	}
	@SuppressWarnings("CheckReturnValue")
	public static class SwitchCaseExprContext extends ExpressionSwitchCaseContext {
		public ExpressionListContext expressionList() {
			return getRuleContext(ExpressionListContext.class,0);
		}
		public SwitchCaseExprContext(ExpressionSwitchCaseContext ctx) { copyFrom(ctx); }
	}

	public final ExpressionSwitchCaseContext expressionSwitchCase() throws RecognitionException {
		ExpressionSwitchCaseContext _localctx = new ExpressionSwitchCaseContext(_ctx, getState());
		enterRule(_localctx, 80, RULE_expressionSwitchCase);
		try {
			setState(452);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__62:
				_localctx = new SwitchCaseExprContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(449);
				match(T__62);
				setState(450);
				expressionList();
				}
				break;
			case T__63:
				_localctx = new SwitchCaseDefaultContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(451);
				match(T__63);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 18:
			return expression_sempred((ExpressionContext)_localctx, predIndex);
		case 20:
			return primaryExpression_sempred((PrimaryExpressionContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean expression_sempred(ExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 5);
		case 1:
			return precpred(_ctx, 4);
		case 2:
			return precpred(_ctx, 3);
		case 3:
			return precpred(_ctx, 2);
		case 4:
			return precpred(_ctx, 1);
		}
		return true;
	}
	private boolean primaryExpression_sempred(PrimaryExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 5:
			return precpred(_ctx, 6);
		case 6:
			return precpred(_ctx, 5);
		case 7:
			return precpred(_ctx, 4);
		}
		return true;
	}

	public static final String _serializedATN =
		"\u0004\u0001J\u01c7\u0002\u0000\u0007\u0000\u0002\u0001\u0007\u0001\u0002"+
		"\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004\u0007\u0004\u0002"+
		"\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002\u0007\u0007\u0007\u0002"+
		"\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002\u000b\u0007\u000b\u0002"+
		"\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e\u0002\u000f\u0007\u000f"+
		"\u0002\u0010\u0007\u0010\u0002\u0011\u0007\u0011\u0002\u0012\u0007\u0012"+
		"\u0002\u0013\u0007\u0013\u0002\u0014\u0007\u0014\u0002\u0015\u0007\u0015"+
		"\u0002\u0016\u0007\u0016\u0002\u0017\u0007\u0017\u0002\u0018\u0007\u0018"+
		"\u0002\u0019\u0007\u0019\u0002\u001a\u0007\u001a\u0002\u001b\u0007\u001b"+
		"\u0002\u001c\u0007\u001c\u0002\u001d\u0007\u001d\u0002\u001e\u0007\u001e"+
		"\u0002\u001f\u0007\u001f\u0002 \u0007 \u0002!\u0007!\u0002\"\u0007\"\u0002"+
		"#\u0007#\u0002$\u0007$\u0002%\u0007%\u0002&\u0007&\u0002\'\u0007\'\u0002"+
		"(\u0007(\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001\u0000\u0001"+
		"\u0000\u0001\u0001\u0001\u0001\u0001\u0001\u0005\u0001\\\b\u0001\n\u0001"+
		"\f\u0001_\t\u0001\u0001\u0002\u0001\u0002\u0001\u0002\u0001\u0002\u0001"+
		"\u0002\u0001\u0002\u0001\u0002\u0003\u0002h\b\u0002\u0001\u0002\u0001"+
		"\u0002\u0003\u0002l\b\u0002\u0001\u0003\u0001\u0003\u0001\u0003\u0001"+
		"\u0003\u0001\u0003\u0005\u0003s\b\u0003\n\u0003\f\u0003v\t\u0003\u0001"+
		"\u0004\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0004\u0001\u0004\u0001"+
		"\u0004\u0001\u0004\u0001\u0004\u0001\u0004\u0003\u0004\u0082\b\u0004\u0001"+
		"\u0005\u0001\u0005\u0001\u0005\u0001\u0006\u0001\u0006\u0001\u0006\u0001"+
		"\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0003\u0006\u008e\b\u0006\u0001"+
		"\u0006\u0001\u0006\u0003\u0006\u0092\b\u0006\u0001\u0007\u0001\u0007\u0001"+
		"\u0007\u0001\u0007\u0001\u0007\u0005\u0007\u0099\b\u0007\n\u0007\f\u0007"+
		"\u009c\t\u0007\u0001\b\u0001\b\u0001\b\u0001\t\u0001\t\u0001\t\u0001\t"+
		"\u0001\n\u0001\n\u0001\n\u0001\n\u0003\n\u00a9\b\n\u0001\n\u0001\n\u0003"+
		"\n\u00ad\b\n\u0001\u000b\u0001\u000b\u0001\u000b\u0005\u000b\u00b2\b\u000b"+
		"\n\u000b\f\u000b\u00b5\t\u000b\u0001\f\u0001\f\u0001\f\u0001\f\u0001\f"+
		"\u0001\f\u0001\f\u0001\f\u0003\f\u00bf\b\f\u0001\r\u0001\r\u0001\r\u0001"+
		"\r\u0001\u000e\u0001\u000e\u0001\u000e\u0001\u000e\u0001\u000e\u0001\u000f"+
		"\u0001\u000f\u0001\u000f\u0003\u000f\u00cd\b\u000f\u0001\u000f\u0001\u000f"+
		"\u0001\u0010\u0001\u0010\u0001\u0010\u0001\u0010\u0001\u0010\u0005\u0010"+
		"\u00d6\b\u0010\n\u0010\f\u0010\u00d9\t\u0010\u0001\u0011\u0001\u0011\u0001"+
		"\u0011\u0005\u0011\u00de\b\u0011\n\u0011\f\u0011\u00e1\t\u0011\u0001\u0012"+
		"\u0001\u0012\u0001\u0012\u0001\u0012\u0003\u0012\u00e7\b\u0012\u0001\u0012"+
		"\u0001\u0012\u0001\u0012\u0001\u0012\u0001\u0012\u0001\u0012\u0001\u0012"+
		"\u0001\u0012\u0001\u0012\u0001\u0012\u0001\u0012\u0001\u0012\u0001\u0012"+
		"\u0001\u0012\u0001\u0012\u0005\u0012\u00f8\b\u0012\n\u0012\f\u0012\u00fb"+
		"\t\u0012\u0001\u0013\u0001\u0013\u0001\u0013\u0005\u0013\u0100\b\u0013"+
		"\n\u0013\f\u0013\u0103\t\u0013\u0001\u0014\u0001\u0014\u0001\u0014\u0001"+
		"\u0014\u0001\u0014\u0003\u0014\u010a\b\u0014\u0001\u0014\u0001\u0014\u0001"+
		"\u0014\u0001\u0014\u0001\u0014\u0001\u0014\u0005\u0014\u0112\b\u0014\n"+
		"\u0014\f\u0014\u0115\t\u0014\u0001\u0015\u0001\u0015\u0001\u0015\u0001"+
		"\u0015\u0001\u0015\u0001\u0015\u0003\u0015\u011d\b\u0015\u0001\u0016\u0001"+
		"\u0016\u0001\u0017\u0001\u0017\u0001\u0017\u0001\u0017\u0001\u0018\u0001"+
		"\u0018\u0003\u0018\u0127\b\u0018\u0001\u0018\u0001\u0018\u0001\u0019\u0001"+
		"\u0019\u0001\u0019\u0001\u001a\u0001\u001a\u0001\u001a\u0001\u001a\u0001"+
		"\u001a\u0001\u001a\u0001\u001a\u0001\u001b\u0001\u001b\u0001\u001b\u0001"+
		"\u001b\u0001\u001b\u0001\u001c\u0001\u001c\u0001\u001c\u0001\u001c\u0001"+
		"\u001c\u0001\u001d\u0005\u001d\u0140\b\u001d\n\u001d\f\u001d\u0143\t\u001d"+
		"\u0001\u001e\u0001\u001e\u0001\u001e\u0001\u001e\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0003\u001f\u014c\b\u001f\u0001\u001f\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0001\u001f\u0003\u001f\u0153\b\u001f\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0001\u001f\u0003\u001f\u0159\b\u001f\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f\u0001\u001f"+
		"\u0001\u001f\u0001\u001f\u0003\u001f\u0171\b\u001f\u0001 \u0001 \u0001"+
		" \u0001 \u0001 \u0001 \u0001 \u0001 \u0001 \u0001 \u0003 \u017d\b \u0001"+
		"!\u0001!\u0001!\u0001!\u0001!\u0001!\u0001!\u0001!\u0003!\u0187\b!\u0001"+
		"\"\u0001\"\u0001\"\u0001\"\u0003\"\u018d\b\"\u0001\"\u0001\"\u0001\"\u0003"+
		"\"\u0192\b\"\u0001#\u0001#\u0001#\u0003#\u0197\b#\u0001$\u0001$\u0001"+
		"$\u0001$\u0001$\u0001$\u0001$\u0001$\u0001$\u0001$\u0003$\u01a3\b$\u0001"+
		"$\u0001$\u0001$\u0001$\u0003$\u01a9\b$\u0001%\u0001%\u0001%\u0001%\u0003"+
		"%\u01af\b%\u0001%\u0003%\u01b2\b%\u0001%\u0001%\u0001%\u0001%\u0001&\u0005"+
		"&\u01b9\b&\n&\f&\u01bc\t&\u0001\'\u0001\'\u0001\'\u0001\'\u0001(\u0001"+
		"(\u0001(\u0003(\u01c5\b(\u0001(\u0000\u0002$()\u0000\u0002\u0004\u0006"+
		"\b\n\f\u000e\u0010\u0012\u0014\u0016\u0018\u001a\u001c\u001e \"$&(*,."+
		"02468:<>@BDFHJLNP\u0000\u0007\u0001\u0000\u000f\u0012\u0001\u0000\u0013"+
		"\u0019\u0003\u0000\u000f\u0010\u0012\u0012\u001a\u001a\u0001\u0000\u001b"+
		" \u0001\u0000BF\u0001\u0000-.\u0001\u0000/9\u01e0\u0000R\u0001\u0000\u0000"+
		"\u0000\u0002]\u0001\u0000\u0000\u0000\u0004k\u0001\u0000\u0000\u0000\u0006"+
		"m\u0001\u0000\u0000\u0000\b\u0081\u0001\u0000\u0000\u0000\n\u0083\u0001"+
		"\u0000\u0000\u0000\f\u0091\u0001\u0000\u0000\u0000\u000e\u0093\u0001\u0000"+
		"\u0000\u0000\u0010\u009d\u0001\u0000\u0000\u0000\u0012\u00a0\u0001\u0000"+
		"\u0000\u0000\u0014\u00a4\u0001\u0000\u0000\u0000\u0016\u00ae\u0001\u0000"+
		"\u0000\u0000\u0018\u00be\u0001\u0000\u0000\u0000\u001a\u00c0\u0001\u0000"+
		"\u0000\u0000\u001c\u00c4\u0001\u0000\u0000\u0000\u001e\u00c9\u0001\u0000"+
		"\u0000\u0000 \u00d0\u0001\u0000\u0000\u0000\"\u00da\u0001\u0000\u0000"+
		"\u0000$\u00e6\u0001\u0000\u0000\u0000&\u00fc\u0001\u0000\u0000\u0000("+
		"\u0109\u0001\u0000\u0000\u0000*\u011c\u0001\u0000\u0000\u0000,\u011e\u0001"+
		"\u0000\u0000\u0000.\u0120\u0001\u0000\u0000\u00000\u0124\u0001\u0000\u0000"+
		"\u00002\u012a\u0001\u0000\u0000\u00004\u012d\u0001\u0000\u0000\u00006"+
		"\u0134\u0001\u0000\u0000\u00008\u0139\u0001\u0000\u0000\u0000:\u0141\u0001"+
		"\u0000\u0000\u0000<\u0144\u0001\u0000\u0000\u0000>\u0170\u0001\u0000\u0000"+
		"\u0000@\u017c\u0001\u0000\u0000\u0000B\u0186\u0001\u0000\u0000\u0000D"+
		"\u0188\u0001\u0000\u0000\u0000F\u0193\u0001\u0000\u0000\u0000H\u01a8\u0001"+
		"\u0000\u0000\u0000J\u01aa\u0001\u0000\u0000\u0000L\u01ba\u0001\u0000\u0000"+
		"\u0000N\u01bd\u0001\u0000\u0000\u0000P\u01c4\u0001\u0000\u0000\u0000R"+
		"S\u0005\u0001\u0000\u0000ST\u0005A\u0000\u0000TU\u0005\u0002\u0000\u0000"+
		"UV\u0003\u0002\u0001\u0000VW\u0005\u0000\u0000\u0001W\u0001\u0001\u0000"+
		"\u0000\u0000X\\\u0003\u0004\u0002\u0000Y\\\u0003\f\u0006\u0000Z\\\u0003"+
		"\u0012\t\u0000[X\u0001\u0000\u0000\u0000[Y\u0001\u0000\u0000\u0000[Z\u0001"+
		"\u0000\u0000\u0000\\_\u0001\u0000\u0000\u0000][\u0001\u0000\u0000\u0000"+
		"]^\u0001\u0000\u0000\u0000^\u0003\u0001\u0000\u0000\u0000_]\u0001\u0000"+
		"\u0000\u0000`a\u0005\u0003\u0000\u0000ab\u0003\b\u0004\u0000bc\u0005\u0002"+
		"\u0000\u0000cl\u0001\u0000\u0000\u0000de\u0005\u0003\u0000\u0000eg\u0005"+
		"\u0004\u0000\u0000fh\u0003\u0006\u0003\u0000gf\u0001\u0000\u0000\u0000"+
		"gh\u0001\u0000\u0000\u0000hi\u0001\u0000\u0000\u0000ij\u0005\u0005\u0000"+
		"\u0000jl\u0005\u0002\u0000\u0000k`\u0001\u0000\u0000\u0000kd\u0001\u0000"+
		"\u0000\u0000l\u0005\u0001\u0000\u0000\u0000mn\u0003\b\u0004\u0000nt\u0005"+
		"\u0002\u0000\u0000op\u0003\b\u0004\u0000pq\u0005\u0002\u0000\u0000qs\u0001"+
		"\u0000\u0000\u0000ro\u0001\u0000\u0000\u0000sv\u0001\u0000\u0000\u0000"+
		"tr\u0001\u0000\u0000\u0000tu\u0001\u0000\u0000\u0000u\u0007\u0001\u0000"+
		"\u0000\u0000vt\u0001\u0000\u0000\u0000wx\u0003\"\u0011\u0000xy\u0003\u0018"+
		"\f\u0000yz\u0005\u0006\u0000\u0000z{\u0003&\u0013\u0000{\u0082\u0001\u0000"+
		"\u0000\u0000|}\u0003\"\u0011\u0000}~\u0005\u0006\u0000\u0000~\u007f\u0003"+
		"&\u0013\u0000\u007f\u0082\u0001\u0000\u0000\u0000\u0080\u0082\u0003\n"+
		"\u0005\u0000\u0081w\u0001\u0000\u0000\u0000\u0081|\u0001\u0000\u0000\u0000"+
		"\u0081\u0080\u0001\u0000\u0000\u0000\u0082\t\u0001\u0000\u0000\u0000\u0083"+
		"\u0084\u0003\"\u0011\u0000\u0084\u0085\u0003\u0018\f\u0000\u0085\u000b"+
		"\u0001\u0000\u0000\u0000\u0086\u0087\u0005\u0007\u0000\u0000\u0087\u0088"+
		"\u0003\u0010\b\u0000\u0088\u0089\u0005\u0002\u0000\u0000\u0089\u0092\u0001"+
		"\u0000\u0000\u0000\u008a\u008b\u0005\u0007\u0000\u0000\u008b\u008d\u0005"+
		"\u0004\u0000\u0000\u008c\u008e\u0003\u000e\u0007\u0000\u008d\u008c\u0001"+
		"\u0000\u0000\u0000\u008d\u008e\u0001\u0000\u0000\u0000\u008e\u008f\u0001"+
		"\u0000\u0000\u0000\u008f\u0090\u0005\u0005\u0000\u0000\u0090\u0092\u0005"+
		"\u0002\u0000\u0000\u0091\u0086\u0001\u0000\u0000\u0000\u0091\u008a\u0001"+
		"\u0000\u0000\u0000\u0092\r\u0001\u0000\u0000\u0000\u0093\u0094\u0003\u0010"+
		"\b\u0000\u0094\u009a\u0005\u0002\u0000\u0000\u0095\u0096\u0003\u0010\b"+
		"\u0000\u0096\u0097\u0005\u0002\u0000\u0000\u0097\u0099\u0001\u0000\u0000"+
		"\u0000\u0098\u0095\u0001\u0000\u0000\u0000\u0099\u009c\u0001\u0000\u0000"+
		"\u0000\u009a\u0098\u0001\u0000\u0000\u0000\u009a\u009b\u0001\u0000\u0000"+
		"\u0000\u009b\u000f\u0001\u0000\u0000\u0000\u009c\u009a\u0001\u0000\u0000"+
		"\u0000\u009d\u009e\u0005A\u0000\u0000\u009e\u009f\u0003\u0018\f\u0000"+
		"\u009f\u0011\u0001\u0000\u0000\u0000\u00a0\u00a1\u0003\u0014\n\u0000\u00a1"+
		"\u00a2\u0003<\u001e\u0000\u00a2\u00a3\u0005\u0002\u0000\u0000\u00a3\u0013"+
		"\u0001\u0000\u0000\u0000\u00a4\u00a5\u0005\b\u0000\u0000\u00a5\u00a6\u0005"+
		"A\u0000\u0000\u00a6\u00a8\u0005\u0004\u0000\u0000\u00a7\u00a9\u0003\u0016"+
		"\u000b\u0000\u00a8\u00a7\u0001\u0000\u0000\u0000\u00a8\u00a9\u0001\u0000"+
		"\u0000\u0000\u00a9\u00aa\u0001\u0000\u0000\u0000\u00aa\u00ac\u0005\u0005"+
		"\u0000\u0000\u00ab\u00ad\u0003\u0018\f\u0000\u00ac\u00ab\u0001\u0000\u0000"+
		"\u0000\u00ac\u00ad\u0001\u0000\u0000\u0000\u00ad\u0015\u0001\u0000\u0000"+
		"\u0000\u00ae\u00b3\u0003\n\u0005\u0000\u00af\u00b0\u0005\t\u0000\u0000"+
		"\u00b0\u00b2\u0003\n\u0005\u0000\u00b1\u00af\u0001\u0000\u0000\u0000\u00b2"+
		"\u00b5\u0001\u0000\u0000\u0000\u00b3\u00b1\u0001\u0000\u0000\u0000\u00b3"+
		"\u00b4\u0001\u0000\u0000\u0000\u00b4\u0017\u0001\u0000\u0000\u0000\u00b5"+
		"\u00b3\u0001\u0000\u0000\u0000\u00b6\u00b7\u0005\u0004\u0000\u0000\u00b7"+
		"\u00b8\u0003\u0018\f\u0000\u00b8\u00b9\u0005\u0005\u0000\u0000\u00b9\u00bf"+
		"\u0001\u0000\u0000\u0000\u00ba\u00bf\u0005A\u0000\u0000\u00bb\u00bf\u0003"+
		"\u001a\r\u0000\u00bc\u00bf\u0003\u001c\u000e\u0000\u00bd\u00bf\u0003\u001e"+
		"\u000f\u0000\u00be\u00b6\u0001\u0000\u0000\u0000\u00be\u00ba\u0001\u0000"+
		"\u0000\u0000\u00be\u00bb\u0001\u0000\u0000\u0000\u00be\u00bc\u0001\u0000"+
		"\u0000\u0000\u00be\u00bd\u0001\u0000\u0000\u0000\u00bf\u0019\u0001\u0000"+
		"\u0000\u0000\u00c0\u00c1\u0005\n\u0000\u0000\u00c1\u00c2\u0005\u000b\u0000"+
		"\u0000\u00c2\u00c3\u0003\u0018\f\u0000\u00c3\u001b\u0001\u0000\u0000\u0000"+
		"\u00c4\u00c5\u0005\n\u0000\u0000\u00c5\u00c6\u0005C\u0000\u0000\u00c6"+
		"\u00c7\u0005\u000b\u0000\u0000\u00c7\u00c8\u0003\u0018\f\u0000\u00c8\u001d"+
		"\u0001\u0000\u0000\u0000\u00c9\u00ca\u0005\f\u0000\u0000\u00ca\u00cc\u0005"+
		"\r\u0000\u0000\u00cb\u00cd\u0003 \u0010\u0000\u00cc\u00cb\u0001\u0000"+
		"\u0000\u0000\u00cc\u00cd\u0001\u0000\u0000\u0000\u00cd\u00ce\u0001\u0000"+
		"\u0000\u0000\u00ce\u00cf\u0005\u000e\u0000\u0000\u00cf\u001f\u0001\u0000"+
		"\u0000\u0000\u00d0\u00d1\u0003\n\u0005\u0000\u00d1\u00d7\u0005\u0002\u0000"+
		"\u0000\u00d2\u00d3\u0003\n\u0005\u0000\u00d3\u00d4\u0005\u0002\u0000\u0000"+
		"\u00d4\u00d6\u0001\u0000\u0000\u0000\u00d5\u00d2\u0001\u0000\u0000\u0000"+
		"\u00d6\u00d9\u0001\u0000\u0000\u0000\u00d7\u00d5\u0001\u0000\u0000\u0000"+
		"\u00d7\u00d8\u0001\u0000\u0000\u0000\u00d8!\u0001\u0000\u0000\u0000\u00d9"+
		"\u00d7\u0001\u0000\u0000\u0000\u00da\u00df\u0005A\u0000\u0000\u00db\u00dc"+
		"\u0005\t\u0000\u0000\u00dc\u00de\u0005A\u0000\u0000\u00dd\u00db\u0001"+
		"\u0000\u0000\u0000\u00de\u00e1\u0001\u0000\u0000\u0000\u00df\u00dd\u0001"+
		"\u0000\u0000\u0000\u00df\u00e0\u0001\u0000\u0000\u0000\u00e0#\u0001\u0000"+
		"\u0000\u0000\u00e1\u00df\u0001\u0000\u0000\u0000\u00e2\u00e3\u0006\u0012"+
		"\uffff\uffff\u0000\u00e3\u00e7\u0003(\u0014\u0000\u00e4\u00e5\u0007\u0000"+
		"\u0000\u0000\u00e5\u00e7\u0003$\u0012\u0006\u00e6\u00e2\u0001\u0000\u0000"+
		"\u0000\u00e6\u00e4\u0001\u0000\u0000\u0000\u00e7\u00f9\u0001\u0000\u0000"+
		"\u0000\u00e8\u00e9\n\u0005\u0000\u0000\u00e9\u00ea\u0007\u0001\u0000\u0000"+
		"\u00ea\u00f8\u0003$\u0012\u0006\u00eb\u00ec\n\u0004\u0000\u0000\u00ec"+
		"\u00ed\u0007\u0002\u0000\u0000\u00ed\u00f8\u0003$\u0012\u0005\u00ee\u00ef"+
		"\n\u0003\u0000\u0000\u00ef\u00f0\u0007\u0003\u0000\u0000\u00f0\u00f8\u0003"+
		"$\u0012\u0004\u00f1\u00f2\n\u0002\u0000\u0000\u00f2\u00f3\u0005!\u0000"+
		"\u0000\u00f3\u00f8\u0003$\u0012\u0003\u00f4\u00f5\n\u0001\u0000\u0000"+
		"\u00f5\u00f6\u0005\"\u0000\u0000\u00f6\u00f8\u0003$\u0012\u0002\u00f7"+
		"\u00e8\u0001\u0000\u0000\u0000\u00f7\u00eb\u0001\u0000\u0000\u0000\u00f7"+
		"\u00ee\u0001\u0000\u0000\u0000\u00f7\u00f1\u0001\u0000\u0000\u0000\u00f7"+
		"\u00f4\u0001\u0000\u0000\u0000\u00f8\u00fb\u0001\u0000\u0000\u0000\u00f9"+
		"\u00f7\u0001\u0000\u0000\u0000\u00f9\u00fa\u0001\u0000\u0000\u0000\u00fa"+
		"%\u0001\u0000\u0000\u0000\u00fb\u00f9\u0001\u0000\u0000\u0000\u00fc\u0101"+
		"\u0003$\u0012\u0000\u00fd\u00fe\u0005\t\u0000\u0000\u00fe\u0100\u0003"+
		"$\u0012\u0000\u00ff\u00fd\u0001\u0000\u0000\u0000\u0100\u0103\u0001\u0000"+
		"\u0000\u0000\u0101\u00ff\u0001\u0000\u0000\u0000\u0101\u0102\u0001\u0000"+
		"\u0000\u0000\u0102\'\u0001\u0000\u0000\u0000\u0103\u0101\u0001\u0000\u0000"+
		"\u0000\u0104\u0105\u0006\u0014\uffff\uffff\u0000\u0105\u010a\u0003*\u0015"+
		"\u0000\u0106\u010a\u00034\u001a\u0000\u0107\u010a\u00036\u001b\u0000\u0108"+
		"\u010a\u00038\u001c\u0000\u0109\u0104\u0001\u0000\u0000\u0000\u0109\u0106"+
		"\u0001\u0000\u0000\u0000\u0109\u0107\u0001\u0000\u0000\u0000\u0109\u0108"+
		"\u0001\u0000\u0000\u0000\u010a\u0113\u0001\u0000\u0000\u0000\u010b\u010c"+
		"\n\u0006\u0000\u0000\u010c\u0112\u00032\u0019\u0000\u010d\u010e\n\u0005"+
		"\u0000\u0000\u010e\u0112\u0003.\u0017\u0000\u010f\u0110\n\u0004\u0000"+
		"\u0000\u0110\u0112\u00030\u0018\u0000\u0111\u010b\u0001\u0000\u0000\u0000"+
		"\u0111\u010d\u0001\u0000\u0000\u0000\u0111\u010f\u0001\u0000\u0000\u0000"+
		"\u0112\u0115\u0001\u0000\u0000\u0000\u0113\u0111\u0001\u0000\u0000\u0000"+
		"\u0113\u0114\u0001\u0000\u0000\u0000\u0114)\u0001\u0000\u0000\u0000\u0115"+
		"\u0113\u0001\u0000\u0000\u0000\u0116\u011d\u0003,\u0016\u0000\u0117\u011d"+
		"\u0005A\u0000\u0000\u0118\u0119\u0005\u0004\u0000\u0000\u0119\u011a\u0003"+
		"$\u0012\u0000\u011a\u011b\u0005\u0005\u0000\u0000\u011b\u011d\u0001\u0000"+
		"\u0000\u0000\u011c\u0116\u0001\u0000\u0000\u0000\u011c\u0117\u0001\u0000"+
		"\u0000\u0000\u011c\u0118\u0001\u0000\u0000\u0000\u011d+\u0001\u0000\u0000"+
		"\u0000\u011e\u011f\u0007\u0004\u0000\u0000\u011f-\u0001\u0000\u0000\u0000"+
		"\u0120\u0121\u0005\n\u0000\u0000\u0121\u0122\u0003$\u0012\u0000\u0122"+
		"\u0123\u0005\u000b\u0000\u0000\u0123/\u0001\u0000\u0000\u0000\u0124\u0126"+
		"\u0005\u0004\u0000\u0000\u0125\u0127\u0003&\u0013\u0000\u0126\u0125\u0001"+
		"\u0000\u0000\u0000\u0126\u0127\u0001\u0000\u0000\u0000\u0127\u0128\u0001"+
		"\u0000\u0000\u0000\u0128\u0129\u0005\u0005\u0000\u0000\u01291\u0001\u0000"+
		"\u0000\u0000\u012a\u012b\u0005#\u0000\u0000\u012b\u012c\u0005A\u0000\u0000"+
		"\u012c3\u0001\u0000\u0000\u0000\u012d\u012e\u0005$\u0000\u0000\u012e\u012f"+
		"\u0005\u0004\u0000\u0000\u012f\u0130\u0003$\u0012\u0000\u0130\u0131\u0005"+
		"\t\u0000\u0000\u0131\u0132\u0003$\u0012\u0000\u0132\u0133\u0005\u0005"+
		"\u0000\u0000\u01335\u0001\u0000\u0000\u0000\u0134\u0135\u0005%\u0000\u0000"+
		"\u0135\u0136\u0005\u0004\u0000\u0000\u0136\u0137\u0003$\u0012\u0000\u0137"+
		"\u0138\u0005\u0005\u0000\u0000\u01387\u0001\u0000\u0000\u0000\u0139\u013a"+
		"\u0005&\u0000\u0000\u013a\u013b\u0005\u0004\u0000\u0000\u013b\u013c\u0003"+
		"$\u0012\u0000\u013c\u013d\u0005\u0005\u0000\u0000\u013d9\u0001\u0000\u0000"+
		"\u0000\u013e\u0140\u0003>\u001f\u0000\u013f\u013e\u0001\u0000\u0000\u0000"+
		"\u0140\u0143\u0001\u0000\u0000\u0000\u0141\u013f\u0001\u0000\u0000\u0000"+
		"\u0141\u0142\u0001\u0000\u0000\u0000\u0142;\u0001\u0000\u0000\u0000\u0143"+
		"\u0141\u0001\u0000\u0000\u0000\u0144\u0145\u0005\r\u0000\u0000\u0145\u0146"+
		"\u0003:\u001d\u0000\u0146\u0147\u0005\u000e\u0000\u0000\u0147=\u0001\u0000"+
		"\u0000\u0000\u0148\u0149\u0005\'\u0000\u0000\u0149\u014b\u0005\u0004\u0000"+
		"\u0000\u014a\u014c\u0003&\u0013\u0000\u014b\u014a\u0001\u0000\u0000\u0000"+
		"\u014b\u014c\u0001\u0000\u0000\u0000\u014c\u014d\u0001\u0000\u0000\u0000"+
		"\u014d\u014e\u0005\u0005\u0000\u0000\u014e\u0171\u0005\u0002\u0000\u0000"+
		"\u014f\u0150\u0005(\u0000\u0000\u0150\u0152\u0005\u0004\u0000\u0000\u0151"+
		"\u0153\u0003&\u0013\u0000\u0152\u0151\u0001\u0000\u0000\u0000\u0152\u0153"+
		"\u0001\u0000\u0000\u0000\u0153\u0154\u0001\u0000\u0000\u0000\u0154\u0155"+
		"\u0005\u0005\u0000\u0000\u0155\u0171\u0005\u0002\u0000\u0000\u0156\u0158"+
		"\u0005)\u0000\u0000\u0157\u0159\u0003$\u0012\u0000\u0158\u0157\u0001\u0000"+
		"\u0000\u0000\u0158\u0159\u0001\u0000\u0000\u0000\u0159\u015a\u0001\u0000"+
		"\u0000\u0000\u015a\u0171\u0005\u0002\u0000\u0000\u015b\u015c\u0005*\u0000"+
		"\u0000\u015c\u0171\u0005\u0002\u0000\u0000\u015d\u015e\u0005+\u0000\u0000"+
		"\u015e\u0171\u0005\u0002\u0000\u0000\u015f\u0160\u0003@ \u0000\u0160\u0161"+
		"\u0005\u0002\u0000\u0000\u0161\u0171\u0001\u0000\u0000\u0000\u0162\u0163"+
		"\u0003<\u001e\u0000\u0163\u0164\u0005\u0002\u0000\u0000\u0164\u0171\u0001"+
		"\u0000\u0000\u0000\u0165\u0166\u0003J%\u0000\u0166\u0167\u0005\u0002\u0000"+
		"\u0000\u0167\u0171\u0001\u0000\u0000\u0000\u0168\u0169\u0003D\"\u0000"+
		"\u0169\u016a\u0005\u0002\u0000\u0000\u016a\u0171\u0001\u0000\u0000\u0000"+
		"\u016b\u016c\u0003H$\u0000\u016c\u016d\u0005\u0002\u0000\u0000\u016d\u0171"+
		"\u0001\u0000\u0000\u0000\u016e\u0171\u0003\f\u0006\u0000\u016f\u0171\u0003"+
		"\u0004\u0002\u0000\u0170\u0148\u0001\u0000\u0000\u0000\u0170\u014f\u0001"+
		"\u0000\u0000\u0000\u0170\u0156\u0001\u0000\u0000\u0000\u0170\u015b\u0001"+
		"\u0000\u0000\u0000\u0170\u015d\u0001\u0000\u0000\u0000\u0170\u015f\u0001"+
		"\u0000\u0000\u0000\u0170\u0162\u0001\u0000\u0000\u0000\u0170\u0165\u0001"+
		"\u0000\u0000\u0000\u0170\u0168\u0001\u0000\u0000\u0000\u0170\u016b\u0001"+
		"\u0000\u0000\u0000\u0170\u016e\u0001\u0000\u0000\u0000\u0170\u016f\u0001"+
		"\u0000\u0000\u0000\u0171?\u0001\u0000\u0000\u0000\u0172\u017d\u0001\u0000"+
		"\u0000\u0000\u0173\u0174\u0003&\u0013\u0000\u0174\u0175\u0005,\u0000\u0000"+
		"\u0175\u0176\u0003&\u0013\u0000\u0176\u017d\u0001\u0000\u0000\u0000\u0177"+
		"\u017d\u0003B!\u0000\u0178\u0179\u0003$\u0012\u0000\u0179\u017a\u0007"+
		"\u0005\u0000\u0000\u017a\u017d\u0001\u0000\u0000\u0000\u017b\u017d\u0003"+
		"$\u0012\u0000\u017c\u0172\u0001\u0000\u0000\u0000\u017c\u0173\u0001\u0000"+
		"\u0000\u0000\u017c\u0177\u0001\u0000\u0000\u0000\u017c\u0178\u0001\u0000"+
		"\u0000\u0000\u017c\u017b\u0001\u0000\u0000\u0000\u017dA\u0001\u0000\u0000"+
		"\u0000\u017e\u017f\u0003&\u0013\u0000\u017f\u0180\u0005\u0006\u0000\u0000"+
		"\u0180\u0181\u0003&\u0013\u0000\u0181\u0187\u0001\u0000\u0000\u0000\u0182"+
		"\u0183\u0003$\u0012\u0000\u0183\u0184\u0007\u0006\u0000\u0000\u0184\u0185"+
		"\u0003$\u0012\u0000\u0185\u0187\u0001\u0000\u0000\u0000\u0186\u017e\u0001"+
		"\u0000\u0000\u0000\u0186\u0182\u0001\u0000\u0000\u0000\u0187C\u0001\u0000"+
		"\u0000\u0000\u0188\u018c\u0005:\u0000\u0000\u0189\u018a\u0003@ \u0000"+
		"\u018a\u018b\u0005\u0002\u0000\u0000\u018b\u018d\u0001\u0000\u0000\u0000"+
		"\u018c\u0189\u0001\u0000\u0000\u0000\u018c\u018d\u0001\u0000\u0000\u0000"+
		"\u018d\u018e\u0001\u0000\u0000\u0000\u018e\u018f\u0003$\u0012\u0000\u018f"+
		"\u0191\u0003<\u001e\u0000\u0190\u0192\u0003F#\u0000\u0191\u0190\u0001"+
		"\u0000\u0000\u0000\u0191\u0192\u0001\u0000\u0000\u0000\u0192E\u0001\u0000"+
		"\u0000\u0000\u0193\u0196\u0005;\u0000\u0000\u0194\u0197\u0003D\"\u0000"+
		"\u0195\u0197\u0003<\u001e\u0000\u0196\u0194\u0001\u0000\u0000\u0000\u0196"+
		"\u0195\u0001\u0000\u0000\u0000\u0197G\u0001\u0000\u0000\u0000\u0198\u0199"+
		"\u0005<\u0000\u0000\u0199\u01a9\u0003<\u001e\u0000\u019a\u019b\u0005<"+
		"\u0000\u0000\u019b\u019c\u0003$\u0012\u0000\u019c\u019d\u0003<\u001e\u0000"+
		"\u019d\u01a9\u0001\u0000\u0000\u0000\u019e\u019f\u0005<\u0000\u0000\u019f"+
		"\u01a0\u0003@ \u0000\u01a0\u01a2\u0005\u0002\u0000\u0000\u01a1\u01a3\u0003"+
		"$\u0012\u0000\u01a2\u01a1\u0001\u0000\u0000\u0000\u01a2\u01a3\u0001\u0000"+
		"\u0000\u0000\u01a3\u01a4\u0001\u0000\u0000\u0000\u01a4\u01a5\u0005\u0002"+
		"\u0000\u0000\u01a5\u01a6\u0003@ \u0000\u01a6\u01a7\u0003<\u001e\u0000"+
		"\u01a7\u01a9\u0001\u0000\u0000\u0000\u01a8\u0198\u0001\u0000\u0000\u0000"+
		"\u01a8\u019a\u0001\u0000\u0000\u0000\u01a8\u019e\u0001\u0000\u0000\u0000"+
		"\u01a9I\u0001\u0000\u0000\u0000\u01aa\u01ae\u0005=\u0000\u0000\u01ab\u01ac"+
		"\u0003@ \u0000\u01ac\u01ad\u0005\u0002\u0000\u0000\u01ad\u01af\u0001\u0000"+
		"\u0000\u0000\u01ae\u01ab\u0001\u0000\u0000\u0000\u01ae\u01af\u0001\u0000"+
		"\u0000\u0000\u01af\u01b1\u0001\u0000\u0000\u0000\u01b0\u01b2\u0003$\u0012"+
		"\u0000\u01b1\u01b0\u0001\u0000\u0000\u0000\u01b1\u01b2\u0001\u0000\u0000"+
		"\u0000\u01b2\u01b3\u0001\u0000\u0000\u0000\u01b3\u01b4\u0005\r\u0000\u0000"+
		"\u01b4\u01b5\u0003L&\u0000\u01b5\u01b6\u0005\u000e\u0000\u0000\u01b6K"+
		"\u0001\u0000\u0000\u0000\u01b7\u01b9\u0003N\'\u0000\u01b8\u01b7\u0001"+
		"\u0000\u0000\u0000\u01b9\u01bc\u0001\u0000\u0000\u0000\u01ba\u01b8\u0001"+
		"\u0000\u0000\u0000\u01ba\u01bb\u0001\u0000\u0000\u0000\u01bbM\u0001\u0000"+
		"\u0000\u0000\u01bc\u01ba\u0001\u0000\u0000\u0000\u01bd\u01be\u0003P(\u0000"+
		"\u01be\u01bf\u0005>\u0000\u0000\u01bf\u01c0\u0003:\u001d\u0000\u01c0O"+
		"\u0001\u0000\u0000\u0000\u01c1\u01c2\u0005?\u0000\u0000\u01c2\u01c5\u0003"+
		"&\u0013\u0000\u01c3\u01c5\u0005@\u0000\u0000\u01c4\u01c1\u0001\u0000\u0000"+
		"\u0000\u01c4\u01c3\u0001\u0000\u0000\u0000\u01c5Q\u0001\u0000\u0000\u0000"+
		")[]gkt\u0081\u008d\u0091\u009a\u00a8\u00ac\u00b3\u00be\u00cc\u00d7\u00df"+
		"\u00e6\u00f7\u00f9\u0101\u0109\u0111\u0113\u011c\u0126\u0141\u014b\u0152"+
		"\u0158\u0170\u017c\u0186\u018c\u0191\u0196\u01a2\u01a8\u01ae\u01b1\u01ba"+
		"\u01c4";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}