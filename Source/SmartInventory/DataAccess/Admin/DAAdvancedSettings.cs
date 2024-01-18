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
    public class DAAdvancedSettings : Repository<layerColumnSettings>
    {
        DAAdvancedSettings()
        {

        }

        private static DAAdvancedSettings objDAAdvancedSettings = null;
        private static readonly object lockObject = new object();
        public static DAAdvancedSettings Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDAAdvancedSettings == null)
                    {
                        objDAAdvancedSettings = new DAAdvancedSettings();
                    }
                }
                return objDAAdvancedSettings;
            }
        }

        public List<layerColumnSettings> GetLayerColumnSettings(int layerId, string strSettingType)
        {
            try
            {
                var res = repo.GetAll(m => m.layer_id == layerId && m.setting_type.ToUpper().Trim() == strSettingType.ToUpper().Trim()).OrderBy(s => s.column_sequence).ToList();
                return res;
            }
            catch { throw; }
        }
        public List<layerColumnSettings> GetLayerColumnSettingsForOffline(string strSettingType)
        {
            try
            {
                var res = repo.GetAll(m => m.setting_type.ToUpper().Trim() == strSettingType.ToUpper().Trim()).OrderBy(s => s.column_sequence).ToList();
                return res;
            }
            catch { throw; }
        }

        public bool SaveLayerColumnSettings(vwLayerColumnSettings objLayerColumnSettings)
        {
            try
            {
                var lstExisitingReportColumns = repo.GetAll(m => m.layer_id == objLayerColumnSettings.layer_id && m.setting_type.ToUpper().Trim() == objLayerColumnSettings.settingType.ToUpper().Trim()).ToList();

                foreach (var item in objLayerColumnSettings.lstLayerColumns)
                {
                    var itemToChange = lstExisitingReportColumns.FirstOrDefault(d => d.id == item.id);
                    if (itemToChange != null)
                    {
                        itemToChange.modified_by = objLayerColumnSettings.user_id;
                        itemToChange.modified_on = DateTimeHelper.Now;
                        itemToChange.column_sequence = item.column_sequence;
                        itemToChange.display_name = item.display_name;
                        itemToChange.res_field_key = item.res_field_key;
                        itemToChange.is_active = item.is_active;
                        if (objLayerColumnSettings.settingType.ToUpper() == "REPORT")
                        {
                            itemToChange.is_kml_column_required = item.is_kml_column_required;
                        }
                    }
                }

                repo.Update(lstExisitingReportColumns);
                return true;
            }
            catch { throw; }
        }


        public List<Modules> GetModuleMasterList(bool isActive)
        {
            try
            {
                return repo.ExecuteProcedure<Modules>("fn_get_modules", new { p_is_active = isActive }, false);

            }
            catch { throw; }

        }

        public bool SaveModuleSettings(List<Modules> sett, int user_id)
        {
            try
            {
                foreach (var item in sett)
                {

                    repo.ExecuteProcedure<bool>("fn_save_module_setting",
                        new
                        {
                            p_module_id = item.id,
                            p_is_active = item.is_selected,
                            p_user_id = user_id

                        }, false);
                }
                return true;
            }
            catch { return false; }

        }

    }

    public class DALayerActionSettings : Repository<LayerActionDetails>
    {
        DALayerActionSettings()
        {

        }

        private static DALayerActionSettings objDALayerActionSettings = null;
        private static readonly object lockObject = new object();
        public static DALayerActionSettings Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDALayerActionSettings == null)
                    {
                        objDALayerActionSettings = new DALayerActionSettings();
                    }
                }
                return objDALayerActionSettings;
            }
        }
        public List<LayerActionDetails> GetLayerActionSettings(int layerId)
        {
            try
            {
                return repo.ExecuteProcedure<LayerActionDetails>("fn_get_layer_action_details",
                     new { p_layer_id = layerId }, true);
            }
            catch { throw; }
        }


         public bool SaveLayerActionSettings(List<LayerActionDetails> actionDetails, int layer_id, int user_id)
        {
            try
            {
                var lstExisitingAction = repo.GetAll(m => m.layer_id == layer_id).ToList();

                foreach (var item in actionDetails)
                {
                    var itemToChange = lstExisitingAction.FirstOrDefault(d => d.action_name.ToUpper() == item.action_name.ToUpper());
                    if (itemToChange != null)
                    {                       
                        itemToChange.is_active = item.is_active;                      
                        itemToChange.is_web_action = item.is_web_action;
                        itemToChange.is_mobile_action = item.is_mobile_action;
                        itemToChange.action_sequence = item.action_sequence;
                        itemToChange.action_title = item.action_title;
                    }
                }

                repo.Update(lstExisitingAction);
                return true;
            }
            catch { throw; }
        }
    }
}
