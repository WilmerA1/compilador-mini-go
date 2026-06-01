using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using MiniGoCompiler;

namespace MiniGoCompiler.UI
{
    public sealed class DiagnosticRow
    {
        public int    Linea     { get; init; }
        public int    Columna   { get; init; }
        public string Tipo      { get; init; } = "";
        public string Mensaje   { get; init; } = "";
    }

    public sealed class MainWindow : Window
    {
        private readonly TextBox   _editor;
        private readonly DataGrid  _errorGrid;
        private readonly TextBlock _statusBar;
        private readonly Button    _btnAbrir;
        private readonly Button    _btnGuardar;
        private readonly Button    _btnCompilar;

        private readonly ObservableCollection<DiagnosticRow> _diagnosticRows = new();

        private string? _currentFilePath;

        private readonly TextBlock _fileInfoLabel;

        public MainWindow()
        {            
            Title           = "Mini-GO Compiler";
            Width           = 1100;
            Height          = 720;
            MinWidth        = 700;
            MinHeight       = 480;
            Background      = new SolidColorBrush(Color.Parse("#1E1E2E")); 

            _btnAbrir    = BuildToolButton("Abrir",    "#3A86FF");
            _btnGuardar  = BuildToolButton("Guardar",  "#6A994E");
            _btnCompilar = BuildToolButton("COMPILAR", "#FF006E");

            _btnAbrir.Click    += OnAbrirClick;
            _btnGuardar.Click  += OnGuardarClick;
            _btnCompilar.Click += OnCompilarClick;

            var toolbar = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing     = 8,
                Margin      = new Thickness(12, 8, 12, 8),
                Children    = { _btnAbrir, _btnGuardar, _btnCompilar }
            };

            _fileInfoLabel = new TextBlock
            {
                Text       = "Sin archivo abierto",
                Foreground = new SolidColorBrush(Color.Parse("#A6A6A6")),
                FontSize   = 12,
                Margin     = new Thickness(14, 0, 14, 8),
                FontFamily = new FontFamily("Cascadia Code,Consolas,monospace")
            };

            var toolbarPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children    = { toolbar, _fileInfoLabel }
            };

            _editor = new TextBox
            {
                AcceptsReturn    = true,
                AcceptsTab       = true,
                FontFamily       = new FontFamily("Cascadia Code,Consolas,Courier New,monospace"),
                FontSize         = 14,
                Foreground       = new SolidColorBrush(Color.Parse("#CDD6F4")),
                Background       = new SolidColorBrush(Color.Parse("#181825")),
                CaretBrush       = new SolidColorBrush(Color.Parse("#F38BA8")),
                SelectionBrush   = new SolidColorBrush(Color.Parse("#45475A")),
                Padding          = new Thickness(16),
                BorderThickness  = new Thickness(0),
                TextWrapping     = TextWrapping.NoWrap,
                Watermark        = "// Escribe tu código Mini-GO aquí...",
                Text             = GetStarterCode(),
                VerticalContentAlignment = VerticalAlignment.Top,
            };

            var editorBorder = new Border
            {
                BorderBrush     = new SolidColorBrush(Color.Parse("#313244")),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Margin          = new Thickness(12, 0, 12, 6),
                Child           = new ScrollViewer
                {
                    HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility   = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    Content = _editor
                }
            };
            
            var errorLabel = new TextBlock
            {
                Text       = "DIAGNÓSTICOS",
                Foreground = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize   = 11,
                FontWeight = FontWeight.Bold,
                Margin     = new Thickness(14, 4, 0, 4),
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

            _errorGrid.Columns.Add(new DataGridTextColumn
            {
                Header  = "Línea",
                Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Linea)),
                Width   = new DataGridLength(70)
            });
            _errorGrid.Columns.Add(new DataGridTextColumn
            {
                Header  = "Columna",
                Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Columna)),
                Width   = new DataGridLength(80)
            });
            _errorGrid.Columns.Add(new DataGridTextColumn
            {
                Header  = "Tipo",
                Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Tipo)),
                Width   = new DataGridLength(110)
            });
            _errorGrid.Columns.Add(new DataGridTextColumn
            {
                Header  = "Mensaje",
                Binding = new Avalonia.Data.Binding(nameof(DiagnosticRow.Mensaje)),
                Width   = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            
            _errorGrid.DoubleTapped += OnErrorRowDoubleTapped;

            var errorPanel = new Border
            {
                BorderBrush     = new SolidColorBrush(Color.Parse("#313244")),
                BorderThickness = new Thickness(1),
                CornerRadius    = new CornerRadius(6),
                Margin          = new Thickness(12, 0, 12, 6),
                Child           = new ScrollViewer
                {
                    VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                    Content = _errorGrid
                }
            };
            
            _statusBar = new TextBlock
            {
                Text       = "Listo.",
                Foreground = new SolidColorBrush(Color.Parse("#6C7086")),
                FontSize   = 12,
                Margin     = new Thickness(14, 0, 14, 8),
                FontFamily = new FontFamily("Cascadia Code,Consolas,monospace")
            };    

            var grid = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,*,Auto,180,Auto"),
                
            };

            Grid.SetRow(toolbarPanel,    0);
            Grid.SetRow(editorBorder, 1);
            Grid.SetRow(errorLabel,  2);
            Grid.SetRow(errorPanel,  3);
            Grid.SetRow(_statusBar,  4);            
            grid.Children.Add(toolbarPanel);
            grid.Children.Add(editorBorder);
            grid.Children.Add(errorLabel);
            grid.Children.Add(errorPanel);
            grid.Children.Add(_statusBar);            

            Content = grid;
        }

        private async void OnAbrirClick(object? sender, RoutedEventArgs e)
        {
            string testFilesPath = Path.Combine(
                AppContext.BaseDirectory,           
                "..", "..", "..", "TestFiles"       
            );

            testFilesPath = Path.GetFullPath(testFilesPath); 

            Avalonia.Platform.Storage.IStorageFolder? startFolder = null;
                if (Directory.Exists(testFilesPath))
                {
                    startFolder = await StorageProvider.TryGetFolderFromPathAsync(new Uri($"file:///{testFilesPath}"));
                }

                var options = new Avalonia.Platform.Storage.FilePickerOpenOptions
                {
                    Title                = "Abrir archivo Mini-GO",
                    AllowMultiple        = false,
                    SuggestedStartLocation = startFolder, 
                    FileTypeFilter = new[]
                    {
                        new Avalonia.Platform.Storage.FilePickerFileType("Mini-GO")
                        {
                            Patterns = new[] { "*.mgo", "*.go" }
                        },
                        new Avalonia.Platform.Storage.FilePickerFileType("Todos")
                        {
                            Patterns = new[] { "*" }
                        }
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
                }
            }

        private async void OnGuardarClick(object? sender, RoutedEventArgs e)
        {
            if (_currentFilePath is null)
            {
                var options = new Avalonia.Platform.Storage.FilePickerSaveOptions
                {
                    Title                = "Guardar archivo Mini-GO",
                    DefaultExtension     = "mgo",
                    FileTypeChoices = new[]
                    {
                        new Avalonia.Platform.Storage.FilePickerFileType("Mini-GO")
                        {
                            Patterns = new[] { "*.mgo", "*.go" }
                        }
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

        private void OnCompilarClick(object? sender, RoutedEventArgs e)
        {
            string sourceCode = _editor.Text ?? "";

            ClearDiagnostics();
            SetStatus("Compilando...", "#FAB387");

            var pipeline = new CompilerPipeline();
            var result   = pipeline.Run(sourceCode);            

            if (!DiagnosticCollector.Instance.HasErrors)
            {
                SetStatus("Compilación exitosa — 0 errores.", "#A6E3A1");
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

            File.AppendAllText("debug_output.txt", $"\nDiagnosticRows después: {_diagnosticRows.Count}");

            int total  = DiagnosticCollector.Instance.TotalCount;
            int shown  = _diagnosticRows.Count;
            string extra = total > shown ? $" (+{total - shown} más)" : "";
            SetStatus($"{total} error(es) encontrado(s){extra}.", "#F38BA8");
        }

        private void OnErrorRowDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (_errorGrid.SelectedItem is not DiagnosticRow row) return;

            string text = _editor.Text ?? "";
            if (string.IsNullOrEmpty(text)) return;

            int targetLine = row.Linea;
            int charIndex  = GetCharIndexForLine(text, targetLine);

            if (charIndex >= 0)
            {
                _editor.Focus();
                _editor.CaretIndex = charIndex;
                
                int lineEnd = text.IndexOf('\n', charIndex);
                _editor.SelectionStart = charIndex;
                _editor.SelectionEnd   = lineEnd >= 0 ? lineEnd : text.Length;
            }
        }

        private static int GetCharIndexForLine(string text, int lineNumber)
        {
            if (lineNumber <= 1) return 0;

            int currentLine = 1;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    currentLine++;
                    if (currentLine == lineNumber)
                        return i + 1;
                }
            }
            return -1;
        }

        private void ClearDiagnostics() => _diagnosticRows.Clear();

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