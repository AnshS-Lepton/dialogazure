using BusinessLogics;
using BusinessLogics.WFM;
using Models.WFM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class NotificationHelper
    {
        public void sendNotification(int userId, int IssueId, string mess = null, string header = null, int role = 0)
        {
            NOTIFICATION_ALERTS alerts = new NOTIFICATION_ALERTS();
            alerts.route_assignment_id = IssueId;

            alerts.last_notified_on = DateTime.Now;
            alerts.seq_id = 0;
            string message = "";
            if (header != null)
            {
                message = mess;
                alerts.notification_type = header;
            }
            else if (mess != null)
            {
                message = mess;
                alerts.notification_type = "Task Closed";
            }
            else
            {
                message = "New Task Is Assigned.";
                alerts.notification_type = "Task Assigned";
            }
            //BLRoute.SaveLastNotifiedOn(alerts);


            VW_USER_MANAGER_RELATION objUserData = BLUser.GetUserManagerFcmKey(userId);
            if (objUserData == null)
                return;

            string fcm_key = "";
            string[] deviceIds = null;
            if (role != null && role == 2)
            {
                if (message.Contains(userId.ToString()))
                {
                    message = message.Replace(userId.ToString(), objUserData.full_name);
                }


                fcm_key = objUserData.manager_fcmkey;
                deviceIds = new string[] { objUserData.manager_fcmkey };
            }
            else
            {
                fcm_key = objUserData.fcm_key;
                deviceIds = new string[] { objUserData.fcm_key };
            }



            SendNotification(deviceIds, alerts.notification_type, message);

            SaveNotificationMessageInHistory(alerts, userId, message, fcm_key);
        }

        private void SaveNotificationMessageInHistory(NOTIFICATION_ALERTS alerts, int userId, string message, string fcm_key)
        {
            NOTIFICATION_ALERTS_HISTORY objNotification = new NOTIFICATION_ALERTS_HISTORY();
            objNotification.fcm_key = fcm_key;
            objNotification.message = message;
            objNotification.route_assignment_id = alerts.route_assignment_id;
            objNotification.notification_type = alerts.notification_type;
            objNotification.user_id = userId;
            objNotification.seq_id = alerts.seq_id;
            BLWFMTicket.SaveNotificationMessageInHistory(objNotification);

        }

        public bool SendNotification(string[] _tokenId, string _title, string _message)
        {
            bool result = false;
            string serverKey = ConfigurationManager.AppSettings["serverKey"];
            string senderId = ConfigurationManager.AppSettings["senderId"];
            string fcmURL = ConfigurationManager.AppSettings["fcmURL"];

            WebRequest tRequest = WebRequest.Create(fcmURL);
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";
            var objNotification = new
            {
                //Token the device you want to push notification to
                registration_ids = _tokenId,
                notification = new
                {
                    title = _title,
                    body = _message
                }
            };
            string jsonNotificationFormat = Newtonsoft.Json.JsonConvert.SerializeObject(objNotification);

            Byte[] byteArray = Encoding.UTF8.GetBytes(jsonNotificationFormat);
            tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            tRequest.ContentLength = byteArray.Length;
            tRequest.ContentType = "application/json";
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);

                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String responseFromFirebaseServer = tReader.ReadToEnd();

                            FCMResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<FCMResponse>(responseFromFirebaseServer);
                            if (response.success >= 1)
                            {
                                result = true;
                            }
                            else if (response.failure >= 1)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }

            return result;
        }

    }
}