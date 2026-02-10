using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Notification
    {
        public int id { get; set; }
        public int? receiver_id { get; set; }
        public string receiver_fcm_key { get; set; }
        public DateTime? created_on { get; set; }
        public int? sender_id { get; set; }
        public string message { get; set; }
        public string notification_type { get; set; }
        public string notification_source_system { get; set; }
        public string feature_name { get; set; }
        public string notification_status { get; set; }
        public bool is_read { get; set; }
        public string title { get; set; }
        public string min_id { get; set; }
        public string matcode { get; set; }
        public string action { get; set; }
        public string username { get; set; }
        public string mobilenumber { get; set; }
        public DateTime? process_start_time { get; set; }
        public DateTime? process_end_time { get; set; }
        public bool is_app_notification { get; set; }
        public string ticket_name { get; set; }
        public bool is_processed { get; set; }

    }
    public class NotificationRequest
    {
        public int userId { get; set; }
        public int pageNo { get; set; }
        public int id { get; set; }
    }
    public class NotificationHistoryResponse
    {
        public List<Notification> Notifications { get; set; }
    }
}
