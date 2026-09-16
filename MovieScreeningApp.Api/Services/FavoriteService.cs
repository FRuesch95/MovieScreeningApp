using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.DTOs;
using MovieScreeningApp.Api.Entities;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IRepository<Favorite> _favoriteRepository;

    public FavoriteService(IRepository<Favorite> favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<List<FavoriteDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var favorites = await _favoriteRepository
            .ReadonlyQuery()
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        return favorites.Select(ToDto).ToList();
    }

    public async Task<FavoriteDto> AddAsync(AddFavoriteRequestDto request, CancellationToken cancellationToken = default)
    {
        var existing = await _favoriteRepository
            .ReadonlyQuery()
            .FirstOrDefaultAsync(x => x.ShowId == request.ShowId, cancellationToken);

        if (existing is not null)
        {
            return ToDto(existing);
        }

        var favorite = new Favorite(
            request.ShowId,
            request.Title,
            request.Overview,
            request.ReleaseYear,
            request.Rating,
            request.Genres,
            request.PosterUrl);

        await _favoriteRepository.AddAsync(favorite);
        await _favoriteRepository.SaveChangesAsync();

        return ToDto(favorite);
    }

    public async Task<bool> RemoveAsync(string showId, CancellationToken cancellationToken = default)
    {
        var favorite = await _favoriteRepository
            .Query()
            .FirstOrDefaultAsync(x => x.ShowId == showId, cancellationToken);

        if (favorite is null)
        {
            return false;
        }

        _favoriteRepository.Delete(favorite);
        await _favoriteRepository.SaveChangesAsync();

        return true;
    }

    private static FavoriteDto ToDto(Favorite favorite)
    {
        return new FavoriteDto
        {
            Id = favorite.Id,
            ShowId = favorite.ShowId,
            Title = favorite.Title,
            Overview = favorite.Overview,
            ReleaseYear = favorite.ReleaseYear,
            Rating = favorite.Rating,
            Genres = favorite.Genres,
            PosterUrl = favorite.PosterUrl
        };
    }
}
