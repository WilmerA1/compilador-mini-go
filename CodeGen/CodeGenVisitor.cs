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
            PreRegisterFuncs(top);            // forward references
            VisitTopDeclarationList(top);     // genera el módulo
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
            if (ctx.appendExpression()  != null) return Visit(ctx.appendExpression());
            if (ctx.lengthExpression()  != null) return Visit(ctx.lengthExpression());
            if (ctx.capExpression()     != null) return Visit(ctx.capExpression());
            // selector / index: soporte en commit 6
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
    }
}
