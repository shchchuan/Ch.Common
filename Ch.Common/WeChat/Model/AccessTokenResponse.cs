using Newtonsoft.Json;

namespace Ch.Common.WeChat.Model
{
    public class AuthResponse
    {
        public string Session_Key { get; set; }
        public string OpenId { get; set; }
    }
    public class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }
        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }
    }
}
