using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MovieScreeningApp.Api.Clients;
using MovieScreeningApp.Api.Clients.StreamingAvailability;
using MovieScreeningApp.Api.Data;
using MovieScreeningApp.Api.Interfaces;
using MovieScreeningApp.Api.Providers;
using MovieScreeningApp.Api.Services;

namespace MovieScreeningApp.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        builder.Services.AddDataProtection()
            .SetApplicationName("MovieScreeningApp")
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "FileStore", "KeyRing")));
        
        builder.Services.AddDbContextPool<AppDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DbConnection"),
                    new MariaDbServerVersion(new Version(11, 7, 2)))
            , 1024);

        var streamingAvailabilityApiUrl = builder.Configuration.GetValue<string>("StreamingAvailabilityApi:BaseUrl");

        if (string.IsNullOrWhiteSpace(streamingAvailabilityApiUrl))
        {
            throw new InvalidOperationException("Configuration 'StreamingAvailabilityApi:BaseUrl' is missing or empty.");
        }
        
        builder.Services.AddHttpClient(StreamingAvailabilityApiClient.ClientName, client =>
        {
            client.BaseAddress = new Uri(streamingAvailabilityApiUrl.TrimEnd('/') + "/");
        });

        
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IApiKeyProvider, ApiKeyProvider>();
        builder.Services.AddScoped<IStreamingAvailabilityApiClient, StreamingAvailabilityApiClient>();
        builder.Services.AddScoped<IStreamingAvailabilityService, StreamingAvailabilityService>();
        builder.Services.AddScoped<IFavoriteService, FavoriteService>();
        builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
        
        
        
        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapControllers();

        app.Run();
    }
}