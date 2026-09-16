using System.Net;
using Microsoft.AspNetCore.WebUtilities;
using MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;
using MovieScreeningApp.Api.Exceptions;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Clients.StreamingAvailability;

public class StreamingAvailabilityApiClient : IStreamingAvailabilityApiClient
{
    public const string ClientName = "StreamingAvailabilityApi";

    private const string ApiKeyHeader = "X-API-Key";
    private const string Country = "de";
    private const string OutputLanguage = "en";
    private const string ShowTypeMovie = "movie";

    private string? _cachedApiKey;

    private readonly HttpClient _httpClient;
    private readonly IApiKeyProvider _apiKeyProvider;

    public StreamingAvailabilityApiClient(
        IHttpClientFactory httpClientFactory,
        IApiKeyProvider apiKeyProvider)
    {
        _httpClient = httpClientFactory.CreateClient(ClientName);
        _apiKeyProvider = apiKeyProvider;
    }

    public async Task<ShowSearchResponseDto> SearchMoviesAsync(ShowSearchRequestDto request, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["country"] = Country,
            ["show_type"] = ShowTypeMovie,
            ["output_language"] = OutputLanguage,
            ["keyword"] = request.Title,
            ["year_min"] = request.Year?.ToString(),
            ["year_max"] = request.Year?.ToString(),
            ["genres"] = request.Genre,
            ["order_by"] = request.OrderBy,
            ["order_direction"] = request.OrderDirection,
            ["cursor"] = request.Cursor
        };

        return await GetAsync<ShowSearchResponseDto>("shows/search/filters", query, cancellationToken);
    }

    public async Task<List<GenreDto>> GetGenresAsync(CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["output_language"] = OutputLanguage
        };

        return await GetAsync<List<GenreDto>>("genres", query, cancellationToken);
    }

    private async Task<T> GetAsync<T>(string path, Dictionary<string, string?> query, CancellationToken cancellationToken)
    {
        var uri = QueryHelpers.AddQueryString(path, query.Where(x => !string.IsNullOrWhiteSpace(x.Value)));

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Add(ApiKeyHeader, await GetApiKeyAsync());

        HttpResponseMessage response;

        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            throw new StreamingAvailabilityApiException(
                "Could not connect to the Streaming Availability API. Please check the network connection and try again.",
                innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new StreamingAvailabilityApiException(
                "The Streaming Availability API did not respond in time. Please try again later.",
                innerException: ex);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw await CreateApiExceptionAsync(response, cancellationToken);
            }

            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken);

            return result ?? throw new StreamingAvailabilityApiException(
                "The Streaming Availability API returned an empty response.",
                response.StatusCode);
        }
    }

    private static async Task<StreamingAvailabilityApiException> CreateApiExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var apiMessage = await ReadApiErrorMessageAsync(response, cancellationToken);

        var message = response.StatusCode switch
        {
            HttpStatusCode.BadRequest => "The Streaming Availability API rejected the request due to invalid parameters.",
            HttpStatusCode.Unauthorized => "The Streaming Availability API key is missing or invalid.",
            HttpStatusCode.NotFound => "The requested resource was not found on the Streaming Availability API.",
            HttpStatusCode.TooManyRequests => "The monthly request quota of the Streaming Availability API has been exceeded.",
            >= HttpStatusCode.InternalServerError => "The Streaming Availability API is currently unavailable due to a server error.",
            _ => $"The Streaming Availability API returned an unexpected status code ({(int)response.StatusCode})."
        };

        if (!string.IsNullOrWhiteSpace(apiMessage))
        {
            message += $" API message: {apiMessage}";
        }

        return new StreamingAvailabilityApiException(message, response.StatusCode);
    }

    private static async Task<string?> ReadApiErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponseDto>(cancellationToken);
            return error?.Message;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string> GetApiKeyAsync()
    {
        if (string.IsNullOrWhiteSpace(_cachedApiKey))
        {
            _cachedApiKey = await _apiKeyProvider.GetKeyAsync(ClientName);
        }

        return _cachedApiKey;
    }
}
