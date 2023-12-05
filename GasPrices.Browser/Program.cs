using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Serilog;

[assembly: SupportedOSPlatform("browser")]

namespace GasPrices.Browser;

internal static class Program
{
    private static async Task Main(string[] _)
    {
        await JSHost.ImportAsync("local_storage", "/local_storage.js");
        await JSHost.ImportAsync("webapi_client", "/webapi_client.js");
        await JSHost.ImportAsync("url_handler", "/url_handler.js");

        await BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        var appBuilder = AppBuilder.Configure<App>();

        return appBuilder;
    }
}
