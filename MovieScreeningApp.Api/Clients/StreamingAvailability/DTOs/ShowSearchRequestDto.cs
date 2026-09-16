namespace MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;

public class ShowSearchRequestDto
{
    public string? Title { get; set; }
    public int? Year { get; set; }
    public string? Genre { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDirection { get; set; }
    public string? Cursor { get; set; }
}
