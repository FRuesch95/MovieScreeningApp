using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.Entities;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Providers;

public class ApiKeyProvider : IApiKeyProvider
{

    private readonly IRepository<ApiKey> _apiKeyRepository;

    public ApiKeyProvider(IRepository<ApiKey>  apiKeyRepository)
    {
       _apiKeyRepository = apiKeyRepository;
    }
    
    
    public async Task<string> GetKeyAsync(string apiName)
    {
        var apiKey = await _apiKeyRepository
            .ReadonlyQuery()
            .Where(x => x.ApiName == apiName)
            .Select(x => x.Value)
            .FirstOrDefaultAsync();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException($" API key for {apiName} is missing.");
        }

        return apiKey;
    }
    
}