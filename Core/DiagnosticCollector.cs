using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MiniGoCompiler
{
    public enum DiagnosticPhase
    {
        Lexical,    
        Syntactic,  
        Semantic    
    }

    public sealed record Diagnostic(
        string Message,
        int Line,
        int Column,
        DiagnosticPhase Phase
    )
    {
        public override string ToString() =>
            $"[{Phase.ToString().ToUpperInvariant()}] Línea {Line}, Col {Column} — {Message}";
    }

    public sealed class DiagnosticCollector
    {
        private static readonly Lazy<DiagnosticCollector> _lazy =
            new(() => new DiagnosticCollector(), isThreadSafe: true);

        public static DiagnosticCollector Instance => _lazy.Value;

        private DiagnosticCollector() { }

        private readonly object _lock = new();
        private readonly List<Diagnostic> _diagnostics = new();

        public int MaxDiagnostics { get; set; } = 20;

        public IReadOnlyList<Diagnostic> Diagnostics
        {
            get
            {
                lock (_lock)
                {
                    int count = Math.Min(_diagnostics.Count, MaxDiagnostics);
                    return _diagnostics.GetRange(0, count).AsReadOnly();
                }
            }
        }

        public int TotalCount
        {
            get { lock (_lock) { return _diagnostics.Count; } }
        }

        public bool HasErrors
        {
            get { lock (_lock) { return _diagnostics.Count > 0; } }
        }

        public bool HasLexicalErrors
        {
            get
            {
                lock (_lock)
                {
                    return _diagnostics.Exists(d => d.Phase == DiagnosticPhase.Lexical);
                }
            }
        }

        public bool HasSyntacticErrors
        {
            get
            {
                lock (_lock)
                {
                    return _diagnostics.Exists(d => d.Phase == DiagnosticPhase.Syntactic);
                }
            }
        }

        public bool HasSemanticErrors
        {
            get
            {
                lock (_lock)
                {
                    return _diagnostics.Exists(d => d.Phase == DiagnosticPhase.Semantic);
                }
            }
        }

        public void Add(Diagnostic diagnostic)
        {
            if (diagnostic is null)
                throw new ArgumentNullException(nameof(diagnostic));

            lock (_lock)
            {
                _diagnostics.Add(diagnostic);
            }
        }

        public void Add(string message, int line, int column, DiagnosticPhase phase) =>
            Add(new Diagnostic(message, line, column, phase));

        public void Clear()
        {
            lock (_lock)
            {
                _diagnostics.Clear();
            }
        }

        public string GetSummary()
        {
            lock (_lock)
            {
                if (_diagnostics.Count == 0)
                    return "Compilación exitosa — 0 errores.";

                var sb = new System.Text.StringBuilder();
                int shown = Math.Min(_diagnostics.Count, MaxDiagnostics);
                int hidden = _diagnostics.Count - shown;

                sb.AppendLine($"Se encontraron {_diagnostics.Count} error(es):");
                sb.AppendLine(new string('─', 60));

                for (int i = 0; i < shown; i++)
                    sb.AppendLine(_diagnostics[i].ToString());

                if (hidden > 0)
                    sb.AppendLine($"  ... y {hidden} error(es) adicional(es) no mostrado(s).");

                return sb.ToString();
            }
        }
    }
}