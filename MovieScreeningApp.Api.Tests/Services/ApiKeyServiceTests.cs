using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.Clients.StreamingAvailability;
using MovieScreeningApp.Api.Data;
using MovieScreeningApp.Api.Entities;
using MovieScreeningApp.Api.Providers;
using MovieScreeningApp.Api.Services;

namespace MovieScreeningApp.Api.Tests.Services;

public class ApiKeyServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly ApiKeyService _service;
    private readonly ApiKeyProvider _provider;

    public ApiKeyServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options, new EphemeralDataProtectionProvider());
        _service = new ApiKeyService(new Repository<ApiKey>(_dbContext));
        _provider = new ApiKeyProvider(new Repository<ApiKey>(_dbContext));
    }

    private const string ApiName = StreamingAvailabilityApiClient.ClientName;

    [Fact]
    public async Task SetAsync_NewApiName_CreatesKeyReadableByProvider()
    {
        await _service.SetAsync(ApiName, "secret-1");

        Assert.Equal("secret-1", await _provider.GetKeyAsync(ApiName));
        Assert.Equal(1, await _dbContext.ApiKeys.CountAsync());
    }

    [Fact]
    public async Task SetAsync_ExistingApiName_ReplacesValue()
    {
        await _service.SetAsync(ApiName, "secret-1");
        await _service.SetAsync(ApiName, "secret-2");

        Assert.Equal("secret-2", await _provider.GetKeyAsync(ApiName));
        Assert.Equal(1, await _dbContext.ApiKeys.CountAsync());
    }

    [Theory]
    [InlineData("UnknownApi")]
    [InlineData("streamingavailabilityapi")]
    public async Task SetAsync_UnknownApiName_ThrowsAndStoresNothing(string apiName)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.SetAsync(apiName, "secret"));

        Assert.Contains(apiName, ex.Message);
        Assert.Contains(ApiName, ex.Message);
        Assert.Equal(0, await _dbContext.ApiKeys.CountAsync());
    }

    [Fact]
    public async Task GetKeyAsync_UnknownApiName_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _provider.GetKeyAsync("Unknown"));
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
