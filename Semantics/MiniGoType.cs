using System.Collections.Generic;
using System.Linq;

namespace MiniGoCompiler
{
    public abstract record MiniGoType;

    public record PrimitiveType(string Name) : MiniGoType
    {
        public override string ToString() => Name;
    }

    public record ArrayType(int Size, MiniGoType ElemType) : MiniGoType
    {
        public override string ToString() => $"[{Size}]{ElemType}";
    }

    public record SliceType(MiniGoType ElemType) : MiniGoType
    {
        public override string ToString() => $"[]{ElemType}";
    }

    public record StructType(Dictionary<string, MiniGoType> Fields) : MiniGoType
    {
        public override string ToString() =>
            $"struct{{ {string.Join("; ", Fields.Select(kv => $"{kv.Key} {kv.Value}"))} }}";
    }

    public record FunctionType(List<MiniGoType> ParamTypes, MiniGoType ReturnType) : MiniGoType
    {
        public override string ToString() =>
            $"func({string.Join(", ", ParamTypes)}) {ReturnType}";
    }

    public sealed record ErrorType : MiniGoType
    {
        public static readonly ErrorType Instance = new();
        private ErrorType() { }
        public override string ToString() => "<error>";
    }
}
