using ApiClients.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClients;

public interface IOpenSpritpreiseClient
{
    Task<List<Station>?> GetStationsAsync(string apiKey, Coords coords, int radius);
}