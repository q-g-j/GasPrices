using ApiClients.Models;
using HttpClient;
using Newtonsoft.Json;
using System.Globalization;
using System.Threading.Tasks;
using ApiClients;


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
        var url =
            $"https://nominatim.openstreetmap.org/search?q={address.GetUriData()}&format=json&polygon=1&addressdetails=1";

        var result = await _httpClientRepository.GetAsync(url);

        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        dynamic? coordsObject = JsonConvert.DeserializeObject(result);
        if (coordsObject?.Count == 0)
        {
            return null;
        }

        var isValidLon = double.TryParse(coordsObject?[0]?.lon?.ToString(), NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out double lon);
        var isValidLat = double.TryParse(coordsObject?[0]?.lat?.ToString(), NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out double lat);

        if (isValidLon && isValidLat)
        {
            coords = new Coords(lat, lon);
        }

        return coords;
    }

    public async Task<Address?> GetAddressAsync(Coords coords)
    {
        var url = string.Format(CultureInfo.InvariantCulture,
            "https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=geocodejson&addressdetails=1",
            coords.Latitude, coords.Longitude);

        var result = await _httpClientRepository.GetAsync(url);

        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        dynamic? addressObject = JsonConvert.DeserializeObject(result);
        var geoData = addressObject?.features?[0]?.properties?.geocoding;

        string? street = geoData?.street?.ToString();
        string? houseNumber = geoData?.housenumber?.ToString();
        string? postalCode = geoData?.postcode?.ToString();
        string? city = geoData?.city?.ToString();
        string? country = geoData?.country?.ToString();

        if (!string.IsNullOrEmpty(street) && !string.IsNullOrEmpty(houseNumber))
        {
            street += " " + houseNumber;
        }

        return new Address(street, city, postalCode, country);
    }
}