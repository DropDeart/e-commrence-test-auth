using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Text;
using ECommrenceApıAuth.Interface;
using ECommrenceApıAuth.Domain;
using System.Security.Cryptography;

public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly ExternalApiSettings _apiSettings;
    private TokenResponse _cachedToken; // Token cache'leme için

    public OrderService(HttpClient httpClient, IAuthService authService, IOptions<ExternalApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _authService = authService;
        _apiSettings = apiSettings.Value;
    }

    public async Task<List<Order>> GetOrdersAsync() // Order domain modelini de ekleyip kodu şişirmek istmedim. :)
    {
        var tokenResponse = await GetTokenAsync();

        // Authorization header'a AccessToken ekliyoruz
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

        var response = await _httpClient.GetAsync(_apiSettings.ApiUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Sipariş listesi alınamadı. Hata: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<Order>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task<TokenResponse> GetTokenAsync()
    {
        // Önceden alınmış ve geçerli olan token'ı kullan
        if (_cachedToken != null && DateTime.UtcNow < _cachedToken.ExpiresIn)
        {
            return _cachedToken;
        }

        var newToken = await _authService.GetValidTokenAsync(Guid.NewGuid(), "kullanıcı-email-adresi@ornek.com");
        _cachedToken = newToken;
        return newToken;
    }
}
