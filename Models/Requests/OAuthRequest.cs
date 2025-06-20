namespace MiniSocialAPI.Models.Requests
{
    public class OAuthRequest
    {
        public string Provider { get; set; } = "google";
        public string IdToken { get; set; }
    }
}
