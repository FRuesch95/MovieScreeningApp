namespace MovieScreeningApp.Api.Interfaces;

public interface IApiKeyProvider
{
    Task<string> GetKeyAsync(string apiName);
}