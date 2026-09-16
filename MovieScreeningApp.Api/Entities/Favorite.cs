namespace MovieScreeningApp.Api.Entities;

public class Favorite : BaseEntity
{
    public string ShowId { get; set; }
    public string Title { get; set; }
    public string? Overview { get; set; }
    public int? ReleaseYear { get; set; }
    public int Rating { get; set; }
    public string? Genres { get; set; }
    public string? PosterUrl { get; set; }

    private Favorite()
    {

    }

    public Favorite(string showId, string title, string? overview, int? releaseYear, int rating, string? genres, string? posterUrl)
    {
        ShowId = showId;
        Title = title;
        Overview = overview;
        ReleaseYear = releaseYear;
        Rating = rating;
        Genres = genres;
        PosterUrl = posterUrl;
    }
}
