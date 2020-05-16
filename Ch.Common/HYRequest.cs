using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Ch.Common
{
    public class HYRequest
    {
        /// <summary>
        /// 接收参数,返回整型
        /// </summary>
        public static int GetIntByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    return int.Parse(fvalue.Trim());
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 接收能数,返回Int数组
        /// </summary>
        public static int[] GetIntArrayByParams(string name)
        {
            string fvalue = GetCheckBoxValue(name);
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    string[] array = fvalue.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    int[] reint = new int[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        reint[i] = int.Parse(array[i].Trim());
                    }
                    return reint;
                }
                catch
                {
                    return new int[0];
                }
            }
            else
            {
                return new int[0];
            }
        }

        /// <summary>
        /// 接收参数,返回对HTML特殊字符进行编码后的值
        /// </summary>
        public static string GetStringByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                return fvalue;
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 接收参数,返回对HTML特殊字符进行编码后的值
        /// </summary>
        public static bool GetBoolByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                if ("true".Equals(fvalue) || "1".Equals(fvalue))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 过滤了部分危险HTML内容后的参数值
        /// </summary>
        public static string GetStringByParamsWithHTML(string name)
        {
            HttpRequest request = HttpContext.Current.Request;

            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                return ProcessRequest(fvalue);
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 返回原始输入值，不进行任何过滤处理
        /// </summary>
        public static string GetStringByParamsRAW(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (request.QueryString[name] != null)
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                return fvalue;
            }
            else
            {
                return string.Empty;
            }
        }

        private static Regex reScript = new Regex(@"<(\s*)script[^>]*>(.*)</(\s*)script(\s*)>",
                                                  RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                  RegexOptions.Singleline);
        static Regex reStyle = new Regex(@"<(\s*)style[^>]*>(.*)</(\s*)style(\s*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static Regex reFrame = new Regex(@"<(\s*)iframe[^>]*>(.*)</(\s*)iframe(\s*)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        static Regex reOn = new Regex(@"\son[a-z]+=""[^""]*""", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static string ProcessRequest(string value)
        {
            value = reScript.Replace(value, "");
            value = reStyle.Replace(value, "");
            value = reFrame.Replace(value, "");
            value = reOn.Replace(value, "");
            return value;
        }
        /// <summary>
        /// 接收参数,返回String数组
        /// </summary>
        public static string[] GetStringArrayByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    string[] array = ProcessRequest(fvalue).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    return array;
                }
                catch
                {
                    return new string[0];
                }
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 接收参数,正则过滤特殊字符,并返回CheckBox值
        /// </summary>
        public static string GetCheckBoxValue(string name)
        {
            string checkvalue = GetStringByParams(name);
            if (checkvalue != string.Empty)
            {
                string regular = @"^[\d,]+$";
                if (!Regex.IsMatch(checkvalue, regular))
                    checkvalue = string.Empty;
            }
            return checkvalue;
        }

        /// <summary>
        /// 接收参数,返回长整型
        /// </summary>
        public static long GetLongByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    return long.Parse(fvalue.Trim());
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 接收参数,返回DateTime?
        /// </summary>
        public static DateTime? GetNDateTimeByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    return DateTime.Parse(fvalue.Trim());
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 接收参数,返回DateTime
        /// </summary>
        public static DateTime GetDateTimeByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    return DateTime.Parse(fvalue.Trim());
                }
                catch
                {
                    return new DateTime(2000, 1, 1);
                }
            }
            else
            {
                return new DateTime(2000, 1, 1);
            }
        }
        /// <summary>
        /// 接收参数,返回Double
        /// </summary>
        public static double GetDoubleByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    return double.Parse(fvalue.Trim());
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 接收参数,返回Decimal
        /// </summary>
        public static Decimal GetDecimalByParams(string name)
        {
            HttpRequest request = HttpContext.Current.Request;
            string fvalue = request.Form[name];
            if (string.IsNullOrEmpty(fvalue))
                fvalue = request.QueryString[name];
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    return Decimal.Parse(fvalue.Trim());
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 接收能数,返回String数组
        /// </summary>
        public static long[] GetLongArrayByParams(string name)
        {
            string fvalue = GetCheckBoxValue(name);
            if (!string.IsNullOrEmpty(fvalue))
            {
                try
                {
                    string[] array = fvalue.Split(',');
                    long[] reint = new long[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        reint[i] = long.Parse(array[i].Trim());
                    }
                    return reint;
                }
                catch
                {
                    return new long[0];
                }
            }
            else
            {
                return new long[0];
            }
        }
    }
}
