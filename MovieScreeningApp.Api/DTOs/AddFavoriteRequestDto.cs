using System.ComponentModel.DataAnnotations;

namespace MovieScreeningApp.Api.DTOs;

public class AddFavoriteRequestDto
{
    [Required]
    [MaxLength(50)]
    public string ShowId { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    public string? Overview { get; set; }
    public int? ReleaseYear { get; set; }
    public int Rating { get; set; }

    [MaxLength(500)]
    public string? Genres { get; set; }

    [MaxLength(1000)]
    public string? PosterUrl { get; set; }
}
