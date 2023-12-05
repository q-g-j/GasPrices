using System.Runtime.Versioning;
using System.Threading.Tasks;
using SettingsHandling.Models;
using BrowserInterop;

namespace SettingsHandling;

[SupportedOSPlatform("browser")]
public class SettingsLocalStorageWriter : ISettingsWriter
{
    public Task WriteAsync(Settings? settings)
    {
        if (settings == null) return Task.CompletedTask;
        
        JsLocalStorage.Set(nameof(settings.TankerkoenigApiKey), settings.TankerkoenigApiKey);
        JsLocalStorage.Set(nameof(settings.LastKnownStreet), settings.LastKnownStreet);
        JsLocalStorage.Set(nameof(settings.LastKnownCity), settings.LastKnownCity);
        JsLocalStorage.Set(nameof(settings.LastKnownPostalCode), settings.LastKnownPostalCode);
        JsLocalStorage.Set(nameof(settings.LastKnownLatitude), settings.LastKnownLatitude.ToString());
        JsLocalStorage.Set(nameof(settings.LastKnownLongitude), settings.LastKnownLongitude.ToString());
        JsLocalStorage.Set(nameof(settings.LastKnownRadius), settings.LastKnownRadius);
        JsLocalStorage.Set(nameof(settings.SortBy), settings.SortBy);
        JsLocalStorage.Set(nameof(settings.GasType), settings.GasType);

        return Task.CompletedTask;
    }
}