using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using SettingsHandling.Models;

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
                Interop.Interop.Get("LastKnownLatitude") ?? string.Empty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var lastKnownLatitude);
            double.TryParse(
                Interop.Interop.Get("LastKnownLongitude") ?? string.Empty, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var lastKnownLongitude);

            settings = new Settings
            {
                TankerkoenigApiKey = Interop.Interop.Get("TankerkoenigApiKey") ?? "",
                LastKnownStreet = Interop.Interop.Get("LastKnownStreet") ?? "",
                LastKnownPostalCode = Interop.Interop.Get("LastKnownPostalCode") ?? "",
                LastKnownCity = Interop.Interop.Get("LastKnownCity") ?? "",
                LastKnownLatitude = lastKnownLatitude == 0.0 ? null : lastKnownLatitude,
                LastKnownLongitude = lastKnownLongitude == 0.0 ? null : lastKnownLongitude,
                LastKnownRadius = Interop.Interop.Get("LastKnownRadius") ?? "5",
                GasType = Interop.Interop.Get("GasType") ?? "E5",
                SortBy = Interop.Interop.Get("SortBy") ?? "Price"
            };
        }
        catch (Exception)
        {
            // ignore
        }

        return Task.FromResult(settings);
    }
}