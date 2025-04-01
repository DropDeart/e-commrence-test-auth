using ECommrenceApıAuth.Domain;

namespace ECommrenceApıAuth.Interface
{
    public interface IAuthService
    {
        public Task<TokenResponse> GetValidTokenAsync(Guid userId, string userEmail); // Token'ı valide et ve yenisini al
        public Task<TokenResponse> RequestNewTokenAysnc(Guid userId, string userEmail); // Yeni token al
        public Task<RefreshToken> GenerateRefreshTokenAsync(); // Refresh token üret
    }
}