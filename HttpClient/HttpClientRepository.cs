using HttpClient.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClient;

public class HttpClientRepository(IHttpClientFactory httpClientFactory)
{
    public async Task<string> GetAsync(string url)
    {
        
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0");
        client.Timeout = TimeSpan.FromMilliseconds(8000);

        HttpResponseMessage? response = null;
        Console.WriteLine(response);

        try
        {
            response = await client.GetAsync(url);
        }
        catch (Exception ex)
        {
            throw new HttpClientException(ex.Message);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new BadStatuscodeException(response.StatusCode, response.ReasonPhrase!);
        }

        return await response.Content.ReadAsStringAsync();
    }
}