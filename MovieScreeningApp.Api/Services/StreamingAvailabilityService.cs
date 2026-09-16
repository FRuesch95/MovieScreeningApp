using Microsoft.Extensions.Caching.Memory;
using MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Services;

public class StreamingAvailabilityService : IStreamingAvailabilityService
{
    private const string GenresCacheKey = "StreamingAvailability:Genres";
    private static readonly TimeSpan GenresCacheDuration = TimeSpan.FromDays(1);

    private readonly IStreamingAvailabilityApiClient _apiClient;
    private readonly IMemoryCache _cache;

    public StreamingAvailabilityService(
        IStreamingAvailabilityApiClient apiClient,
        IMemoryCache cache)
    {
        _apiClient = apiClient;
        _cache = cache;
    }

    public async Task<ShowSearchResponseDto> SearchMoviesAsync(ShowSearchRequestDto request, CancellationToken cancellationToken = default)
    {
        return await _apiClient.SearchMoviesAsync(request, cancellationToken);
    }

    public async Task<List<GenreDto>> GetGenresAsync(CancellationToken cancellationToken = default)
    {
        var genres = await _cache.GetOrCreateAsync(GenresCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = GenresCacheDuration;
            return await _apiClient.GetGenresAsync(cancellationToken);
        });

        return genres ?? [];
    }
}
