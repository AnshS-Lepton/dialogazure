using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Configuration;
using System.Net.Security;
using System.Diagnostics;

namespace UtilizationEmailScheduler
{
    public class Email
    {
        public static void SendEmailWithAttachment(string emailBody, string subject, string filePath, string[] receipentList)
        {
            //receipentList = { "avinash.kamath@leptonsoftware.com", "rahul.sharma@leptonsoftware.com" };
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            string mailSentMessage = "";
            EmailSettingsModel objEmailSettingsModel = new EmailSettingsModel()
            {
                id = 1,
                port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString()),
                email_address = ConfigurationManager.AppSettings["Sender"].ToString(),
                smtp_host = ConfigurationManager.AppSettings["SMTP"].ToString(),
                email_password = ConfigurationManager.AppSettings["Password"].ToString(),
                enablessl = Convert.ToBoolean(ConfigurationManager.AppSettings["SSlEnable"].ToString()),
                usedefaultcredentials = false
            };
            List<EmailSettingsModel> objEmailSettingsModelList = new List<EmailSettingsModel>();
            objEmailSettingsModelList.Add(objEmailSettingsModel);
            try
            {
                SendEmail(receipentList, subject, emailBody, out mailSentMessage, objEmailSettingsModelList, filePath);

            }
            catch 
            {
                throw;
            }
        }

        public static void SendEmail(string[] receivers, string subject, string message, out string mailSentMsg, List<EmailSettingsModel> listEmail, string filepath)
        {
            mailSentMsg = "";            
            try
            {               
                foreach (var objEmailSetting in listEmail)
                {
                    foreach (string receiver in receivers)
                    {
                        var senderEmail = new MailAddress(objEmailSetting.email_address);
                        var sub = subject;
                        var body = message;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = objEmailSetting.smtp_host;
                        smtp.Timeout = 100000;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.EnableSsl = true;
                        NetworkCredential networkCredential = new NetworkCredential(objEmailSetting.email_address, objEmailSetting.email_password);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = networkCredential;
                        smtp.Port = objEmailSetting.port;
                        using (MailMessage mail = new MailMessage())
                        {
                            if (filepath != "")
                                mail.Attachments.Add(new Attachment(filepath));
                            mail.From = new MailAddress(objEmailSetting.email_address);
                            mail.To.Add(new MailAddress(receiver));
                            mail.Subject = subject;
                            mail.Body = message;
                            mail.IsBodyHtml = true;
                            smtp.Send(mail);
                        }
                    }
                }              
                
            }
            catch 
            {
                throw;
            }
            finally
            {

            }
        }



        public class EmailSettingsModel
        {
            public int id { get; set; }
            public string smtp_host { get; set; }
            public int port { get; set; }
            public string email_address { get; set; }
            public string email_password { get; set; }
            public bool auth { get; set; }
            public int created_by { get; set; }
            public DateTime created_on { get; set; }
            public int? modified_by { get; set; }
            public DateTime modified_on { get; set; }
            public bool enablessl { get; set; }
            public bool usedefaultcredentials { get; set; }
        }

        public static void CaptureErrorInFile(string message,string innerexception,string stacktrace)
        {
            var filename = "ErrorLog" + "_" + DateTime.Now.ToString("yyyyMMdd")+ ".txt";
            string logFilePath = Directory.GetCurrentDirectory() + "//ErrorLog//" + filename;
            // Get the current date and time for the log message
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Log message to be written
            StringBuilder objErrorMessage = new StringBuilder();
            objErrorMessage.Append($"[{timestamp}] : Message :" + message+"\r\n");
            objErrorMessage.Append($"[{timestamp}] : InnerException :" + innerexception + "\r\n");
            objErrorMessage.Append($"[{timestamp}] : StackTrace :" + stacktrace);
            string logMessage = $"[{timestamp}] : Message :"+message;
            if (!File.Exists(logFilePath))
            {
                // Create the file
                using (FileStream fs = File.Create(logFilePath))
                {
                    
                }
            }
            // Write the log message to the file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(objErrorMessage);
            }
        }

    }


}
