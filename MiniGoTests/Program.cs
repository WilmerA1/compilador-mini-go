using System;
using System.IO;
using MiniGoCompiler;

int pass = 0, fail = 0;

void RunTest(string label, string source, bool expectSuccess)
{
    DiagnosticCollector.Instance.Clear();
    var result = new CompilerPipeline().Run(source);
    var diags  = DiagnosticCollector.Instance.Diagnostics;
    bool ok    = result.Success == expectSuccess;

    Console.WriteLine($"\n  {(ok ? "✅ PASS" : "❌ FAIL")}  {label}");

    if (diags.Count > 0)
        foreach (var d in diags)
            Console.WriteLine($"         [{d.Phase,-10}] L{d.Line}:{d.Column} — {d.Message}");
    else
        Console.WriteLine("         Compilación exitosa — 0 errores.");

    if (ok) pass++; else fail++;
}

Console.WriteLine("══════════════════════════════════════════════════════════");
Console.WriteLine("  MINI-GO  —  Test Suite (Fases 1-3)");
Console.WriteLine("══════════════════════════════════════════════════════════");

// ── FASE 1 / 2: casos existentes ──────────────────────────────────────────
Console.WriteLine("\n▸ Fase 1 & 2 — scope");

RunTest("válido simple (función vacía)", @"
package main;
func saludo() int { return 1; };
", true);

RunTest("válido con struct y suma", @"
package main;
var contador int = 0;
type Punto struct { x int; y int; };
func suma(a int, b int) int { return a; };
func main() int { var x int = 10; suma(x, 20); return 0; };
", true);

RunTest("error léxico (@)", @"
package main;
var x @ int;
func main() int { return 0; };
", false);

RunTest("error sintáctico (falta ;)", @"
package main;
func main() int { var x int = 10
return x; };
", false);

RunTest("forward reference entre funciones", @"
package main;
func main() int { return helper(); };
func helper() int { return 42; };
", true);

RunTest("for con scope de init", @"
package main;
func main() int {
    for i := 0; i < 10; i++ { };
    return 0;
};
", true);

// ── FASE 3: type checking ──────────────────────────────────────────────────
Console.WriteLine("\n▸ Fase 3 — verificación de tipos");

RunTest("tipos correctos", @"
package main;
func suma(a int, b int) int { return a + b; };
func esMayor(a int, b int) bool { return a > b; };
func main() int {
    var x int = 10;
    var y int = 20;
    var r int = suma(x, y);
    var ok bool = esMayor(x, y);
    var s string = ""hola"" + "" mundo"";
    return 0;
};
", true);

RunTest("error: int + string", @"
package main;
func main() int {
    var x int = 10;
    var y string = ""hola"";
    var mal int = x + y;
    return 0;
};
", false);

RunTest("error: condición if no es bool", @"
package main;
func main() int {
    var x int = 5;
    if x { println(); };
    return 0;
};
", false);

RunTest("error: return tipo incorrecto", @"
package main;
func dameInt() int { return ""texto""; };
func main() int { return 0; };
", false);

RunTest("error: argumento tipo incorrecto", @"
package main;
func suma(a int, b int) int { return a + b; };
func main() int {
    var s string = ""x"";
    suma(1, s);
    return 0;
};
", false);

RunTest("condición for debe ser bool", @"
package main;
func main() int {
    var x int = 5;
    for x { };
    return 0;
};
", false);

RunTest("inferencia de tipo := correcto", @"
package main;
func main() int {
    x := 10;
    y := 20;
    z := x + y;
    return 0;
};
", true);

RunTest("operadores lógicos && || correctos", @"
package main;
func main() int {
    var a bool = true;
    var b bool = false;
    var c bool = a && b;
    var d bool = a || b;
    return 0;
};
", true);

// ── FASE 3 extendida: todos los casos del enunciado ───────────────────────
Console.WriteLine("\n▸ Fase 3 — operadores extendidos");

RunTest("operadores multiplicativos correctos", @"
package main;
func main() int {
    var a int = 10;
    var b int = 3;
    var c int = a * b;
    var d int = a / b;
    var e int = a % b;
    var f int = a << 1;
    var g int = a >> 1;
    var h int = a & b;
    return 0;
};
", true);

RunTest("error: operador multiplicativo con string", @"
package main;
func main() int {
    var a int = 10;
    var b string = ""x"";
    var c int = a * b;
    return 0;
};
", false);

RunTest("operadores unarios correctos", @"
package main;
func main() int {
    var a int = 5;
    var b int = -a;
    var c bool = !true;
    var d int = ^a;
    return 0;
};
", true);

RunTest("error: NOT sobre int", @"
package main;
func main() int {
    var a int = 5;
    var b bool = !a;
    return 0;
};
", false);

RunTest("float64 operaciones", @"
package main;
func main() int {
    var a float64 = 3.14;
    var b float64 = 2.71;
    var c float64 = a + b;
    var d float64 = a * b;
    var e bool = a > b;
    return 0;
};
", true);

RunTest("comparación igual tipo correcto", @"
package main;
func main() int {
    var a int = 1;
    var b int = 2;
    var c bool = a == b;
    var d bool = a != b;
    var s1 string = ""a"";
    var s2 string = ""b"";
    var e bool = s1 == s2;
    return 0;
};
", true);

RunTest("error: comparación tipos distintos", @"
package main;
func main() int {
    var a int = 1;
    var b string = ""x"";
    var c bool = a == b;
    return 0;
};
", false);

Console.WriteLine("\n▸ Fase 3 — structs, arrays, slices");

RunTest("struct declaración y acceso a campo", @"
package main;
type Punto struct { x int; y int; };
func main() int {
    var p Punto;
    return 0;
};
", true);

RunTest("append y len correctos", @"
package main;
func main() int {
    var s []int;
    s = append(s, 1);
    var n int = len(s);
    return 0;
};
", true);

RunTest("cap correcto", @"
package main;
func main() int {
    var s []int;
    var c int = cap(s);
    return 0;
};
", true);

RunTest("error: len asignado a string", @"
package main;
func main() int {
    var s []int;
    var n string = len(s);
    return 0;
};
", false);

Console.WriteLine("\n▸ Fase 3 — funciones y retorno");

RunTest("función void (sin retorno)", @"
package main;
func imprimir(x int) {
    println(x);
};
func main() int {
    imprimir(42);
    return 0;
};
", true);

RunTest("función recursiva", @"
package main;
func factorial(n int) int {
    if n <= 1 { return 1; };
    return n * factorial(n - 1);
};
func main() int {
    var r int = factorial(5);
    return 0;
};
", true);

RunTest("error: aridad incorrecta", @"
package main;
func suma(a int, b int) int { return a + b; };
func main() int {
    suma(1, 2, 3);
    return 0;
};
", false);

RunTest("múltiples parámetros tipos correctos", @"
package main;
func combinar(a int, b string, c bool) int { return a; };
func main() int {
    combinar(1, ""texto"", true);
    return 0;
};
", true);

Console.WriteLine("\n▸ Fase 3 — control de flujo");

RunTest("switch básico correcto", @"
package main;
func main() int {
    var x int = 5;
    switch x {
        case 1: println(x);
        case 2: println(x);
        default: println(x);
    };
    return 0;
};
", true);

RunTest("switch con init statement", @"
package main;
func main() int {
    switch x := 5; x {
        case 5: println(x);
        default: println(x);
    };
    return 0;
};
", true);

RunTest("for infinito correcto", @"
package main;
func main() int {
    for {
        break;
    };
    return 0;
};
", true);

RunTest("for tres partes sin condición", @"
package main;
func main() int {
    for i := 0; ; i++ {
        break;
    };
    return 0;
};
", true);

RunTest("if con else correcto", @"
package main;
func main() int {
    var x int = 5;
    if x > 3 {
        println(x);
    } else {
        println(x);
    };
    return 0;
};
", true);

RunTest("if con init statement", @"
package main;
func main() int {
    if x := 10; x > 5 {
        println(x);
    };
    return 0;
};
", true);

Console.WriteLine("\n▸ Fase 3 — declaraciones de variable");

RunTest("var múltiple con tipo", @"
package main;
func main() int {
    var a, b int = 1, 2;
    var c int = a + b;
    return 0;
};
", true);

RunTest("var con inferencia de tipo", @"
package main;
func main() int {
    var x = 42;
    var s = ""hola"";
    var f = 3.14;
    return 0;
};
", true);

RunTest("type alias", @"
package main;
type Entero int;
func main() int {
    var x Entero;
    return 0;
};
", true);

RunTest("asignación compuesta correcta", @"
package main;
func main() int {
    var x int = 10;
    x += 5;
    x -= 2;
    x *= 3;
    return 0;
};
", true);

RunTest("error: asignación compuesta tipos distintos", @"
package main;
func main() int {
    var x int = 10;
    x += ""texto"";
    return 0;
};
", false);

RunTest("string concatenación con +", @"
package main;
func main() int {
    var a string = ""hola"";
    var b string = "" mundo"";
    var c string = a + b;
    return 0;
};
", true);

RunTest("error: && con no-bool", @"
package main;
func main() int {
    var x int = 1;
    var y int = 2;
    var z bool = x && y;
    return 0;
};
", false);

// ── Resumen ────────────────────────────────────────────────────────────────
Console.WriteLine($"\n{'═',58}");
Console.WriteLine($"  Resultado: {pass} ✅ pass  |  {fail} ❌ fail  |  {pass+fail} total");
Console.WriteLine($"{'═',58}\n");
Environment.Exit(fail > 0 ? 1 : 0);
