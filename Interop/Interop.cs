using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Interop;

[SupportedOSPlatform("browser")]
public static partial class Interop
{
    [JSImport("set", "local_storage")]
    public static partial void Set(string key, string? value);

    [JSImport("get", "local_storage")]
    public static partial string? Get(string key);

    [JSImport("clear", "local_storage")]
    public static partial void Clear();
}