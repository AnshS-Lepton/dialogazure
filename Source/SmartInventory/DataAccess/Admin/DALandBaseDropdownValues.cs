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
    public class DALandBaseDropdownValues : Repository<ManageDropdownValues>
    {
        public int SaveLandBaseDropdownValues(ManageDropdownValues obj, int user_id)
        {
            try
            {
                if (obj.id != 0)
                {
                    obj.modified_by = user_id;
                    obj.modified_on = DateTimeHelper.Now;
                    repo.Update(obj);
                    return 1;
                }
                else
                {
                    obj.created_by = user_id;
                    obj.created_on = DateTimeHelper.Now;
                    repo.Insert(obj);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public List<ManageDropdownValues> GetDropdownMasterSettingsList
            (ViewLandBaseDropdownMasterSettingsFilter objDropdownMasterSettingsFilter)
        {
            try
            {
                return repo.ExecuteProcedure<ManageDropdownValues>("fn_LandBase_get_dropdown_settings", new
                {
                    P_PAGENO = objDropdownMasterSettingsFilter.currentPage,
                    P_PAGERECORD = objDropdownMasterSettingsFilter.pageSize,
                    P_SORTCOLNAME = objDropdownMasterSettingsFilter.sort,
                    P_SORTTYPE = objDropdownMasterSettingsFilter.orderBy,
                    layer_id = objDropdownMasterSettingsFilter.layer_id,
                    fieldname = objDropdownMasterSettingsFilter.type,
                    value = objDropdownMasterSettingsFilter.value,
                    is_active = objDropdownMasterSettingsFilter.is_active,
                    id = objDropdownMasterSettingsFilter.id
                }, true);
            }
            catch (Exception ex) { throw ex; }
        }
        public ManageDropdownValues GetLandBasedropdown_master_DetailByID(int id)
        {
            var obj = repo.Get(m => m.id == id);
            return obj;
        }

        public List<ManageDropdownValues> ChkDuplicate_ValueExist(ManageDropdownValues obj)
        {
            try
            {
                return repo.GetAll(u => u.value.ToLower().Trim() == obj.value.ToLower().Trim() && u.type.ToUpper() == obj.type.ToUpper() && u.landbase_layer_id == obj.landbase_layer_id).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteLandBaseDropdownMasterById(int id)
        {
            try
            {
                var objUserId = repo.Get(x => x.id == id);
                if (objUserId != null)
                {
                    return repo.Delete(objUserId.id);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
