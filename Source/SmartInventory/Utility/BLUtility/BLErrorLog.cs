using Utility.DAUtility;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.BlUtility
{
    public class BLErrorLog
    {
        #region ErrorLog
        public static void SaveErrorLog(ErrorLog entity)
        {
            DAErrorLog.Instance.SaveErrorLog(entity);
        }
        public List<ErrorLog> GetErrorLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            return DAErrorLog.Instance.GetErrorLogDetails(objErrorLogFilter);

        }
       
        public ErrorLog getFullText(int id)
        {
            return DAErrorLog.Instance.getFullText(id);

        }
        #endregion


        #region ApiErrorLog
        public static void SaveApiErrorLog(ApiErrorLog entity)
        {
            DAApiErrorLog.Instance.SaveApiErrorLog(entity);
        }
        public List<ApiErrorLog> GetApiErrorLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            return DAApiErrorLog.Instance.GetApiErrorLogDetails(objErrorLogFilter);

        }
        public ApiErrorLog getApiFullText(int id)
        {
            return DAApiErrorLog.Instance.getApiFullText(id);

        }
        public bool SaveGisApiLogs(GisApiLogs objGisapiLogs)
        {
            return new DAGisUtility().SaveGisApiLogs(objGisapiLogs);
        }

        #endregion


    }
}
