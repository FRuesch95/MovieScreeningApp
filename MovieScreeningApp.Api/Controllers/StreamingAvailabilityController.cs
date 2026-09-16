using System.Net;
using Microsoft.AspNetCore.Mvc;
using MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;
using MovieScreeningApp.Api.Exceptions;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamingAvailabilityController : ControllerBase
{
    private readonly IStreamingAvailabilityService _streamingAvailabilityService;
    private readonly ILogger<StreamingAvailabilityController> _logger;

    public StreamingAvailabilityController(
        IStreamingAvailabilityService streamingAvailabilityService,
        ILogger<StreamingAvailabilityController> logger)
    {
        _streamingAvailabilityService = streamingAvailabilityService;
        _logger = logger;
    }

    [HttpGet("movies")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<ShowSearchResponseDto>> SearchMovies([FromQuery] ShowSearchRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _streamingAvailabilityService.SearchMoviesAsync(request, cancellationToken));
        }
        catch (StreamingAvailabilityApiException ex)
        {
            return ApiErrorResult(ex);
        }
    }

    [HttpGet("genres")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<List<GenreDto>>> GetGenres(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _streamingAvailabilityService.GetGenresAsync(cancellationToken));
        }
        catch (StreamingAvailabilityApiException ex)
        {
            return ApiErrorResult(ex);
        }
    }

    private ObjectResult ApiErrorResult(StreamingAvailabilityApiException ex)
    {
        _logger.LogError(ex, "Streaming Availability API call failed with status {StatusCode}", ex.StatusCode);

        var statusCode = ex.StatusCode switch
        {
            HttpStatusCode.BadRequest => StatusCodes.Status400BadRequest,
            HttpStatusCode.TooManyRequests => StatusCodes.Status429TooManyRequests,
            _ => StatusCodes.Status502BadGateway
        };

        return Problem(detail: ex.Message, statusCode: statusCode, title: "Streaming Availability API error");
    }
}
