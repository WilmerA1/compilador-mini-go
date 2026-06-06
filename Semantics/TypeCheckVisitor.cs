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

        // for — scope del init + verificación de condición bool
        public override MiniGoType? VisitLoopStatement(
            [NotNull] MiniGoParser.LoopStatementContext context)
        {
            var simpleStmts = context.simpleStatement();
            bool hasInit    = simpleStmts is { Length: > 0 };

            if (hasInit)
            {
                // Forma: for init; cond?; post { }
                _scopes.PushScope();
                Visit(simpleStmts[0]);                      // init
                var cond = context.expression();
                if (cond != null)
                {
                    var condType = Visit(cond);
                    if (condType is not null and not ErrorType
                        && !TypesCompatible(TBool, condType))
                    {
                        AddError(cond.Start.Line, cond.Start.Column,
                            $"condición del 'for' debe ser bool, se recibió '{condType}'");
                    }
                }
                if (simpleStmts.Length > 1) Visit(simpleStmts[1]); // post
                Visit(context.block());
                _scopes.PopScope();
            }
            else
            {
                // Forma: for expr? { }
                var cond = context.expression();
                if (cond != null)
                {
                    var condType = Visit(cond);
                    if (condType is not null and not ErrorType
                        && !TypesCompatible(TBool, condType))
                    {
                        AddError(cond.Start.Line, cond.Start.Column,
                            $"condición del 'for' debe ser bool, se recibió '{condType}'");
                    }
                }
                Visit(context.block());
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

        // ═════════════════════════════════════════════════════════════════════
        // STATEMENTS — return, if, for (condición), asignaciones, print
        // ═════════════════════════════════════════════════════════════════════

        // return expr?
        public override MiniGoType? VisitReturnStmt(
            [NotNull] MiniGoParser.ReturnStmtContext context)
        {
            if (context.expression() == null)
            {
                // return vacío → función debe ser void
                if (_currentReturnType is not null
                    && !TypesCompatible(TVoid, _currentReturnType))
                {
                    AddError(context.Start.Line, context.Start.Column,
                        $"función debe retornar '{_currentReturnType}', " +
                        $"no se puede retornar vacío");
                }
                return null;
            }

            var retType = Visit(context.expression());
            if (_currentReturnType is not null
                && retType is not null and not ErrorType
                && !TypesCompatible(_currentReturnType, retType))
            {
                AddError(context.Start.Line, context.Start.Column,
                    $"tipo de retorno incorrecto: se esperaba '{_currentReturnType}', " +
                    $"se retornó '{retType}'");
            }
            return retType;
        }

        // if (simpleStmt;)? expr block elseClause?
        public override MiniGoType? VisitIfStmtWrapper(
            [NotNull] MiniGoParser.IfStmtWrapperContext context)
            => Visit(context.ifStatement());

        public override MiniGoType? VisitIfStatement(
            [NotNull] MiniGoParser.IfStatementContext context)
        {
            bool hasInit = context.simpleStatement() != null;
            if (hasInit) _scopes.PushScope();

            if (hasInit) Visit(context.simpleStatement());

            var condType = Visit(context.expression());
            if (condType is not null and not ErrorType
                && !TypesCompatible(TBool, condType))
            {
                AddError(context.expression().Start.Line,
                         context.expression().Start.Column,
                    $"condición del 'if' debe ser bool, se recibió '{condType}'");
            }

            Visit(context.block());
            if (context.elseClause() != null) Visit(context.elseClause());

            if (hasInit) _scopes.PopScope();
            return null;
        }

        // for: verificar condición bool (el scope del init ya lo maneja VisitLoopStatement)
        // Se sobreescribe VisitLoopStatement para agregar la verificación de condición.
        // NOTA: reemplaza la versión de scope-only del primer commit.

        // Asignación simple: lhs = rhs
        public override MiniGoType? VisitAssignSimple(
            [NotNull] MiniGoParser.AssignSimpleContext context)
        {
            var lists    = context.expressionList();
            var lhsExprs = lists[0].expression();
            var rhsExprs = lists[1].expression();

            for (int i = 0; i < Math.Min(lhsExprs.Length, rhsExprs.Length); i++)
            {
                var lhsType = Visit(lhsExprs[i]);
                var rhsType = Visit(rhsExprs[i]);
                if (lhsType is not null and not ErrorType
                    && rhsType is not null and not ErrorType
                    && !TypesCompatible(lhsType, rhsType))
                {
                    AddError(lhsExprs[i].Start.Line, lhsExprs[i].Start.Column,
                        $"no se puede asignar '{rhsType}' a '{lhsType}'");
                }
            }
            return null;
        }

        // Asignación compuesta: lhs += rhs  etc.
        public override MiniGoType? VisitAssignCompound(
            [NotNull] MiniGoParser.AssignCompoundContext context)
        {
            var lhsType = Visit(context.expression(0));
            var rhsType = Visit(context.expression(1));
            if (lhsType is not null and not ErrorType
                && rhsType is not null and not ErrorType
                && !TypesCompatible(lhsType, rhsType))
            {
                AddError(context.Start.Line, context.Start.Column,
                    $"asignación compuesta: tipos incompatibles '{lhsType}' y '{rhsType}'");
            }
            return null;
        }

        // print / println — no restringen tipos, solo visitan argumentos
        public override MiniGoType? VisitPrintStmt(
            [NotNull] MiniGoParser.PrintStmtContext context)
        {
            if (context.expressionList() != null) Visit(context.expressionList());
            return null;
        }

        public override MiniGoType? VisitPrintlnStmt(
            [NotNull] MiniGoParser.PrintlnStmtContext context)
        {
            if (context.expressionList() != null) Visit(context.expressionList());
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
        // PRIMARY EXPRESSION — llamadas, selectores, índices, append/len/cap
        // ═════════════════════════════════════════════════════════════════════

        public override MiniGoType? VisitPrimaryExpression(
            [NotNull] MiniGoParser.PrimaryExpressionContext context)
        {
            // Caso base: solo un operand
            if (context.operand() != null)
                return Visit(context.operand());

            // append(slice, elem) → devuelve el mismo tipo de slice
            if (context.appendExpression() != null)
            {
                var ae        = context.appendExpression();
                var sliceType = Visit(ae.expression(0));
                Visit(ae.expression(1));
                return sliceType;
            }

            // len(expr) → int
            if (context.lengthExpression() != null)
            {
                Visit(context.lengthExpression().expression());
                return TInt;
            }

            // cap(expr) → int
            if (context.capExpression() != null)
            {
                Visit(context.capExpression().expression());
                return TInt;
            }

            // primaryExpression.field → acceso a campo de struct
            if (context.selector() != null)
            {
                var baseType = Visit(context.primaryExpression());
                if (baseType is StructType st)
                {
                    string field = context.selector().IDENTIFIER().GetText();
                    if (st.Fields.TryGetValue(field, out var fieldType))
                        return fieldType;
                    return TypeError(
                        context.selector().IDENTIFIER().Symbol.Line,
                        context.selector().IDENTIFIER().Symbol.Column,
                        $"el tipo struct no tiene campo '{field}'");
                }
                if (baseType is not null and not ErrorType)
                    AddError(context.selector().Start.Line, context.selector().Start.Column,
                        $"acceso a campo '.' en tipo '{baseType}' que no es struct");
                return ErrorType.Instance;
            }

            // primaryExpression[expr] → acceso a array o slice
            if (context.index() != null)
            {
                var baseType = Visit(context.primaryExpression());
                var idxType  = Visit(context.index().expression());
                if (idxType is not null and not ErrorType && !TypesCompatible(TInt, idxType))
                    AddError(context.index().Start.Line, context.index().Start.Column,
                        $"índice debe ser int, se recibió '{idxType}'");
                if (baseType is ArrayType at) return at.ElemType;
                if (baseType is SliceType sl) return sl.ElemType;
                if (baseType is not null and not ErrorType)
                    AddError(context.index().Start.Line, context.index().Start.Column,
                        $"índice aplicado a tipo '{baseType}' que no es array ni slice");
                return ErrorType.Instance;
            }

            // primaryExpression(args) → llamada a función
            if (context.arguments() != null)
            {
                var calleeType = Visit(context.primaryExpression());
                var argExprs   = context.arguments().expressionList()?.expression()
                                 ?? Array.Empty<MiniGoParser.ExpressionContext>();

                if (calleeType is FunctionType ft)
                {
                    if (argExprs.Length != ft.ParamTypes.Count)
                    {
                        AddError(context.arguments().Start.Line,
                                 context.arguments().Start.Column,
                            $"se esperaban {ft.ParamTypes.Count} argumento(s), " +
                            $"se proporcionaron {argExprs.Length}");
                    }
                    else
                    {
                        for (int i = 0; i < argExprs.Length; i++)
                        {
                            var argType = Visit(argExprs[i]);
                            if (argType is not null and not ErrorType
                                && !TypesCompatible(ft.ParamTypes[i], argType))
                            {
                                AddError(argExprs[i].Start.Line, argExprs[i].Start.Column,
                                    $"argumento {i + 1}: se esperaba '{ft.ParamTypes[i]}', " +
                                    $"se recibió '{argType}'");
                            }
                        }
                    }
                    return ft.ReturnType is PrimitiveType { Name: "void" }
                        ? null
                        : ft.ReturnType;
                }

                // Callee no es función reconocida → visitar argumentos igualmente
                foreach (var arg in argExprs) Visit(arg);
                return null;
            }

            return null;
        }

        // ═════════════════════════════════════════════════════════════════════
        // OPERADORES — inferencia y verificación de tipos
        // ═════════════════════════════════════════════════════════════════════

        // Unarios: + - ! ^
        public override MiniGoType? VisitUnaryExpr(
            [NotNull] MiniGoParser.UnaryExprContext context)
        {
            var operandType = Visit(context.expression());
            if (operandType is ErrorType) return ErrorType.Instance;

            string op = context.GetChild(0).GetText();
            return op switch
            {
                "!" => TypesCompatible(TBool, operandType)
                        ? TBool
                        : TypeError(context.Start.Line, context.Start.Column,
                            $"operador '!' requiere bool, se recibió '{operandType}'"),

                "-" or "+" => IsNumericOrRune(operandType)
                        ? operandType
                        : TypeError(context.Start.Line, context.Start.Column,
                            $"operador unario '{op}' requiere int o float64, " +
                            $"se recibió '{operandType}'"),

                "^" => TypesCompatible(TInt, operandType)
                        ? TInt
                        : TypeError(context.Start.Line, context.Start.Column,
                            $"operador '^' requiere int, se recibió '{operandType}'"),

                _ => operandType
            };
        }

        // Multiplicativos: * / % << >> & &^
        public override MiniGoType? VisitMulExpr(
            [NotNull] MiniGoParser.MulExprContext context)
        {
            var left  = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            if (left is ErrorType || right is ErrorType) return ErrorType.Instance;
            return VerifyArithmetic(
                context.GetChild(1).GetText(), left, right,
                context.Start.Line, context.Start.Column);
        }

        // Aditivos: + - | ^
        public override MiniGoType? VisitAddExpr(
            [NotNull] MiniGoParser.AddExprContext context)
        {
            var left  = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            if (left is ErrorType || right is ErrorType) return ErrorType.Instance;
            string op = context.GetChild(1).GetText();
            return op == "+"
                ? VerifyAdd(left, right, context.Start.Line, context.Start.Column)
                : VerifyArithmetic(op, left, right, context.Start.Line, context.Start.Column);
        }

        // Comparación: == != < <= > >=
        public override MiniGoType? VisitRelExpr(
            [NotNull] MiniGoParser.RelExprContext context)
        {
            var left  = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            if (left is ErrorType || right is ErrorType) return TBool;

            string op = context.GetChild(1).GetText();
            if (op is "==" or "!=")
            {
                if (!TypesCompatible(left!, right))
                    AddError(context.Start.Line, context.Start.Column,
                        $"operador '{op}' requiere operandos del mismo tipo " +
                        $"('{left}' vs '{right}')");
                return TBool;
            }
            // < <= > >=
            if (!IsNumericOrRune(left) || !IsNumericOrRune(right))
                AddError(context.Start.Line, context.Start.Column,
                    $"operador '{op}' requiere int, float64 o rune " +
                    $"(se recibió '{left}' y '{right}')");
            return TBool;
        }

        // AND lógico: &&
        public override MiniGoType? VisitAndExpr(
            [NotNull] MiniGoParser.AndExprContext context)
        {
            var left  = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            if (left is ErrorType || right is ErrorType) return TBool;
            if (!TypesCompatible(TBool, left) || !TypesCompatible(TBool, right))
                AddError(context.Start.Line, context.Start.Column,
                    $"operador '&&' requiere bool en ambos lados " +
                    $"(se recibió '{left}' y '{right}')");
            return TBool;
        }

        // OR lógico: ||
        public override MiniGoType? VisitOrExpr(
            [NotNull] MiniGoParser.OrExprContext context)
        {
            var left  = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            if (left is ErrorType || right is ErrorType) return TBool;
            if (!TypesCompatible(TBool, left) || !TypesCompatible(TBool, right))
                AddError(context.Start.Line, context.Start.Column,
                    $"operador '||' requiere bool en ambos lados " +
                    $"(se recibió '{left}' y '{right}')");
            return TBool;
        }

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

        // Verifica operadores aritméticos (-, *, /, %, <<, >>, &, &^, |, ^)
        private MiniGoType? VerifyArithmetic(string op, MiniGoType? left, MiniGoType? right,
                                             int line, int col)
        {
            if (left is PrimitiveType lp && right is PrimitiveType rp)
            {
                if (lp.Name == rp.Name && IsNumericOrRune(left)) return left;
                if ((lp.Name == "int"     && rp.Name == "float64") ||
                    (lp.Name == "float64" && rp.Name == "int"))
                    return TFloat;
            }
            return TypeError(line, col,
                $"operador '{op}' no compatible con tipos '{left}' y '{right}'");
        }

        // + admite: int+int, float64+float64, string+string, rune+rune, int+float64
        private MiniGoType? VerifyAdd(MiniGoType? left, MiniGoType? right, int line, int col)
        {
            if (left is PrimitiveType lp && right is PrimitiveType rp)
            {
                if (lp.Name == rp.Name && lp.Name is "int" or "float64" or "string" or "rune")
                    return left;
                if ((lp.Name == "int"     && rp.Name == "float64") ||
                    (lp.Name == "float64" && rp.Name == "int"))
                    return TFloat;
            }
            return TypeError(line, col,
                $"operador '+' no compatible con tipos '{left}' y '{right}'");
        }

        private static bool IsNumericOrRune(MiniGoType? t) =>
            t is PrimitiveType { Name: "int" or "float64" or "rune" };

        // Expone constantes de tipos para uso en clases derivadas o externas
        internal static PrimitiveType BoolType   => TBool;
        internal static PrimitiveType IntType    => TInt;
        internal static PrimitiveType FloatType  => TFloat;
        internal static PrimitiveType StringType => TString;
        internal static PrimitiveType RuneType   => TRune;
        internal static PrimitiveType VoidType   => TVoid;
    }
}
