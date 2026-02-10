using DataAccess.DBHelpers;
using Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DANotification : Repository<Notification>
    {
        public List<Notification> GetNotificationHistory(int userId)
        {
            try
            {
                return repo.ExecuteProcedure<Notification>("fn_get_notification_history_details", new { p_user_id = userId }, true);
            }
            catch { throw; }
        }
        public Notification saveNotification(int userId, FCMRequest fcmreq)
        {
            try
            {
                Notification notification = new Notification();
                notification.receiver_id = userId;
                notification.notification_source_system = "SAP";
                notification.feature_name = fcmreq.feature_name;
                notification.receiver_fcm_key = "test";
                notification.created_on = DateTime.Now;
                notification.mobilenumber = fcmreq.mobileNumber;
                notification.min_id = fcmreq.minId;
                notification.username = fcmreq.userName;
                notification.action = fcmreq.action;
                notification.title = fcmreq.title;
                notification.matcode = fcmreq.matCode;
                notification.notification_type = fcmreq.notificationType;
                notification.message = fcmreq.body;
                notification.sender_id = fcmreq.sender_id;
                notification.process_start_time = fcmreq.process_start_time;
                notification.process_end_time = fcmreq.process_end_time;
                notification.is_app_notification = fcmreq.is_app_notification;
                notification.ticket_name = fcmreq.ticket_name;
                return repo.Insert(notification);
            }
            catch { throw; }
        }
    }
}
