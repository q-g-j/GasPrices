using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SettingsHandling.Models;

namespace SettingsHandling;

public class SettingsFileReader : ISettingsReader
{
    private readonly string? _settingsFileFullPath;

    public SettingsFileReader(string? settingsFileFullPath)
    {
        _settingsFileFullPath = settingsFileFullPath;
    }


    public async Task<Settings?> ReadAsync()
    {
        Settings? settings = null;

        if (!File.Exists(_settingsFileFullPath))
        {
            return settings;
        }

        using var streamReader = new StreamReader(_settingsFileFullPath!);
        var settingsJson = await streamReader.ReadToEndAsync();

        settings = JsonConvert.DeserializeObject<Settings>(settingsJson);

        return settings;
    }
}