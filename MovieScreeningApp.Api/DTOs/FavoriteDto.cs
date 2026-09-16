namespace MovieScreeningApp.Api.DTOs;

public class FavoriteDto
{
    public int Id { get; set; }
    public string ShowId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Overview { get; set; }
    public int? ReleaseYear { get; set; }
    public int Rating { get; set; }
    public string? Genres { get; set; }
    public string? PosterUrl { get; set; }
}
