using Microsoft.AspNetCore.Mvc;
using MovieScreeningApp.Api.DTOs;
using MovieScreeningApp.Api.Interfaces;

namespace MovieScreeningApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FavoriteDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _favoriteService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FavoriteDto>> Add([FromBody] AddFavoriteRequestDto request, CancellationToken cancellationToken)
    {
        return Ok(await _favoriteService.AddAsync(request, cancellationToken));
    }

    [HttpDelete("{showId}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(string showId, CancellationToken cancellationToken)
    {
        var removed = await _favoriteService.RemoveAsync(showId, cancellationToken);

        if (!removed)
        {
            return Problem(detail: $"No favorite found for show '{showId}'.", statusCode: StatusCodes.Status404NotFound, title: "Favorite not found");
        }

        return NoContent();
    }
}
