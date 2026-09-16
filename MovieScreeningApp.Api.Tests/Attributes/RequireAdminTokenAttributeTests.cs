using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieScreeningApp.Api.Attributes;

namespace MovieScreeningApp.Api.Tests.Attributes;

public class RequireAdminTokenAttributeTests
{
    private static AuthorizationFilterContext CreateContext(string? configuredToken, string? providedToken)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [RequireAdminTokenAttribute.ConfigurationKey] = configuredToken
            })
            .Build();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .BuildServiceProvider()
        };

        if (providedToken is not null)
        {
            httpContext.Request.Headers[RequireAdminTokenAttribute.HeaderName] = providedToken;
        }

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        return new AuthorizationFilterContext(actionContext, []);
    }

    [Fact]
    public void ValidToken_AllowsRequest()
    {
        var context = CreateContext("secret", "secret");

        new RequireAdminTokenAttribute().OnAuthorization(context);

        Assert.Null(context.Result);
    }

    [Theory]
    [InlineData("wrong")]
    [InlineData("")]
    [InlineData(null)]
    public void MissingOrWrongToken_ReturnsUnauthorized(string? providedToken)
    {
        var context = CreateContext("secret", providedToken);

        new RequireAdminTokenAttribute().OnAuthorization(context);

        Assert.IsType<UnauthorizedResult>(context.Result);
    }

    [Fact]
    public void NoTokenConfigured_ReturnsServiceUnavailable()
    {
        var context = CreateContext(null, "secret");

        new RequireAdminTokenAttribute().OnAuthorization(context);

        var result = Assert.IsType<StatusCodeResult>(context.Result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, result.StatusCode);
    }
}
