using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Up.Bsky.PostBot.Util.Auth;

public class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyAuthorizationFilter))
    {
    }
}

public interface IApiKeyValidator
{
    bool IsValid(string apiKey);
}

public class ApiKeyAuthorizationFilter : IAuthorizationFilter
{
    private const string ApiKeyHeader = "x-api-key";
    
    private readonly IApiKeyValidator _apiKeyValidator;
    
    public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers.FirstOrDefault(it => it.Key.ToLowerInvariant() == ApiKeyHeader).Value.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey) || !_apiKeyValidator.IsValid(apiKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
