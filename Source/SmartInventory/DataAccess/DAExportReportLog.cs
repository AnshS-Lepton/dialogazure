using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using Models.WFM;
using DataAccess.DBHelpers;
using DataAccess.DBContext;
using DataAccess.Contracts;
using System.Data;
using Newtonsoft.Json;
using Npgsql;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Net;
using Utility;

namespace DataAccess
{
    public class DAExportReportLog : Repository<ExportReportLog>
    {
        public ExportReportLog SaveExportReportLog(ExportReportLog exportReportLog)
        {
            try
            {
                var objExportReportLog = repo.Get(x => x.sno == exportReportLog.sno);
                if (objExportReportLog != null)
                {
                    repo.Update(exportReportLog);
                }
                else
                {
                    repo.Insert(exportReportLog);
                }
                return exportReportLog;

            }
            catch (Exception ex)
            { throw ex; }
        }

        public List<ExportReportLogInfo> GetExportExportLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval, string log_type)
        {
            try
            {

                return repo.ExecuteProcedure<ExportReportLogInfo>("fn_get_entity_export_log", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_user_id = user_id,
                    p_timeInterval = timeInterval,
                    p_log_type = log_type
                }, true);
            }
            catch { throw; }
        }
        public List<ExportReportLogInfo> GetAuditlogExportExportLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval)
        {
            try
            {

                return repo.ExecuteProcedure<ExportReportLogInfo>("fn_get_auditlog_entity_export_log", new
                {
                    p_searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    p_user_id = user_id,
                    p_timeInterval = timeInterval
                }, true);
            }
            catch { throw; }
        }
    }
}
