using Utility.DAUtility.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Utility.DAUtility
{
    #region Errorlog
    public class DAErrorLog : Repository<ErrorLog>
    {
        DAErrorLog()
        {

        }
        private static DAErrorLog objErrorlog = null;
        private static readonly object lockObject = new object();
        public static DAErrorLog Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objErrorlog == null)
                    {
                        objErrorlog = new DAErrorLog();
                    }
                }
                return objErrorlog;
            }
        }

        public ErrorLog SaveErrorLog(ErrorLog entity)
        {
            try
            {
                return repo.Insert(entity);
            }
            catch(Exception ex) 
            {
                throw ex; 
            }
        }

        public List<ErrorLog> GetErrorLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ErrorLog>("fn_get_error_log_details", new
                {
                    p_pageno = objErrorLogFilter.currentPage,
                    p_pagerecord = objErrorLogFilter.pageSize,
                    p_sortcolname = objErrorLogFilter.sort,
                    p_sorttype = objErrorLogFilter.orderBy,
                    p_searchBy = objErrorLogFilter.searchBy,
                    p_searchText = objErrorLogFilter.searchText,
                    p_searchfrom = objErrorLogFilter.fromDate,
                    p_searchto = objErrorLogFilter.toDate,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        
        public ErrorLog getFullText(int id)
        {
            try
            {
                var queryDetails = repo.GetById(m => m.id == id);
                if (queryDetails != null) { return queryDetails; }
              return null ;
            }
            catch { throw; }
        }
    }
    #endregion

    #region ApiErrorlog
    public class DAApiErrorLog : Repository<ApiErrorLog>
    {
        DAApiErrorLog()
        {

        }
        private static DAApiErrorLog objApiErrlog = null;
        private static readonly object lockObject = new object();
        public static DAApiErrorLog Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objApiErrlog == null)
                    {
                        objApiErrlog = new DAApiErrorLog();
                    }
                }
                return objApiErrlog;
            }
        }

        public ApiErrorLog SaveApiErrorLog(ApiErrorLog entity)
        {
            try
            {
                return repo.Insert(entity);
            }
            catch { throw; }

        }

        public List<ApiErrorLog> GetApiErrorLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ApiErrorLog>("fn_get_api_error_log_details", new
                {
                    p_pageno = objErrorLogFilter.currentPage,
                    p_pagerecord = objErrorLogFilter.pageSize,
                    p_sortcolname = objErrorLogFilter.sort,
                    p_sorttype = objErrorLogFilter.orderBy,
                    p_searchBy = objErrorLogFilter.searchBy,
                    p_searchText = objErrorLogFilter.searchText,
                    p_searchfrom = objErrorLogFilter.fromDate,
                    p_searchto = objErrorLogFilter.toDate,
                }, true);
                return lst;

            }
            catch { throw; }
        }


        public ApiErrorLog getApiFullText(int id)
        {
            try
            {
                var queryDetails = repo.GetById(m => m.id == id);
                if (queryDetails != null) { return queryDetails; }
                return null;
            }
            catch { throw; }
        }

    
    }
    #endregion

   
}
