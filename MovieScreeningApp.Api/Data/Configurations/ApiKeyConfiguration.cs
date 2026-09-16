using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieScreeningApp.Api.Entities;

namespace MovieScreeningApp.Api.Data.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApiName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.ApiName)
            .IsUnique();
        
        builder.Property(x => x.Value)
            .IsRequired();

    }
}