using System;
using System.IO;

namespace GasPrices.Store;

public class GlobalsStore
{
    public GlobalsStore(string? settingsFolderName, string? settingsFileName)
    {
        ApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        SettingsFolderFullPath = Path.Combine(ApplicationDataFolder, settingsFolderName!);
        SettingsFileFullPath = Path.Combine(SettingsFolderFullPath, settingsFileName!);
    }

    public string? ApplicationDataFolder { get; set; }
    public string? SettingsFolderFullPath { get; set; }
    public string? SettingsFileFullPath { get; set; }
}