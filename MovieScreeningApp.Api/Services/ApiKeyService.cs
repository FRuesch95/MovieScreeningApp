using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.Clients.StreamingAvailability;
using MovieScreeningApp.Api.Entities;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Services;

public class ApiKeyService : IApiKeyService
{
    public static readonly IReadOnlyCollection<string> KnownApiNames =
    [
        StreamingAvailabilityApiClient.ClientName
    ];

    private readonly IRepository<ApiKey> _apiKeyRepository;

    public ApiKeyService(IRepository<ApiKey> apiKeyRepository)
    {
        _apiKeyRepository = apiKeyRepository;
    }

    public async Task SetAsync(string apiName, string value, CancellationToken cancellationToken = default)
    {
        if (!KnownApiNames.Contains(apiName))
        {
            throw new ArgumentException(
                $"Unknown API name '{apiName}'. Valid names are: {string.Join(", ", KnownApiNames)}.",
                nameof(apiName));
        }

        var apiKey = await _apiKeyRepository
            .Query()
            .FirstOrDefaultAsync(x => x.ApiName == apiName, cancellationToken);

        if (apiKey is null)
        {
            await _apiKeyRepository.AddAsync(new ApiKey(apiName, value));
        }
        else
        {
            apiKey.Value = value;
            _apiKeyRepository.Update(apiKey);
        }

        await _apiKeyRepository.SaveChangesAsync();
    }
}
