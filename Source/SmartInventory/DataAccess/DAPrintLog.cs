using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    #region PrintLog
    public class DAPrintLog : Repository<PrintExportLog>
    {

        public PrintExportLog SavePrintLog(PrintExportLog entity)
        {
            try
            {
                var objLog = repo.Get(u => u.id == entity.id);
                if (objLog != null)
                {
                    objLog.file_path = string.IsNullOrEmpty(entity.file_path) ? objLog.file_path : entity.file_path;
                    objLog.export_progress = string.IsNullOrEmpty(entity.export_progress) ? objLog.export_progress : entity.export_progress;
                    objLog.file_size = string.IsNullOrEmpty(entity.file_size) ? objLog.file_size : entity.file_size;
                    objLog.file_extension = string.IsNullOrEmpty(entity.file_extension) ? objLog.file_extension : entity.file_extension;
                    objLog.end_on = entity.end_on == null ? objLog.end_on : entity.end_on;
                    objLog.export_status = string.IsNullOrEmpty(entity.export_status) ? objLog.export_status : entity.export_status;
                    objLog.error_message = string.IsNullOrEmpty(entity.error_message) ? objLog.error_message : entity.error_message;
                    repo.Update(objLog);
                }
                else
                {
                    objLog = repo.Insert(entity);
                }
                return objLog;
            }
            catch (Exception ex) { throw ex; }
        }
        public List<PrintExportLogInfo> GetPrintExportLogList(CommonGridAttributes objGridAttributes, int user_id, string timeInterval)
        {
            try
            {

                return repo.ExecuteProcedure<PrintExportLogInfo>("fn_get_print_export_log", new
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

        //public DbMessage ValidatePrintArea(string geom, int userId, string geomType, double buff_Radius)
        //{
        //    try
        //    {
        //        return repo.ExecuteProcedure<DbMessage>("fn_get_validate_print_area", new { p_geom = geom, p_userId = userId, p_selectiontype = geomType, p_radius = buff_Radius }).FirstOrDefault();

        //    }
        //    catch { throw; }
        //}
        public bool DeleteexportlogById(int p_id, int userId)
        {
            try
            {
                var chk = repo.Get(u => u.id == p_id && u.user_id == userId);
                if (chk != null)
                {
                    repo.Delete(chk);
                    return true;
                }
                else
                    return false;
            }
            catch(Exception ex) { throw ex; }
        }

        public string GetPrintExportLogJson(int p_id)
        {
            try
            {
                var objLog = repo.Get(u => u.id == p_id);
                return objLog.export_request_params;
            }
            catch { throw; }
        }
        public GeometryDetail GetPrintMapGeoms(int p_historyID)
        {
            try
            {
                var lstGeomDetails = repo.ExecuteProcedure<GeometryDetail>("fn_get_PrintMapZoomGeom", new { p_selectedGeom = p_historyID });
                return lstGeomDetails != null && lstGeomDetails.Count > 0 ? lstGeomDetails[0] : new GeometryDetail();
            }
            catch { throw; }
        }
    }
    #endregion


}
