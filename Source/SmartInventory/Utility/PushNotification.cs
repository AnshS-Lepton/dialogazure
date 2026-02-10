using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Utility
{
    public class PushNotification
    {
        private PushNotification() { }

        private static PushNotification _instance;
        private static string firebaseCredentialFilePath = "";
        private static string firebaseTokenEndpoint = "";
        private static string firebaseNotificationEndpoint = "";
        public static PushNotification GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PushNotification();
                    firebaseTokenEndpoint = ConfigurationManager.AppSettings["FCMTokenEndpoint"].ToString();
                    firebaseNotificationEndpoint = ConfigurationManager.AppSettings["FCMNotificationEndpoint"].ToString();
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    firebaseCredentialFilePath = Path.Combine(baseDirectory, ConfigurationManager.AppSettings["FCMCredentialFilePath"].ToString());
                }
                return _instance;
            }
        }

        public FCMRequest SendNotification(Models.User user, string notificationMsg, string notificationType, int senderId, string featureName, string NetworkId, int ticketId, string ticketName, string moduleAbbr, string ticketStatus, string title)
        {
            FCMRequest fCMReq = new FCMRequest();
            ErrorLogHelper logHelper = new ErrorLogHelper();
            PartnerAPILog log = new PartnerAPILog();
            string accessToken = string.Empty;
            logHelper.WriteDebugLog(" FCM API start ");
            try
            {
                try
                {
                    accessToken = GetAccessToken(firebaseCredentialFilePath, firebaseTokenEndpoint);
                }
                catch (Exception ex)
                {
                    log.URL = firebaseTokenEndpoint;
                    log.request = firebaseCredentialFilePath;
                    log.response = ex.Message;
                    logHelper.PartnerAPILogs(log);
                    var x = "{\"message\":" + ex.Message + "}";
                    logHelper.WriteDebugLog("Exception GetAccessToken " + ex.Message);
                }
                //Create a JSON object for the request payload
                var payload = new
                {
                    message = new
                    {
                        token = user.fcmkey,
                        data = new
                        {
                            title = title,
                            body = notificationMsg,
                            action = "click",
                            userId = user.user_id.ToString(),
                            userName = user.user_name,
                            mobileNumber = MiscHelper.Decrypt(user.mobile_number),
                            notificationType = notificationType,
                            featureName = featureName,
                            ticketId = ticketId.ToString(),
                            networkId = NetworkId,
                            name = ticketName,
                            module_abbr = moduleAbbr,
                            ticket_status = ticketStatus
                        }
                    }
                };

                fCMReq.mobileNumber = payload.message.data.mobileNumber;
                fCMReq.body = notificationMsg;
                fCMReq.title = title;
                fCMReq.action = payload.message.data.action;
                fCMReq.userName = payload.message.data.userName;
                fCMReq.mobileNumber = payload.message.data.mobileNumber;
                fCMReq.notificationType = payload.message.data.notificationType;
                fCMReq.sender_id = senderId;
                fCMReq.feature_name = featureName;
                fCMReq.ticket_name = NetworkId;

                logHelper.WriteDebugLog(" FCM API Integration Payload" + payload.ToString());
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    logHelper.WriteDebugLog("Call FCM API URL " + firebaseNotificationEndpoint);
                    var response = client.PostAsync(firebaseNotificationEndpoint, content).Result;           

                }
            }
            catch (Exception ex)
            {
                log.URL = firebaseTokenEndpoint;
                log.request = firebaseCredentialFilePath;
                log.response = ex.Message;
                logHelper.PartnerAPILogs(log);
                var x = "{\"message\":" + ex.Message + "}";
                logHelper.WriteDebugLog("Exception FCMAPIIntegration " + ex.Message);
            }
            return fCMReq;
        }
        public static string GetAccessToken(string firebaseCredentialFilePath, string firebaseTokenEndpoint)
        {
            ErrorLogHelper logHelper = new ErrorLogHelper();
            PartnerAPILog log = new PartnerAPILog();
            try
            {
                var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential
                    .FromFile(firebaseCredentialFilePath)
                    .CreateScoped(firebaseTokenEndpoint);

                var token = googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;
                return token;
            }
            catch (Exception ex)
            {
                log.URL = firebaseTokenEndpoint;
                log.request = firebaseCredentialFilePath;
                log.response = ex.Message;
                logHelper.PartnerAPILogs(log);
                var x = "{\"message\":" + ex.Message + "}";
                logHelper.WriteDebugLog("Exception GetAccessToken " + ex.Message);
                return null;
            }
        }
    }
}
