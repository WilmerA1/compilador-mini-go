using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Misc;

namespace MiniGoCompiler
{
    public sealed record CompilationResult(
        bool    Success,
        IParseTree? ParseTree,
        MiniGoParser? Parser,
        string? LLVMIr = null        // IR de LLVM generado por la Fase 4
    )
    {
        public int ErrorCount => DiagnosticCollector.Instance.TotalCount;
    }

    public sealed class CompilerPipeline
    {
        public bool DebugPrintParseTree { get; set; } = false;

        public CompilationResult Run(string sourceCode)
        {
            if (sourceCode is null)
                throw new ArgumentNullException(nameof(sourceCode));

            DiagnosticCollector.Instance.Clear();

            // PASO 1
            ICharStream inputStream = new AntlrInputStream(sourceCode);

            // PASO 2
            var lexer         = new MiniGoLexer(inputStream);
            var errorListener = new SyntaxErrorListener(DiagnosticCollector.Instance);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(errorListener);

            // PASO 3
            var tokenStream = new CommonTokenStream(lexer);

            // PASO 3B — inspección de ERROR_CHAR  ← NUEVO BLOQUE
            tokenStream.Fill();
            foreach (var token in tokenStream.GetTokens())
            {
                if (token.Type == MiniGoLexer.ERROR_CHAR)
                {
                    DiagnosticCollector.Instance.Add(new Diagnostic(
                        Message : $"Carácter no reconocido '{token.Text}'",
                        Line    : token.Line,
                        Column  : token.Column,
                        Phase   : DiagnosticPhase.Lexical
                    ));
                }
            }

            // Gate: si hay errores léxicos reales, no continuar al parser
            if (DiagnosticCollector.Instance.HasErrors)
                return new CompilationResult(false, null, null);

            // PASO 4
            var parser = new MiniGoParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            // PASO 5
            MiniGoParser.RootContext? parseTree = null;
            try
            {
                parseTree = parser.root();
            }
            catch (Exception ex)
            {
                DiagnosticCollector.Instance.Add(
                    message : $"Error inesperado durante el parseo: {ex.Message}",
                    line    : 0,
                    column  : 0,
                    phase   : DiagnosticPhase.Syntactic
                );
            }

            bool syntaxOk = !DiagnosticCollector.Instance.HasErrors;

            if (!syntaxOk)
                return new CompilationResult(false, null, null);

            // FASE 2 — Tabla de símbolos y chequeo de ámbitos
            var scopeChecker = new ScopeCheckVisitor();
            scopeChecker.Visit(parseTree);

            if (DiagnosticCollector.Instance.HasSemanticErrors)
                return new CompilationResult(false, null, null);

            // FASE 3 — Verificación de tipos
            var typeChecker = new TypeCheckVisitor();
            typeChecker.Visit(parseTree);

            if (DiagnosticCollector.Instance.HasSemanticErrors)
                return new CompilationResult(false, null, null);

            // FASE 4 — Generación de código LLVM IR
            var codeGen = new CodeGenVisitor();
            codeGen.Visit(parseTree);
            string llvmIr = codeGen.GetIR();

            // Escribir output.ll junto al directorio de trabajo (útil para flujo de terminal)
            try { File.WriteAllText("output.ll", llvmIr, new System.Text.UTF8Encoding(false)); }
            catch { /* no crítico */ }

            return new CompilationResult(true, parseTree, parser, llvmIr);
        }

        public CompilationResult RunFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(
                    $"No se encontró el archivo Mini-GO: '{filePath}'", filePath);

            string sourceCode = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            return Run(sourceCode);
        }
    }
}