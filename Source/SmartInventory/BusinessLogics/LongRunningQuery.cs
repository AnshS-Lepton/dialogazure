using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class LongRunningQuery
    {
    
        public List<LongRunningQueries> GetLongRunningQueriesData(ViewLongRunningQueries objViewLongRunningQueries)
        {
            return new LongRunningQueriesData().GetLongRunningQueriesData(objViewLongRunningQueries);
        }
        public LongRunningQueries getLongRunningQueryDetailById(int id)
            {
                return new LongRunningQueriesData().getLongRunningQueryDetailById(id);
            }
           
        public bool TerminateQueryByPid(int id)
        {
            return new LongRunningQueriesData().TerminateQueryByPid(id);
        }
    }  
}
