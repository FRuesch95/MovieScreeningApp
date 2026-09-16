using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.Data;
using MovieScreeningApp.Api.DTOs;
using MovieScreeningApp.Api.Entities;
using MovieScreeningApp.Api.Services;

namespace MovieScreeningApp.Api.Tests.Services;

public class FavoriteServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly FavoriteService _service;

    public FavoriteServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options, new EphemeralDataProtectionProvider());
        _service = new FavoriteService(new Repository<Favorite>(_dbContext));
    }

    private static AddFavoriteRequestDto CreateRequest(string showId = "123", string title = "Dune")
    {
        return new AddFavoriteRequestDto
        {
            ShowId = showId,
            Title = title,
            Overview = "Desert planet.",
            ReleaseYear = 2021,
            Rating = 78,
            Genres = "Science Fiction, Adventure",
            PosterUrl = "https://img/w360.jpg"
        };
    }

    [Fact]
    public async Task AddAsync_StoresFavoriteAndReturnsDto()
    {
        var result = await _service.AddAsync(CreateRequest());

        Assert.True(result.Id > 0);
        Assert.Equal("123", result.ShowId);
        Assert.Equal("Dune", result.Title);
        Assert.Equal(2021, result.ReleaseYear);
        Assert.Equal(78, result.Rating);
        Assert.Equal("Science Fiction, Adventure", result.Genres);
        Assert.Equal("https://img/w360.jpg", result.PosterUrl);
        Assert.Equal(1, await _dbContext.Favorites.CountAsync());
    }

    [Fact]
    public async Task AddAsync_SameShowTwice_IsIdempotent()
    {
        var first = await _service.AddAsync(CreateRequest());
        var second = await _service.AddAsync(CreateRequest(title: "Dune (changed)"));

        Assert.Equal(first.Id, second.Id);
        Assert.Equal("Dune", second.Title);
        Assert.Equal(1, await _dbContext.Favorites.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNewestFirst()
    {
        await _service.AddAsync(CreateRequest("1", "First"));
        await _service.AddAsync(CreateRequest("2", "Second"));

        var favorites = await _service.GetAllAsync();

        Assert.Equal(["Second", "First"], favorites.Select(x => x.Title));
    }

    [Fact]
    public async Task RemoveAsync_ExistingShow_RemovesAndReturnsTrue()
    {
        await _service.AddAsync(CreateRequest());

        var removed = await _service.RemoveAsync("123");

        Assert.True(removed);
        Assert.Equal(0, await _dbContext.Favorites.CountAsync());
    }

    [Fact]
    public async Task RemoveAsync_UnknownShow_ReturnsFalse()
    {
        var removed = await _service.RemoveAsync("does-not-exist");

        Assert.False(removed);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
