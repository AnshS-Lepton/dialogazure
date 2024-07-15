using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using Models;
using System.Net;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.DirectoryServices;

namespace Utility
{
    public class ADOIDSecoAuth
    {
        public string GenerateSecoTokenOld(string userName, string password, bool isAgent, string source, out SecoApiResponse secoApiResponce, out ADOIDAuthentication aDOIDAuthentication)
        {
            secoApiResponce = new SecoApiResponse();
            string accessToken = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(ConfigurationManager.AppSettings["ADOIDSecoAuthURL"].ToString());
            //return accessToken = "abc";
            if (isAgent == true)
            {
                sb.Append("?state=state_test&scope=agent&client_id=" + ConfigurationManager.AppSettings["ADOIDSecoClientId"].ToString() + "&client_secret=" + ConfigurationManager.AppSettings["ADOIDSecoClientSecret"].ToString() + "&grant_type=password");
            }
            else
            {
                sb.Append("?state=state_test&scope=employee&client_id=" + ConfigurationManager.AppSettings["ADOIDSecoClientId"].ToString() + "&client_secret=" + ConfigurationManager.AppSettings["ADOIDSecoClientSecret"].ToString() + "&grant_type=password");
            }
            using (HttpClient client = new HttpClient())
            {
                //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                client.BaseAddress = new Uri(sb.ToString());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("username", (userName));
                client.DefaultRequestHeaders.Add("password", (password));
                HttpResponseMessage httpResponse = new HttpResponseMessage();
                try
                {
                    httpResponse = client.PostAsync("", null).Result;
                    var json = httpResponse.Content.ReadAsStringAsync().Result;
                    //var json = JsonConvert.SerializeObject(httpResponse);
                    secoApiResponce = JsonConvert.DeserializeObject<SecoApiResponse>(json);
                    accessToken = secoApiResponce.access_token;
                    ////store the authentication details in table
                    aDOIDAuthentication = new ADOIDAuthentication();
                    aDOIDAuthentication.user_id = 0;
                    aDOIDAuthentication.user_name = userName;
                    aDOIDAuthentication.access_token = secoApiResponce.access_token;
                    aDOIDAuthentication.token_type = secoApiResponce.token_type;
                    aDOIDAuthentication.expires_in = secoApiResponce.expires_in;
                    aDOIDAuthentication.refresh_token = secoApiResponce.refresh_token;
                    aDOIDAuthentication.scope = secoApiResponce.scope;
                    aDOIDAuthentication.source = source;
                    aDOIDAuthentication.mobile = !isAgent ? RemoveCountryCodeFromMobile(secoApiResponce.associate.mobile) : RemoveCountryCodeFromMobile(secoApiResponce.associate.ContactDetails.phoneNumber);
                    aDOIDAuthentication.email_id = !isAgent ? secoApiResponce.associate.Email : secoApiResponce.associate.ContactDetails.email;

                    //if (source.ToLower() == "mobile")
                    //{
                    //    ApiLogHelper.GetInstance.WriteDebugLog("ADOID Seco Response mobile:" + json.ToString());
                    //}
                    //else
                    //{
                    //    LogHelper.GetInstance.WriteDebugLog("ADOID Seco Response web" + json.ToString());
                    //}

                }
                catch (Exception ex)
                {
                    aDOIDAuthentication = null;
                    //if (source.ToLower() == "mobile")
                    //{
                    //    ApiErrorLogInput errorLogInput = new ApiErrorLogInput();
                    //    errorLogInput.exception = ex;
                    //    errorLogInput.UserName = userName;
                    //    errorLogInput.ERR_DESCRIPTION = "ADOID seco authentication calling error ";
                    //    ApiLogHelper.GetInstance.WriteErrorLog(errorLogInput);
                    //}
                    //else
                    //{
                    //    ErrorLogInput errorLogInput = new ErrorLogInput();
                    //    errorLogInput.exception = ex;
                    //    errorLogInput.UserName = userName;
                    //    errorLogInput.ERR_DESCRIPTION = "ADOID seco authentication calling error ";
                    //    LogHelper.GetInstance.WriteErrorLog(errorLogInput);
                    //}
                    secoApiResponce = null;
                    accessToken = string.Empty;
                }
            }
            return accessToken;
        }

        public string GenerateRefreshToken(string refreshToken, string uid, string source, out SecoApiResponse secoApiResponce)
        {
            secoApiResponce = new SecoApiResponse();
            string accessToken = string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(ConfigurationManager.AppSettings["ADOIDSecoAuthURL"].ToString());
            sb.Append("?client_id=" + ConfigurationManager.AppSettings["ADOIDSecoClientId"].ToString() + "&client_secret=" + ConfigurationManager.AppSettings["ADOIDSecoClientSecret"].ToString() + "&refresh_token&refresh_token=" + refreshToken);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(sb.ToString());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage httpResponse = new HttpResponseMessage();
                try
                {
                    //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    new ErrorLogHelper().WriteDebugLog("Request Url:" + sb.ToString());
                    httpResponse = client.PostAsync("", null).Result;
                    var json = JsonConvert.SerializeObject(httpResponse);
                    secoApiResponce = JsonConvert.DeserializeObject<SecoApiResponse>(json);
                    accessToken = secoApiResponce.access_token;

                    new ErrorLogHelper().WriteDebugLog("ADOID Seco Request URL:" + sb.ToString());
                }
                catch (Exception ex)
                {
                    new ErrorLogHelper().ApiLogWriter("GenerateRefreshToken()", "ADOIDSecoAuth", sb.ToString(), ex);
                    secoApiResponce = null;
                    accessToken = string.Empty;
                }
            }
            return accessToken;
        }

        public string RemoveCountryCodeFromMobile(string phonenumber)
        {
            List<string> countries_list = new List<string>();
            countries_list.Add("+1");
            countries_list.Add("+91");
            foreach (var country in countries_list)
            {
                phonenumber = phonenumber.Replace(country, "");
            }
            return phonenumber;
        }

        public string GenerateSecoToken(string userName, string password, bool isAgent, string source, out SecoApiResponse secoApiResponce, out ADOIDAuthentication aDOIDAuthentication)
        {
            secoApiResponce = new SecoApiResponse();
            string accessToken = string.Empty;
            string SoapUrl = string.Empty;
            string HeaderUsername = string.Empty;
            string HeaderPassword = string.Empty;
            string ClientId = string.Empty;
            try
            {
                XmlDocument xmlDocumentSoapRequest = new XmlDocument();
                XmlDocument xmlDocumentSoapResponse = new XmlDocument();

                if (isAgent)
                {
                    SoapUrl = ConfigurationManager.AppSettings["OIDAuthURL"].ToString();
                    HeaderUsername = ConfigurationManager.AppSettings["OIDHeaderUserName"];
                    HeaderPassword = ConfigurationManager.AppSettings["OIDHeaderPassword"];
                    ClientId = ConfigurationManager.AppSettings["OIDClientId"];
                    xmlDocumentSoapRequest = OIDUserSoapXml(userName, password, ClientId);
                }
                else
                {
                    SoapUrl = ConfigurationManager.AppSettings["ADAuthURL"].ToString();
                    HeaderUsername = ConfigurationManager.AppSettings["ADHeaderUserName"];
                    HeaderPassword = ConfigurationManager.AppSettings["ADHeaderPassword"];
                    ClientId = ConfigurationManager.AppSettings["ADClientId"];
                    xmlDocumentSoapRequest = ADUserSoapXml(userName, password, ClientId);
                }

                WriteDebugLog((isAgent ? "OID" : "AD") + " Soap URL: " + SoapUrl);
                WriteDebugLog("Soap Request Username: " + userName);
                xmlDocumentSoapResponse = InvokeSoapService(SoapUrl, xmlDocumentSoapRequest, HeaderUsername, HeaderPassword);
                WriteDebugLog("Soap Response: " + xmlDocumentSoapResponse.InnerXml);
                accessToken = GetADOIDResponseObject(xmlDocumentSoapResponse, out aDOIDAuthentication, isAgent, userName);
            }
            catch (Exception ex)
            {
                WriteDebugLog("Soap Exception " + ex.Message + ex.StackTrace.ToString());
                aDOIDAuthentication = null;
                secoApiResponce = null;
                accessToken = string.Empty;
            }
            return accessToken;
        }
        public XmlDocument InvokeSoapService(string url, XmlDocument xmlSoapReq, string headerUserName = "", string headerPassword = "")
        {
            XmlDocument xmlDocument = new XmlDocument();

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            HttpWebRequest request = CreateSOAPWebRequest(url, headerUserName, headerPassword);

            ////for Logging the input request
            //ApiErrorLogInput objErrorLog = new ApiErrorLogInput();
            //Exception ex1 = new Exception();
            //objErrorLog.userId = 0;  // in case token does not exists pass userId
            //objErrorLog.UserName = userName;
            //objErrorLog.fromPage = "HPSMHelper";
            //objErrorLog.fromMethod = "InvokeSoapService (" + apiName + " WSDL Request :)";
            //objErrorLog.requestData = xmlSoapReq.InnerXml;
            //objErrorLog.exception = ex1;
            //ApiLogHelper.GetInstance.WriteErrorLog(objErrorLog);
            //Call the WebRequest for the same
            using (Stream stream = request.GetRequestStream())
            {
                xmlSoapReq.Save(stream);
            }
            using (WebResponse Serviceres = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    var ServiceResult = rd.ReadToEnd();
                    xmlDocument.LoadXml(ServiceResult);
                    //    objErrorLog = new ApiErrorLogInput();
                    //    ex1 = new Exception();
                    //    objErrorLog.userId = 0;  // in case token does not exists pass userId
                    //    objErrorLog.UserName = userName;
                    //    objErrorLog.fromPage = "HPSMHelper";
                    //    objErrorLog.fromMethod = "InvokeService (" + apiName + " WSDL Response)";
                    //    objErrorLog.requestData = ServiceResult;
                    //    objErrorLog.exception = ex1;
                    //    ApiLogHelper.GetInstance.WriteErrorLog(objErrorLog);
                }
            }

            return xmlDocument;
        }

        private XmlDocument ADUserSoapXml(string userName, string password, string clientId)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://services.jio.com/\">");
            sb.Append("<soapenv:Header/>");
            sb.Append("<soapenv:Body>");
            sb.Append("<ser:ldapConnection>");
            sb.Append("<arg0>" + userName + "</arg0>");
            sb.Append("<arg1>" + password + "</arg1>");
            sb.Append("<arg2>" + clientId + "</arg2>");
            sb.Append("</ser:ldapConnection>");
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());
            return xmlDocument;
        }

        private XmlDocument OIDUserSoapXml(string userName, string password, string clientId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://services.reliance.com/\">");
            sb.Append("<soapenv:Header/>");
            sb.Append("<soapenv:Body>");
            sb.Append("<ser:ldapConenction>");
            sb.Append("<arg0>" + userName + "</arg0>");
            sb.Append("<arg1>" + password + "</arg1>");
            sb.Append("<arg2>" + clientId + "</arg2>");
            sb.Append("</ser:ldapConenction>");
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(sb.ToString());
            return xmlDocument;
        }

        public HttpWebRequest CreateSOAPWebRequest(string wsdlUrl, string headerUserName = "", string headerPassword = "")
        {

            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(wsdlUrl);
            if (!string.IsNullOrEmpty(headerUserName) && !string.IsNullOrEmpty(headerPassword))
            {
                //Req.Credentials = new NetworkCredential(headerUserName, headerPassword);
                Req.Headers.Add("Username", headerUserName);
                Req.Headers.Add("Password", headerPassword);
            }
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            Req.Method = "POST";
            return Req;
        }

        private string GetADOIDResponseObject(XmlDocument xmlDocument, out ADOIDAuthentication aDOIDAuthentication, bool isAgent, string userName)
        {
            try
            {
                XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);
                XmlNodeList xnList = null;
                string xmlInnerText = string.Empty;
                string AuthStatus = string.Empty;
                aDOIDAuthentication = new ADOIDAuthentication();
                aDOIDAuthentication.user_id = 0;
                aDOIDAuthentication.user_name = userName;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (isAgent)
                {
                    manager.AddNamespace("ns2", "http://services.reliance.com/");
                    xmlInnerText = xmlDocument.SelectSingleNode("//ns2:ldapConenctionResponse", manager).InnerText;
                    xnList = xmlDocument.SelectNodes("//ns2:ldapConenctionResponse/return", manager);
                }
                else
                {
                    manager.AddNamespace("ns2", "http://services.jio.com/");
                    xmlInnerText = xmlDocument.SelectSingleNode("//ns2:ldapConnectionResponse", manager).InnerText;
                    xnList = xmlDocument.SelectNodes("//ns2:ldapConnectionResponse/return", manager);
                }
                aDOIDAuthentication.access_token = xmlInnerText;

                foreach (XmlElement xmlNodeList in xnList)
                {

                    string[] arr = xmlNodeList.InnerText.Split('=');
                    dic.Add(arr[0], arr[1]);
                }
                if (dic.ContainsKey("authnstatus"))
                {
                    AuthStatus = Convert.ToString(dic["authnstatus"]);
                }
                if (AuthStatus.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {

                    if (dic.ContainsKey("mobile"))
                    {
                        aDOIDAuthentication.mobile = RemoveCountryCodeFromMobile(Convert.ToString(dic["mobile"]));
                    }
                    if (dic.ContainsKey("mail"))
                    {
                        aDOIDAuthentication.email_id = Convert.ToString(dic["mail"]);
                    }
                }
                else
                {
                    aDOIDAuthentication.access_token = String.Empty;

                    aDOIDAuthentication = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return aDOIDAuthentication.access_token;

        }

        public void WriteDebugLog(string LogMessage)
        {
            string errDesc = string.Empty;

            using (StreamWriter sw = File.AppendText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLogs\\DebugLog-" + DateTimeHelper.Now.ToString("dd-MM-yyyy") + ".txt")))
            {
                sw.WriteLine("\r\nLog Entry:==========>");
                sw.WriteLine("Log on Time: {0}", DateTimeHelper.Now);
                sw.WriteLine("Log Message: {0}", LogMessage);
            }
        }

        
       
    }
    public class SecoApiResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public employee associate { get; set; }
    }
    public class employee
    {
        public string agentCode { get; set; }
        public string FirstName { get; set; }
        public string lastName { get; set; }
        public string displayname { get; set; }
        public string username { get; set; }
        public string Email { get; set; }
        public string employeeID { get; set; }
        public string mobile { get; set; }
        public string streetAddress { get; set; }
        public string dn { get; set; }

        public ContactDetails ContactDetails { get; set; }
    }
    public class ContactDetails
    {
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }


}
