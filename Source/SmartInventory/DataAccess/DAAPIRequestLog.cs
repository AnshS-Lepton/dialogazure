using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAAPIRequestLog : Repository<APIRequestLog>
    {
        public APIRequestLog SaveApiRequestLog(APIRequestLog obj)
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
        public APIRequestLog SavePartnerApiRequestLog(APIRequestLog obj)
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

        public List<APIRequestLog> GetApiRequestLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<APIRequestLog>("fn_get_api_request_log_details", new
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

        public List<GisApiLog> GetGisApiLogDetails(ViewErrorLogFilter objErrorLogFilter)
        {
            try
            {
                var lst = repo.ExecuteProcedure<GisApiLog>("fn_get_gis_api_logs", new
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

        public APIRequestLog getApiFullText(int id)
        {
            try
            {
                var queryDetails = repo.GetById(m => m.log_id == id);
                if (queryDetails != null) { return queryDetails; }
                return null;
            }
            catch { throw; }
        }
       
    }
    public class DAAPIRequest : Repository<GisApiLog>
    {
        public GisApiLog getGisApiLogDetailById(int id)
        {
            try
            {
                var gisApiLogDetailById = repo.GetById(m => m.id == id);
                if (gisApiLogDetailById != null)
                { return gisApiLogDetailById; }
                return null;
            }
            catch { throw; }
        }
    }
}
