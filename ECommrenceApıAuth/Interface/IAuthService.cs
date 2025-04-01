using ECommrenceApıAuth.Domain;

namespace ECommrenceApıAuth.Interface
{
    public interface IAuthService
    {
        Task<string> GetValidAccessTokenAsync();
    }
}