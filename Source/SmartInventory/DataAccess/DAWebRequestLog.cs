using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAWebRequestLog : Repository<WebRequestLog>
    {
        //DAWebRequestLog()
        //{

        //}
        //private static DAWebRequestLog objWebRequestLog = null;
        //private static readonly object lockObject = new object();
        //public static DAWebRequestLog Instance
        //{
        //    get
        //    {
        //        lock (lockObject)
        //        {
        //            if (objWebRequestLog == null)
        //            {
        //                objWebRequestLog = new DAWebRequestLog();
        //            }
        //        }
        //        return objWebRequestLog;
        //    }
        //}
        public List<WebRequestLog> GetWebRequestLog()
        {
            try
            {
                return repo.GetAll().ToList();

            }
            catch { throw; }
        }
        public WebRequestLog SaveWebRequestLog(WebRequestLog obj)
        {
            try
            {
                var objrequest = repo.GetAll(x => x.transaction_id == obj.transaction_id).FirstOrDefault();
                obj.created_on = DateTimeHelper.Now;
                if (objrequest == null)
                {
                    return repo.Insert(obj);
                }
                else
                {
                    objrequest.out_date_time = DateTimeHelper.Now;
                    objrequest.response = obj.response;
                    objrequest.latency = obj.latency;
                    return repo.Update(objrequest);
                }
            }
            catch { throw; }
        }

    }
}

