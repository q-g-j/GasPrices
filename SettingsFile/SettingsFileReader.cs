using Newtonsoft.Json;
using SettingsFile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsFile.SettingsFile
{
    public class SettingsFileReader
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

            try
            {
                using var streamReader = new StreamReader(_settingsFileFullPath);
                var settingsJson = await streamReader.ReadToEndAsync();

                settings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            }
            catch (Exception)
            {

            }

            return settings;
        }
    }
}
