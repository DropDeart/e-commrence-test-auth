using ECommrenceApıAuth.Domain;
using ECommrenceApıAuth.Interface;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ECommrenceApıAuth.Service
{
    public class AuthService : IAuthService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly HttpClient _httpClient;
        private readonly ExternalApiSettings _externalApiSettings;

        private TokenResponse _currentToken;
        private int _requestCount = 0;

        public AuthService(IOptions<JwtOptions> jwtOptions, IOptions<ExternalApiSettings> externalApiSettings, HttpClient httpClient)
        {
            _jwtOptions = jwtOptions.Value;
            _externalApiSettings = externalApiSettings.Value;
            _httpClient = httpClient;
            _currentToken = new TokenResponse();
        }

        public async Task<TokenResponse> GetValidTokenAsync(Guid userId, string userEmail)
        {
            // Eğer token yoksa veya süresi dolmuşsa yenisini al
            if (_currentToken.AccessToken == null || DateTime.UtcNow >= _currentToken.ExpiresIn)
            {
                _currentToken = await RequestNewTokenAsync(userId, userEmail);
                _requestCount = 0; // Yeni token alındığında sayaç sıfırlanır
            }

            // Eğer 5 isteklik limit dolmuşsa, yeni token almak zorundayız
            if (_requestCount >= _externalApiSettings.HourlyTokenLimit)
            {
                _currentToken = await RequestNewTokenAsync(userId, userEmail);
                _requestCount = 0; // Sayaç sıfırla
            }

            _requestCount++; // Her çağrıda sayacı arttır
            return _currentToken;
        }

        public async Task<TokenResponse> RequestNewTokenAysnc(Guid userId, string userEmail)
        {
            var requestBody = new
            {
                client_id = _externalApiSettings.ClientId,
                client_secret = _externalApiSettings.ClientSecret,
                grant_type = "client_credentials"
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_externalApiSettings.AuthUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Token alınamadı. Hata: {response.StatusCode}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenData = JsonSerializer.Deserialize<TokenResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Token'ın geçerlilik süresi (expires_in) saniye cinsinden olduğundan, ExpiryTime hesaplanmalı
            return new TokenResponse
            {
                AccessToken = tokenData.AccessToken,
                TokenType = tokenData.TokenType,
                ExpiresIn = DateTime.UtcNow // Geçerlilik süresi hesaplanır
            };
        }

        public async Task<RefreshToken> GenerateRefreshTokenAsync()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Created = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddDays(7) // Refresh Token 7 gün geçerli, Genelde daha uzun da olabilir.
            };
        }
    }
}
