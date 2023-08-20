namespace Up.Bsky.PostBot.Util.Auth;

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly string _apiKey;
    public ApiKeyValidator(IConfiguration config)
    {
        _apiKey = config["ApiKey"] ?? throw new Exception("ApiKey not found in configuration");
    }
    public bool IsValid(string apiKey) => apiKey == _apiKey;
}
