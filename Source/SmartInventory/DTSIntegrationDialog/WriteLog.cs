using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSIntegrationDialog
{
    public class WriteLog
    {
        public static bool WriteLogFile(string strMessage)
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
                //string strFileName = $"log_{DateTime.UtcNow:yyyyMMdd}.txt";
                string strFileName = Path.Combine(logDirectory, $"log_{DateTime.UtcNow:yyyyMMdd}.txt");

                using (StreamWriter objStreamWriter = File.AppendText(strFileName))
                {
                    string logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {strMessage}";
                    objStreamWriter.WriteLine(logEntry);
                    objStreamWriter.WriteLine();
                }
                return true;
            }
            catch (Exception ex)
            {
                // Optionally log the exception to a separate error log
                return false;
            }
        }
    }
}
