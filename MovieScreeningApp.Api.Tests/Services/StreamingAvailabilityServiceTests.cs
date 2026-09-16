using Microsoft.Extensions.Caching.Memory;
using MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;
using MovieScreeningApp.Api.Interfaces;
using MovieScreeningApp.Api.Services;
using NSubstitute;

namespace MovieScreeningApp.Api.Tests.Services;

public class StreamingAvailabilityServiceTests
{
    private readonly IStreamingAvailabilityApiClient _apiClient = Substitute.For<IStreamingAvailabilityApiClient>();
    private readonly StreamingAvailabilityService _service;

    public StreamingAvailabilityServiceTests()
    {
        _service = new StreamingAvailabilityService(_apiClient, new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task GetGenresAsync_CachesResult_AndCallsApiOnlyOnce()
    {
        _apiClient.GetGenresAsync(Arg.Any<CancellationToken>())
            .Returns([new GenreDto { Id = "action", Name = "Action" }]);

        var first = await _service.GetGenresAsync();
        var second = await _service.GetGenresAsync();

        Assert.Single(first);
        Assert.Same(first, second);
        await _apiClient.Received(1).GetGenresAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchMoviesAsync_PassesRequestThroughToClient()
    {
        var request = new ShowSearchRequestDto { Title = "dune" };
        var expected = new ShowSearchResponseDto { HasMore = true };
        _apiClient.SearchMoviesAsync(request, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await _service.SearchMoviesAsync(request);

        Assert.Same(expected, result);
    }
}
