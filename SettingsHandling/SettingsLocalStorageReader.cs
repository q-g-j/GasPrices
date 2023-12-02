using System;
using System.Globalization;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using SettingsHandling.Models;
using BrowserInterop;

namespace SettingsHandling;

[SupportedOSPlatform("browser")]
public class SettingsLocalStorageReader : ISettingsReader
{
    public Task<Settings?> ReadAsync()
    {
        Settings? settings = null;
        
        try
        {
            double.TryParse(
                LocalStorage.Get("LastKnownLatitude") ?? string.Empty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var lastKnownLatitude);
            double.TryParse(
                LocalStorage.Get("LastKnownLongitude") ?? string.Empty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var lastKnownLongitude);

            settings = new Settings
            {
                TankerkoenigApiKey = LocalStorage.Get("TankerkoenigApiKey") ?? "",
                LastKnownStreet = LocalStorage.Get("LastKnownStreet") ?? "",
                LastKnownPostalCode = LocalStorage.Get("LastKnownPostalCode") ?? "",
                LastKnownCity = LocalStorage.Get("LastKnownCity") ?? "",
                LastKnownLatitude = lastKnownLatitude == 0.0 ? null : lastKnownLatitude,
                LastKnownLongitude = lastKnownLongitude == 0.0 ? null : lastKnownLongitude,
                LastKnownRadius = LocalStorage.Get("LastKnownRadius") ?? "5",
                GasType = LocalStorage.Get("GasType") ?? "E5",
                SortBy = LocalStorage.Get("SortBy") ?? "Price"
            };
        }
        catch (Exception)
        {
            // ignore
        }

        return Task.FromResult(settings);
    }
}