using System.Runtime.InteropServices.JavaScript;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Serilog;

[assembly: SupportedOSPlatform("browser")]

namespace GasPrices.Browser;

internal static class Program
{
    private static Task Main(string[] _)
    {
        JSHost.ImportAsync("local_storage", "./local_storage.js");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.BrowserConsole()
            .CreateLogger();

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}