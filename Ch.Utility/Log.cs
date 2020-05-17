using System;
using System.IO;
using System.Text;

namespace Ch.Utility
{
    public class Log
    {
        /// <summary>
        /// 写操作日志文件
        /// </summary>
        /// <returns>返回值true成功,false失败</returns>
        public static bool WriteLog(string fileName, string errorMsg)
        {
            bool ret = true;
            try
            {
                string filePath = GetLogPath(fileName);
                FileInfo logFile = new FileInfo(filePath);
                if (logFile.Exists)
                {
                    if (logFile.Length >= 800000)
                    {
                        logFile.CopyTo(filePath.Replace("Log.txt", "_Log.txt"));
                        File.Delete(filePath);
                    }
                }
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter w = new StreamWriter(fs, Encoding.UTF8);
                w.BaseStream.Seek(0, SeekOrigin.End);
                w.Write(errorMsg);
                w.Write(" ---" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                w.Write("\n");
                w.Write("------------------------------------\n");
                w.Flush();
                w.Close();
                w.Dispose();
                fs.Close();
                fs.Dispose();
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
        public static bool Error(string fileName, string errorMsg)
        {
            WriteLog("error_" + fileName, errorMsg);
            return true;
        }
        public static bool Debug(string fileName, string errorMsg)
        {
            WriteLog("debug_" + fileName, errorMsg);
            return true;
        }
        public static bool Info(string fileName, string errorMsg)
        {
            WriteLog("info_" + fileName, errorMsg);
            return true;
        }
        /// <summary>
        /// 日志目录
        /// </summary>
        /// <returns></returns>
        protected static string GetLogPath(string fileName)
        {
            string LogPath = AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + DateTime.Now.ToString("yyyyMMdd");
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
            LogPath = Path.Combine(LogPath, fileName + ".txt");
            return LogPath;
        }
    }
}
