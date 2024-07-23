using System;
using System.IO;

namespace CheckingPaymentService
{
    public static class LogHelper
    {
        private static readonly string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        private const string ERROR_PREFIX = "_ER";
        private const string ERROR_TAG = "_ER";

        public static void WriteEvent(string message, EventType eventType)
        {
            lock (currentDir)
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs\\"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs\\");
                }
                string genFileName = CheckLogFile();
                string specFileName = string.Empty;
                string tag = string.Empty;
                string timeStampStr = DateTime.Now.ToLongTimeString();

                switch (eventType)
                {
                    case EventType.Error:
                        specFileName = CheckLogFile(ERROR_PREFIX);
                        tag = ERROR_TAG;
                        break;
                }

                using (var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + genFileName, true))
                {
                    file.WriteLine(FormatLogString(message, tag, timeStampStr));
                    file.Close();
                }

                if (!String.IsNullOrEmpty(specFileName))
                {
                    using (var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + specFileName, true))
                    {
                        file.WriteLine(FormatLogString(message, tag, timeStampStr));
                        file.Close();
                    }
                }
            }
        }

        private static string FormatLogString(string message, string tag, string timeStampStr)
        {
            return string.Format("{0} {1}     {2}", tag, timeStampStr, message);
        }

        private static string CheckLogFile(string prefix)
        {
            string name = DateTime.Now.ToShortDateString().Replace(".", "_").Replace("/", "_") + prefix + ".txt";
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + name))
            {
                FileStream fs = File.Create(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + name);
                fs.Close();
                fs.Dispose();
            }

            return name;
        }

        private static string CheckLogFile()
        {
            string name = DateTime.Now.ToShortDateString().Replace(".", "_").Replace("/", "_") + ".txt";

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + name))
            {
                FileStream fs = File.Create(AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + name);
                fs.Close();
                fs.Dispose();
            }

            return name;
        }
    }


    public enum EventType
    {
        Info,
        Error
    }
}
