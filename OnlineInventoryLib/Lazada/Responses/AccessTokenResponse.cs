namespace OnlineInventoryLib.Lazada.Responses
{
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int refresh_expires_in { get; set; }
        public int expires_in { get; set; }
    }
}
