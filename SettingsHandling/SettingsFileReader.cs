using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SettingsHandling.Models;

namespace SettingsHandling;

public class SettingsFileReader(string? settingsFileFullPath) : ISettingsReader
{
    public async Task<Settings?> ReadAsync()
    {
        Settings? settings = null;

        if (!File.Exists(settingsFileFullPath))
        {
            return settings;
        }

        using var streamReader = new StreamReader(settingsFileFullPath!);
        var settingsJson = await streamReader.ReadToEndAsync();

        settings = JsonConvert.DeserializeObject<Settings>(settingsJson);

        return settings;
    }
}