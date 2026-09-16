namespace MovieScreeningApp.Api.Interfaces;

public interface IApiKeyService
{
    Task SetAsync(string apiName, string value, CancellationToken cancellationToken = default);
}
