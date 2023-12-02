using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SettingsHandling.Models;

namespace SettingsHandling;

public class SettingsFileWriter : ISettingsWriter
{
    private readonly string? _settingsFolderFullPath;
    private readonly string? _settingsFileFullPath;

    public SettingsFileWriter(string? settingsFolderFullPath, string? settingsFileFullPath)
    {
        _settingsFolderFullPath = settingsFolderFullPath;
        _settingsFileFullPath = settingsFileFullPath;
    }

    public async Task WriteAsync(Settings? settings)
    {
        if (!Directory.Exists(_settingsFolderFullPath))
        {
            Directory.CreateDirectory(_settingsFolderFullPath!);
        }

        if (settings != null)
        {
            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);

            await using var streamWriter = new StreamWriter(_settingsFileFullPath!);
            await streamWriter.WriteAsync(settingsJson);
        }
    }
}