using Ch.Common.WeChat.Model;
using Newtonsoft.Json;
using RestSharp;
using System;

namespace Ch.Common.WeChat
{
    public class AuthService
    {
        protected string _appId = string.Empty;
        protected string _appSecret = string.Empty;
        public AuthService(string AppId, string AppSecret)
        {
            _appId = AppId;
            _appSecret = AppSecret;
        }
        public AuthResponse Code2Session(string code)
        {
            RestClient client = new RestClient("https://api.weixin.qq.com/");
            string path = $"sns/jscode2session?appid={_appId}&secret={_appSecret}&js_code={code}&grant_type=authorization_code";
            var restRequest = new RestRequest(path, Method.GET);
            var response = client.Execute(restRequest);
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    AuthResponse res = JsonConvert.DeserializeObject<AuthResponse>(response.Content);
                    return res;
                }
                catch (Exception ex)
                {
                    throw new Exception("code to session failed:" + ex.Message, ex);
                }
            }
            return new AuthResponse();
        }
        /// <summary>
        /// get access token
        /// </summary>
        /// <returns></returns>
        public AccessTokenResponse GetAccessToken()
        {
            RestClient client = new RestClient("https://api.weixin.qq.com/");
            string path = $"cgi-bin/token?grant_type=client_credential&appid={_appId}&secret={_appSecret}";
            var restRequest = new RestRequest(path, Method.GET);
            var response = client.Execute(restRequest);
            if (!string.IsNullOrEmpty(response.Content))
            {
                try
                {
                    AccessTokenResponse res = JsonConvert.DeserializeObject<AccessTokenResponse>(response.Content);
                    return res;
                }
                catch (Exception ex)
                {
                    throw new Exception("get access token failed:" + ex.Message, ex);
                }
            }
            return new AccessTokenResponse();
        }
    }
}
