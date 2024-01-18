using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{

    public class BLWebRequestLog
    {
        DAWebRequestLog obj = new DAWebRequestLog();
        public List<WebRequestLog> GetWebRequestLog()
        {
            return obj.GetWebRequestLog();

        }
        public WebRequestLog SaveWebRequestLog(WebRequestLog obj)
        {
            return new DAWebRequestLog().SaveWebRequestLog(obj);
        }
    }
}

