using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLNotification
    {
        public List<Notification> GetNotificationHistory(int userId)
        {
            return new DANotification().GetNotificationHistory(userId);
        }
        public Notification SaveNotification(int userId, FCMRequest fcmreq)
        {
            return new DANotification().saveNotification(userId, fcmreq);
        }
    }
}
