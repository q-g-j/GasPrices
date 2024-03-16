using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using OpenSpritpreise;

[assembly: SupportedOSPlatform("browser")]

internal sealed partial class Program
{
    private static async Task Main(string[] _)
    {
        await JSHost.ImportAsync("local_storage", "/local_storage.js");
        await JSHost.ImportAsync("webapi_client", "/webapi_client.js");
        await JSHost.ImportAsync("url_handler", "/url_handler.js");

        await BuildAvaloniaApp()
            .StartBrowserAppAsync("out");
    }
    public static AppBuilder BuildAvaloniaApp()
           => AppBuilder.Configure<App>();
}