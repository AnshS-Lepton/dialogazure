using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Configuration;

namespace WFMNotificationService
{
    class LogWriter
    {
        //public static string SeletecdFilePath;
        //private static string m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + "log.txt";
        //private static string m_exePath = @"E:\Durgesh\log.txt";

        public static void LogWrite(string logMessage)
        {
            WriteToLog(logMessage, 1);
            //try
            //{
            //    using (StreamWriter w = File.AppendText(m_exePath))
            //    {
            //        Log(logMessage, w);
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        private static void WriteToLog(string logDetail, int logType)
        {

            try
            {

                string ErrorMsg = Environment.NewLine + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " " + logDetail.ToString();

                string logPath = ConfigurationManager.AppSettings["LogPath"].ToString();
                DirectoryInfo directoryInfo = new DirectoryInfo(logPath);

                if (!directoryInfo.Exists)
                    directoryInfo.Create();


                string newfName = "";
                string dd = DateTime.Now.Day.ToString();
                string mm = DateTime.Now.Month.ToString();
                string yyyy = DateTime.Now.Year.ToString();
                newfName = dd + "-" + mm + "-" + yyyy + "Log.txt";

                string fileName = Path.Combine(logPath, newfName);
                if (!File.Exists(fileName))
                {
                    ErrorMsg = "WFM Notification Service version no 001 dated 24-01-2023 " + ErrorMsg;
                    if (logType != 3)
                    {
                        DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(fileName));
                        foreach (FileInfo fi in di.GetFiles("*Log.txt"))
                        {
                            double d = 90;
                            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["logPurgedays"]))
                            {
                                d = Convert.ToDouble(ConfigurationManager.AppSettings["logPurgedays"]);
                            }
                            if (fi.CreationTime < DateTime.Now.AddDays(-d))
                            {
                                fi.Delete();
                            }
                        }
                    }
                }
                File.AppendAllText(fileName, ErrorMsg.ToString());
            }
            catch (Exception)
            {

            }

        }

    }
}
