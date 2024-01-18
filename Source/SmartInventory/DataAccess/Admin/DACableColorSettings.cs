using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Admin
{
    public class DACableColorSettings : Repository<CableMapColorSettings>
    {
        public List<CableMapColorSettingVw> GetCableMapColorSettingsList(VwCableMapColorSettingsFilter objCblClrFilter)
        {
            try
            {
                return repo.ExecuteProcedure<CableMapColorSettingVw>("fn_get_cable_color_settings", new
                {
                    p_cable_type = objCblClrFilter.cable_type,
                    p_cable_category = objCblClrFilter.cable_category,
                    p_fiber_count = objCblClrFilter.fiber_count,
                    p_PAGENO = objCblClrFilter.currentPage,
                    p_PAGERECORD = objCblClrFilter.pageSize,
                    p_SORTCOLNAME = objCblClrFilter.sort,
                    p_SORTTYPE = objCblClrFilter.orderBy
                }, true);
            }
            catch { throw; }
        }
        public CableMapColorSettings GetCablMapColorDetailByID(int id)
        {
            var obj = repo.Get(m => m.id == id);
            return obj != null ? obj : new CableMapColorSettings();
        }
        public int DeleteCableColourSettingsById(int id)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }
        public bool SaveCableMapColorSettingDetails(CableMapColorSettings objAddCableColorSetting)
        {
            try
            {
                var objExisiting = repo.GetById(m => m.cable_type == objAddCableColorSetting.cable_type && m.cable_category == objAddCableColorSetting.cable_category && m.fiber_count == objAddCableColorSetting.fiber_count);
                if (objExisiting != null)
                {
                    objExisiting.color_code = objAddCableColorSetting.color_code;
                    objExisiting.modified_by = objAddCableColorSetting.user_id;
                    objExisiting.modified_on = DateTimeHelper.Now;
                    repo.Update(objExisiting);
                }
                else
                {
                    objAddCableColorSetting.created_on = DateTimeHelper.Now;
                    objAddCableColorSetting.created_by = objAddCableColorSetting.user_id;
                    repo.Insert(objAddCableColorSetting);
                }

                return true;
            }
            catch { throw; }
        }
    }
}
