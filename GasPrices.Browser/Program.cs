using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]

namespace GasPrices.Browser;

internal static class Program
{
    private static async Task Main(string[] _)
    {
        await JSHost.ImportAsync("local_storage", "/local_storage.js");

        await BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}