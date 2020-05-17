using System;
using System.IO;
using System.Net;
using System.Text;

namespace Ch.Utility
{
    public class HttpUtil
    {
        private const string sContentType = "application/x-www-form-urlencoded";
        private const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        public static string Send(string data, string url)
        {
            return HttpUtil.Send(Encoding.GetEncoding("UTF-8").GetBytes(data), url);
        }
        public static string Send(byte[] data, string url)
        {
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            bool flag = httpWebRequest == null;
            if (flag)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", url));
            }
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = (long)data.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            Stream responseStream;
            try
            {
                responseStream = httpWebRequest.GetResponse().GetResponseStream();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string result = string.Empty;
            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
            {
                result = streamReader.ReadToEnd();
            }
            responseStream.Close();
            return result;
        }
        public static string GetUrlContent(string urladdress)
        {
            byte[] bytes = new WebClient
            {
                Credentials = CredentialCache.DefaultCredentials
            }.DownloadData(urladdress);
            return Encoding.UTF8.GetString(bytes);
        }
        public static bool DownloadPicture(string picUrl, string savePath, int timeOut)
        {
            bool result = false;
            WebResponse webResponse = null;
            Stream stream = null;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(picUrl);
                bool flag = timeOut != -1;
                if (flag)
                {
                    httpWebRequest.Timeout = timeOut;
                }
                webResponse = httpWebRequest.GetResponse();
                stream = webResponse.GetResponseStream();
                bool flag2 = !webResponse.ContentType.ToLower().StartsWith("text/");
                if (flag2)
                {
                    result = HttpUtil.SaveBinaryFile(webResponse, savePath);
                }
            }
            catch (Exception var_6_60)
            {
            }
            finally
            {
                bool flag3 = stream != null;
                if (flag3)
                {
                    stream.Close();
                }
                bool flag4 = webResponse != null;
                if (flag4)
                {
                    webResponse.Close();
                }
            }
            return result;
        }
        private static bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool result = false;
            byte[] array = new byte[1024];
            Stream stream = null;
            Stream stream2 = null;
            try
            {
                bool flag = File.Exists(savePath);
                if (flag)
                {
                    File.Delete(savePath);
                }
                stream = File.Create(savePath);
                stream2 = response.GetResponseStream();
                int num;
                do
                {
                    num = stream2.Read(array, 0, array.Length);
                    bool flag2 = num > 0;
                    if (flag2)
                    {
                        stream.Write(array, 0, num);
                    }
                }
                while (num > 0);
                result = true;
            }
            finally
            {
                bool flag3 = stream != null;
                if (flag3)
                {
                    stream.Close();
                }
                bool flag4 = stream2 != null;
                if (flag4)
                {
                    stream2.Close();
                }
            }
            return result;
        }
    }
}
