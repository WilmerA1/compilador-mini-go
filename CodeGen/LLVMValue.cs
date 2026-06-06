namespace MiniGoCompiler
{
    /// <summary>
    /// Representa una referencia a un valor SSA en LLVM IR.
    ///   Ref      = nombre de registro ("%t3"), constante ("42") o global ("@g_x").
    ///   LLVMType = cadena de tipo LLVM ("i64", "double", "i1", "i32", "i8*", etc.)
    /// </summary>
    public sealed record LLVMValue(string Ref, string LLVMType)
    {
        /// Valor especial que representa "sin resultado" (instrucciones void).
        public static readonly LLVMValue Void = new("", "void");

        public bool IsVoid   => LLVMType == "void";
        public bool IsInt    => LLVMType == "i64";
        public bool IsDouble => LLVMType == "double";
        public bool IsBool   => LLVMType == "i1";
        public bool IsRune   => LLVMType == "i32";
        public bool IsPtr    => LLVMType == "i8*";

        public override string ToString() => Ref;
    }
}
