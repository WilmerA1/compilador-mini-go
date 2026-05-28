using Antlr4.Runtime;
using System;

namespace MiniGoCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputCode = "package main;";

            // Flujo de lectura de caracteres clásico de ANTLR C#
            AntlrInputStream stream = new AntlrInputStream(inputCode);

            // Crear Lexer y Parser
            MiniGoLexer lexer = new MiniGoLexer(stream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            MiniGoParser parser = new MiniGoParser(tokens);

            // Invocar la regla raíz definida en tu archivo .g4
            var tree = parser.root();

            Console.WriteLine("¡Análisis sintáctico completado con éxito!");
            Console.WriteLine(tree.ToStringTree(parser));
        }
    }
}