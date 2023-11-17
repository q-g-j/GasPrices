using ApiClients.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClients
{
    public interface IMapClient
    {
        Task<Coords?> GetCoordsAsync(Address address);
        Task<Address?> GetAddressAsync(Coords coords);
    }
}
