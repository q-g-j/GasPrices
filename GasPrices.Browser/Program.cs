using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]

namespace GasPrices.Browser;

internal static class Program
{
    private static Task Main(string[] _)
    {
        JSHost.ImportAsync("local_storage", "./local_storage.js");

        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}