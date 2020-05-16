using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ch.Common
{
    public class HttpHelper
    {
        /// <summary>
        /// Post提交数据
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="jsonParas"></param>
        /// <returns></returns>
        public static string Post(string Url, string JsonParas)
        {
            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/x-www-form-urlencoded";
            //设置参数，并进行URL编码 
            string paraUrlCoded = JsonParas;
            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流 
            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                writer = null;
                Log.WriteLog("HttpPost", "连接服务器失败!" + ex.Message);
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            Stream s = response.GetResponseStream();
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();
            return postContent;//返回Json数据
        }
    }
}
