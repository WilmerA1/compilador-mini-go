using System.Collections.Generic;

namespace MiniGoCompiler
{
    public sealed class ScopeStack
    {
        private readonly Stack<Dictionary<string, SymbolInfo>> _stack = new();

        public void PushScope() => _stack.Push(new Dictionary<string, SymbolInfo>());

        public void PopScope() => _stack.Pop();

        public void Define(SymbolInfo symbol)
        {
            var current = _stack.Peek();
            if (current.ContainsKey(symbol.Name))
            {
                DiagnosticCollector.Instance.Add(
                    $"'{symbol.Name}' ya fue declarado en este ámbito",
                    symbol.Line, symbol.Col,
                    DiagnosticPhase.Semantic);
                return;
            }
            current[symbol.Name] = symbol;
        }

        public SymbolInfo? TryLookup(string name)
        {
            foreach (var scope in _stack)
                if (scope.TryGetValue(name, out var sym))
                    return sym;
            return null;
        }

        public SymbolInfo Lookup(string name, int line, int col)
        {
            var found = TryLookup(name);
            if (found != null)
                return found;

            DiagnosticCollector.Instance.Add(
                $"identificador '{name}' no declarado",
                line, col,
                DiagnosticPhase.Semantic);

            // Centinela para silenciar errores en cascada
            var sentinel = new SymbolInfo(name, ErrorType.Instance, SymbolKind.Variable, line, col);
            _stack.Peek()[name] = sentinel;
            return sentinel;
        }
    }
}
