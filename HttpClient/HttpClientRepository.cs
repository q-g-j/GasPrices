using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClient
{
    public class HttpClientRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAsync(string url)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0");
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
            var responseObject = await response.Content.ReadAsStringAsync();
            return responseObject;
        }
    }
}
