using ApiClients.Models;
using HttpClient;
using HttpClient.Exceptions;
using Newtonsoft.Json;
using System;
using System.Globalization;
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

            string? result;

            try
            {
                result = await _httpClientRepository.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (result == null)
            {
                return coords;
            }

            dynamic? coordsObject = JsonConvert.DeserializeObject(result);
            if (coordsObject?.Count == 0)
            {
                return coords;
            }
            var isValidLon = double.TryParse(coordsObject?[0]?.lon?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double lon);
            var isValidLat = double.TryParse(coordsObject?[0]?.lat?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double lat);

            if (isValidLon && isValidLat)
            {
                coords = new Coords(lat, lon);
            }

            return coords;
        }

        public async Task<Address?> GetAddressAsync(Coords coords)
        {
            Address? address = null;

            string url = string.Format(CultureInfo.InvariantCulture, "https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=geocodejson&addressdetails=1", coords.Latitude, coords.Longitude);

            string? result;
            try
            {
                result = await _httpClientRepository.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (result == null)
            {
                return address;
            }

            dynamic? addressObject = JsonConvert.DeserializeObject(result);
            dynamic? geoData = addressObject?.features?[0]?.properties?.geocoding;

            string? street = geoData?.street?.ToString();
            string? postalCode = geoData?.postcode?.ToString();
            string? city = geoData?.city?.ToString();
            string? country = geoData?.country?.ToString();

            if (string.IsNullOrEmpty(street) && string.IsNullOrEmpty(postalCode) &&
                string.IsNullOrEmpty(city))
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
