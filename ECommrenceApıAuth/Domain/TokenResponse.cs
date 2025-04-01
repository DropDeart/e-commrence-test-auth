using System.Text.Json.Serialization;

namespace ECommrenceApıAuth.Domain
{
    public class TokenResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonIgnore]
        public DateTime RetrievedTime { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public DateTime ExpiresAt => RetrievedTime.AddSeconds(ExpiresInSeconds - 30); // 30 saniye buffer
    }
}
