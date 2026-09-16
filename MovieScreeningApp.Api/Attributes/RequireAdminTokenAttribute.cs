using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieScreeningApp.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireAdminTokenAttribute : Attribute, IAuthorizationFilter
{
    public const string HeaderName = "X-Admin-Token";
    public const string ConfigurationKey = "Admin:Token";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedToken = configuration.GetValue<string>(ConfigurationKey);

        if (string.IsNullOrWhiteSpace(expectedToken))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedToken)
            || !FixedTimeEquals(expectedToken, providedToken.ToString()))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    private static bool FixedTimeEquals(string expected, string provided)
    {
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(expected),
            Encoding.UTF8.GetBytes(provided));
    }
}
