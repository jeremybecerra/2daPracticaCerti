using System.Net.Http.Json;
using CitizenApi.Models;

namespace CitizenApi.Services;

public class ExternalObjectService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;
    private readonly ILogger<ExternalObjectService> logger;

    public ExternalObjectService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ExternalObjectService> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task<string> GetRandomPersonalAssetAsync()
    {
        try
        {
            string baseUrl = configuration["ExternalServices:ObjectsApi:BaseUrl"] ?? "";
            string url = $"{baseUrl}objects";

            logger.LogInformation("Calling external API: {Url}", url);

            var client = httpClientFactory.CreateClient();
            var objects = await client.GetFromJsonAsync<List<ExternalObject>>(url);

            if (objects == null || objects.Count == 0)
            {
                logger.LogWarning("External API returned no objects");
                return "Unknown asset";
            }

            Random random = new Random();
            var selectedObject = objects[random.Next(objects.Count)];

            if (string.IsNullOrWhiteSpace(selectedObject.Name))
            {
                logger.LogWarning("Selected external object has no name");
                return "Unknown asset";
            }

            logger.LogInformation("Selected personal asset: {Asset}", selectedObject.Name);

            return selectedObject.Name;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calling external API");
            return "Unknown asset";
        }
    }
}