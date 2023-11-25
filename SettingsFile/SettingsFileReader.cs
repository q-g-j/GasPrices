using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SettingsFile.Models;

namespace SettingsFile
{
    public class SettingsFileReader
    {
        private readonly string? _settingsFileFullPath;

        public SettingsFileReader(string? settingsFileFullPath)
        {
            _settingsFileFullPath = settingsFileFullPath;
        }

        public Settings? Read()
        {
            Settings? settings = null;

            if (!File.Exists(_settingsFileFullPath))
            {
                return settings;
            }

            try
            {
                using var streamReader = new StreamReader(_settingsFileFullPath);
                var settingsJson = streamReader.ReadToEnd();

                settings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            }
            catch (Exception)
            {

            }

            return settings;
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
