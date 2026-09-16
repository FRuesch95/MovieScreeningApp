using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MovieScreeningApp.Api.Attributes;
using MovieScreeningApp.Api.Entities;

namespace MovieScreeningApp.Api.Data;

public class AppDbContext : DbContext
{
    private readonly IDataProtector _protector;
    
    
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    
    
    
    
    public AppDbContext(DbContextOptions options, IDataProtectionProvider dataProtectionProvider) : base(options)
    {
        _protector = dataProtectionProvider.CreateProtector("MovieScreeningApp.DatabaseEncryption.v1");
    }
    
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        ApplyEncryptionConverters(modelBuilder);
    }
    
    
    
    private void ApplyEncryptionConverters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var propertyInfo = property.PropertyInfo;

                if (propertyInfo is null)
                    continue;

                if (!Attribute.IsDefined(propertyInfo, typeof(EncryptedAttribute)))
                    continue;

                if (property.ClrType != typeof(string))
                {
                    throw new InvalidOperationException(
                        $"Property '{entityType.ClrType.Name}.{property.Name}' " +
                        $"uses [Encrypted], but only string properties are supported.");
                }

                var converter = new ValueConverter<string, string>(
                    value => _protector.Protect(value),
                    value => _protector.Unprotect(value));

                property.SetValueConverter(converter);
            }
        }
    }
    
    
}