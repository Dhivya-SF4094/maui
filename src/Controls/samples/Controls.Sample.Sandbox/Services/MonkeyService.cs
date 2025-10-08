using MauiClient.Models;
using System.Net.Http.Json;

namespace MauiClient.Services;

public class MonkeyService
{
    private readonly HttpClient httpClient;
    public MonkeyService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }


    public async ValueTask<IEnumerable<Monkey>?> GetMonkeys()
    {
        var response = await httpClient.GetAsync("https://www.montemagno.com/monkeys.json");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<IEnumerable<Monkey>>();
        }

        return Enumerable.Empty<Monkey>();
    }
}
