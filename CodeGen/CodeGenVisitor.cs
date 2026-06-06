using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace MiniGoCompiler
{
    /// <summary>
    /// Fase 4 — Generación de código LLVM IR.
    /// Recorre el AST (después de las fases de scope y tipo) y emite texto LLVM IR.
    /// El resultado se obtiene con GetIR() y se escribe directamente a un archivo .ll
    /// que luego puede compilarse con: clang output.ll -o output.exe
    /// </summary>
    public sealed class CodeGenVisitor : MiniGoBaseVisitor<LLVMValue?>
    {
        // ── Buffers de salida ────────────────────────────────────────────────
        private readonly StringBuilder _globals = new();  // constantes y vars globales
        private readonly StringBuilder _funcs   = new();  // cuerpos de funciones
        private          StringBuilder _cur;              // buffer activo al emitir

        // ── Contadores SSA ───────────────────────────────────────────────────
        private int _reg = 0;   // registros: %t0, %t1, ...
        private int _lbl = 0;   // etiquetas: L0, L1, ...

        // ── Tabla de string literals globales ────────────────────────────────
        private readonly Dictionary<string, (string Name, int ByteLen)> _strTab = new();
        private int _strIdx = 0;

        // ── Entorno de variables: nombre → (registro alloca, tipo LLVM) ──────
        private record EnvEntry(string AllocaReg, string LLVMType, bool IsGlobal = false);
        private readonly Stack<Dictionary<string, EnvEntry>> _env = new();

        // ── Firmas de funciones registradas ─────────────────────────────────
        private record FuncSig(string RetType, List<string> ParamTypes, List<string> ParamNames);
        private readonly Dictionary<string, FuncSig> _funcSigs = new();

        // ── Estado de la función que se está generando ────────────────────────
        private string _curRetType       = "void";
        private bool   _blockTerminated  = false;  // true tras emitir ret/br
        private string _breakTarget      = "";     // bloque destino de break
        private string _continueTarget   = "";     // bloque destino de continue

        public CodeGenVisitor() => _cur = _funcs;

        // ════════════════════════════════════════════════════════════════════
        //  API PÚBLICA
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Devuelve el módulo LLVM IR completo listo para escribir a un .ll
        /// </summary>
        public string GetIR()
        {
            var sb = new StringBuilder();
            sb.AppendLine("; Mini-GO — módulo compilado");
            sb.AppendLine("target triple = \"x86_64-pc-windows-msvc\"");
            sb.AppendLine();
            sb.AppendLine("; ─── declaraciones externas ─────────────────────────");
            sb.AppendLine("declare i32 @printf(i8* noundef, ...)");
            sb.AppendLine("declare void @exit(i32 noundef)");

            if (_globals.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine("; ─── constantes y variables globales ───────────────");
                sb.Append(_globals);
            }

            if (_funcs.Length > 0)
            {
                sb.AppendLine();
                sb.Append(_funcs);
            }

            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELPERS DE EMISIÓN
        // ════════════════════════════════════════════════════════════════════

        /// Emite una línea al buffer de globales.
        private void G(string line) => _globals.AppendLine(line);

        /// Emite una instrucción indentada al buffer activo.
        private void E(string line) => _cur.AppendLine($"  {line}");

        /// Emite una etiqueta (sin indentación) al buffer activo.
        private void Lbl(string name) => _cur.AppendLine($"\n{name}:");

        /// Emite una línea en blanco al buffer activo.
        private void EBlank() => _cur.AppendLine();

        /// Crea un nuevo nombre de registro SSA único.
        private string NewReg(string hint = "t") => $"%{hint}{_reg++}";

        /// Crea un nuevo nombre de etiqueta único.
        private string NewLbl(string hint = "L")  => $"{hint}{_lbl++}";

        // ════════════════════════════════════════════════════════════════════
        //  CONVERSIÓN DE TIPOS
        // ════════════════════════════════════════════════════════════════════

        /// Convierte un MiniGoType a su representación en tipo LLVM IR.
        public static string LLVMOf(MiniGoType? t) => t switch
        {
            PrimitiveType { Name: "int"     } => "i64",
            PrimitiveType { Name: "float64" } => "double",
            PrimitiveType { Name: "bool"    } => "i1",
            PrimitiveType { Name: "rune"    } => "i32",
            PrimitiveType { Name: "string"  } => "i8*",
            PrimitiveType { Name: "void"    } => "void",
            ArrayType  a => $"[{a.Size} x {LLVMOf(a.ElemType)}]",
            SliceType  _ => "i8*",
            _            => "i64",
        };

        /// Convierte un nodo DeclType del AST a tipo LLVM IR.
        private static string LLVMOfDecl(MiniGoParser.DeclTypeContext? ctx) => ctx switch
        {
            null => "void",
            MiniGoParser.TypePrimitiveOrCustomContext p => p.IDENTIFIER().GetText() switch
            {
                "int"     => "i64",
                "float64" => "double",
                "bool"    => "i1",
                "rune"    => "i32",
                "string"  => "i8*",
                _         => "i64",   // alias de tipo: representación como i64
            },
            MiniGoParser.TypeArrayContext a =>
                $"[{a.arrayDeclType().INTLITERAL().GetText()} x {LLVMOfDecl(a.arrayDeclType().declType())}]",
            MiniGoParser.TypeSliceContext  _ => "i8*",
            MiniGoParser.TypeStructContext _ => "i8*",
            MiniGoParser.TypeParenContext  p => LLVMOfDecl(p.declType()),
            _                               => "i64",
        };

        // ════════════════════════════════════════════════════════════════════
        //  GESTIÓN DE STRING LITERALS GLOBALES
        // ════════════════════════════════════════════════════════════════════

        /// Registra un string en la tabla global y devuelve su nombre @.strN.
        /// Si ya fue registrado, devuelve el nombre existente (deduplicación).
        private string InternString(string raw)
        {
            if (_strTab.TryGetValue(raw, out var existing)) return existing.Name;

            string name    = $"@.str{_strIdx++}";
            string escaped = EscapeForLLVM(raw, out int byteLen);
            G($"{name} = private unnamed_addr constant [{byteLen} x i8] c\"{escaped}\"");
            _strTab[raw] = (name, byteLen);
            return name;
        }

        /// Escapa un string de Mini-GO a la sintaxis de constante LLVM IR.
        private static string EscapeForLLVM(string s, out int byteLen)
        {
            var sb  = new StringBuilder();
            int len = 0;
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\n': sb.Append("\\0A"); break;
                    case '\r': sb.Append("\\0D"); break;
                    case '\t': sb.Append("\\09"); break;
                    case '\\': sb.Append("\\5C"); break;
                    case '"':  sb.Append("\\22"); break;
                    default:
                        if (c < 32 || c > 126) sb.Append($"\\{(int)c:X2}");
                        else sb.Append(c);
                        break;
                }
                len++;
            }
            sb.Append("\\00");   // terminador nulo obligatorio en LLVM
            byteLen = len + 1;
            return sb.ToString();
        }

        /// Emite un getelementptr para obtener un i8* al string global y devuelve el registro.
        private string StrPtr(string globalName, int byteLen)
        {
            string r = NewReg("sp");
            E($"{r} = getelementptr [{byteLen} x i8], [{byteLen} x i8]* {globalName}, i64 0, i64 0");
            return r;
        }

        // ════════════════════════════════════════════════════════════════════
        //  ENTORNO DE VARIABLES
        // ════════════════════════════════════════════════════════════════════

        private void PushScope() => _env.Push(new Dictionary<string, EnvEntry>());
        private void PopScope()  => _env.Pop();

        /// Define una variable local (alloca) en el scope actual.
        private void DefVar(string name, string allocaReg, string llvmType, bool isGlobal = false)
            => _env.Peek()[name] = new EnvEntry(allocaReg, llvmType, isGlobal);

        /// Busca una variable en toda la cadena de scopes.
        private EnvEntry? FindVar(string name)
        {
            foreach (var scope in _env)
                if (scope.TryGetValue(name, out var e)) return e;
            return null;
        }

        // ════════════════════════════════════════════════════════════════════
        //  PRE-REGISTRO DE FIRMAS DE FUNCIONES (forward references)
        // ════════════════════════════════════════════════════════════════════

        private void PreRegisterFuncs(MiniGoParser.TopDeclarationListContext top)
        {
            foreach (var fd in top.funcDecl())
            {
                var front = fd.funcFrontDecl();
                string name   = front.IDENTIFIER().GetText();
                string retLLVM = LLVMOfDecl(front.declType());

                var paramTypes = new List<string>();
                var paramNames = new List<string>();

                if (front.funcArgDecls() != null)
                {
                    foreach (var arg in front.funcArgDecls().singleVarDeclNoExps())
                    {
                        string pt = LLVMOfDecl(arg.declType());
                        foreach (var id in arg.identifierList().IDENTIFIER())
                        {
                            paramTypes.Add(pt);
                            paramNames.Add(id.GetText());
                        }
                    }
                }

                _funcSigs[name] = new FuncSig(retLLVM, paramTypes, paramNames);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  REGLA RAÍZ
        // ════════════════════════════════════════════════════════════════════

        public override LLVMValue? VisitRoot(MiniGoParser.RootContext ctx)
        {
            var top = ctx.topDeclarationList();
            PreRegisterFuncs(top);   // forward references de funciones
            PushScope();             // scope global (accesible desde todas las funciones)
            VisitTopDeclarationList(top);
            // No PopScope() — el scope global vive durante toda la compilación
            return LLVMValue.Void;
        }

        // ════════════════════════════════════════════════════════════════════
        //  DECLARACIÓN DE FUNCIÓN
        // ════════════════════════════════════════════════════════════════════

        public override LLVMValue? VisitFuncDecl(MiniGoParser.FuncDeclContext ctx)
        {
            var front      = ctx.funcFrontDecl();
            string name    = front.IDENTIFIER().GetText();
            string retType = LLVMOfDecl(front.declType());

            // Estado por función
            _curRetType      = retType;
            _blockTerminated = false;
            _reg = 0;
            _lbl = 0;

            // ── Construir lista de parámetros LLVM ───────────────────────────
            var sigParts = new List<string>();
            var pList    = new List<(string LLVMType, string Name)>();

            if (front.funcArgDecls() != null)
            {
                foreach (var arg in front.funcArgDecls().singleVarDeclNoExps())
                {
                    string pt = LLVMOfDecl(arg.declType());
                    foreach (var id in arg.identifierList().IDENTIFIER())
                    {
                        string pn = id.GetText();
                        sigParts.Add($"{pt} %p.{pn}");
                        pList.Add((pt, pn));
                    }
                }
            }

            // ── Cabecera ─────────────────────────────────────────────────────
            _cur = _funcs;
            _cur.AppendLine($"define {retType} @{name}({string.Join(", ", sigParts)}) {{");
            _cur.AppendLine("entry:");

            PushScope();

            // ── Parámetros → alloca + store (los hace mutables) ──────────────
            foreach (var (pt, pn) in pList)
            {
                string r = NewReg("a");
                E($"{r} = alloca {pt}");
                E($"store {pt} %p.{pn}, {pt}* {r}");
                DefVar(pn, r, pt);
            }

            // ── Cuerpo (visita las sentencias del bloque directamente) ────────
            VisitStatementList(ctx.block().statementList());

            // ── Terminador por defecto si el cuerpo no emitió return ─────────
            if (!_blockTerminated)
                E(retType == "void" ? "ret void" : $"ret {retType} 0");

            PopScope();

            _cur.AppendLine("}");
            EBlank();
            _blockTerminated = false;

            return LLVMValue.Void;
        }

        // ════════════════════════════════════════════════════════════════════
        //  BLOQUE ANIDADO  { ... }
        // ════════════════════════════════════════════════════════════════════

        public override LLVMValue? VisitBlock(MiniGoParser.BlockContext ctx)
        {
            PushScope();
            VisitStatementList(ctx.statementList());
            PopScope();
            return LLVMValue.Void;
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELPERS DE EXPRESIONES
        // ════════════════════════════════════════════════════════════════════

        /// Si uno es double y el otro i64, convierte el i64 a double.
        private (LLVMValue L, LLVMValue R) PromoteNumeric(LLVMValue l, LLVMValue r)
        {
            if (l.LLVMType == "double" && r.LLVMType == "i64")
            {
                string x = NewReg(); E($"{x} = sitofp i64 {r} to double");
                return (l, new LLVMValue(x, "double"));
            }
            if (l.LLVMType == "i64" && r.LLVMType == "double")
            {
                string x = NewReg(); E($"{x} = sitofp i64 {l} to double");
                return (new LLVMValue(x, "double"), r);
            }
            return (l, r);
        }

        /// Coerciona un valor al tipo LLVM requerido (p.ej. para argumentos de función).
        private LLVMValue CoerceValue(LLVMValue v, string target)
        {
            if (v.LLVMType == target) return v;
            string r = NewReg();
            if (v.LLVMType == "i64"    && target == "double") { E($"{r} = sitofp i64 {v} to double");    return new LLVMValue(r, "double"); }
            if (v.LLVMType == "double" && target == "i64")    { E($"{r} = fptosi double {v} to i64");    return new LLVMValue(r, "i64"); }
            if (v.LLVMType == "i1"     && target == "i64")    { E($"{r} = zext i1 {v} to i64");          return new LLVMValue(r, "i64"); }
            if (v.LLVMType == "i32"    && target == "i64")    { E($"{r} = sext i32 {v} to i64");         return new LLVMValue(r, "i64"); }
            return v;
        }

        /// Parsea un INTLITERAL de Mini-GO (hex, octal, binario, decimal) a long.
        private static long ParseIntLiteral(string raw)
        {
            if (raw.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt64(raw[2..], 16);
            if (raw.StartsWith("0o", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt64(raw[2..], 8);
            if (raw.StartsWith("0b", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt64(raw[2..], 2);
            return long.Parse(raw);
        }

        /// Parsea un RUNELITERAL (e.g. 'A', '\n') a su valor entero Unicode.
        private static int ParseRuneLiteral(string raw)
        {
            string inner = raw[1..^1];
            if (inner.Length == 1) return inner[0];
            return inner[1] switch
            {
                'n' => '\n', 'r' => '\r', 't' => '\t', '\\' => '\\',
                '\'' => '\'', '"' => '"', '0' => '\0',
                'a' => '\a', 'b' => '\b', 'f' => '\f', 'v' => '\v',
                _ => inner[1]
            };
        }

        /// Elimina escapes de un string interpretado de Go (sin las comillas externas).
        private static string UnescapeGoString(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\\' && i + 1 < s.Length)
                {
                    sb.Append(s[++i] switch
                    {
                        'n' => "\n", 'r' => "\r", 't' => "\t", '\\' => "\\",
                        '"' => "\"", '\'' => "'", '0' => "\0",
                        'a' => "\a", 'b' => "\b", 'f' => "\f", 'v' => "\v",
                        var c => c.ToString()
                    });
                }
                else sb.Append(s[i]);
            }
            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════════════════
        //  EXPRESIONES
        // ════════════════════════════════════════════════════════════════════

        // expression → primaryExpression  (#PrimaryExpr)
        public override LLVMValue? VisitPrimaryExpr(MiniGoParser.PrimaryExprContext ctx)
            => Visit(ctx.primaryExpression());

        // primaryExpression (regla sin etiquetas — varios formas)
        public override LLVMValue? VisitPrimaryExpression(MiniGoParser.PrimaryExpressionContext ctx)
        {
            if (ctx.operand()           != null) return Visit(ctx.operand());
            if (ctx.arguments()         != null) return EmitCall(ctx.primaryExpression(), ctx.arguments());
            if (ctx.index()             != null) return EmitIndexLoad(ctx.primaryExpression(), ctx.index());
            if (ctx.selector()          != null) return Visit(ctx.primaryExpression()); // struct stub
            if (ctx.appendExpression()  != null) return Visit(ctx.appendExpression());
            if (ctx.lengthExpression()  != null) return Visit(ctx.lengthExpression());
            if (ctx.capExpression()     != null) return Visit(ctx.capExpression());
            return null;
        }

        // operand: literal | IDENTIFIER | '(' expression ')'
        public override LLVMValue? VisitOperand(MiniGoParser.OperandContext ctx)
        {
            if (ctx.literal() != null) return Visit(ctx.literal());

            if (ctx.IDENTIFIER() != null)
            {
                string name = ctx.IDENTIFIER().GetText();
                if (name == "true")  return new LLVMValue("1", "i1");
                if (name == "false") return new LLVMValue("0", "i1");

                var e = FindVar(name);
                if (e == null) return null;

                string r = NewReg();
                E(e.IsGlobal
                    ? $"{r} = load {e.LLVMType}, {e.LLVMType}* @g.{name}"
                    : $"{r} = load {e.LLVMType}, {e.LLVMType}* {e.AllocaReg}");
                return new LLVMValue(r, e.LLVMType);
            }

            if (ctx.expression() != null) return Visit(ctx.expression());
            return null;
        }

        // Literales
        public override LLVMValue? VisitLiteral(MiniGoParser.LiteralContext ctx)
        {
            if (ctx.INTLITERAL() != null)
                return new LLVMValue(ParseIntLiteral(ctx.INTLITERAL().GetText()).ToString(), "i64");

            if (ctx.FLOATLITERAL() != null)
            {
                double d = double.Parse(ctx.FLOATLITERAL().GetText(),
                                        System.Globalization.CultureInfo.InvariantCulture);
                return new LLVMValue(d.ToString("R", System.Globalization.CultureInfo.InvariantCulture), "double");
            }

            if (ctx.RUNELITERAL() != null)
                return new LLVMValue(ParseRuneLiteral(ctx.RUNELITERAL().GetText()).ToString(), "i32");

            if (ctx.INTERPRETEDSTRINGLITERAL() != null)
            {
                string raw     = ctx.INTERPRETEDSTRINGLITERAL().GetText();
                string content = UnescapeGoString(raw[1..^1]);
                string gName   = InternString(content);
                var (_, bLen)  = _strTab[content];
                return new LLVMValue(StrPtr(gName, bLen), "i8*");
            }

            if (ctx.RAWSTRINGLITERAL() != null)
            {
                string raw     = ctx.RAWSTRINGLITERAL().GetText();
                string content = raw[1..^1];
                string gName   = InternString(content);
                var (_, bLen)  = _strTab[content];
                return new LLVMValue(StrPtr(gName, bLen), "i8*");
            }

            return null;
        }

        // UnaryExpr: ('+' | '-' | '!' | '^') expression
        public override LLVMValue? VisitUnaryExpr(MiniGoParser.UnaryExprContext ctx)
        {
            string op  = ctx.GetChild(0).GetText();
            var    val = Visit(ctx.expression());
            if (val == null) return null;
            if (op == "+") return val;

            string r = NewReg();
            switch (op)
            {
                case "-":
                    E(val.LLVMType == "double"
                        ? $"{r} = fneg double {val}"
                        : $"{r} = sub {val.LLVMType} 0, {val}");
                    break;
                case "!": E($"{r} = xor i1 {val}, 1"); break;
                case "^": E($"{r} = xor {val.LLVMType} {val}, -1"); break;
            }
            return new LLVMValue(r, val.LLVMType);
        }

        // MulExpr: expression ('*'|'/'|'%'|'<<'|'>>'|'&'|'&^') expression
        public override LLVMValue? VisitMulExpr(MiniGoParser.MulExprContext ctx)
        {
            var lhs = Visit(ctx.expression(0));
            var rhs = Visit(ctx.expression(1));
            if (lhs == null || rhs == null) return null;
            (lhs, rhs) = PromoteNumeric(lhs, rhs);

            string op = ctx.GetChild(1).GetText();
            bool isFloat = lhs.LLVMType == "double";
            string r = NewReg();

            if (op == "&^")
            {
                string nr = NewReg();
                E($"{nr} = xor {lhs.LLVMType} {rhs}, -1");
                E($"{r} = and {lhs.LLVMType} {lhs}, {nr}");
            }
            else
            {
                string instr = (op, isFloat) switch
                {
                    ("*",  false) => $"mul i64 {lhs}, {rhs}",
                    ("/",  false) => $"sdiv i64 {lhs}, {rhs}",
                    ("%",  false) => $"srem i64 {lhs}, {rhs}",
                    ("<<", false) => $"shl i64 {lhs}, {rhs}",
                    (">>", false) => $"ashr i64 {lhs}, {rhs}",
                    ("&",  false) => $"and i64 {lhs}, {rhs}",
                    ("|",  false) => $"or i64 {lhs}, {rhs}",
                    ("^",  false) => $"xor i64 {lhs}, {rhs}",
                    ("*",  true)  => $"fmul double {lhs}, {rhs}",
                    ("/",  true)  => $"fdiv double {lhs}, {rhs}",
                    _             => $"mul i64 {lhs}, {rhs}",
                };
                E($"{r} = {instr}");
            }
            return new LLVMValue(r, lhs.LLVMType);
        }

        // AddExpr: expression ('+'|'-'|'|'|'^') expression
        public override LLVMValue? VisitAddExpr(MiniGoParser.AddExprContext ctx)
        {
            var lhs = Visit(ctx.expression(0));
            var rhs = Visit(ctx.expression(1));
            if (lhs == null || rhs == null) return null;
            (lhs, rhs) = PromoteNumeric(lhs, rhs);

            string op = ctx.GetChild(1).GetText();
            bool isFloat = lhs.LLVMType == "double";

            // Concatenación de strings: retorna lhs (simplificación — IR no tiene strcat nativo)
            if (lhs.LLVMType == "i8*" && op == "+") return lhs;

            string r = NewReg();
            string instr = (op, isFloat) switch
            {
                ("+", false) => $"add i64 {lhs}, {rhs}",
                ("-", false) => $"sub i64 {lhs}, {rhs}",
                ("|", false) => $"or i64 {lhs}, {rhs}",
                ("^", false) => $"xor i64 {lhs}, {rhs}",
                ("+", true)  => $"fadd double {lhs}, {rhs}",
                ("-", true)  => $"fsub double {lhs}, {rhs}",
                _            => $"add i64 {lhs}, {rhs}",
            };
            E($"{r} = {instr}");
            return new LLVMValue(r, lhs.LLVMType);
        }

        // RelExpr: expression ('=='|'!='|'<'|'<='|'>'|'>=') expression  → i1
        public override LLVMValue? VisitRelExpr(MiniGoParser.RelExprContext ctx)
        {
            var lhs = Visit(ctx.expression(0));
            var rhs = Visit(ctx.expression(1));
            if (lhs == null || rhs == null) return null;
            (lhs, rhs) = PromoteNumeric(lhs, rhs);

            string op = ctx.GetChild(1).GetText();
            bool isFloat = lhs.LLVMType == "double";
            string r = NewReg();

            string pred = (op, isFloat) switch
            {
                ("==", false) => "icmp eq",  ("==", true) => "fcmp oeq",
                ("!=", false) => "icmp ne",  ("!=", true) => "fcmp one",
                ("<",  false) => "icmp slt", ("<",  true) => "fcmp olt",
                ("<=", false) => "icmp sle", ("<=", true) => "fcmp ole",
                (">",  false) => "icmp sgt", (">",  true) => "fcmp ogt",
                (">=", false) => "icmp sge", (">=", true) => "fcmp oge",
                _             => "icmp eq",
            };
            E($"{r} = {pred} {lhs.LLVMType} {lhs}, {rhs}");
            return new LLVMValue(r, "i1");
        }

        // AndExpr: expression '&&' expression
        public override LLVMValue? VisitAndExpr(MiniGoParser.AndExprContext ctx)
        {
            var lhs = Visit(ctx.expression(0));
            var rhs = Visit(ctx.expression(1));
            if (lhs == null || rhs == null) return null;
            string r = NewReg();
            E($"{r} = and i1 {lhs}, {rhs}");
            return new LLVMValue(r, "i1");
        }

        // OrExpr: expression '||' expression
        public override LLVMValue? VisitOrExpr(MiniGoParser.OrExprContext ctx)
        {
            var lhs = Visit(ctx.expression(0));
            var rhs = Visit(ctx.expression(1));
            if (lhs == null || rhs == null) return null;
            string r = NewReg();
            E($"{r} = or i1 {lhs}, {rhs}");
            return new LLVMValue(r, "i1");
        }

        // ════════════════════════════════════════════════════════════════════
        //  LLAMADAS A FUNCIÓN (desde expresión)
        // ════════════════════════════════════════════════════════════════════

        private LLVMValue? EmitCall(MiniGoParser.PrimaryExpressionContext callee,
                                    MiniGoParser.ArgumentsContext argsCtx)
        {
            string name = callee.operand()?.IDENTIFIER()?.GetText() ?? "";
            if (string.IsNullOrEmpty(name)) return null;
            if (!_funcSigs.TryGetValue(name, out var sig)) return null;

            var argParts = new List<string>();
            if (argsCtx.expressionList() != null)
            {
                int i = 0;
                foreach (var expr in argsCtx.expressionList().expression())
                {
                    var v = Visit(expr);
                    if (v == null) { i++; continue; }
                    if (i < sig.ParamTypes.Count)
                        v = CoerceValue(v, sig.ParamTypes[i]);
                    argParts.Add($"{v.LLVMType} {v.Ref}");
                    i++;
                }
            }

            string argStr = string.Join(", ", argParts);
            if (sig.RetType == "void")
            {
                E($"call void @{name}({argStr})");
                return LLVMValue.Void;
            }
            string r = NewReg();
            E($"{r} = call {sig.RetType} @{name}({argStr})");
            return new LLVMValue(r, sig.RetType);
        }

        // ════════════════════════════════════════════════════════════════════
        //  HELPERS DE STATEMENTS
        // ════════════════════════════════════════════════════════════════════

        /// Valor cero LLVM para un tipo dado.
        private static LLVMValue ZeroValue(string llvmType) => llvmType switch
        {
            "double"                           => new LLVMValue("0.0",             "double"),
            "i1"                               => new LLVMValue("0",               "i1"),
            "i32"                              => new LLVMValue("0",               "i32"),
            "i8*"                              => new LLVMValue("null",            "i8*"),
            _ when llvmType.StartsWith("[")    => new LLVMValue("zeroinitializer", llvmType),
            _                                  => new LLVMValue("0",               "i64"),
        };

        /// Extrae el tipo de elemento de un tipo array LLVM ("[N x T]" → "T").
        private static string ElemTypeOf(string arrLLVMType)
        {
            int xPos = arrLLVMType.IndexOf(" x ", StringComparison.Ordinal);
            return xPos >= 0 ? arrLLVMType[(xPos + 3)..^1] : "i64";
        }

        /// Emite getelementptr para acceder a arr[idx] y carga el elemento.
        private LLVMValue? EmitIndexLoad(MiniGoParser.PrimaryExpressionContext arrCtx,
                                         MiniGoParser.IndexContext idxCtx)
        {
            string? name = arrCtx.operand()?.IDENTIFIER()?.GetText();
            if (name == null) return null;
            var e = FindVar(name);
            if (e == null || !e.LLVMType.StartsWith("[")) return null;

            var idx = Visit(idxCtx.expression());
            if (idx == null) return null;
            if (idx.LLVMType != "i64") idx = CoerceValue(idx, "i64");

            string elemType = ElemTypeOf(e.LLVMType);
            string allocaRef = e.IsGlobal ? $"@g.{name}" : e.AllocaReg;

            string gep = NewReg("gep");
            E($"{gep} = getelementptr {e.LLVMType}, {e.LLVMType}* {allocaRef}, i64 0, i64 {idx}");
            string r = NewReg();
            E($"{r} = load {elemType}, {elemType}* {gep}");
            return new LLVMValue(r, elemType);
        }

        /// Extrae el nombre de un identificador simple de una expresión, o null.
        private static string? IdentName(MiniGoParser.ExpressionContext expr)
        {
            if (expr is MiniGoParser.PrimaryExprContext pe)
            {
                var pri = pe.primaryExpression();
                if (pri?.operand()?.IDENTIFIER() != null)
                    return pri.operand().IDENTIFIER().GetText();
            }
            return null;
        }

        /// Carga el valor de una expresión lvalue (identificador o arr[i]).
        private LLVMValue? LoadLVal(MiniGoParser.ExpressionContext expr)
        {
            // Caso arr[i]
            if (expr is MiniGoParser.PrimaryExprContext pe)
            {
                var pri = pe.primaryExpression();
                if (pri?.index() != null)
                {
                    string? arrName = pri.primaryExpression()?.operand()?.IDENTIFIER()?.GetText();
                    if (arrName != null)
                    {
                        var gep = EmitIndexGEP(arrName, pri.index());
                        if (gep != null)
                        {
                            string r = NewReg();
                            E($"{r} = load {gep.Value.ElemType}, {gep.Value.ElemType}* {gep.Value.GepReg}");
                            return new LLVMValue(r, gep.Value.ElemType);
                        }
                    }
                }
            }
            // Caso identificador simple
            string? name = IdentName(expr);
            if (name == null) return null;
            var e = FindVar(name);
            if (e == null) return null;
            string reg = NewReg();
            E(e.IsGlobal
                ? $"{reg} = load {e.LLVMType}, {e.LLVMType}* @g.{name}"
                : $"{reg} = load {e.LLVMType}, {e.LLVMType}* {e.AllocaReg}");
            return new LLVMValue(reg, e.LLVMType);
        }

        /// Almacena val en la expresión lvalue (identificador o arr[i]).
        private void StoreLVal(MiniGoParser.ExpressionContext expr, LLVMValue val)
        {
            // Caso arr[i]
            if (expr is MiniGoParser.PrimaryExprContext pe)
            {
                var pri = pe.primaryExpression();
                if (pri?.index() != null)
                {
                    string? arrName = pri.primaryExpression()?.operand()?.IDENTIFIER()?.GetText();
                    if (arrName != null)
                    {
                        var gep = EmitIndexGEP(arrName, pri.index());
                        if (gep != null)
                        {
                            val = CoerceValue(val, gep.Value.ElemType);
                            E($"store {gep.Value.ElemType} {val.Ref}, {gep.Value.ElemType}* {gep.Value.GepReg}");
                            return;
                        }
                    }
                }
            }
            // Caso identificador simple
            string? name = IdentName(expr);
            if (name == null) return;
            var e = FindVar(name);
            if (e == null) return;
            val = CoerceValue(val, e.LLVMType);
            E(e.IsGlobal
                ? $"store {e.LLVMType} {val.Ref}, {e.LLVMType}* @g.{name}"
                : $"store {e.LLVMType} {val.Ref}, {e.LLVMType}* {e.AllocaReg}");
        }

        /// Emite getelementptr para arr[i] y devuelve (gepReg, elemType).
        private (string GepReg, string ElemType)?
            EmitIndexGEP(string arrName, MiniGoParser.IndexContext idxCtx)
        {
            var e = FindVar(arrName);
            if (e == null || !e.LLVMType.StartsWith("[")) return null;

            var idx = Visit(idxCtx.expression());
            if (idx == null) return null;
            if (idx.LLVMType != "i64") idx = CoerceValue(idx, "i64");

            string elemType  = ElemTypeOf(e.LLVMType);
            string allocaRef = e.IsGlobal ? $"@g.{arrName}" : e.AllocaReg;
            string gep       = NewReg("gep");
            E($"{gep} = getelementptr {e.LLVMType}, {e.LLVMType}* {allocaRef}, i64 0, i64 {idx}");
            return (gep, elemType);
        }

        /// Emite una llamada a printf con un valor tipado; agrega '\n' si newline=true.
        private void EmitPrintfVal(LLVMValue val, bool newline)
        {
            string nl   = newline ? "\n" : "";
            LLVMValue actual = val;
            string fmt;

            switch (val.LLVMType)
            {
                case "i64":    fmt = $"%lld{nl}"; break;
                case "double": fmt = $"%g{nl}";   break;
                case "i8*":    fmt = $"%s{nl}";   break;
                case "i32":    fmt = $"%c{nl}";   break;
                case "i1":
                    fmt = $"%d{nl}";
                    string ext = NewReg();
                    E($"{ext} = zext i1 {val} to i32");
                    actual = new LLVMValue(ext, "i32");
                    break;
                default: fmt = $"%lld{nl}"; break;
            }

            string gn = InternString(fmt);
            var (_, bl) = _strTab[fmt];
            string ptr = StrPtr(gn, bl);
            E($"call i32 (i8*, ...) @printf(i8* noundef {ptr}, {actual.LLVMType} {actual.Ref})");
        }

        /// Emite printf con un string literal puro (sin valor dinámico).
        private void EmitPrintfRaw(string s)
        {
            string gn = InternString(s);
            var (_, bl) = _strTab[s];
            string ptr = StrPtr(gn, bl);
            E($"call i32 (i8*, ...) @printf(i8* noundef {ptr})");
        }

        // ════════════════════════════════════════════════════════════════════
        //  DECLARACIONES DE VARIABLE
        // ════════════════════════════════════════════════════════════════════

        // variableDecl como statement  →  delega a variableDecl
        public override LLVMValue? VisitVarDeclWrapper(MiniGoParser.VarDeclWrapperContext ctx)
            => Visit(ctx.variableDecl());

        // variableDecl en top-level (global) o dentro de función
        public override LLVMValue? VisitVariableDecl(MiniGoParser.VariableDeclContext ctx)
        {
            bool isGlobal = _env.Count == 1; // solo el scope global = nivel raíz

            if (isGlobal)
            {
                // Variables globales: emitir declaraciones LLVM con inicializador cero
                // y registrar en el scope global para que las funciones las encuentren
                if (ctx.singleVarDecl() != null)   EmitGlobalSingleDecl(ctx.singleVarDecl());
                if (ctx.innerVarDecls() != null)
                    foreach (var d in ctx.innerVarDecls().singleVarDecl())
                        EmitGlobalSingleDecl(d);
                return LLVMValue.Void;
            }

            return VisitChildren(ctx);
        }

        private void EmitGlobalSingleDecl(MiniGoParser.SingleVarDeclContext ctx)
        {
            string llvmType = "i64";
            IReadOnlyList<Antlr4.Runtime.Tree.ITerminalNode> ids;

            switch (ctx)
            {
                case MiniGoParser.VarDeclWithTypeAndInitContext ti:
                    llvmType = LLVMOfDecl(ti.declType());
                    ids = ti.identifierList().IDENTIFIER();
                    break;
                case MiniGoParser.VarDeclWithInitOnlyContext io:
                    // Tipo inferido — visitamos la expresión más adelante;
                    // para el initializer global usamos i64 como fallback
                    ids = io.identifierList().IDENTIFIER();
                    break;
                case MiniGoParser.VarDeclNoInitContext ni:
                    llvmType = LLVMOfDecl(ni.singleVarDeclNoExps().declType());
                    ids = ni.singleVarDeclNoExps().identifierList().IDENTIFIER();
                    break;
                default: return;
            }

            foreach (var id in ids)
            {
                string name = id.GetText();
                string zero = llvmType switch
                {
                    "double" => "0.0", "i1" => "0", "i32" => "0", "i8*" => "null",
                    _ => "0"
                };
                G($"@g.{name} = global {llvmType} {zero}");
                DefVar(name, $"@g.{name}", llvmType, isGlobal: true);
            }
        }

        // var x T = expr
        public override LLVMValue? VisitVarDeclWithTypeAndInit(MiniGoParser.VarDeclWithTypeAndInitContext ctx)
        {
            string lt  = LLVMOfDecl(ctx.declType());
            var ids    = ctx.identifierList().IDENTIFIER();
            var exprs  = ctx.expressionList().expression();

            for (int i = 0; i < ids.Length; i++)
            {
                var val = i < exprs.Length ? Visit(exprs[i]) : null;
                val = val != null ? CoerceValue(val, lt) : ZeroValue(lt);
                string r = NewReg("v");
                E($"{r} = alloca {lt}");
                E($"store {lt} {val.Ref}, {lt}* {r}");
                DefVar(ids[i].GetText(), r, lt);
            }
            return LLVMValue.Void;
        }

        // var x = expr  (tipo inferido)
        public override LLVMValue? VisitVarDeclWithInitOnly(MiniGoParser.VarDeclWithInitOnlyContext ctx)
        {
            var ids   = ctx.identifierList().IDENTIFIER();
            var exprs = ctx.expressionList().expression();

            for (int i = 0; i < ids.Length; i++)
            {
                var val = i < exprs.Length ? Visit(exprs[i]) : null;
                if (val == null) continue;
                string r = NewReg("v");
                E($"{r} = alloca {val.LLVMType}");
                E($"store {val.LLVMType} {val.Ref}, {val.LLVMType}* {r}");
                DefVar(ids[i].GetText(), r, val.LLVMType);
            }
            return LLVMValue.Void;
        }

        // var x T  (valor cero)
        public override LLVMValue? VisitVarDeclNoInit(MiniGoParser.VarDeclNoInitContext ctx)
            => Visit(ctx.singleVarDeclNoExps());

        public override LLVMValue? VisitSingleVarDeclNoExps(MiniGoParser.SingleVarDeclNoExpsContext ctx)
        {
            string lt = LLVMOfDecl(ctx.declType());
            foreach (var id in ctx.identifierList().IDENTIFIER())
            {
                var z = ZeroValue(lt);
                string r = NewReg("v");
                E($"{r} = alloca {lt}");
                E($"store {lt} {z.Ref}, {lt}* {r}");
                DefVar(id.GetText(), r, lt);
            }
            return LLVMValue.Void;
        }

        // ════════════════════════════════════════════════════════════════════
        //  STATEMENTS SIMPLES
        // ════════════════════════════════════════════════════════════════════

        // x := expr
        public override LLVMValue? VisitShortVarDeclStmt(MiniGoParser.ShortVarDeclStmtContext ctx)
        {
            var lhsList = ctx.expressionList(0).expression();
            var rhsList = ctx.expressionList(1).expression();

            for (int i = 0; i < lhsList.Length; i++)
            {
                string name = lhsList[i].GetText();
                var val = i < rhsList.Length ? Visit(rhsList[i]) : null;
                if (val == null) continue;
                string r = NewReg("v");
                E($"{r} = alloca {val.LLVMType}");
                E($"store {val.LLVMType} {val.Ref}, {val.LLVMType}* {r}");
                DefVar(name, r, val.LLVMType);
            }
            return LLVMValue.Void;
        }

        // x = expr  (asignación simple)
        public override LLVMValue? VisitAssignSimple(MiniGoParser.AssignSimpleContext ctx)
        {
            var lhsList = ctx.expressionList(0).expression();
            var rhsList = ctx.expressionList(1).expression();
            for (int i = 0; i < lhsList.Length; i++)
            {
                var val = i < rhsList.Length ? Visit(rhsList[i]) : null;
                if (val != null) StoreLVal(lhsList[i], val);
            }
            return LLVMValue.Void;
        }

        // x += expr, x -= expr, etc.
        public override LLVMValue? VisitAssignCompound(MiniGoParser.AssignCompoundContext ctx)
        {
            var cur = LoadLVal(ctx.expression(0));
            var rhs = Visit(ctx.expression(1));
            if (cur == null || rhs == null) return LLVMValue.Void;
            (cur, rhs) = PromoteNumeric(cur, rhs);

            string op = ctx.GetChild(1).GetText();
            bool isFloat = cur.LLVMType == "double";
            string r = NewReg();

            string instr = (op, isFloat) switch
            {
                ("+=",  false) => $"add i64 {cur}, {rhs}",
                ("-=",  false) => $"sub i64 {cur}, {rhs}",
                ("*=",  false) => $"mul i64 {cur}, {rhs}",
                ("/=",  false) => $"sdiv i64 {cur}, {rhs}",
                ("%=",  false) => $"srem i64 {cur}, {rhs}",
                ("&=",  false) => $"and i64 {cur}, {rhs}",
                ("|=",  false) => $"or i64 {cur}, {rhs}",
                ("^=",  false) => $"xor i64 {cur}, {rhs}",
                ("<<=", false) => $"shl i64 {cur}, {rhs}",
                (">>=", false) => $"ashr i64 {cur}, {rhs}",
                ("+=",  true)  => $"fadd double {cur}, {rhs}",
                ("-=",  true)  => $"fsub double {cur}, {rhs}",
                ("*=",  true)  => $"fmul double {cur}, {rhs}",
                ("/=",  true)  => $"fdiv double {cur}, {rhs}",
                _              => $"add i64 {cur}, {rhs}",
            };
            E($"{r} = {instr}");
            StoreLVal(ctx.expression(0), new LLVMValue(r, cur.LLVMType));
            return LLVMValue.Void;
        }

        // x++, x--
        public override LLVMValue? VisitIncDecStmt(MiniGoParser.IncDecStmtContext ctx)
        {
            string op = ctx.GetChild(1).GetText();
            var cur   = LoadLVal(ctx.expression());
            if (cur == null) return LLVMValue.Void;
            string r = NewReg();
            E(op == "++"
                ? $"{r} = add {cur.LLVMType} {cur}, 1"
                : $"{r} = sub {cur.LLVMType} {cur}, 1");
            StoreLVal(ctx.expression(), new LLVMValue(r, cur.LLVMType));
            return LLVMValue.Void;
        }

        // return expr?
        public override LLVMValue? VisitReturnStmt(MiniGoParser.ReturnStmtContext ctx)
        {
            if (ctx.expression() != null)
            {
                var val = Visit(ctx.expression()) ?? ZeroValue(_curRetType);
                val = CoerceValue(val, _curRetType);
                E($"ret {val.LLVMType} {val.Ref}");
            }
            else
            {
                E("ret void");
            }
            _blockTerminated = true;
            return LLVMValue.Void;
        }

        // expresión usada como statement (descarta el resultado)
        public override LLVMValue? VisitExprStmt(MiniGoParser.ExprStmtContext ctx)
        {
            Visit(ctx.expression());
            return LLVMValue.Void;
        }

        // tipo vacío / sin código
        public override LLVMValue? VisitEmptyStmt(MiniGoParser.EmptyStmtContext ctx)
            => LLVMValue.Void;

        public override LLVMValue? VisitTypeDeclWrapper(MiniGoParser.TypeDeclWrapperContext ctx)
            => LLVMValue.Void;   // declaraciones de tipo no generan código IR

        // wrappers de statements simples
        public override LLVMValue? VisitSimpleStmtWrapper(MiniGoParser.SimpleStmtWrapperContext ctx)
            => Visit(ctx.simpleStatement());

        public override LLVMValue? VisitAssignStmt(MiniGoParser.AssignStmtContext ctx)
            => Visit(ctx.assignmentStatement());

        public override LLVMValue? VisitBlockStmtWrapper(MiniGoParser.BlockStmtWrapperContext ctx)
            => Visit(ctx.block());

        // ════════════════════════════════════════════════════════════════════
        //  PRINT / PRINTLN
        // ════════════════════════════════════════════════════════════════════

        // ════════════════════════════════════════════════════════════════════
        //  CONTROL DE FLUJO
        // ════════════════════════════════════════════════════════════════════

        // break → salta al bloque de salida del loop actual
        public override LLVMValue? VisitBreakStmt(MiniGoParser.BreakStmtContext ctx)
        {
            if (!string.IsNullOrEmpty(_breakTarget))
            {
                E($"br label %{_breakTarget}");
                _blockTerminated = true;
            }
            return LLVMValue.Void;
        }

        // continue → salta al bloque de condición/post del loop actual
        public override LLVMValue? VisitContinueStmt(MiniGoParser.ContinueStmtContext ctx)
        {
            if (!string.IsNullOrEmpty(_continueTarget))
            {
                E($"br label %{_continueTarget}");
                _blockTerminated = true;
            }
            return LLVMValue.Void;
        }

        // ── IF / ELSE ─────────────────────────────────────────────────────────

        public override LLVMValue? VisitIfStmtWrapper(MiniGoParser.IfStmtWrapperContext ctx)
            => Visit(ctx.ifStatement());

        public override LLVMValue? VisitIfStatement(MiniGoParser.IfStatementContext ctx)
        {
            // Init statement opcional: if v := expr; v > 0 { ... }
            bool hasInit = ctx.simpleStatement() != null;
            if (hasInit) { PushScope(); Visit(ctx.simpleStatement()); }

            bool   hasElse = ctx.elseClause() != null;
            string thenLbl  = NewLbl("if_then");
            string elseLbl  = NewLbl("if_else");
            string mergeLbl = NewLbl("if_merge");

            // Condición
            var cond = Visit(ctx.expression()) ?? new LLVMValue("1", "i1");
            if (cond.LLVMType != "i1")
            {
                string tmp = NewReg(); E($"{tmp} = icmp ne {cond.LLVMType} {cond}, 0");
                cond = new LLVMValue(tmp, "i1");
            }
            E($"br i1 {cond}, label %{thenLbl}, label %{(hasElse ? elseLbl : mergeLbl)}");
            _blockTerminated = true;

            // ── Bloque then ─────────────────────────────────────────────────
            Lbl(thenLbl); _blockTerminated = false;
            PushScope(); VisitStatementList(ctx.block().statementList()); PopScope();
            if (!_blockTerminated) E($"br label %{mergeLbl}");

            // ── Bloque else ─────────────────────────────────────────────────
            if (hasElse)
            {
                Lbl(elseLbl); _blockTerminated = false;
                var ec = ctx.elseClause();
                if (ec.ifStatement() != null)
                    Visit(ec.ifStatement());          // else if — recursivo
                else
                {
                    PushScope(); VisitStatementList(ec.block().statementList()); PopScope();
                }
                if (!_blockTerminated) E($"br label %{mergeLbl}");
            }

            // ── Merge ────────────────────────────────────────────────────────
            Lbl(mergeLbl); _blockTerminated = false;
            if (hasInit) PopScope();
            return LLVMValue.Void;
        }

        // ── FOR — LAS 3 FORMAS ────────────────────────────────────────────────

        public override LLVMValue? VisitLoopStmtWrapper(MiniGoParser.LoopStmtWrapperContext ctx)
            => Visit(ctx.loopStatement());

        public override LLVMValue? VisitLoopStatement(MiniGoParser.LoopStatementContext ctx)
        {
            var simpleStmts = ctx.simpleStatement();
            bool infinite   = simpleStmts.Length == 0 && ctx.expression() == null;
            bool condOnly   = simpleStmts.Length == 0 && ctx.expression() != null;
            bool threePart  = simpleStmts.Length >= 1;

            string lblCond  = NewLbl("for_cond");
            string lblBody  = NewLbl("for_body");
            string lblPost  = NewLbl("for_post");
            string lblAfter = NewLbl("for_after");

            // Guardar targets del loop padre y establecer los nuevos
            string savedBreak    = _breakTarget;
            string savedContinue = _continueTarget;
            _breakTarget    = lblAfter;
            _continueTarget = threePart ? lblPost : lblCond;

            // ── Init (form 3) ────────────────────────────────────────────────
            if (threePart) { PushScope(); Visit(simpleStmts[0]); }

            if (!_blockTerminated) E($"br label %{lblCond}");

            // ── Bloque de condición ──────────────────────────────────────────
            Lbl(lblCond); _blockTerminated = false;

            // Sin condición: bucle infinito (for{}) o for init;;post
            if (ctx.expression() == null)
            {
                E($"br label %{lblBody}");
            }
            else
            {
                var cond = Visit(ctx.expression()) ?? new LLVMValue("1", "i1");
                if (cond.LLVMType != "i1")
                {
                    string tmp = NewReg(); E($"{tmp} = icmp ne {cond.LLVMType} {cond}, 0");
                    cond = new LLVMValue(tmp, "i1");
                }
                E($"br i1 {cond}, label %{lblBody}, label %{lblAfter}");
            }

            // ── Bloque body ──────────────────────────────────────────────────
            Lbl(lblBody); _blockTerminated = false;
            PushScope(); VisitStatementList(ctx.block().statementList()); PopScope();
            if (!_blockTerminated) E($"br label %{(threePart ? lblPost : lblCond)}");

            // ── Bloque post (form 3) ─────────────────────────────────────────
            if (threePart)
            {
                Lbl(lblPost); _blockTerminated = false;
                Visit(simpleStmts[1]);
                if (!_blockTerminated) E($"br label %{lblCond}");
                PopScope();  // scope del init
            }

            // ── After loop ───────────────────────────────────────────────────
            Lbl(lblAfter); _blockTerminated = false;

            _breakTarget    = savedBreak;
            _continueTarget = savedContinue;
            return LLVMValue.Void;
        }

        // ── SWITCH (implementación básica como cadena if-else) ─────────────────

        public override LLVMValue? VisitSwitchStmtWrapper(MiniGoParser.SwitchStmtWrapperContext ctx)
            => Visit(ctx.switchStatement());

        public override LLVMValue? VisitSwitchStatement(MiniGoParser.SwitchStatementContext ctx)
        {
            bool hasInit = ctx.simpleStatement() != null;
            if (hasInit) { PushScope(); Visit(ctx.simpleStatement()); }

            // Evaluar expresión del switch (o usar i1 1 si no hay)
            LLVMValue? swVal = ctx.expression() != null ? Visit(ctx.expression()) : null;

            var clauses = ctx.expressionCaseClauseList().expressionCaseClause();
            string lblAfter = NewLbl("sw_after");
            string savedBreak = _breakTarget;
            _breakTarget = lblAfter;

            // Crear etiquetas para cada cláusula
            var bodyLabels  = new string[clauses.Length];
            var checkLabels = new string[clauses.Length + 1];
            for (int i = 0; i < clauses.Length; i++) bodyLabels[i]  = NewLbl($"sw_body{i}");
            for (int i = 0; i <= clauses.Length; i++) checkLabels[i] = NewLbl($"sw_chk{i}");
            checkLabels[clauses.Length] = lblAfter;  // no default → va a after

            // Encontrar default
            int defaultIdx = -1;
            for (int i = 0; i < clauses.Length; i++)
                if (clauses[i].expressionSwitchCase() is MiniGoParser.SwitchCaseDefaultContext)
                { defaultIdx = i; break; }
            if (defaultIdx >= 0) checkLabels[clauses.Length] = bodyLabels[defaultIdx];

            // Saltar a la primera comprobación
            if (!_blockTerminated) E($"br label %{checkLabels[0]}");

            // ── Cadena de comparaciones ────────────────────────────────────
            for (int i = 0; i < clauses.Length; i++)
            {
                Lbl(checkLabels[i]); _blockTerminated = false;
                var sw = clauses[i].expressionSwitchCase();

                if (sw is MiniGoParser.SwitchCaseDefaultContext)
                {
                    E($"br label %{bodyLabels[i]}");
                }
                else if (sw is MiniGoParser.SwitchCaseExprContext caseExpr)
                {
                    // Comparar con cada expresión del case (OR lógico)
                    var caseExprs = caseExpr.expressionList().expression();
                    string matchReg = "0";
                    for (int j = 0; j < caseExprs.Length; j++)
                    {
                        var cv = Visit(caseExprs[j]);
                        if (cv == null) continue;
                        string r = NewReg();
                        if (swVal != null)
                            E($"{r} = icmp eq {swVal.LLVMType} {swVal}, {cv}");
                        else
                            E($"{r} = icmp ne i1 {cv}, 0");  // switch sin expr → eval bool

                        if (matchReg == "0") { matchReg = r; }
                        else
                        {
                            string orR = NewReg();
                            E($"{orR} = or i1 %{matchReg.TrimStart('%')}, {r}");
                            matchReg = orR;
                        }
                    }
                    E($"br i1 {matchReg}, label %{bodyLabels[i]}, label %{checkLabels[i + 1]}");
                }
                _blockTerminated = true;
            }

            // ── Cuerpos de las cláusulas ───────────────────────────────────
            for (int i = 0; i < clauses.Length; i++)
            {
                Lbl(bodyLabels[i]); _blockTerminated = false;
                PushScope();
                VisitStatementList(clauses[i].statementList());
                PopScope();
                if (!_blockTerminated) E($"br label %{lblAfter}");
            }

            // ── After ──────────────────────────────────────────────────────
            Lbl(lblAfter); _blockTerminated = false;
            _breakTarget = savedBreak;
            if (hasInit) PopScope();
            return LLVMValue.Void;
        }

        // ════════════════════════════════════════════════════════════════════
        //  BUILT-INS: len, cap, append
        // ════════════════════════════════════════════════════════════════════

        public override LLVMValue? VisitLengthExpression(MiniGoParser.LengthExpressionContext ctx)
        {
            // Si la expresión es un identificador de array, devolvemos su tamaño como constante
            string? name = IdentName(ctx.expression());
            if (name != null)
            {
                var e = FindVar(name);
                if (e != null && e.LLVMType.StartsWith("["))
                {
                    // Extraer N de "[N x T]"
                    int xPos = e.LLVMType.IndexOf(" x ", StringComparison.Ordinal);
                    if (xPos > 0)
                        return new LLVMValue(e.LLVMType[1..xPos], "i64");
                }
            }
            // Para slices u otros: devolvemos 0 (stub)
            Visit(ctx.expression());  // visitar de todas formas para side-effects
            return new LLVMValue("0", "i64");
        }

        public override LLVMValue? VisitCapExpression(MiniGoParser.CapExpressionContext ctx)
        {
            // Mismo tratamiento que len
            string? name = IdentName(ctx.expression());
            if (name != null)
            {
                var e = FindVar(name);
                if (e != null && e.LLVMType.StartsWith("["))
                {
                    int xPos = e.LLVMType.IndexOf(" x ", StringComparison.Ordinal);
                    if (xPos > 0)
                        return new LLVMValue(e.LLVMType[1..xPos], "i64");
                }
            }
            Visit(ctx.expression());
            return new LLVMValue("0", "i64");
        }

        public override LLVMValue? VisitAppendExpression(MiniGoParser.AppendExpressionContext ctx)
        {
            // append(slice, elem) — stub: devuelve el slice sin modificar
            // Una implementación completa requiere malloc + memcpy
            return Visit(ctx.expression(0));
        }

        public override LLVMValue? VisitPrintlnStmt(MiniGoParser.PrintlnStmtContext ctx)
        {
            var exprs = ctx.expressionList()?.expression();
            if (exprs == null || exprs.Length == 0) { EmitPrintfRaw("\n"); return LLVMValue.Void; }

            for (int i = 0; i < exprs.Length; i++)
            {
                if (i > 0) EmitPrintfRaw(" ");
                var val = Visit(exprs[i]);
                if (val != null) EmitPrintfVal(val, i == exprs.Length - 1);
            }
            return LLVMValue.Void;
        }

        public override LLVMValue? VisitPrintStmt(MiniGoParser.PrintStmtContext ctx)
        {
            var exprs = ctx.expressionList()?.expression();
            if (exprs == null) return LLVMValue.Void;

            for (int i = 0; i < exprs.Length; i++)
            {
                if (i > 0) EmitPrintfRaw(" ");
                var val = Visit(exprs[i]);
                if (val != null) EmitPrintfVal(val, false);
            }
            return LLVMValue.Void;
        }
    }
}
