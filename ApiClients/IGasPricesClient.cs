using ApiClients.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiClients
{
    public interface IGasPricesClient
    {
        Task<List<Station>?> GetStationsAsync(string apiKey, Coords coords, int radius);
    }
}
