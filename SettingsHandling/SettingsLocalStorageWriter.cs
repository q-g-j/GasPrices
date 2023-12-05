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
        
        LocalStorage.Set(nameof(settings.TankerkoenigApiKey), settings.TankerkoenigApiKey);
        LocalStorage.Set(nameof(settings.LastKnownStreet), settings.LastKnownStreet);
        LocalStorage.Set(nameof(settings.LastKnownCity), settings.LastKnownCity);
        LocalStorage.Set(nameof(settings.LastKnownPostalCode), settings.LastKnownPostalCode);
        LocalStorage.Set(nameof(settings.LastKnownLatitude), settings.LastKnownLatitude.ToString());
        LocalStorage.Set(nameof(settings.LastKnownLongitude), settings.LastKnownLongitude.ToString());
        LocalStorage.Set(nameof(settings.LastKnownRadius), settings.LastKnownRadius);
        LocalStorage.Set(nameof(settings.SortBy), settings.SortBy);
        LocalStorage.Set(nameof(settings.GasType), settings.GasType);

        return Task.CompletedTask;
    }
}