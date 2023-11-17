using ApiClients.Models;
using HttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ApiClients
{
    public class OpenStreetMapClient : IMapClient
    {
        private readonly HttpClientRepository _httpClientRepository;

        public OpenStreetMapClient(HttpClientRepository httpClientRepository)
        {
            _httpClientRepository = httpClientRepository;
        }

        public async Task<Coords?> GetCoordsAsync(Address address)
        {
            Coords? coords = null;
            string url = $"https://nominatim.openstreetmap.org/search?q={address.GetUriData()}&format=json&polygon=1&addressdetails=1";
            string result = await _httpClientRepository.GetAsync(url);

            dynamic? coordsObject = JsonConvert.DeserializeObject(result);
            if (coordsObject?.Count == 0)
            {
                return coords;
            }
            var isValidLon = double.TryParse(coordsObject?[0]?.lon?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double lon);
            var isValidLat = double.TryParse(coordsObject?[0]?.lat?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double lat);

            if (isValidLon && isValidLat)
            {
                coords = new Coords(lon, lat);
            }

            return coords;
        }

        public async Task<Address?> GetAddressAsync(Coords coords)
        {
            Address? address;

            string url = string.Format(CultureInfo.InvariantCulture, "https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=geocodejson&addressdetails=1", coords.Latitude, coords.Longitude);
            string result = await _httpClientRepository.GetAsync(url);

            dynamic? addressObject = JsonConvert.DeserializeObject(result);
            dynamic? geoData = addressObject?.features?[0]?.properties?.geocoding;

            string? street = geoData?.street?.ToString();
            string? postalCode = geoData?.postcode?.ToString();
            string? city = geoData?.city?.ToString();
            string? country = geoData?.country?.ToString();

            if (string.IsNullOrEmpty(street) && string.IsNullOrEmpty(postalCode) &&
                string.IsNullOrEmpty(city) && string.IsNullOrEmpty(country))
            {
                address = null;
            }
            else
            {
                address = new Address(street, city, postalCode, country);
            }

            return address;
        }
    }
}
