using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using SettingsHandling.Models;

namespace SettingsHandling;

[SupportedOSPlatform("browser")]
public class SettingsLocalStorageWriter : ISettingsWriter
{
    public Task WriteAsync(Settings? settings)
    {
        Interop.Interop.Clear();

        Interop.Interop.Set(nameof(settings.TankerkoenigApiKey), settings!.TankerkoenigApiKey);
        Interop.Interop.Set(nameof(settings.LastKnownStreet), settings!.LastKnownStreet);
        Interop.Interop.Set(nameof(settings.LastKnownCity), settings!.LastKnownCity);
        Interop.Interop.Set(nameof(settings.LastKnownPostalCode), settings!.LastKnownPostalCode);
        Interop.Interop.Set(nameof(settings.LastKnownLatitude), settings!.LastKnownLatitude.ToString());
        Interop.Interop.Set(nameof(settings.LastKnownLongitude), settings!.LastKnownLongitude.ToString());
        Interop.Interop.Set(nameof(settings.LastKnownRadius), settings!.LastKnownRadius!.ToString());
        Interop.Interop.Set(nameof(settings.SortBy), settings!.SortBy);
        Interop.Interop.Set(nameof(settings.GasType), settings!.GasType);

        return Task.CompletedTask;
    }
}