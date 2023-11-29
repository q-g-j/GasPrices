using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SettingsFile.Models;

namespace SettingsFile
{
    public class SettingsFileWriter
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

        public void Write(Settings? settings)
        {
            if (!Directory.Exists(_settingsFolderFullPath))
            {
                Directory.CreateDirectory(_settingsFolderFullPath!);
            }

            if (settings != null)
            {
                var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);

                using var streamWriter = new StreamWriter(_settingsFileFullPath!);
                streamWriter.Write(settingsJson);
            }
        }
    }
}