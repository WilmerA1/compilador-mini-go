using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using MiniGoCompiler;

namespace MiniGoCompiler.UI
{
    public sealed class DiagnosticRow
    {
        public int    Linea   { get; init; }
        public int    Columna { get; init; }
        public string Tipo    { get; init; } = "";
        public string Mensaje { get; init; } = "";
    }

    public sealed class MainWindow : Window
    {
        // ── Controles principales ─────────────────────────────────────────────────
        private readonly TextBox   _editor;
        private readonly DataGrid  _errorGrid;
        private readonly TextBlock _statusBar;
        private readonly Button    _btnAbrir;
        private readonly Button    _btnGuardar;
        private readonly Button    _btnCompilar;
        private readonly Button    _btnGenerar;
        private readonly Button    _btnEjecutar;
        private readonly TextBox   _outputBox;
        private readonly TextBlock _fileInfoLabel;

        // ── Sidebar — sección tests ───────────────────────────────────────────────
        private ListBox _testList = null!;
        private readonly Dictionary<string, string> _testFiles = new();

        // ── Sidebar — sección .ll ────────────────────────────────────────────────
        private ListBox _llList   = null!;
        private Button  _btnLimpiar = null!;
        private readonly Dictionary<string, string> _llFiles = new();

        // ── Estado ────────────────────────────────────────────────────────────────
        private readonly ObservableCollection<DiagnosticRow> _diagnosticRows = new();
        private string? _currentFilePath;
        private string? _lastLlvmIr;
        private string? _testFolder;

        // ── Rutas ─────────────────────────────────────────────────────────────────
        private static readonly string DocsFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string LlPath  => Path.Combine(DocsFolder, "minigo_output.ll");
        private string ExePath => Path.Combine(DocsFolder, "minigo_output.exe");
        private static readonly string ClangExe =
            @"C:\Program Files\LLVM\bin\clang.exe";

        public MainWindow()
        {
            Title      = "Mini-GO Compiler";
            Width      = 1200;
            Height     = 820;
            MinWidth   = 800;
            MinHeight  = 540;
            Background = new SolidColorBrush(Color.Parse("#1E1E2E"));

            // ── Botones de toolbar ────────────────────────────────────────────────
            _btnAbrir    = BuildToolButton("Abrir",       "#3A86FF");
            _btnGuardar  = BuildToolButton("Guardar",     "#6A994E");
            _btnCompilar = BuildToolButton("COMPILAR",    "#FF006E");
            _btnGenerar  = BuildToolButton("Generar .ll", "#7C3AED");
            _btnEjecutar = BuildToolButton("Ejecutar",    "#059669");

            _btnGenerar.IsVisible  = false;
            _btnEjecutar.IsVisible = false;

            _btnAbrir.Click    += OnAbrirClick;
            _btnGuardar.Click  += OnGuardarClick;
            _btnCompilar.Click += OnCompilarClick;
            _btnGenerar.Click  += OnGenerarClick;
            _btnEjecutar.Click += OnEjecutarClick;

            var btnInfo = new Button
            {
                Content         = "ⓘ",
                Background      = new SolidColorBrush(Color.Parse("#313244")),
                Foreground      = new SolidColorBrush(Color.Parse("#89DCEB")),
                FontSize        = 16,
                Width           = 34,
                Height          = 34,
                Padding         = new Thickness(0),
                CornerRadius    = new CornerRadius(17),
                BorderThickness = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
                Cursor          = new Cursor(StandardCursorType.Hand),
            };
            btnInfo.Click += OnInfoClick;

            var leftButtons = new StackPanel
            {
                Orientation       = Orientation.Horizontal,
                Spacing           = 8,
                VerticalAlignment = VerticalAlignment.Center,
                Children          = { _btnAbrir, _btnGuardar, _btnCompilar, _btnGenerar, _btnEjecutar }
            };

            var toolbarRow = new DockPanel { Margin = new Thickness(12, 8, 12, 8) };
            DockPanel.SetDock(btnInfo, Dock.Right);
            toolbarRow.Children.Add(btnInfo);
            toolbarRow.Children.Add(leftButtons);

            _fileInfoLabel = new TextBlock
            {
                Text       = "Sin archivo abierto",
                Foreground = new SolidColorBrush(Color.Parse("#A6A6A6")),
                FontSize   = 12,
                Margin     = new Thickness(14, 0, 14, 8),
                FontFamily = new FontFamily("Cascadia Code,Consolas,monospace")
            };

            var topPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children    = { toolbarRow, _fileInfoLabel }
            };

            // ── Editor ───────────────────────────────────────────────────────────
            _editor = new TextBox
            {
                AcceptsReturn  = true,
                AcceptsTab     = true,
                FontFamily     = new FontFamily("Cascadia Code,Consolas,Courier New,monospace"),
                FontSize       = 14,
                Foreground     = new SolidColorBrush(Color.Parse("#CDD6F4")),
                Background     = new SolidColorBrush(Color.Parse("#181825")),
                CaretBrush     = new SolidColorBrush(Color.Parse("#F38BA8")),
                SelectionBrush = new SolidColorBrush(Color.Parse("#45475A")),
                Padding        = new Thickness(16),
                BorderThickness= new Thickness(0),
                TextWrapping   = TextWrapping.NoWrap,
                Watermark      = "// Escribe tu código Mini-GO aquí...",
                Text           = GetStarterCode(),
                VerticalContentAlignment = VerticalAlignment.Top,
            };

            var editorBorder = new Border
            {
                BorderBrush     = new SolidColorBrush(Color.Parse("#313244")),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Margin          = new Thickness(0, 0, 12, 6),
                Child           = new ScrollViewer
                {
                    HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility   = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    Content = _editor
                }
            };

            // ── DataGrid de errores ───────────────────────────────────────────────
            var diagLabel = new TextBlock
            {
                Text          = "DIAGNÓSTICOS",
                Foreground    = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize      = 11,
                FontWeight    = FontWeight.Bold,
                Margin        = new Thickness(0, 4, 12, 4),
                LetterSpacing = 1.5
            };

            _errorGrid = new DataGrid
            {
                AutoGenerateColumns  = false,
                IsReadOnly           = true,
                CanUserResizeColumns = true,
                CanUserSortColumns   = true,
                GridLinesVisibility  = DataGridGridLinesVisibility.Horizontal,
                Background           = new SolidColorBrush(Color.Parse("#181825")),
                Foreground           = new SolidColorBrush(Color.Parse("#CDD6F4")),
                FontFamily           = new FontFamily("Cascadia Code,Consolas,monospace"),
                FontSize             = 13,
                RowBackground        = new SolidColorBrush(Color.Parse("#181825")),
                BorderThickness      = new Thickness(0),
                ItemsSource          = _diagnosticRows,
            };
            _errorGrid.Columns.Add(new DataGridTextColumn { Header = "Línea",   Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Linea)),   Width = new DataGridLength(70) });
            _errorGrid.Columns.Add(new DataGridTextColumn { Header = "Columna", Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Columna)), Width = new DataGridLength(80) });
            _errorGrid.Columns.Add(new DataGridTextColumn { Header = "Tipo",    Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Tipo)),    Width = new DataGridLength(110) });
            _errorGrid.Columns.Add(new DataGridTextColumn { Header = "Mensaje", Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Mensaje)), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
            _errorGrid.DoubleTapped += OnErrorRowDoubleTapped;

            var diagPanel = new Border
            {
                BorderBrush     = new SolidColorBrush(Color.Parse("#313244")),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Margin          = new Thickness(0, 0, 12, 6),
                Child           = new ScrollViewer
                {
                    VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    Content = _errorGrid
                }
            };

            // ── Panel de salida ───────────────────────────────────────────────────
            var outputLabel = new TextBlock
            {
                Text          = "SALIDA",
                Foreground    = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize      = 11,
                FontWeight    = FontWeight.Bold,
                Margin        = new Thickness(0, 4, 12, 4),
                LetterSpacing = 1.5
            };

            _outputBox = new TextBox
            {
                IsReadOnly      = true,
                AcceptsReturn   = true,
                FontFamily      = new FontFamily("Cascadia Code,Consolas,Courier New,monospace"),
                FontSize        = 13,
                Foreground      = new SolidColorBrush(Color.Parse("#A6E3A1")),
                Background      = new SolidColorBrush(Color.Parse("#0D0D1A")),
                Padding         = new Thickness(16),
                BorderThickness = new Thickness(0),
                TextWrapping    = TextWrapping.Wrap,
                Watermark       = "La salida del programa aparecerá aquí...",
            };

            var outputPanel = new Border
            {
                BorderBrush     = new SolidColorBrush(Color.Parse("#313244")),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Margin          = new Thickness(0, 0, 12, 6),
                Child           = new ScrollViewer
                {
                    VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    Content = _outputBox
                }
            };

            // ── Status bar ────────────────────────────────────────────────────────
            _statusBar = new TextBlock
            {
                Text       = "Listo.",
                Foreground = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize   = 12,
                Margin     = new Thickness(14, 2, 14, 6),
                FontFamily = new FontFamily("Cascadia Code,Consolas,monospace")
            };

            // ── Contenido principal (columna derecha) ─────────────────────────────
            var contentGrid = new Grid
            {
                RowDefinitions = new RowDefinitions("*,Auto,150,Auto,120"),
            };
            Grid.SetRow(editorBorder, 0);
            Grid.SetRow(diagLabel,    1);
            Grid.SetRow(diagPanel,    2);
            Grid.SetRow(outputLabel,  3);
            Grid.SetRow(outputPanel,  4);
            contentGrid.Children.Add(editorBorder);
            contentGrid.Children.Add(diagLabel);
            contentGrid.Children.Add(diagPanel);
            contentGrid.Children.Add(outputLabel);
            contentGrid.Children.Add(outputPanel);

            // ── Sidebar ───────────────────────────────────────────────────────────
            var sidebar = BuildSidebar();

            // ── Layout raíz ──────────────────────────────────────────────────────
            var root = new Grid
            {
                RowDefinitions    = new RowDefinitions("Auto,*,Auto"),
                ColumnDefinitions = new ColumnDefinitions("190,*"),
            };

            Grid.SetRow(topPanel, 0);
            Grid.SetColumnSpan(topPanel, 2);
            root.Children.Add(topPanel);

            Grid.SetRow(sidebar, 1);
            Grid.SetColumn(sidebar, 0);
            root.Children.Add(sidebar);

            Grid.SetRow(contentGrid, 1);
            Grid.SetColumn(contentGrid, 1);
            root.Children.Add(contentGrid);

            Grid.SetRow(_statusBar, 2);
            Grid.SetColumnSpan(_statusBar, 2);
            root.Children.Add(_statusBar);

            Content = root;
        }

        // ── Construcción del sidebar ──────────────────────────────────────────────

        private Border BuildSidebar()
        {
            // ── Sección superior: Tests ───────────────────────────────────────────
            var testsLabel = new TextBlock
            {
                Text          = "TESTS",
                Foreground    = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize      = 11,
                FontWeight    = FontWeight.Bold,
                Margin        = new Thickness(12, 10, 12, 6),
                LetterSpacing = 1.5
            };

            _testList = new ListBox
            {
                Background      = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding         = new Thickness(0, 2),
                FontFamily      = new FontFamily("Cascadia Code,Consolas,monospace"),
                FontSize        = 12,
            };
            _testList.SelectionChanged += OnTestSelected;
            LoadTestFiles();

            var testsSection = new DockPanel { LastChildFill = true };
            DockPanel.SetDock(testsLabel, Dock.Top);
            testsSection.Children.Add(testsLabel);
            testsSection.Children.Add(new ScrollViewer
            {
                VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                Content = _testList
            });

            // ── Separador ─────────────────────────────────────────────────────────
            var separator = new Border
            {
                Height          = 1,
                Background      = new SolidColorBrush(Color.Parse("#313244")),
                Margin          = new Thickness(8, 0, 8, 0),
            };

            // ── Sección inferior: Archivos .ll ────────────────────────────────────
            var llLabel = new TextBlock
            {
                Text          = "ARCHIVOS .ll",
                Foreground    = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize      = 11,
                FontWeight    = FontWeight.Bold,
                LetterSpacing = 1.5,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _btnLimpiar = new Button
            {
                Content         = "🗑",
                Background      = new SolidColorBrush(Color.Parse("#3B1F1F")),
                Foreground      = new SolidColorBrush(Color.Parse("#F38BA8")),
                FontSize        = 13,
                Width           = 28,
                Height          = 24,
                Padding         = new Thickness(0),
                CornerRadius    = new CornerRadius(4),
                BorderThickness = new Thickness(0),
                Cursor          = new Cursor(StandardCursorType.Hand),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
            };
            _btnLimpiar.Click += OnLimpiarLlClick;

            var llHeader = new DockPanel
            {
                Margin        = new Thickness(12, 8, 8, 4),
                LastChildFill = false,
            };
            DockPanel.SetDock(_btnLimpiar, Dock.Right);
            DockPanel.SetDock(llLabel, Dock.Left);
            llHeader.Children.Add(_btnLimpiar);
            llHeader.Children.Add(llLabel);

            _llList = new ListBox
            {
                Background      = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding         = new Thickness(0, 2),
                FontFamily      = new FontFamily("Cascadia Code,Consolas,monospace"),
                FontSize        = 11,
                Foreground      = new SolidColorBrush(Color.Parse("#CBA6F7")),
            };
            LoadLlFiles();

            var llSection = new DockPanel { LastChildFill = true };
            DockPanel.SetDock(llHeader, Dock.Top);
            llSection.Children.Add(llHeader);
            llSection.Children.Add(new ScrollViewer
            {
                VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                Content = _llList
            });

            // ── Grid del sidebar ──────────────────────────────────────────────────
            var sidebarGrid = new Grid
            {
                RowDefinitions = new RowDefinitions("*,Auto,185"),
            };
            Grid.SetRow(testsSection, 0);
            Grid.SetRow(separator,    1);
            Grid.SetRow(llSection,    2);
            sidebarGrid.Children.Add(testsSection);
            sidebarGrid.Children.Add(separator);
            sidebarGrid.Children.Add(llSection);

            return new Border
            {
                Background      = new SolidColorBrush(Color.Parse("#181825")),
                BorderBrush     = new SolidColorBrush(Color.Parse("#313244")),
                BorderThickness = new Thickness(0, 0, 1, 0),
                Child           = sidebarGrid
            };
        }

        // ── Carga de archivos en el sidebar ───────────────────────────────────────

        private string? FindTestFolder()
        {
            string[] candidates =
            {
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestFiles")),
                Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TestFiles")),
                Path.Combine(AppContext.BaseDirectory, "TestFiles"),
                @"C:\Users\jalpi\Desktop\Proyecto_compiladores\compilador-mini-go\TestFiles",
            };
            foreach (var c in candidates)
            {
                try { if (Directory.Exists(c)) return c; }
                catch { }
            }
            return null;
        }

        private void LoadTestFiles()
        {
            _testFolder = FindTestFolder();
            _testFiles.Clear();
            _testList.Items.Clear();

            if (_testFolder is null)
            {
                _testList.Items.Add("(carpeta Tests no encontrada)");
                return;
            }

            var files = Directory.GetFiles(_testFolder, "*.mgo");
            Array.Sort(files);
            foreach (var f in files)
            {
                string name = Path.GetFileName(f);
                _testFiles[name] = f;
                _testList.Items.Add(name);
            }

            if (files.Length == 0)
                _testList.Items.Add("(sin archivos .mgo)");
        }

        private void LoadLlFiles()
        {
            _llFiles.Clear();
            _llList.Items.Clear();

            var searchFolders = new List<string>();
            if (_testFolder != null)
            {
                searchFolders.Add(_testFolder);
                string? parent = Path.GetDirectoryName(_testFolder);
                if (parent != null) searchFolders.Add(parent);
            }

            foreach (var folder in searchFolders)
            {
                try
                {
                    foreach (var f in Directory.GetFiles(folder, "*.ll"))
                    {
                        string name = Path.GetRelativePath(folder, f);
                        if (!_llFiles.ContainsKey(f))
                        {
                            _llFiles[f] = f;
                            _llList.Items.Add(name);
                        }
                    }
                }
                catch { }
            }

            if (_llList.Items.Count == 0)
                _llList.Items.Add("(sin archivos .ll)");
        }

        // ── Handlers del sidebar ──────────────────────────────────────────────────

        private async void OnTestSelected(object? sender, SelectionChangedEventArgs e)
        {
            if (_testList.SelectedItem is not string name) return;
            if (!_testFiles.TryGetValue(name, out string? path)) return;
            if (!File.Exists(path)) return;

            SetBusy(true);
            try
            {
                _currentFilePath    = path;
                _editor.Text        = await File.ReadAllTextAsync(path);
                _fileInfoLabel.Text = $"Test: {name}";
                SetStatus($"Test abierto: {name}", "#A6E3A1");
                ClearDiagnostics();
                ResetGenerarEjecutar();
            }
            finally { SetBusy(false); }
        }

        private void OnLimpiarLlClick(object? sender, RoutedEventArgs e)
        {
            SetBusy(true);
            try
            {
                int deleted = 0;
                foreach (var path in _llFiles.Keys)
                {
                    try { File.Delete(path); deleted++; }
                    catch { }
                }
                LoadLlFiles();
                SetStatus($"{deleted} archivo(s) .ll eliminado(s).", "#FAB387");
            }
            finally { SetBusy(false); }
        }

        // ── Handlers de botones principales ──────────────────────────────────────

        private async void OnAbrirClick(object? sender, RoutedEventArgs e)
        {
            SetBusy(true);
            try
            {
            string testFilesPath = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestFiles"));

            Avalonia.Platform.Storage.IStorageFolder? startFolder = null;
            if (Directory.Exists(testFilesPath))
                startFolder = await StorageProvider.TryGetFolderFromPathAsync(
                    new Uri($"file:///{testFilesPath}"));

            var options = new Avalonia.Platform.Storage.FilePickerOpenOptions
            {
                Title          = "Abrir archivo Mini-GO",
                AllowMultiple  = false,
                SuggestedStartLocation = startFolder,
                FileTypeFilter = new[]
                {
                    new Avalonia.Platform.Storage.FilePickerFileType("Mini-GO")
                        { Patterns = new[] { "*.mgo", "*.go" } },
                    new Avalonia.Platform.Storage.FilePickerFileType("Todos")
                        { Patterns = new[] { "*" } }
                }
            };

            var files = await StorageProvider.OpenFilePickerAsync(options);
            if (files is { Count: > 0 })
            {
                _currentFilePath = files[0].Path.LocalPath;
                _editor.Text     = await File.ReadAllTextAsync(_currentFilePath);
                _fileInfoLabel.Text = $"Archivo: {Path.GetFileName(_currentFilePath)}";
                SetStatus($"Archivo abierto: {Path.GetFileName(_currentFilePath)}", "#A6E3A1");
                ClearDiagnostics();
                ResetGenerarEjecutar();
                _testList.SelectedItem = null;
            }
            }
            finally { SetBusy(false); }
        }

        private async void OnGuardarClick(object? sender, RoutedEventArgs e)
        {
            SetBusy(true);
            try
            {
            if (_currentFilePath is null)
            {
                var options = new Avalonia.Platform.Storage.FilePickerSaveOptions
                {
                    Title            = "Guardar archivo Mini-GO",
                    DefaultExtension = "mgo",
                    FileTypeChoices  = new[]
                    {
                        new Avalonia.Platform.Storage.FilePickerFileType("Mini-GO")
                            { Patterns = new[] { "*.mgo", "*.go" } }
                    }
                };
                var file = await StorageProvider.SaveFilePickerAsync(options);
                if (file is null) return;
                _currentFilePath = file.Path.LocalPath;
            }

            await File.WriteAllTextAsync(_currentFilePath, _editor.Text ?? "");
            _fileInfoLabel.Text = $"Archivo: {Path.GetFileName(_currentFilePath)}";
            SetStatus($"Guardado: {Path.GetFileName(_currentFilePath)}", "#A6E3A1");
            }
            finally { SetBusy(false); }
        }

        private void OnCompilarClick(object? sender, RoutedEventArgs e)
        {
            SetBusy(true);
            try
            {
            ClearDiagnostics();
            ResetGenerarEjecutar();
            SetStatus("Compilando...", "#FAB387");

            var pipeline = new CompilerPipeline();
            var result   = pipeline.Run(_editor.Text ?? "");

            if (!DiagnosticCollector.Instance.HasErrors)
            {
                _lastLlvmIr           = result.LLVMIr;
                _btnGenerar.IsVisible = true;
                SetStatus("Compilación exitosa — 0 errores. Presioná \"Generar .ll\" para continuar.", "#A6E3A1");
                LoadLlFiles(); // refrescar lista .ll por si se generó output.ll
                return;
            }

            foreach (var diag in DiagnosticCollector.Instance.Diagnostics)
            {
                _diagnosticRows.Add(new DiagnosticRow
                {
                    Linea   = diag.Line,
                    Columna = diag.Column,
                    Tipo    = diag.Phase switch
                    {
                        DiagnosticPhase.Lexical   => "Léxico",
                        DiagnosticPhase.Syntactic => "Sintáctico",
                        DiagnosticPhase.Semantic  => "Semántico",
                        _                         => diag.Phase.ToString()
                    },
                    Mensaje = diag.Message
                });
            }

            SetStatus($"{DiagnosticCollector.Instance.TotalCount} error(es) encontrado(s).", "#F38BA8");
            }
            finally { SetBusy(false); }
        }

        private void OnGenerarClick(object? sender, RoutedEventArgs e)
        {
            if (_lastLlvmIr is null) return;
            SetBusy(true);
            try
            {
                File.WriteAllText(LlPath, _lastLlvmIr, new System.Text.UTF8Encoding(false));
                _btnEjecutar.IsVisible = true;
                SetStatus($"Archivo .ll generado en: {LlPath}  —  Presioná \"Ejecutar\" para correr el programa.", "#CBA6F7");
                LoadLlFiles();
            }
            catch (Exception ex)
            {
                SetStatus($"Error al escribir .ll: {ex.Message}", "#F38BA8");
            }
            finally { SetBusy(false); }
        }

        private async void OnEjecutarClick(object? sender, RoutedEventArgs e)
        {
            if (!File.Exists(ClangExe))
            {
                _outputBox.Text = $"[Error] No se encontró clang en:\n  {ClangExe}\n\nInstalá LLVM desde https://releases.llvm.org/";
                SetStatus("clang no encontrado.", "#F38BA8");
                return;
            }

            SetBusy(true);
            _outputBox.Text = "";
            SetStatus("Configurando entorno de compilación...", "#FAB387");

            try
            {
                string? vsPath = await Task.Run(() => FindVsInstallPath());
                string? vcvars = vsPath is not null
                    ? Path.Combine(vsPath, "VC", "Auxiliary", "Build", "vcvars64.bat")
                    : null;

                Dictionary<string, string>? vsEnv = null;
                if (vcvars != null && File.Exists(vcvars))
                    vsEnv = await Task.Run(() => CaptureVsEnvironment(vcvars));

                SetStatus("Compilando con clang...", "#FAB387");

                var clangPsi = new ProcessStartInfo
                {
                    FileName               = ClangExe,
                    Arguments              = $"\"{LlPath}\" -o \"{ExePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true,
                };
                if (vsEnv != null)
                    foreach (var kv in vsEnv)
                        clangPsi.Environment[kv.Key] = kv.Value;

                using var clangProc = new Process { StartInfo = clangPsi };
                clangProc.Start();
                string clangOut = await clangProc.StandardOutput.ReadToEndAsync();
                string clangErr = await clangProc.StandardError.ReadToEndAsync();
                await clangProc.WaitForExitAsync();

                if (clangProc.ExitCode != 0)
                {
                    string details = (clangOut + "\n" + clangErr).Trim();
                    _outputBox.Text = $"[Error de clang — código {clangProc.ExitCode}]\n{details}";
                    SetStatus("Error al compilar con clang.", "#F38BA8");
                    return;
                }

                SetStatus("Ejecutando programa...", "#FAB387");
                var (_, stdout, stderr) = await RunProcessAsync(ExePath, "");

                _outputBox.Text = string.IsNullOrWhiteSpace(stdout) && string.IsNullOrWhiteSpace(stderr)
                    ? "(sin salida)"
                    : stdout + (string.IsNullOrEmpty(stderr) ? "" : $"\n[stderr]\n{stderr}");

                SetStatus("Ejecución completada.", "#A6E3A1");
            }
            catch (Exception ex)
            {
                _outputBox.Text = $"[Excepción]\n{ex.Message}";
                SetStatus("Error durante la ejecución.", "#F38BA8");
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void OnInfoClick(object? sender, RoutedEventArgs e)
        {
            var content = new StackPanel { Spacing = 0 };

            // ── Sección GUI ───────────────────────────────────────────────────────
            content.Children.Add(SectionTitle("Flujo con la GUI"));
            content.Children.Add(MakeInfoLine("1.", "Seleccioná un test del panel izquierdo o usá Abrir.", "#A6A6A6"));
            content.Children.Add(MakeInfoLine("2.", "Presioná COMPILAR. Los errores aparecen en la grilla con línea y columna. Si no hay errores, aparece Generar .ll.", "#FAB387"));
            content.Children.Add(MakeInfoLine("3.", "Presioná Generar .ll → escribe el IR en:\n     Documents\\minigo_output.ll", "#CBA6F7"));
            content.Children.Add(MakeInfoLine("4.", "Presioná Ejecutar → clang compila el .ll y la salida aparece en el panel SALIDA.", "#A6E3A1"));
            content.Children.Add(MakeInfoLine("•", "Doble-clic en un error del DataGrid navega a esa línea en el editor.", "#6C7086"));

            content.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.Parse("#313244")), Margin = new Thickness(0, 14, 0, 14) });

            // ── Sección Terminal ──────────────────────────────────────────────────
            content.Children.Add(SectionTitle("Flujo por terminal (PowerShell)"));
            content.Children.Add(MakeInfoLine("1.", "Publicar y correr la GUI:", "#A6A6A6"));
            content.Children.Add(CodeBlock(
                "cd compilador-mini-go\n" +
                "dotnet publish -c Release -o $env:LOCALAPPDATA\\Temp\\mgo_gui\n" +
                "$env:LOCALAPPDATA\\Temp\\mgo_gui\\MiniGoCompiler.exe"));
            content.Children.Add(MakeInfoLine("2.", "Publicar y correr las pruebas automáticas:", "#A6A6A6"));
            content.Children.Add(CodeBlock(
                "cd compilador-mini-go\\MiniGoTests\n" +
                "dotnet publish -c Release -o $env:LOCALAPPDATA\\Temp\\mgo_test\n" +
                "$env:LOCALAPPDATA\\Temp\\mgo_test\\MiniGoTests.exe"));
            content.Children.Add(MakeInfoLine("3.", "Compilar un .mgo a .ll desde la terminal:", "#A6A6A6"));
            content.Children.Add(CodeBlock(
                "$env:LOCALAPPDATA\\Temp\\mgo_test\\MiniGoTests.exe \"ruta\\al\\archivo.mgo\"\n" +
                "# genera archivo.ll junto al .mgo"));
            content.Children.Add(MakeInfoLine("4.", "Compilar el .ll a .exe (desde Developer Command Prompt for VS 2022):", "#A6A6A6"));
            content.Children.Add(CodeBlock(
                "\"C:\\Program Files\\LLVM\\bin\\clang.exe\" archivo.ll -o C:\\Users\\jalpi\\Documents\\prog.exe\n" +
                "C:\\Users\\jalpi\\Documents\\prog.exe"));

            content.Children.Add(new Border { Height = 1, Background = new SolidColorBrush(Color.Parse("#313244")), Margin = new Thickness(0, 14, 0, 10) });
            content.Children.Add(new TextBlock
            {
                Text         = "Nota: en esta máquina, los .exe deben generarse en Documents\\ o AppData\\\n(Device Guard bloquea el Escritorio y Temp raíz).",
                Foreground   = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize     = 11,
                FontStyle    = FontStyle.Italic,
                TextWrapping = TextWrapping.Wrap,
                Margin       = new Thickness(0, 0, 0, 4),
            });

            var dlg = new Window
            {
                Title                 = "Mini-GO Compiler — Instrucciones",
                Width                 = 600,
                Height                = 660,
                Background            = new SolidColorBrush(Color.Parse("#1E1E2E")),
                CanResize             = true,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content               = new ScrollViewer
                {
                    Padding = new Thickness(28, 22, 28, 22),
                    VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    Content = content
                }
            };

            dlg.ShowDialog(this);
        }

        private void OnErrorRowDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (_errorGrid.SelectedItem is not DiagnosticRow row) return;
            string text = _editor.Text ?? "";
            if (string.IsNullOrEmpty(text)) return;

            int charIndex = GetCharIndexForLine(text, row.Linea);
            if (charIndex >= 0)
            {
                _editor.Focus();
                _editor.CaretIndex     = charIndex;
                int lineEnd            = text.IndexOf('\n', charIndex);
                _editor.SelectionStart = charIndex;
                _editor.SelectionEnd   = lineEnd >= 0 ? lineEnd : text.Length;
            }
        }

        // ── Helpers de UI ─────────────────────────────────────────────────────────

        private static TextBlock SectionTitle(string text) => new TextBlock
        {
            Text       = text,
            Foreground = new SolidColorBrush(Color.Parse("#CDD6F4")),
            FontSize   = 14,
            FontWeight = FontWeight.Bold,
            Margin     = new Thickness(0, 0, 0, 10),
        };

        private static Grid MakeInfoLine(string step, string text, string hexColor) =>
            new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("26,*"),
                Margin            = new Thickness(0, 0, 0, 6),
                Children =
                {
                    new TextBlock
                    {
                        Text       = step,
                        Foreground = new SolidColorBrush(Color.Parse(hexColor)),
                        FontSize   = 13,
                        FontWeight = FontWeight.Bold,
                        [Grid.ColumnProperty] = 0,
                    },
                    new TextBlock
                    {
                        Text         = text,
                        Foreground   = new SolidColorBrush(Color.Parse(hexColor)),
                        FontSize     = 13,
                        TextWrapping = TextWrapping.Wrap,
                        [Grid.ColumnProperty] = 1,
                    }
                }
            };

        private static Border CodeBlock(string code) => new Border
        {
            Background      = new SolidColorBrush(Color.Parse("#11111B")),
            CornerRadius    = new CornerRadius(5),
            Padding         = new Thickness(12, 8),
            Margin          = new Thickness(26, 2, 0, 8),
            Child           = new TextBlock
            {
                Text         = code,
                Foreground   = new SolidColorBrush(Color.Parse("#CBA6F7")),
                FontFamily   = new FontFamily("Cascadia Code,Consolas,monospace"),
                FontSize     = 11,
                TextWrapping = TextWrapping.Wrap,
            }
        };

        private static int GetCharIndexForLine(string text, int lineNumber)
        {
            if (lineNumber <= 1) return 0;
            int current = 1;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    current++;
                    if (current == lineNumber) return i + 1;
                }
            }
            return -1;
        }

        // ── Helpers de proceso / entorno ─────────────────────────────────────────

        private static string? FindVsInstallPath()
        {
            string vswhere = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe";
            if (!File.Exists(vswhere)) return null;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = vswhere,
                    Arguments              = "-latest -property installationPath",
                    RedirectStandardOutput = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };
                using var p = Process.Start(psi)!;
                string output = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit();
                return string.IsNullOrEmpty(output) ? null : output;
            }
            catch { return null; }
        }

        private static Dictionary<string, string> CaptureVsEnvironment(string vcvarsPath)
        {
            string batPath = Path.Combine(Path.GetTempPath(), "minigo_getenv.bat");
            File.WriteAllText(batPath,
                $"@echo off\r\ncall \"{vcvarsPath}\" >nul 2>&1\r\nset\r\n",
                System.Text.Encoding.ASCII);

            var psi = new ProcessStartInfo
            {
                FileName               = "cmd.exe",
                Arguments              = $"/c \"{batPath}\"",
                RedirectStandardOutput = true,
                UseShellExecute        = false,
                CreateNoWindow         = true
            };

            var env = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var p = Process.Start(psi)!;
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int eq = line.IndexOf('=');
                    if (eq > 0) env[line[..eq]] = line[(eq + 1)..];
                }
            }
            catch { }
            return env;
        }

        private static async Task<(int ExitCode, string Stdout, string Stderr)> RunProcessAsync(
            string fileName, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName               = fileName,
                Arguments              = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true
            };
            using var proc = new Process { StartInfo = psi };
            proc.Start();
            string stdout = await proc.StandardOutput.ReadToEndAsync();
            string stderr = await proc.StandardError.ReadToEndAsync();
            await proc.WaitForExitAsync();
            return (proc.ExitCode, stdout, stderr);
        }

        private void ResetGenerarEjecutar()
        {
            _lastLlvmIr            = null;
            _btnGenerar.IsVisible  = false;
            _btnEjecutar.IsVisible = false;
            _outputBox.Text        = "";
        }

        private void ClearDiagnostics() => _diagnosticRows.Clear();

        // Bloquea / desbloquea todos los botones de acción durante operaciones.
        // Cambia la opacidad para dar feedback visual claro de que no se pueden usar.
        private void SetBusy(bool busy)
        {
            _btnAbrir.IsEnabled    = !busy;
            _btnGuardar.IsEnabled  = !busy;
            _btnCompilar.IsEnabled = !busy;
            _btnGenerar.IsEnabled  = !busy;
            _btnEjecutar.IsEnabled = !busy;
            _btnLimpiar.IsEnabled  = !busy;
            _testList.IsEnabled    = !busy;

            double btnOp  = busy ? 0.38 : 1.0;
            double listOp = busy ? 0.50 : 1.0;
            _btnAbrir.Opacity    = btnOp;
            _btnGuardar.Opacity  = btnOp;
            _btnCompilar.Opacity = btnOp;
            _btnGenerar.Opacity  = btnOp;
            _btnEjecutar.Opacity = btnOp;
            _btnLimpiar.Opacity  = btnOp;
            _testList.Opacity    = listOp;
        }

        private void SetStatus(string message, string hexColor)
        {
            _statusBar.Text       = message;
            _statusBar.Foreground = new SolidColorBrush(Color.Parse(hexColor));
        }

        private static Button BuildToolButton(string text, string hexColor) => new Button
        {
            Content         = text,
            Background      = new SolidColorBrush(Color.Parse(hexColor)),
            Foreground      = Brushes.White,
            FontSize        = 13,
            FontWeight      = FontWeight.SemiBold,
            Padding         = new Thickness(16, 8),
            CornerRadius    = new CornerRadius(5),
            BorderThickness = new Thickness(0),
            Cursor          = new Cursor(StandardCursorType.Hand),
        };

        private static string GetStarterCode() =>
            """
            package main;

            var contador int = 0;

            func suma(a int, b int) int {
                return a;
            };

            func main() int {
                var x int = 10;
                var y int = 20;
                suma(x, y);
                return 0;
            };
            """;
    }
}
