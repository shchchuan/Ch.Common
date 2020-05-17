using Ch.Utility.WeChat.Model;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Ch.Utility.WeChat
{
    public class SeniorService
    {
        public static WeiXinUserModel WeiXinUserInfo(string CommonToken, string openid)
        {
            string urladdress = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}", CommonToken, openid);
            string urlContent = HttpUtil.GetUrlContent(urladdress);
            WeiXinUserModel weiXinUserInfoEntity = new WeiXinUserModel();
            return JsonConvert.DeserializeObject<WeiXinUserModel>(urlContent);
        }
        public static string GetTicket(string CommonToken)
        {
            string s = "{\"expire_seconds\": 1800, \"action_name\": \"QR_SCENE\", \"action_info\": {\"scene\": {\"scene_id\": 123}}}";
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            string address = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + CommonToken;
            byte[] bytes2 = new WebClient
            {
                Headers =
                {

                    {
                        "Content-Type",
                        "application/x-www-form-urlencoded"
                    }
                }
            }.UploadData(address, "POST", bytes);
            return Encoding.UTF8.GetString(bytes2);
        }
        public static OAuthModel UserOAuthInfo(string AppId, string AppSecret, string code)
        {
            string host = "https://api.weixin.qq.com/sns/oauth2/access_token";
            string url = host + $"?appid={AppId}&secret={AppSecret}&code={code}&grant_type=authorization_code";
            string urlContent = HttpUtil.GetUrlContent(url);
            OAuthModel oAuthEntity = JsonConvert.DeserializeObject<OAuthModel>(urlContent);
            return oAuthEntity;
        }
    }
}
