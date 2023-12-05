using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BrowserInterop;

[SupportedOSPlatform("browser")]
public static partial class WebApiClient
{
    [JSImport("getAsync", "webapi_client")]
    public static partial Task<string> GetAsync(string url);
}