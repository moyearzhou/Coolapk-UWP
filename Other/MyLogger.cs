using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coolapk_UWP.Other
{
    public class MyLogger
    {
        private readonly static MyLogger logger = new MyLogger();

        private string _className;

        private MyLogger()
        {

        }

        public static void i(String TAG, String msg)
        {
        }


        public static void d(String TAG, String msg)
        {
        }


        public static void w(String TAG, String msg)
        {
        }

        public static void e(String TAG, String msg)
        {
        }


        public void WriteLogs(string dirName, string type, string content)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                path = AppDomain.CurrentDomain.BaseDirectory + dirName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!File.Exists(path))
                {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path))
                {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    sw.WriteLineAsync(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + (_className ?? "") + " : " + type + " --> " + content);
                    sw.Close();
                }
            }
        }

        private void Log(string type, string content)
        {
            WriteLogs("logs", type, content);
        }

        public void Debug(string content)
        {
            Log("Debug", content);
        }

        public void Info(string content)
        {
            Log("Info", content);
        }

        public void Warn(string content)
        {
            Log("Warn", content);
        }

        public void Error(string content)
        {
            Log("Error", content);
        }

        public void Fatal(string content)
        {
            Log("Fatal", content);
        }
    }
}
