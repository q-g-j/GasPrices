using System.Threading.Tasks;
using SettingsHandling.Models;

namespace SettingsHandling;

public interface ISettingsWriter
{
    public Task WriteAsync(Settings? settings);
}