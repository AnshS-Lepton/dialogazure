using Models;
using System;
using System.Collections.Generic;
using DataAccess;
using System.Data;

namespace BusinessLogics
{
    public class BLUploadSummary
    { 
        PostgreSQL postgreSQL; 
        public BLUploadSummary()
        {
            //postgreSQL = new PostgreSQL();
            //postgreSQL.PostgresNotificationEvent += NotifyUpdatedStatus;
        }
        public delegate void NotificationEventHandlerBusiness(dynamic data);

 
        public event NotificationEventHandlerBusiness BLUploadSummayEvent; 
        //private static BLUploadSummary objBLUploadSummary = null;
        //private static readonly object lockObject = new object();
        //public static BLUploadSummary Instance
        //{
        //    get
        //    {
        //        lock (lockObject)
        //        {
        //            if (objBLUploadSummary == null)
        //            {
        //                objBLUploadSummary = new BLUploadSummary();
        //            }
        //        }
        //        return objBLUploadSummary;
        //    }
        //}
        //public void UpdateStatus(UploadSummary summary)
        //{

        //    UpdateStatus(summary);
        //}
        
    }
}
