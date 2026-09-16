using MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;

namespace MovieScreeningApp.Api.Interfaces;

public interface IStreamingAvailabilityApiClient
{
    Task<ShowSearchResponseDto> SearchMoviesAsync(ShowSearchRequestDto request, CancellationToken cancellationToken = default);
    Task<List<GenreDto>> GetGenresAsync(CancellationToken cancellationToken = default);
}
