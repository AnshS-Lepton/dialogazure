using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess.Admin
{
    public class DAConnectionLabelSettings : Repository<ConnectionLabelSettings>
    {
        public List<ConnectionLabelSettingsVw> GetConnectionLabelSettingsList(VwConnectionLabelSettingsFilter objConcLablStngFilter)
        {
            try
            {
                return repo.ExecuteProcedure<ConnectionLabelSettingsVw>("fn_get_connection_label_setting_with_filter", new
                {
                    p_searchby = objConcLablStngFilter.searchBy,
                    p_searchtext = objConcLablStngFilter.searchText,
                    p_PAGENO = objConcLablStngFilter.currentPage,
                    p_PAGERECORD = objConcLablStngFilter.pageSize,
                    p_SORTCOLNAME = objConcLablStngFilter.sort,
                    p_SORTTYPE = objConcLablStngFilter.orderBy
                }, true);
            }
            catch { throw; }
        }
        public List<KeyValueDropDown> GetConnectionLabel(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_connection_label", new { p_layerId = layerId});
            }
            catch { throw; }
        }
        public bool SaveConnectionLabelInfo(ConnectionLabelSettings objCon)
        {
            try
            {
                var objConLbl = repo.Get(u => u.id == objCon.id);
                if (objConLbl != null)
                {
                    objConLbl.display_column_name = objCon.display_column_name;
                    objConLbl.status = "Pending";
                    objConLbl.modified_by = objCon.user_id;
                    objConLbl.modified_on = DateTimeHelper.Now;
                    repo.Update(objConLbl);
                }
            }
            catch { throw; }
            return true;
        }


        public ConnectionLabelSettings GetConnectionLabelSettingbyId(int id)
        {
            try
            {
                return repo.Get(x => x.id == id);
            }
            catch { throw; }
        }
        public bool SyncAllLabelColumn(int layerId)
        {
            try
            {
                repo.ExecuteProcedure<object>("fn_splicing_sync_connection_label", new { p_layer_id = layerId });
                return true;
            }
            catch { throw; }
        }

    }

}
