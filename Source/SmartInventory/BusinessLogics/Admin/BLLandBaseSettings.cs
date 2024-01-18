using DataAccess.Admin;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLLandBaseSettings
    {
        public LandBaseMaster SaveLandBaseLayer(LandBaseMaster obj, int user_id)
        {
            return new DALandBaseSettings().SaveLandBaseLayer(obj, user_id);
        }
        public List<LandBaseMaster> GetLayerList(CommonGridAttributes objGridAttributes)
        {
            return new DALandBaseSettings().GetLayerList(objGridAttributes);
        }
        public LandBaseMaster GetLandBaseDetailByID(int id)
        {
            return new DALandBaseSettings().GetLandBaseDetailByID(id);
        }
        public List<LandBaseMaster> GetLandBaseLayerDetailByID(int id)
        {
            return new DALandBaseSettings().GetLandBaseLayerDetailByID(id);
        }
        public int DeleteLandBaseSettingById(int id)
        {
            return new DALandBaseSettings().DeleteLandBaseSettingById(id);

        }
        public List<LandBaseMaster> ChkDuplicate_abbrExist(LandBaseMaster obj)
        {
            return new DALandBaseSettings().ChkDuplicate_abbrExist(obj);
        }

        public List<RowCountResult> GetLandBaseLayerSettingRowCount(int layer_id)
        {
            return new DALandBaseSettings().GetLandBaseLayerSettingRowCount(layer_id);
        }
        public int SaveDropdownValues(ManageDropdownValues obj, int user_id)
        {
            return new DALandBaseDropdownValues().SaveLandBaseDropdownValues(obj, user_id);
        }

        public List<ManageDropdownValues> GetDropdownMasterSettingsList
          (ViewLandBaseDropdownMasterSettingsFilter objDropdownMasterSettingsFilter)
        {
            return new DALandBaseDropdownValues().GetDropdownMasterSettingsList(objDropdownMasterSettingsFilter);
        }
        public ManageDropdownValues GetLandBasedropdown_master_DetailByID(int id)
        {
            return new DALandBaseDropdownValues().GetLandBasedropdown_master_DetailByID(id);
        }

        public List<RowCountResult> GetLandBaseDropdownMasterRowCount(int layer_id, int id, string layer_name, string fieldname, string value)
        {
            return new DALandBaseSettings().GetLandBaseDropdownMasterRowCount(layer_id, id, layer_name, fieldname, value);
        }
        public List<ManageDropdownValues> ChkDuplicate_ValueExist(ManageDropdownValues obj)
        {
            return new DALandBaseDropdownValues().ChkDuplicate_ValueExist(obj);
        }
        public int DeleteLandBaseDropdownMasterById(int id)
        {
            return new DALandBaseDropdownValues().DeleteLandBaseDropdownMasterById(id);

        }
    }
}
