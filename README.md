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
> Deben estar instaladas a través del workload "Desktop development with C++" de Visual Studio 2022.

---

## Estructura del proyecto

```
compilador-mini-go/
├── Grammar/          MiniGo.g4 — gramática ANTLR4
├── Core/             CompilerPipeline.cs, DiagnosticCollector.cs
├── Semantics/        ScopeCheckVisitor.cs, TypeCheckVisitor.cs
├── CodeGen/          CodeGenVisitor.cs, LLVMValue.cs
├── UI/               App.cs, MainWindow.cs (Avalonia)
├── MiniGoTests/      Suite de pruebas automatizadas (Fases 1-4)
├── TestFiles/        Archivos .mgo de prueba
└── README.md
```

---

## Ejecutar la GUI (Avalonia)

### Nota sobre Device Guard / AppLocker

En esta máquina, Windows aplica una política de Control de Aplicaciones (WDAC/AppLocker) que **bloquea ejecutables en el Escritorio y en carpetas Temp raíz**. Por eso `dotnet run` falla con error de carga de ensamblado.

La solución es **publicar la aplicación a una carpeta permitida** (`AppData\Local\Temp\<subcarpeta>`) y correrla desde ahí.

### Paso 1 — Publicar la GUI

Abrí PowerShell en la carpeta del proyecto y ejecutá:

```powershell
# Ejemplo con la ruta del proyecto de jalpi
cd C:\Users\jalpi\Desktop\Proyecto_compiladores\compilador-mini-go
dotnet publish -c Release -o C:\Users\jalpi\AppData\Local\Temp\mgo_gui
```

```powershell
# Ejemplo genérico (reemplazá con tu ruta)
cd C:\Users\TU_USUARIO\ruta\al\compilador-mini-go
dotnet publish -c Release -o C:\Users\TU_USUARIO\AppData\Local\Temp\mgo_gui
```

### Paso 2 — Ejecutar

```powershell
# Ejemplo con la ruta de jalpi
C:\Users\jalpi\AppData\Local\Temp\mgo_gui\MiniGoCompiler.exe
```

```powershell
# Ejemplo genérico
C:\Users\TU_USUARIO\AppData\Local\Temp\mgo_gui\MiniGoCompiler.exe
```

### Flujo de trabajo en la GUI

| Paso | Acción | Resultado |
|------|--------|-----------|
| 1 | Seleccioná un test del **panel izquierdo** o usá **Abrir** | El código se carga en el editor |
| 2 | Presioná **COMPILAR** | Si hay errores aparecen en la grilla con línea y columna. Si no hay errores, aparece el botón **Generar .ll** |
| 3 | Presioná **Generar .ll** | Escribe el IR de LLVM en `Documentos\minigo_output.ll`. Aparece el botón **Ejecutar** |
| 4 | Presioná **Ejecutar** | La GUI llama a `clang`, produce el `.exe` y muestra el stdout en el panel **SALIDA** |
| ⓘ | Botón de info (arriba a la derecha) | Dialog con el resumen del flujo |

> **Doble-clic en un error** del DataGrid navega directamente a la línea en el editor.

---

## Suite de pruebas automatizadas (Fases 1-4)

La suite corre 48 pruebas automáticas que cubren las fases 1, 2, 3 y 4.

### Paso 1 — Publicar el ejecutable de pruebas

```powershell
# Ejemplo con la ruta de jalpi
cd C:\Users\jalpi\Desktop\Proyecto_compiladores\compilador-mini-go\MiniGoTests
dotnet publish -c Release -o C:\Users\jalpi\AppData\Local\Temp\mgo_test
```

```powershell
# Ejemplo genérico
cd C:\Users\TU_USUARIO\ruta\al\compilador-mini-go\MiniGoTests
dotnet publish -c Release -o C:\Users\TU_USUARIO\AppData\Local\Temp\mgo_test
```

### Paso 2 — Correr la suite

```powershell
# Ejemplo con la ruta de jalpi
C:\Users\jalpi\AppData\Local\Temp\mgo_test\MiniGoTests.exe

# Ejemplo genérico
C:\Users\TU_USUARIO\AppData\Local\Temp\mgo_test\MiniGoTests.exe
```

Resultado esperado:

```
Resultado: 48 pass  |  0 fail  |  48 total
```

---

## Prueba manual end-to-end (.mgo → .ll → .exe)

Para compilar un `.mgo` manualmente sin usar la GUI:

### Paso 1 — Generar el IR desde el ejecutable de pruebas

```powershell
# Ejemplo con la ruta de jalpi
C:\Users\jalpi\AppData\Local\Temp\mgo_test\MiniGoTests.exe "C:\Users\jalpi\Desktop\Proyecto_compiladores\compilador-mini-go\TestFiles\test_completo_codegen.mgo"

# Ejemplo genérico
C:\Users\TU_USUARIO\AppData\Local\Temp\mgo_test\MiniGoTests.exe "C:\ruta\al\archivo.mgo"
```

Esto genera `archivo.ll` en la misma carpeta que el `.mgo`.

### Paso 2 — Compilar a .exe con clang

Abrí el **Developer Command Prompt for VS 2022** (menú inicio) y ejecutá:

```cmd
# Ejemplo con la ruta de jalpi
"C:\Program Files\LLVM\bin\clang.exe" "C:\Users\jalpi\Desktop\Proyecto_compiladores\compilador-mini-go\TestFiles\test_completo_codegen.ll" -o "C:\Users\jalpi\Documents\programa.exe"

# Ejemplo genérico
"C:\Program Files\LLVM\bin\clang.exe" "C:\ruta\al\archivo.ll" -o "C:\Users\TU_USUARIO\Documents\programa.exe"
```

> El `.exe` debe generarse en `Documents\` u otra carpeta de usuario.  
> El Escritorio y carpetas Temp raíz pueden estar bloqueados por Device Guard.

### Paso 3 — Ejecutar

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

```powershell
# 1. Generar .ll (desde el ejecutable de tests)
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

## Notas sobre Device Guard

En máquinas con política WDAC/AppLocker, los ejecutables nativos compilados con clang  
pueden bloquearse si se guardan en ciertas rutas (Desktop, Temp raíz).  
**Solución:** usar `C:\Users\<usuario>\Documents\` como carpeta de salida para el `.exe`.

La GUI maneja esto automáticamente: siempre genera `minigo_output.exe` en `Documents\`.
