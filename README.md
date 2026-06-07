# Compilador Mini-GO

Compilador del lenguaje Mini-GO implementado en C# (.NET 10) con ANTLR4 y Avalonia UI.  
Genera código LLVM IR que puede compilarse a ejecutable nativo con clang.

---

## Requisitos

| Herramienta | Versión | Instalación |
|---|---|---|
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download |
| LLVM / clang | >= 19 | `winget install LLVM.LLVM` |
| Visual Studio 2022 | Community o Build Tools | Instalar workload **Desktop development with C++** |

> **Importante:** clang necesita las librerías MSVC para enlazar en Windows.  
> Deben estar instaladas a través del workload "Desktop development with C++" de Visual Studio.

---

## Estructura del proyecto

```
compilador-mini-go/
├── Grammar/          MiniGo.g4 — gramática ANTLR4
├── Core/             CompilerPipeline.cs, DiagnosticCollector.cs
├── Semantics/        ScopeCheckVisitor.cs, TypeCheckVisitor.cs
├── CodeGen/          CodeGenVisitor.cs, LLVMValue.cs
├── UI/               Ventana Avalonia (MainWindow, ViewModels)
├── MiniGoTests/      Suite de pruebas automatizadas
│   └── TestFiles/    Archivos .mgo de prueba
└── README.md
```

---

## Suite de pruebas automatizadas (Fases 1-4)

La suite corre 48 pruebas automáticas que cubren las fases 1, 2, 3 y 4.

### Paso 1 — Publicar el ejecutable de pruebas

Abrir PowerShell y ejecutar:

```powershell
cd C:\ruta\al\compilador-mini-go\MiniGoTests
dotnet publish -c Release -o C:\Users\jalpi\AppData\Local\Temp\mgo_test
```

> Se publica en `AppData\Local\Temp` para evitar restricciones de AppLocker en el escritorio.

### Paso 2 — Correr la suite

```powershell
C:\Users\jalpi\AppData\Local\Temp\mgo_test\MiniGoTests.exe
```

Resultado esperado:

```
Resultado: 48 pass  |  0 fail  |  48 total
```

---

## Prueba manual end-to-end (.mgo → .ll → .exe)

Permite compilar cualquier archivo `.mgo` y obtener el IR generado y un ejecutable nativo.

### Paso 1 — Publicar (si no se hizo antes)

```powershell
cd C:\ruta\al\compilador-mini-go\MiniGoTests
dotnet publish -c Release -o C:\Users\jalpi\AppData\Local\Temp\mgo_test
```

### Paso 2 — Compilar el archivo .mgo a LLVM IR

```powershell
C:\Users\jalpi\AppData\Local\Temp\mgo_test\MiniGoTests.exe "C:\ruta\al\archivo.mgo"
```

Esto genera `archivo.ll` en la misma carpeta que el `.mgo`.

### Paso 3 — Producir el ejecutable con clang

Abrir el **Developer Command Prompt for VS 2022** (buscarlo en el menú inicio).  
En esa terminal ejecutar:

```cmd
"C:\Program Files\LLVM\bin\clang.exe" "C:\ruta\al\archivo.ll" -o "C:\Users\jalpi\Documents\programa.exe"
```

> El ejecutable debe generarse en `C:\Users\jalpi\Documents\` u otra carpeta de usuario.  
> La carpeta `Temp` y el `Desktop` pueden estar bloqueados por Device Guard en esta máquina.

### Paso 4 — Ejecutar

```cmd
"C:\Users\jalpi\Documents\programa.exe"
```

---

## Ejemplo completo — Factorial

Crear el archivo `C:\Users\jalpi\Documents\factorial.mgo`:

```go
package main;

func factorial(n int) int {
    if n <= 1 { return 1; };
    return n * factorial(n - 1);
};

func main() int {
    var r int = factorial(5);
    println(r);
    return 0;
};
```

Compilar y ejecutar:

```powershell
# 1. Generar .ll
C:\Users\jalpi\AppData\Local\Temp\mgo_test\MiniGoTests.exe "C:\Users\jalpi\Documents\factorial.mgo"
```

En el Developer Command Prompt for VS 2022:

```cmd
# 2. Compilar a .exe
"C:\Program Files\LLVM\bin\clang.exe" "C:\Users\jalpi\Documents\factorial.ll" -o "C:\Users\jalpi\Documents\factorial.exe"

# 3. Ejecutar
"C:\Users\jalpi\Documents\factorial.exe"
```

Salida esperada: `120`

---

## Ejecutar la GUI (Avalonia)

```powershell
cd C:\ruta\al\compilador-mini-go
dotnet run
```

Desde la GUI se puede:
- Abrir archivos `.mgo` con el boton Abrir
- Compilar con el boton Compilar
- Ver errores con linea y columna en el panel inferior
- Hacer doble clic en un error para navegar a la linea en el editor

---

## Notas sobre Device Guard

En maquinas con politica de Device Guard/AppLocker, los ejecutables nativos compilados con clang  
pueden bloquearse si se guardan en ciertas rutas (Desktop, Temp raiz).  
**Solucion:** usar `C:\Users\<usuario>\Documents\` como carpeta de salida para el `.exe`.
