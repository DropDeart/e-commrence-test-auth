namespace ECommrenceApıAuth.Domain
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
