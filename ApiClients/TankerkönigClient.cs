using System;
using ApiClients.Models;
using HttpClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BrowserInterop;

namespace ApiClients;

public class TankerkönigClient(HttpClientRepository httpClientRepository) : IGasPricesClient
{
    public async Task<List<Station>?> GetStationsAsync(string apiKey, Coords coords, int radius)
    {
        var url = string.Format(CultureInfo.InvariantCulture,
            "https://creativecommons.tankerkoenig.de/json/list.php?lat={0}&lng={1}&rad={2}&type=all&apikey={3}",
            coords.Latitude, coords.Longitude, radius, apiKey);

        var result = string.Empty;

        try
        {
            if (OperatingSystem.IsBrowser())
            {
                result = await JsWebApiClient.GetAsync(url);
            }
            else
            {
                result = await httpClientRepository.GetAsync(url);
            }
        }
        catch
        {
            Console.WriteLine(result);
            throw;
        }

        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        dynamic? jsonObject = JsonConvert.DeserializeObject(result);

        if (jsonObject == null || jsonObject?.stations == null) return null;

        var stations = new List<Station>();
        var stationsObject = JsonConvert.DeserializeObject<List<dynamic>>(jsonObject?.stations.ToString());
        foreach (var station in stationsObject)
        {
            var stationModel = new Station
            {
                Name = station.name,
                Brand = station.brand,
                Distance = station.dist,
                Street = station.street,
                HouseNumber = station.houseNumber,
                City = station.place,
                IsOpen = station.isOpen,
                PostalCode = station.postCode,
                E5 = station.e5,
                E10 = station.e10,
                Diesel = station.diesel
            };

            stations.Add(stationModel);
        }

        return stations;
    }
}