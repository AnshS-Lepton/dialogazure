using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Models.WFM
{
    public class Email
    {
        public static bool sendMail(string Status, string To, string Subject, string Message)
        {
            bool isMailSent = false;
            try
            {
                //To = "durgesh.singh@leptonsoftware.com";
                string From = ConfigurationManager.AppSettings["From"];
                //string FromTitle = "Notification";
                StringBuilder sb = new StringBuilder();

                sb.Append("<p class='MsoNoSpacing'>" + Message + "</p>");
                sb.AppendLine();

                sb.Append("<br />");
                sb.AppendLine();

                sb.Append("<br />");
                sb.AppendLine();

                sb.Append("<br />");
                sb.AppendLine();


                sb.Append("<span><b>System Admin</b></span>");
                sb.Append("<br />");
                sb.Append("<span style='color:#124191;'>(Converge Team)</span>");
                sb.Append("<br />");
                sb.Append("<span>Phone – 0000001</span>");
                sb.Append("<br />");
                sb.Append("<span>Mobile – 999999999999/span>");
                sb.Append("<br />");
                sb.Append("<span>Email : test1@xyz.com</span>");
                sb.Append("<br />");
                string Body = sb.ToString();

                string Password = ConfigurationManager.AppSettings["Password"];
                string Host = ConfigurationManager.AppSettings["Host"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);

                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(From, "Converge");
                    message.To.Add(new MailAddress(To));
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.Body = Body;
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(From, Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                    isMailSent = true;
                }
                catch (Exception ex)
                {

                }

            }
            catch (Exception ex)
            {

            }
            return isMailSent;
        }
        public static bool sendMailnnd(string To, string Subject, string Message)
        {
            bool isMailSent = false;
            try
            {
                string From = ConfigurationManager.AppSettings["From"];
                //string FromTitle = "Notification";
                StringBuilder sb = new StringBuilder();
                sb.Append("<p class='MsoNoSpacing'>Dear Sir,</p>");
                sb.AppendLine();
                sb.Append("<p class='MsoNoSpacing'>Kindly take necessary action. </p>");
                sb.AppendLine();
                sb.Append("<pre class='MsoNoSpacing'>" + Message + "</pre>");
                sb.AppendLine();
                sb.Append("<br />");
                sb.AppendLine();
                sb.Append("<br />");
                sb.AppendLine();
                sb.Append("<br />");
                sb.AppendLine();

                sb.Append("<span><b>System Admin</b></span>");
                sb.Append("<br />");
                sb.Append("<span style='color:#124191;'>(Converge Team)</span>");
                sb.Append("<br />");
                sb.Append("<span>Phone – 0000001</span>");
                sb.Append("<br />");
                sb.Append("<span>Mobile – 999999999999</span>");
                sb.Append("<br />");
                sb.Append("<span>Email : test1@xyz.com</span>");
                sb.Append("<br />");
                string Body = sb.ToString();
                string Password = ConfigurationManager.AppSettings["Password"];
                string Host = ConfigurationManager.AppSettings["Host"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]);
                try
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress(From, "Converge");
                    string[] Multi = To.Split(',');
                    foreach (string s in Multi)
                    {
                        message.To.Add(new MailAddress(s));
                    }
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.Body = Body;
                    smtp.Port = Port;
                    smtp.Host = Host;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(From, Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                    isMailSent = true;
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {
            }
            return isMailSent;
        }

    }

    public static class SMS
    {
        static string smsusername = "";
        static string smspassword = "";
        static string originator = "";
        static string smsapi = "";
        static SMS()
        {
            smsusername = Convert.ToString(ConfigurationManager.AppSettings["smsusername"]);
            smspassword = Convert.ToString(ConfigurationManager.AppSettings["smspassword"]);
            originator = Convert.ToString(ConfigurationManager.AppSettings["originator"]);
            smsapi = ConfigurationManager.AppSettings["smsapi"];
        }
       
        public static int SendSms(ref string smsremark, int MobileNumabr)
        {
            int status = 2;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string requestXml = "username=convergeapie&password=5fb1b60aa470d2ad2118d8c9b659ca97&originator=CONVERGE&fullmesg=Test message 3&mobilenum=9540022085A";
                string destinationUrl = ConfigurationManager.AppSettings["smsapi"]; ;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    if (responseStr.Contains("ACK"))
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }

                    smsremark = responseStr;

                }
                else
                {
                    smsremark = "Error in sending sms, Response code OK is not recieved from api !";
                }
            }
            catch (Exception ex)
            {
                smsremark = "Error oocured while sending sms";
                status = 2;
            }
            return status;

        }

        public static int SendSms(ref string smsremark, string MobileNumabr, string Message)
        {
            int status = 2;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string requestXml = "username=" + smsusername + "&password=" + smspassword + "&originator=" + originator + "&fullmesg=" + Message + "&mobilenum=" + MobileNumabr + "";
                string destinationUrl = ConfigurationManager.AppSettings["smsapi"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    if (responseStr.Contains("ACK"))
                    {
                        status = 1;
                    }
                    else
                    {
                        status = 0;
                    }
                    smsremark = responseStr;
                }
                else
                {
                    smsremark = "Error in sending sms, Response code OK is not recieved from api !";
                }
            }
            catch (Exception ex)
            {
                smsremark = "Error oocured while sending sms";
                status = 2;
            }
            return status;

        }


        public static bool SendSms(string MobileNumabr, string Message)
        {
            bool status = false;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                string requestXml = "username=" + smsusername + "&password=" + smspassword + "&originator=" + originator + "&fullmesg=" + Message + "&mobilenum=" + MobileNumabr + "";
                string destinationUrl = ConfigurationManager.AppSettings["smsapi"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
                request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    if (responseStr.Contains("ACK"))
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }

            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;

        }

        public static bool SendSms(OTPResponse objOTPResponse, string MobileNumber, string Message, out string responserMessage)
        {
            responserMessage = "";
            bool status = false;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.SecurityProtocol &= ~System.Net.SecurityProtocolType.Ssl3;
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                var username = smsusername;
                var password = smspassword;
                string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(username + ":" + password));
                string destinationUrl = smsapi;
                Guid uniqueId = Guid.NewGuid();
                string uniqueidentifier = uniqueId.ToString();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                byte[] bytes;
                bytes = bytes = Encoding.UTF8.GetBytes(SMS.GetJsonString(MobileNumber, Message));
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                request.Headers.Add("Authorization", "Basic " + encoded);
                request.Headers.Add("x-source-system", "camel");
                request.Headers.Add("x-correlation-conversationid", uniqueidentifier);
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();

                    string responseStr = new StreamReader(responseStream).ReadToEnd();
                    SMSResponse objSMSResponse = JsonConvert.DeserializeObject<SMSResponse>(responseStr);
                    List<CharacteristicValue> objcharacteristicValuesList = new List<CharacteristicValue>();
                    objcharacteristicValuesList = objSMSResponse.parts.specification.characteristicValue;
                    foreach (CharacteristicValue objCharacteristicValue in objcharacteristicValuesList)
                    {
                        if (objCharacteristicValue.characteristicName == "ResponseCode")
                        {
                            if (objCharacteristicValue.value == "1000")
                            {
                                // responserMessage = "MobileNumber : " + MobileNumber + "  : Response --" + responseStr;                                
                                responserMessage = "OTP sent successfully on your registered mobile number.";
                                status = true;
                            }
                            else
                            {

                                status = false;
                            }

                        }
                        if (objCharacteristicValue.characteristicName == "ResponseDescription")
                        {
                            if (status == false)
                            {
                                // responserMessage = "MobileNumber : " + MobileNumber + "  : Response --" + responseStr;                                
                                responserMessage = objCharacteristicValue.value.ToString();

                            }


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;

        }

        public static string GetJsonString(string MobileNumber, string Message)
        {
            SMSRequest objSMSRequest = new SMSRequest();
            Id objId = new Id();
            objId.value = MobileNumber;
            List<Id> objListId = new List<Id>();
            objListId.Add(objId);
            Receiver objReceiver = new Receiver();
            objReceiver.id = objListId;
            Roles objRoles = new Roles();
            objRoles.receiver = objReceiver;
            objSMSRequest.roles = objRoles;
            Body objBody = new Body();
            objBody.text = Message;
            Trailer objTrailer = new Trailer();
            objTrailer.text = "SAF-SMART";
            Parts objParts = new Parts();
            objParts.trailer = objTrailer;
            objParts.body = objBody;
            objSMSRequest.parts = objParts;


            return JsonConvert.SerializeObject(objSMSRequest);

        }
    }
    #region  Safaricom SMS API Requrest Response 
    public class SMSRequest
    {
        public Roles roles { get; set; }
        public Parts parts { get; set; }
    }
    public class Roles
    {
        public Receiver receiver { get; set; }
    }
    public class Receiver
    {
        public List<Id> id { get; set; }
    }
    public class Parts
    {
        public Body body { get; set; }
        public Trailer trailer { get; set; }


    }
    public class Body
    {
        public string text { get; set; }
    }
    public class Trailer
    {
        public string text { get; set; }
    }
    public class Id
    {
        public string value { get; set; }
    }
    public class CharacteristicValue
    {
        public string characteristicName { get; set; }
        public string value { get; set; }
    }
    public class SMSResponse
    {
        public ResponseParts parts { get; set; }
    }
    public class Specification
    {
        public List<CharacteristicValue> characteristicValue { get; set; }
    }
    public class ResponseParts
    {

        public Specification specification { get; set; }
    }
    #endregion

}
