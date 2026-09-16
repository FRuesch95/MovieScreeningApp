namespace MovieScreeningApp.Api.Clients.StreamingAvailability.DTOs;

public class ShowSearchResponseDto
{
    public List<ShowDto> Shows { get; set; } = [];
    public bool HasMore { get; set; }
    public string? NextCursor { get; set; }
}
