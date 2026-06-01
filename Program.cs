using Avalonia;
using MiniGoCompiler.UI;

BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

static AppBuilder BuildAvaloniaApp() =>
    AppBuilder.Configure<App>()
              .UsePlatformDetect()
              .WithInterFont()
              .LogToTrace();