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
                JsLocalStorage.Get("LastKnownLatitude") ?? string.Empty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var lastKnownLatitude);
            double.TryParse(
                JsLocalStorage.Get("LastKnownLongitude") ?? string.Empty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var lastKnownLongitude);

            settings = new Settings
            {
                TankerkoenigApiKey = JsLocalStorage.Get("TankerkoenigApiKey") ?? "",
                LastKnownStreet = JsLocalStorage.Get("LastKnownStreet") ?? "",
                LastKnownPostalCode = JsLocalStorage.Get("LastKnownPostalCode") ?? "",
                LastKnownCity = JsLocalStorage.Get("LastKnownCity") ?? "",
                LastKnownLatitude = lastKnownLatitude == 0.0 ? null : lastKnownLatitude,
                LastKnownLongitude = lastKnownLongitude == 0.0 ? null : lastKnownLongitude,
                LastKnownRadius = JsLocalStorage.Get("LastKnownRadius") ?? "5",
                GasType = JsLocalStorage.Get("GasType") ?? "E5",
                SortBy = JsLocalStorage.Get("SortBy") ?? "Price"
            };
        }
        catch (Exception)
        {
            // ignore
        }

        return Task.FromResult(settings);
    }
}