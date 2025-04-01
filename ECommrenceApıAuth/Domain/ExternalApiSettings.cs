namespace ECommrenceApıAuth.Domain
{
    public class ExternalApiSettings
    {
        public string AuthUrl { get; set; }
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int HourlyTokenLimit { get; set; }
    }

}
