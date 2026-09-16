using MovieScreeningApp.Api.DTOs;

namespace MovieScreeningApp.Api.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FavoriteDto> AddAsync(AddFavoriteRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(string showId, CancellationToken cancellationToken = default);
}
