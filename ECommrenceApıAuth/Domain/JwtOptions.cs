namespace ECommrenceApıAuth.Domain
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int TokenExpiryInMinutes { get; set; }
        public int RefreshTokenExpiryInMinutes { get; set; }
        
    }
}
