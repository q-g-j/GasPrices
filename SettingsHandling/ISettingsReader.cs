using System.Threading.Tasks;
using SettingsHandling.Models;

namespace SettingsHandling;

public interface ISettingsReader
{
    public Task<Settings?> ReadAsync();
}