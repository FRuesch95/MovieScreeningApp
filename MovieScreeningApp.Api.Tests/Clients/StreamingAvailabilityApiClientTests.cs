using System.Net;
using System.Web;
using MovieScreeningApp.Api.Clients.StreamingAvailability;
using MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;
using MovieScreeningApp.Api.Exceptions;
using MovieScreeningApp.Api.Interfaces;
using MovieScreeningApp.Api.Tests.Helpers;
using NSubstitute;

namespace MovieScreeningApp.Api.Tests.Clients;

public class StreamingAvailabilityApiClientTests
{
    private const string ApiKey = "test-api-key";
    private const string BaseUrl = "https://api.example.com/v4/";

    private static StreamingAvailabilityApiClient CreateClient(FakeHttpMessageHandler handler)
    {
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory
            .CreateClient(StreamingAvailabilityApiClient.ClientName)
            .Returns(new HttpClient(handler) { BaseAddress = new Uri(BaseUrl) });

        var apiKeyProvider = Substitute.For<IApiKeyProvider>();
        apiKeyProvider.GetKeyAsync(StreamingAvailabilityApiClient.ClientName).Returns(ApiKey);

        return new StreamingAvailabilityApiClient(httpClientFactory, apiKeyProvider);
    }

    [Fact]
    public async Task SearchMoviesAsync_BuildsExpectedQueryAndHeader()
    {
        var handler = FakeHttpMessageHandler.Json(HttpStatusCode.OK, """{ "shows": [], "hasMore": false, "nextCursor": null }""");
        var client = CreateClient(handler);

        await client.SearchMoviesAsync(new ShowSearchRequestDto
        {
            Title = "dune",
            Year = 2021,
            Genre = "scifi",
            OrderBy = "rating",
            OrderDirection = "desc",
            Cursor = "abc:1"
        });

        var request = handler.LastRequest!;
        var query = HttpUtility.ParseQueryString(request.RequestUri!.Query);

        Assert.Equal("/v4/shows/search/filters", request.RequestUri.AbsolutePath);
        Assert.Equal(ApiKey, request.Headers.GetValues("X-API-Key").Single());
        Assert.Equal("de", query["country"]);
        Assert.Equal("movie", query["show_type"]);
        Assert.Equal("en", query["output_language"]);
        Assert.Equal("dune", query["keyword"]);
        Assert.Equal("2021", query["year_min"]);
        Assert.Equal("2021", query["year_max"]);
        Assert.Equal("scifi", query["genres"]);
        Assert.Equal("rating", query["order_by"]);
        Assert.Equal("desc", query["order_direction"]);
        Assert.Equal("abc:1", query["cursor"]);
    }

    [Fact]
    public async Task SearchMoviesAsync_OmitsEmptyFilters()
    {
        var handler = FakeHttpMessageHandler.Json(HttpStatusCode.OK, """{ "shows": [], "hasMore": false }""");
        var client = CreateClient(handler);

        await client.SearchMoviesAsync(new ShowSearchRequestDto { Title = "  " });

        var query = HttpUtility.ParseQueryString(handler.LastRequest!.RequestUri!.Query);

        Assert.Null(query["keyword"]);
        Assert.Null(query["year_min"]);
        Assert.Null(query["year_max"]);
        Assert.Null(query["genres"]);
        Assert.Null(query["order_by"]);
        Assert.Null(query["order_direction"]);
        Assert.Null(query["cursor"]);
    }

    [Fact]
    public async Task SearchMoviesAsync_DeserializesResponse()
    {
        const string json = """
            {
              "shows": [
                {
                  "id": "123",
                  "title": "Dune",
                  "overview": "Desert planet.",
                  "releaseYear": 2021,
                  "rating": 78,
                  "genres": [ { "id": "scifi", "name": "Science Fiction" } ],
                  "imageSet": { "verticalPoster": { "w240": "https://img/w240.jpg" } }
                }
              ],
              "hasMore": true,
              "nextCursor": "24733940:65"
            }
            """;
        var client = CreateClient(FakeHttpMessageHandler.Json(HttpStatusCode.OK, json));

        var result = await client.SearchMoviesAsync(new ShowSearchRequestDto());

        Assert.True(result.HasMore);
        Assert.Equal("24733940:65", result.NextCursor);
        var show = Assert.Single(result.Shows);
        Assert.Equal("123", show.Id);
        Assert.Equal("Dune", show.Title);
        Assert.Equal(2021, show.ReleaseYear);
        Assert.Equal(78, show.Rating);
        Assert.Equal("Science Fiction", Assert.Single(show.Genres).Name);
        Assert.Equal("https://img/w240.jpg", show.ImageSet.VerticalPoster.W240);
        Assert.Null(show.ImageSet.VerticalPoster.W720);
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, "invalid parameters")]
    [InlineData(HttpStatusCode.Unauthorized, "API key is missing or invalid")]
    [InlineData(HttpStatusCode.NotFound, "was not found")]
    [InlineData(HttpStatusCode.TooManyRequests, "quota")]
    [InlineData(HttpStatusCode.InternalServerError, "server error")]
    [InlineData(HttpStatusCode.BadGateway, "server error")]
    public async Task SearchMoviesAsync_MapsErrorStatusCodesToClearMessages(HttpStatusCode statusCode, string expectedMessagePart)
    {
        var client = CreateClient(FakeHttpMessageHandler.Json(statusCode, """{ "message": "upstream detail" }"""));

        var ex = await Assert.ThrowsAsync<StreamingAvailabilityApiException>(() => client.SearchMoviesAsync(new ShowSearchRequestDto()));

        Assert.Equal(statusCode, ex.StatusCode);
        Assert.Contains(expectedMessagePart, ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("API message: upstream detail", ex.Message);
    }

    [Fact]
    public async Task SearchMoviesAsync_ErrorWithoutJsonBody_StillThrowsClearMessage()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("not json")
        });
        var client = CreateClient(handler);

        var ex = await Assert.ThrowsAsync<StreamingAvailabilityApiException>(() => client.SearchMoviesAsync(new ShowSearchRequestDto()));

        Assert.Equal(HttpStatusCode.TooManyRequests, ex.StatusCode);
        Assert.Contains("quota", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("API message", ex.Message);
    }

    [Fact]
    public async Task SearchMoviesAsync_ConnectionFailure_ThrowsWithoutStatusCode()
    {
        var client = CreateClient(FakeHttpMessageHandler.Throws(new HttpRequestException("no route to host")));

        var ex = await Assert.ThrowsAsync<StreamingAvailabilityApiException>(() => client.SearchMoviesAsync(new ShowSearchRequestDto()));

        Assert.Null(ex.StatusCode);
        Assert.Contains("Could not connect", ex.Message);
        Assert.IsType<HttpRequestException>(ex.InnerException);
    }

    [Fact]
    public async Task SearchMoviesAsync_Timeout_ThrowsWithoutStatusCode()
    {
        var client = CreateClient(FakeHttpMessageHandler.Throws(new TaskCanceledException("timeout")));

        var ex = await Assert.ThrowsAsync<StreamingAvailabilityApiException>(() => client.SearchMoviesAsync(new ShowSearchRequestDto()));

        Assert.Null(ex.StatusCode);
        Assert.Contains("did not respond in time", ex.Message);
    }

    [Fact]
    public async Task GetGenresAsync_CallsGenresEndpointAndDeserializes()
    {
        var handler = FakeHttpMessageHandler.Json(HttpStatusCode.OK, """[ { "id": "action", "name": "Action" }, { "id": "drama", "name": "Drama" } ]""");
        var client = CreateClient(handler);

        var genres = await client.GetGenresAsync();

        Assert.Equal("/v4/genres", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Equal(2, genres.Count);
        Assert.Equal("action", genres[0].Id);
        Assert.Equal("Drama", genres[1].Name);
    }
}
