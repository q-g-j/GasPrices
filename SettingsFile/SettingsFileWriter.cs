using Newtonsoft.Json;
using SettingsFile.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsFile.SettingsFile
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

        public async Task WriteAsync(Settings settings)
        {
            if (!Directory.Exists(_settingsFolderFullPath))
            {
                Directory.CreateDirectory(_settingsFolderFullPath);
            }

            if (settings != null)
            {
                var settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);

                try
                {
                    using var streamWriter = new StreamWriter(_settingsFileFullPath);
                    await streamWriter.WriteAsync(settingsJson);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
