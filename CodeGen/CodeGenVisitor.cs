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
        private string _curRetType     = "void";
        private string _breakTarget    = "";     // bloque destino de break
        private string _continueTarget = "";     // bloque destino de continue

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
    }
}
