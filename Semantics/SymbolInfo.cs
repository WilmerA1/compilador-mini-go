namespace MiniGoCompiler
{
    public enum SymbolKind { Variable, Function, TypeAlias, Parameter }

    public record SymbolInfo(
        string Name,
        MiniGoType Type,
        SymbolKind Kind,
        int Line,
        int Col
    );
}
