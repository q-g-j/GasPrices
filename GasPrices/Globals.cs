using ApiClients.Models;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasPrices
{
    public class Globals
    {
        public Globals(string? settingsFolderName, string? settingsFileName)
        {
            ApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            SettingsFolderFullPath = Path.Combine(ApplicationDataFolder, settingsFolderName!);
            SettingsFileFullPath = Path.Combine(SettingsFolderFullPath, settingsFileName!);
        }

        public string? ApplicationDataFolder { get; set; }
        public string? SettingsFolderFullPath { get; set; }
        public string? SettingsFileFullPath { get; set; }
    }
}
