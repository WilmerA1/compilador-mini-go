using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace MiniGoCompiler
{
    /// <summary>
    /// Fase 3 — Verificación de tipos.
    /// Recorre el árbol de parseo, infiere el tipo de cada expresión y verifica:
    ///   - Compatibilidad de operadores con sus operandos.
    ///   - Tipo de retorno de funciones vs. declaración.
    ///   - Condiciones de if/for deben ser bool.
    ///   - Compatibilidad en asignaciones.
    /// </summary>
    public sealed class TypeCheckVisitor : MiniGoBaseVisitor<MiniGoType?>
    {
        // ── Estado interno ────────────────────────────────────────────────────
        private readonly ScopeStack      _scopes                 = new();
        private readonly HashSet<string> _preRegisteredFunctions = new();
        private          MiniGoType?     _currentReturnType      = null;

        // ── Constantes de tipos (evita strings literales dispersos) ───────────
        private static readonly PrimitiveType TInt    = new("int");
        private static readonly PrimitiveType TFloat  = new("float64");
        private static readonly PrimitiveType TString = new("string");
        private static readonly PrimitiveType TBool   = new("bool");
        private static readonly PrimitiveType TRune   = new("rune");
        private static readonly PrimitiveType TVoid   = new("void");

        // ═════════════════════════════════════════════════════════════════════
        // SCOPE — Root, funciones, bloques, for
        // ═════════════════════════════════════════════════════════════════════

        public override MiniGoType? VisitRoot([NotNull] MiniGoParser.RootContext context)
        {
            _scopes.PushScope();

            // Pre-declarar true y false para que se puedan usar como bool
            _scopes.Define(new SymbolInfo("true",  TBool, SymbolKind.Variable, 0, 0));
            _scopes.Define(new SymbolInfo("false", TBool, SymbolKind.Variable, 0, 0));

            // Pre-paso: registrar firmas de todas las funciones top-level
            // antes de visitar sus cuerpos → permite forward references
            foreach (var fd in context.topDeclarationList().funcDecl())
                RegisterFuncSignature(fd);

            VisitChildren(context);
            _scopes.PopScope();
            return null;
        }

        public override MiniGoType? VisitFuncDecl([NotNull] MiniGoParser.FuncDeclContext context)
        {
            var front   = context.funcFrontDecl();
            string name = front.IDENTIFIER().GetText();

            // Recolectar parámetros
            var paramInfos = new List<SymbolInfo>();
            var argDecls   = front.funcArgDecls();
            if (argDecls != null)
            {
                foreach (var decl in argDecls.singleVarDeclNoExps())
                {
                    var type = ResolveType(decl.declType());
                    foreach (var id in decl.identifierList().IDENTIFIER())
                        paramInfos.Add(new SymbolInfo(
                            id.GetText(), type, SymbolKind.Parameter,
                            id.Symbol.Line, id.Symbol.Column));
                }
            }

            var retType = front.declType() != null
                ? ResolveType(front.declType())
                : TVoid;

            // Registrar si no fue pre-registrada en el pre-paso
            if (!_preRegisteredFunctions.Contains(name))
            {
                var pts = new List<MiniGoType>();
                foreach (var p in paramInfos) pts.Add(p.Type);
                _scopes.Define(new SymbolInfo(
                    name, new FunctionType(pts, retType),
                    SymbolKind.Function,
                    front.IDENTIFIER().Symbol.Line,
                    front.IDENTIFIER().Symbol.Column));
            }

            // Guardar y restaurar el tipo de retorno al salir de la función
            var savedReturn    = _currentReturnType;
            _currentReturnType = retType;

            // Scope compartido: parámetros + cuerpo (no dispara VisitBlock)
            _scopes.PushScope();
            foreach (var p in paramInfos)
                _scopes.Define(p);
            Visit(context.block().statementList());
            _scopes.PopScope();

            _currentReturnType = savedReturn;
            return null;
        }

        public override MiniGoType? VisitBlock([NotNull] MiniGoParser.BlockContext context)
        {
            _scopes.PushScope();
            VisitChildren(context);
            _scopes.PopScope();
            return null;
        }

        // Scope extra para la variable del init en: for init; cond; post { }
        public override MiniGoType? VisitLoopStatement(
            [NotNull] MiniGoParser.LoopStatementContext context)
        {
            bool hasInit = context.simpleStatement() is { Length: > 0 };
            if (hasInit)
            {
                _scopes.PushScope();
                VisitChildren(context);
                _scopes.PopScope();
            }
            else
            {
                VisitChildren(context);
            }
            return null;
        }

        // ═════════════════════════════════════════════════════════════════════
        // DECLARACIONES DE VARIABLE
        // ═════════════════════════════════════════════════════════════════════

        // var x T
        public override MiniGoType? VisitVarDeclNoInit(
            [NotNull] MiniGoParser.VarDeclNoInitContext context)
        {
            var inner = context.singleVarDeclNoExps();
            var type  = ResolveType(inner.declType());
            foreach (var id in inner.identifierList().IDENTIFIER())
                _scopes.Define(new SymbolInfo(
                    id.GetText(), type, SymbolKind.Variable,
                    id.Symbol.Line, id.Symbol.Column));
            return null;
        }

        // var x T = expr   → verifica compatibilidad tipo declarado vs. RHS
        public override MiniGoType? VisitVarDeclWithTypeAndInit(
            [NotNull] MiniGoParser.VarDeclWithTypeAndInitContext context)
        {
            var declaredType = ResolveType(context.declType());
            var exprs        = context.expressionList().expression();

            int i = 0;
            foreach (var id in context.identifierList().IDENTIFIER())
            {
                MiniGoType? rhsType = i < exprs.Length ? Visit(exprs[i]) : null;
                if (rhsType is not null && rhsType is not ErrorType
                    && !TypesCompatible(declaredType, rhsType))
                {
                    AddError(id.Symbol.Line, id.Symbol.Column,
                        $"no se puede asignar '{rhsType}' a variable de tipo '{declaredType}'");
                }
                _scopes.Define(new SymbolInfo(
                    id.GetText(), declaredType, SymbolKind.Variable,
                    id.Symbol.Line, id.Symbol.Column));
                i++;
            }
            return null;
        }

        // var x = expr   → infiere tipo del RHS
        public override MiniGoType? VisitVarDeclWithInitOnly(
            [NotNull] MiniGoParser.VarDeclWithInitOnlyContext context)
        {
            var exprs = context.expressionList().expression();
            int i     = 0;
            foreach (var id in context.identifierList().IDENTIFIER())
            {
                var inferred = i < exprs.Length
                    ? (Visit(exprs[i]) ?? new PrimitiveType("?"))
                    : new PrimitiveType("?");
                _scopes.Define(new SymbolInfo(
                    id.GetText(), inferred, SymbolKind.Variable,
                    id.Symbol.Line, id.Symbol.Column));
                i++;
            }
            return null;
        }

        // x := expr   → infiere tipo del RHS
        public override MiniGoType? VisitShortVarDeclStmt(
            [NotNull] MiniGoParser.ShortVarDeclStmtContext context)
        {
            var lists    = context.expressionList();
            var rhsExprs = lists[1].expression();

            int i = 0;
            foreach (var lhsExpr in lists[0].expression())
            {
                var inferred = i < rhsExprs.Length
                    ? (Visit(rhsExprs[i]) ?? new PrimitiveType("?"))
                    : new PrimitiveType("?");

                var id = ExtractIdentifier(lhsExpr);
                if (id is not null)
                    _scopes.Define(new SymbolInfo(
                        id.GetText(), inferred, SymbolKind.Variable,
                        id.Symbol.Line, id.Symbol.Column));
                i++;
            }
            return null;
        }

        // type Alias T
        public override MiniGoType? VisitSingleTypeDecl(
            [NotNull] MiniGoParser.SingleTypeDeclContext context)
        {
            var id   = context.IDENTIFIER();
            var type = ResolveType(context.declType());
            _scopes.Define(new SymbolInfo(
                id.GetText(), type, SymbolKind.TypeAlias,
                id.Symbol.Line, id.Symbol.Column));
            return null;
        }

        // ═════════════════════════════════════════════════════════════════════
        // LITERALES E IDENTIFICADORES → inferencia de tipo
        // ═════════════════════════════════════════════════════════════════════

        public override MiniGoType? VisitLiteral(
            [NotNull] MiniGoParser.LiteralContext context)
        {
            if (context.INTLITERAL()               != null) return TInt;
            if (context.FLOATLITERAL()             != null) return TFloat;
            if (context.RUNELITERAL()              != null) return TRune;
            if (context.RAWSTRINGLITERAL()         != null) return TString;
            if (context.INTERPRETEDSTRINGLITERAL() != null) return TString;
            return null;
        }

        public override MiniGoType? VisitOperand(
            [NotNull] MiniGoParser.OperandContext context)
        {
            // Literal
            if (context.literal() != null)
                return Visit(context.literal());

            // Identificador → buscar en scope
            var id = context.IDENTIFIER();
            if (id != null)
            {
                var sym = _scopes.TryLookup(id.GetText());
                if (sym is null)
                {
                    AddError(id.Symbol.Line, id.Symbol.Column,
                        $"identificador '{id.GetText()}' no declarado");
                    return ErrorType.Instance;
                }
                return sym.Type;
            }

            // Expresión entre paréntesis
            if (context.expression() != null)
                return Visit(context.expression());

            return null;
        }

        // PrimaryExpr (alternativa etiquetada) → delega a primaryExpression
        public override MiniGoType? VisitPrimaryExpr(
            [NotNull] MiniGoParser.PrimaryExprContext context)
            => Visit(context.primaryExpression());

        // ═════════════════════════════════════════════════════════════════════
        // HELPERS
        // ═════════════════════════════════════════════════════════════════════

        /// Registra la firma de una función sin visitar su cuerpo.
        private void RegisterFuncSignature(MiniGoParser.FuncDeclContext context)
        {
            var front   = context.funcFrontDecl();
            string name = front.IDENTIFIER().GetText();
            int line    = front.IDENTIFIER().Symbol.Line;
            int col     = front.IDENTIFIER().Symbol.Column;

            var paramTypes = new List<MiniGoType>();
            var argDecls   = front.funcArgDecls();
            if (argDecls != null)
                foreach (var decl in argDecls.singleVarDeclNoExps())
                {
                    var type = ResolveType(decl.declType());
                    foreach (var _ in decl.identifierList().IDENTIFIER())
                        paramTypes.Add(type);
                }

            var retType = front.declType() != null
                ? ResolveType(front.declType())
                : TVoid;

            _preRegisteredFunctions.Add(name);
            _scopes.Define(new SymbolInfo(
                name, new FunctionType(paramTypes, retType),
                SymbolKind.Function, line, col));
        }

        /// Dos tipos son compatibles si tienen el mismo nombre (primitivos)
        /// o la misma representación estructural.
        /// ErrorType siempre es compatible para silenciar errores en cascada.
        internal static bool TypesCompatible(MiniGoType expected, MiniGoType? actual)
        {
            if (actual is null)             return false;
            if (actual  is ErrorType)       return true;
            if (expected is ErrorType)      return true;
            if (expected is PrimitiveType pe && actual is PrimitiveType pa)
                return pe.Name == pa.Name || pa.Name == "?";
            return expected.GetType() == actual.GetType()
                && expected.ToString() == actual.ToString();
        }

        /// Registra un error de tipo semántico y retorna ErrorType.
        internal MiniGoType? TypeError(int line, int col, string msg)
        {
            AddError(line, col, msg);
            return ErrorType.Instance;
        }

        internal static void AddError(int line, int col, string msg) =>
            DiagnosticCollector.Instance.Add(msg, line, col, DiagnosticPhase.Semantic);

        // Resolución de tipos desde el nodo gramatical
        private MiniGoType ResolveType(MiniGoParser.DeclTypeContext ctx) =>
            ctx switch
            {
                MiniGoParser.TypePrimitiveOrCustomContext p =>
                    new PrimitiveType(p.IDENTIFIER().GetText()),

                MiniGoParser.TypeParenContext par =>
                    ResolveType(par.declType()),

                MiniGoParser.TypeSliceContext s =>
                    new SliceType(ResolveType(s.sliceDeclType().declType())),

                MiniGoParser.TypeArrayContext a =>
                    new ArrayType(
                        int.Parse(a.arrayDeclType().INTLITERAL().GetText()),
                        ResolveType(a.arrayDeclType().declType())),

                MiniGoParser.TypeStructContext st =>
                    ResolveStructType(st.structDeclType()),

                _ => ErrorType.Instance
            };

        private StructType ResolveStructType(MiniGoParser.StructDeclTypeContext ctx)
        {
            var fields   = new Dictionary<string, MiniGoType>();
            var memDecls = ctx.structMemDecls();
            if (memDecls != null)
                foreach (var decl in memDecls.singleVarDeclNoExps())
                {
                    var type = ResolveType(decl.declType());
                    foreach (var id in decl.identifierList().IDENTIFIER())
                        fields[id.GetText()] = type;
                }
            return new StructType(fields);
        }

        private static ITerminalNode? ExtractIdentifier(MiniGoParser.ExpressionContext expr)
        {
            if (expr is MiniGoParser.PrimaryExprContext pe)
                return pe.primaryExpression()?.operand()?.IDENTIFIER();
            return null;
        }

        // Expone constantes de tipos para uso en clases derivadas o externas
        internal static PrimitiveType BoolType   => TBool;
        internal static PrimitiveType IntType    => TInt;
        internal static PrimitiveType FloatType  => TFloat;
        internal static PrimitiveType StringType => TString;
        internal static PrimitiveType RuneType   => TRune;
        internal static PrimitiveType VoidType   => TVoid;
    }
}
