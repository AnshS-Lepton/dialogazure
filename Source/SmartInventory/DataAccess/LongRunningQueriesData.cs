using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class LongRunningQueriesData : Repository<LongRunningQueries>
    {
       
        public List<LongRunningQueries> GetLongRunningQueriesData(ViewLongRunningQueries objViewLongRunningQueries)
        {
            try
            {
                var lst = repo.ExecuteProcedure<LongRunningQueries>("fn_long_running_queries", new
                {
                    p_pageno = objViewLongRunningQueries.currentPage,
                    p_pagerecord = objViewLongRunningQueries.pageSize,
                    p_sortcolname = objViewLongRunningQueries.sort,
                    p_sorttype = objViewLongRunningQueries.orderBy,
                    p_searchBy = objViewLongRunningQueries.searchBy,
                    p_searchText = objViewLongRunningQueries.searchText,
                    p_searchfrom = objViewLongRunningQueries.fromDate,
                    p_searchto = objViewLongRunningQueries.toDate,
                    p_runningtime = objViewLongRunningQueries.runningTime,
                }, true);
                return lst; 

            }

            catch { throw; }
        }

        public LongRunningQueries getLongRunningQueryDetailById(int id)
        {
            try
            {
                var longRunningQueryDetailById = repo.GetById(m => m.pid == id);
                if (longRunningQueryDetailById != null)
                { return longRunningQueryDetailById; }
                return null;
            }
            catch { throw; }
        }

        public bool TerminateQueryByPid(int id)
        {
            try
            {
                var result = repo.ExecuteProcedure<bool>("pg_terminate_backend", new { pid = id },false);
                return result != null && result.Count > 0 ? true : false;
            }
            catch { throw; }

        }

    }
}
