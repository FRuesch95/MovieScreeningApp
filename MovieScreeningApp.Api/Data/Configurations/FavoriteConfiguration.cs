using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieScreeningApp.Api.Entities;

namespace MovieScreeningApp.Api.Data.Configurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ShowId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.ShowId)
            .IsUnique();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Genres)
            .HasMaxLength(500);

        builder.Property(x => x.PosterUrl)
            .HasMaxLength(1000);
    }
}
