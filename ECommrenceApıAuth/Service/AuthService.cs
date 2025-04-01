// Services/AuthService.cs
using ECommrenceApıAuth.Domain;
using ECommrenceApıAuth.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiSettings _settings;
    private readonly IMemoryCache _cache;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private const string TokenCacheKey = "ExternalApiToken";
    private const string RequestCounterKey = "TokenRequestCounter";

    public AuthService(HttpClient httpClient,IOptions<ExternalApiSettings> settings,IMemoryCache cache)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _cache = cache;
    }

    public async Task<string> GetValidAccessTokenAsync()
    {
        // Mevcut token kontrolü
        if (_cache.TryGetValue(TokenCacheKey, out TokenResponse cachedToken) &&
            DateTime.UtcNow < cachedToken.ExpiresAt)
        {
            return cachedToken.AccessToken;
        }

        // Rate limit kontrolü
        var (count, windowStart) = _cache.GetOrCreate(RequestCounterKey, entry =>
        {
            entry.AbsoluteExpiration = DateTime.UtcNow.AddHours(1);
            return (0, DateTime.UtcNow);
        });

        if (count >= _settings.HourlyTokenLimit)
            throw new RateLimitException("Saatlik token limiti aşıldı.");

        await _semaphore.WaitAsync();
        try
        {
            // Double-check
            if (_cache.TryGetValue(TokenCacheKey, out cachedToken) &&
                DateTime.UtcNow < cachedToken.ExpiresAt)
            {
                return cachedToken.AccessToken;
            }

            // Yeni token al
            var newToken = await RequestNewTokenAsync();
            _cache.Set(TokenCacheKey, newToken, newToken.ExpiresAt);

            // Counter'ı güncelle
            _cache.Set(RequestCounterKey, (count + 1, windowStart),
                new DateTimeOffset(windowStart.AddHours(1)));

            return newToken.AccessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<TokenResponse> RequestNewTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, _settings.AuthUrl)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "client_credentials"},
                {"client_id", _settings.ClientId},
                {"client_secret", _settings.ClientSecret}
            })
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        token.RetrievedTime = DateTime.UtcNow;
        return token;
    }
}

public class RateLimitException : Exception
{
    public RateLimitException(string message) : base(message) { }
}