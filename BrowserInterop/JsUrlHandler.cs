using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BrowserInterop;

[SupportedOSPlatform("browser")]
public static partial class JsUrlHandler
{
    [JSImport("openInNewTab", "url_handler")]
    public static partial void OpenInNewTab(string url);
}