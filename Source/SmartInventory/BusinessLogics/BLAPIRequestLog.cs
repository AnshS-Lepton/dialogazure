using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLAPIRequestLog
    {
        public APIRequestLog SaveApiRequestLog(APIRequestLog obj)
        {
            return new DAAPIRequestLog().SaveApiRequestLog(obj);
        }
        public List<APIRequestLog> GetApiRequestLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            return new DAAPIRequestLog().GetApiRequestLogDetails(objErrorLogFilter);
        }

        public List<GisApiLog> GetGisApiLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            return new DAAPIRequestLog().GetGisApiLogDetails(objErrorLogFilter);
        }
        public APIRequestLog getApiFullText(int id)
        {
            return new DAAPIRequestLog().getApiFullText(id);

        }
        
        public APIRequestLog SavePartnerApiRequestLog(APIRequestLog obj)
        {
            return new DAAPIRequestLog().SavePartnerApiRequestLog(obj);
        }
    }
    public class BLAPIRequest
    {
        public GisApiLog getGisApiLogDetailById(int id)
        {
            return new DAAPIRequest().getGisApiLogDetailById(id);

        }
    }
}
