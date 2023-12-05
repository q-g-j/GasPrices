using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SettingsHandling.Models;

namespace SettingsHandling;

public class SettingsFileWriter(string? settingsFolderFullPath, string? settingsFileFullPath)
    : ISettingsWriter
{
    public async Task WriteAsync(Settings? settings)
    {
        if (!Directory.Exists(settingsFolderFullPath))
        {
            Directory.CreateDirectory(settingsFolderFullPath!);
        }

        if (settings != null)
        {
            var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);

            await using var streamWriter = new StreamWriter(settingsFileFullPath!);
            await streamWriter.WriteAsync(settingsJson);
        }
    }
}