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
    
    public class DAAssociationReportLog : Repository<AssociationReportLog>
    {
        public AssociationReportLog SaveAssociationReportLog(AssociationReportLog associationReportLog)
        {
            try
            {
                var objAssociationReportLog = repo.Get(x => x.sno == associationReportLog.sno);
                if (objAssociationReportLog != null)
                {
                    repo.Update(associationReportLog);
                }
                else
                {
                    repo.Insert(associationReportLog);
                }
                return associationReportLog;

            }
            catch (Exception ex)
            { throw ex; }
        }

        public List<AssociationReportLogInfo> GetAssociationAssociationLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval)
        {
            try
            {

                return repo.ExecuteProcedure<AssociationReportLogInfo>("fn_get_entity_association_log", new
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
