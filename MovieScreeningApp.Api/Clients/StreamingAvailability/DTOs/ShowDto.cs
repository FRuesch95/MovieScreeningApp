namespace MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;

public class ShowDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Overview { get; set; } = string.Empty;
    public int? ReleaseYear { get; set; }
    public List<GenreDto> Genres { get; set; } = [];
    public int Rating { get; set; }
    public ShowImageSetDto ImageSet { get; set; } = new();
}
