using System;
using Antlr4.Runtime;

namespace MiniGoCompiler
{
    public sealed class SyntaxErrorListener
        : IAntlrErrorListener<IToken>,
          IAntlrErrorListener<int>
    {
        private readonly DiagnosticCollector _collector;

        public SyntaxErrorListener()
            : this(DiagnosticCollector.Instance) { }

        public SyntaxErrorListener(DiagnosticCollector collector)
        {
            _collector = collector
                ?? throw new ArgumentNullException(nameof(collector));
        }

        public void SyntaxError(
            System.IO.TextWriter output,
            IRecognizer          recognizer,
            IToken               offendingSymbol,
            int                  line,
            int                  charPositionInLine,
            string               msg,
            RecognitionException e)
        {
            string enrichedMessage = BuildSyntacticMessage(msg, offendingSymbol);

            _collector.Add(new Diagnostic(
                Message : enrichedMessage,
                Line    : line,
                Column  : charPositionInLine,
                Phase   : DiagnosticPhase.Syntactic
            ));
        }

        /// <summary>
        /// NOTA ARQUITECTURAL: En la gramática MiniGo.g4 existe la regla
        /// ERROR_CHAR : . ; que captura cualquier carácter no reconocido
        /// como token válido. Por eso ANTLR nunca invoca este método en
        /// la práctica — los errores léxicos reales son detectados por el
        /// foreach de ERROR_CHAR en CompilerPipeline.Run().
        /// Esta sobrecarga se mantiene para cumplir la interfaz y como
        /// salvaguarda ante futuras modificaciones de la gramática.
        /// </summary>
        public void SyntaxError(
            System.IO.TextWriter output,
            IRecognizer          recognizer,
            int                  offendingSymbol,
            int                  line,
            int                  charPositionInLine,
            string               msg,
            RecognitionException e)
        {
            string charDisplay = offendingSymbol >= 0
                ? $"'{(char)offendingSymbol}'"
                : "EOF";

            string enrichedMessage = $"Carácter no reconocido {charDisplay}. {msg}";

            _collector.Add(new Diagnostic(
                Message : enrichedMessage,
                Line    : line,
                Column  : charPositionInLine,
                Phase   : DiagnosticPhase.Lexical
            ));
        }

        private static string BuildSyntacticMessage(string antlrMessage, IToken? offendingToken)
        {
            if (offendingToken is null)
                return antlrMessage;

            string tokenText = offendingToken.Text ?? "<desconocido>";

            if (antlrMessage.Contains(tokenText, StringComparison.Ordinal))
                return antlrMessage;

            return $"{antlrMessage} (token ofensivo: '{tokenText}')";
        }
    }
}