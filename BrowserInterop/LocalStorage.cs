using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace BrowserInterop;

[SupportedOSPlatform("browser")]
public static partial class LocalStorage
{
    [JSImport("set", "local_storage")]
    public static partial void Set(string key, string? value);

    [JSImport("get", "local_storage")]
    public static partial string? Get(string key);

    [JSImport("clear", "local_storage")]
    public static partial void Clear();

    [JSImport("remove", "local_storage")]
    public static partial void Remove(string key);
}