using Microsoft.AspNetCore.Mvc;
using MovieScreeningApp.Api.Attributes;
using MovieScreeningApp.Api.DTOs;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireAdminToken]
public class ApiKeysController : ControllerBase
{
    private readonly IApiKeyService _apiKeyService;

    public ApiKeysController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    [HttpPut("{apiName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Set(string apiName, [FromBody] SetApiKeyRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            await _apiKeyService.SetAsync(apiName, request.Value, cancellationToken);
        }
        catch (ArgumentException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest, title: "Invalid API name");
        }

        return NoContent();
    }
}
