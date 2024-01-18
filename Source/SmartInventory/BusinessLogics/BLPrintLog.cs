using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

namespace BusinessLogics
{
    public class BLPrintLog
    {
        //public List<PrintExportLog> GetPrintLog(int user_id)
        //{
        //    return new DAPrintLog().GetPrintLog(user_id);
        //}
        public PrintExportLog SavePrintLog(PrintExportLog printLog)
        {
            return new DAPrintLog().SavePrintLog(printLog);
        }
        public List<PrintExportLogInfo> GetPrintExportLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval)
        {
            return new DAPrintLog().GetPrintExportLogList(objGridAttributes, user_id, timeInterval);
        }
        //public DbMessage ValidatePrintArea(string geom, int userId, string geomType, double buff_Radius)
        //{
        //    return new DAPrintLog().ValidatePrintArea(geom, userId, geomType, buff_Radius);
        //}
        public string GetPrintExportLogJson(int p_id)
        {
            return new DAPrintLog().GetPrintExportLogJson( p_id);
        }
      
        public GeometryDetail GetPrintMapGeoms(int p_historyID)
        {
            return new DAPrintLog().GetPrintMapGeoms(p_historyID);
        }


        public GeometryDetail GetEntityGeometry(int p_historyID)
        {
            return new DAPrintLog().GetPrintMapGeoms(p_historyID);
        }
        public bool DeleteexportlogById(int p_id, int userId)
        {
            return new DAPrintLog().DeleteexportlogById(p_id, userId);
        
        }
    }
}
