using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace MiniGoCompiler
{
    public class ScopeCheckVisitor : MiniGoBaseVisitor<object?>
    {
        private readonly ScopeStack      _scopes                 = new();
        private readonly HashSet<string> _preRegisteredFunctions = new();

        // == Scope global ===================================
        public override object? VisitRoot([NotNull] MiniGoParser.RootContext context)
        {
            _scopes.PushScope();

            // Pre-declarar true y false como identificadores bool válidos
            // (en Mini-GO no son keywords del lexer, son identificadores built-in)
            _scopes.Define(new SymbolInfo("true",  new PrimitiveType("bool"), SymbolKind.Variable, 0, 0));
            _scopes.Define(new SymbolInfo("false", new PrimitiveType("bool"), SymbolKind.Variable, 0, 0));

            // PRE-PASO: registrar firmas de todas las funciones top-level antes de
            // visitar sus cuerpos. Permite forward references entre funciones,
            // consistente con la semántica de Go (el orden de declaración no importa).
            foreach (var funcDecl in context.topDeclarationList().funcDecl())
                RegisterFuncSignature(funcDecl);

            VisitChildren(context);
            _scopes.PopScope();
            return null;
        }

        // == Declaración de función ===================================
        public override object? VisitFuncDecl([NotNull] MiniGoParser.FuncDeclContext context)
        {
            var front = context.funcFrontDecl();
            string funcName = front.IDENTIFIER().GetText();
            int funcLine = front.IDENTIFIER().Symbol.Line;
            int funcCol  = front.IDENTIFIER().Symbol.Column;

            var paramTypes = new List<MiniGoType>();
            var paramInfos = new List<SymbolInfo>();

            var argDecls = front.funcArgDecls();
            if (argDecls != null)
            {
                foreach (var decl in argDecls.singleVarDeclNoExps())
                {
                    var type = ResolveType(decl.declType());
                    foreach (var id in decl.identifierList().IDENTIFIER())
                    {
                        paramTypes.Add(type);
                        paramInfos.Add(new SymbolInfo(
                            id.GetText(), type, SymbolKind.Parameter,
                            id.Symbol.Line, id.Symbol.Column));
                    }
                }
            }

            var retType = front.declType() != null
                ? ResolveType(front.declType())
                : new PrimitiveType("void");

            // Si la función fue pre-registrada en el pre-paso, no volver a
            // definirla para evitar un falso error de redeclaración.
            if (!_preRegisteredFunctions.Contains(funcName))
            {
                _scopes.Define(new SymbolInfo(
                    funcName, new FunctionType(paramTypes, retType),
                    SymbolKind.Function, funcLine, funcCol));
            }

            // Scope compartido para parámetros + cuerpo
            _scopes.PushScope();
            foreach (var p in paramInfos)
                _scopes.Define(p);

            // Visitar el cuerpo del bloque directamente para no disparar VisitBlock
            Visit(context.block().statementList());

            _scopes.PopScope();
            return null;
        }

        // == Bloques anidados (if / for / switch / standalone) ===================================
        public override object? VisitBlock([NotNull] MiniGoParser.BlockContext context)
        {
            _scopes.PushScope();
            VisitChildren(context);
            _scopes.PopScope();
            return null;
        }

        // == Loop statement: scope propio para variable del init ===================================
        // En la forma: for simpleStatement ; expression? ; simpleStatement block
        // la variable declarada en el init (ej: i := 0) no debe escapar al scope exterior,
        // consistente con la semántica de Go.
        public override object? VisitLoopStatement(
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

        // == Declaraciones de variable ===================================
        public override object? VisitVarDeclNoInit(
            [NotNull] MiniGoParser.VarDeclNoInitContext context)
        {
            var inner = context.singleVarDeclNoExps();
            var type  = ResolveType(inner.declType());
            foreach (var id in inner.identifierList().IDENTIFIER())
            {
                _scopes.Define(new SymbolInfo(
                    id.GetText(), type, SymbolKind.Variable,
                    id.Symbol.Line, id.Symbol.Column));
            }
            return null;
        }

        public override object? VisitVarDeclWithTypeAndInit(
            [NotNull] MiniGoParser.VarDeclWithTypeAndInitContext context)
        {
            Visit(context.expressionList()); // resolver RHS primero
            var type = ResolveType(context.declType());
            foreach (var id in context.identifierList().IDENTIFIER())
            {
                _scopes.Define(new SymbolInfo(
                    id.GetText(), type, SymbolKind.Variable,
                    id.Symbol.Line, id.Symbol.Column));
            }
            return null;
        }

        public override object? VisitVarDeclWithInitOnly(
            [NotNull] MiniGoParser.VarDeclWithInitOnlyContext context)
        {
            Visit(context.expressionList()); // resolver RHS primero
            foreach (var id in context.identifierList().IDENTIFIER())
            {
                _scopes.Define(new SymbolInfo(
                    id.GetText(), new PrimitiveType("?"), SymbolKind.Variable,
                    id.Symbol.Line, id.Symbol.Column));
            }
            return null;
        }

        // == Declaración corta ':=' ===========================================
        public override object? VisitShortVarDeclStmt(
            [NotNull] MiniGoParser.ShortVarDeclStmtContext context)
        {
            var lists = context.expressionList();
            Visit(lists[1]); // visitar RHS antes de declarar LHS

            foreach (var expr in lists[0].expression())
            {
                var id = ExtractIdentifier(expr);
                if (id != null)
                {
                    _scopes.Define(new SymbolInfo(
                        id.GetText(), new PrimitiveType("?"), SymbolKind.Variable,
                        id.Symbol.Line, id.Symbol.Column));
                }
            }
            return null;
        }

        // == Alias de tipo ========================================================
        public override object? VisitSingleTypeDecl(
            [NotNull] MiniGoParser.SingleTypeDeclContext context)
        {
            var id   = context.IDENTIFIER();
            var type = ResolveType(context.declType());
            _scopes.Define(new SymbolInfo(
                id.GetText(), type, SymbolKind.TypeAlias,
                id.Symbol.Line, id.Symbol.Column));
            return null;
        }

        // == Uso de identificador en expresión ===================================
        public override object? VisitOperand([NotNull] MiniGoParser.OperandContext context)
        {
            var id = context.IDENTIFIER();
            if (id != null)
            {
                _scopes.Lookup(id.GetText(), id.Symbol.Line, id.Symbol.Column);
                return null;
            }
            return VisitChildren(context);
        }

        // == Llamada a función: verificar aridad ===================================
        public override object? VisitArguments([NotNull] MiniGoParser.ArgumentsContext context)
        {
            if (context.Parent is MiniGoParser.PrimaryExpressionContext parentPrim)
            {
                var calleeId = parentPrim.primaryExpression()?.operand()?.IDENTIFIER();
                if (calleeId != null)
                {
                    var sym = _scopes.TryLookup(calleeId.GetText());
                    if (sym?.Type is FunctionType ft)
                    {
                        int argCount = context.expressionList()?.expression().Length ?? 0;
                        if (argCount != ft.ParamTypes.Count)
                        {
                            DiagnosticCollector.Instance.Add(
                                $"función '{calleeId.GetText()}' espera {ft.ParamTypes.Count} " +
                                $"argumento(s), se proporcionaron {argCount}",
                                context.Start.Line, context.Start.Column,
                                DiagnosticPhase.Semantic);
                        }
                    }
                }
            }
            return VisitChildren(context);
        }

        // == Acceso a campo de struct ===================================
        public override object? VisitSelector([NotNull] MiniGoParser.SelectorContext context)
        {
            if (context.Parent is MiniGoParser.PrimaryExpressionContext parentPrim)
            {
                var baseId = parentPrim.primaryExpression()?.operand()?.IDENTIFIER();
                if (baseId != null)
                {
                    var sym = _scopes.TryLookup(baseId.GetText());
                    if (sym?.Type is StructType st)
                    {
                        string field = context.IDENTIFIER().GetText();
                        if (!st.Fields.ContainsKey(field))
                        {
                            DiagnosticCollector.Instance.Add(
                                $"el tipo struct no tiene campo '{field}'",
                                context.IDENTIFIER().Symbol.Line,
                                context.IDENTIFIER().Symbol.Column,
                                DiagnosticPhase.Semantic);
                        }
                    }
                }
            }
            return null; // el IDENTIFIER del campo no es una variable
        }

        // == Helpers ===================================

        // Registra solo la firma de una función (nombre + parámetros + retorno)
        // sin visitar su cuerpo. Usado en el pre-paso de VisitRoot.
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
                : new PrimitiveType("void");

            _preRegisteredFunctions.Add(name);
            _scopes.Define(new SymbolInfo(
                name, new FunctionType(paramTypes, retType),
                SymbolKind.Function, line, col));
        }

        private MiniGoType ResolveType(MiniGoParser.DeclTypeContext ctx)
        {
            return ctx switch
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
        }

        private StructType ResolveStructType(MiniGoParser.StructDeclTypeContext ctx)
        {
            var fields   = new Dictionary<string, MiniGoType>();
            var memDecls = ctx.structMemDecls();
            if (memDecls != null)
            {
                foreach (var decl in memDecls.singleVarDeclNoExps())
                {
                    var type = ResolveType(decl.declType());
                    foreach (var id in decl.identifierList().IDENTIFIER())
                        fields[id.GetText()] = type;
                }
            }
            return new StructType(fields);
        }

        // Extrae el token IDENTIFIER de una expresión simple (primaryExpr -> operand -> ID)
        private static ITerminalNode? ExtractIdentifier(MiniGoParser.ExpressionContext expr)
        {
            if (expr is MiniGoParser.PrimaryExprContext pe)
                return pe.primaryExpression()?.operand()?.IDENTIFIER();
            return null;
        }
    }
}
