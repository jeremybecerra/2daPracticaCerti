using System.Net.Http.Json;
using CitizenApi.Models;

namespace CitizenApi.Services;

public class ExternalObjectService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;

    public ExternalObjectService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
    }

    public async Task<string> GetRandomPersonalAssetAsync()
    {
        string url = configuration["ExternalApi:ObjectsUrl"] ?? "";

        var client = httpClientFactory.CreateClient();
        var objects = await client.GetFromJsonAsync<List<ExternalObject>>(url);

        if (objects == null || objects.Count == 0)
        {
            return "Unknown asset";
        }

        Random random = new Random();
        var selectedObject = objects[random.Next(objects.Count)];

        if (string.IsNullOrWhiteSpace(selectedObject.Name))
        {
            return "Unknown asset";
        }

        return selectedObject.Name;
    }
}