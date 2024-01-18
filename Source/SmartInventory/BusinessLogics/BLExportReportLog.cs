using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess;

namespace BusinessLogics
{
    public class BLExportReportLog
    {
        public ExportReportLog SaveExportReportLog(ExportReportLog exportReportLog)
        {
            return new DAExportReportLog().SaveExportReportLog(exportReportLog);
        }
        public List<ExportReportLogInfo> GetExportExportLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval)
        {
            return new DAExportReportLog().GetExportExportLogList(objGridAttributes, user_id, timeInterval);
        }

    }
}
