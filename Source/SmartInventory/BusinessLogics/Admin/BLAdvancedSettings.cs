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
  public  class BLAdvancedSettings
    {
        BLAdvancedSettings()
        {

        }

        private static BLAdvancedSettings objBLAdvancedSettings = null;
        private static readonly object lockObject = new object();
        public static BLAdvancedSettings Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLAdvancedSettings == null)
                    {
                        objBLAdvancedSettings = new BLAdvancedSettings();
                    }
                }
                return objBLAdvancedSettings;
            }
        }

        public List<layerColumnSettings> GetLayerColumnSettings(int layerId, string strSettingType)
        {
            return DAAdvancedSettings.Instance.GetLayerColumnSettings(layerId, strSettingType);
        }
        public List<layerColumnSettings> GetLayerColumnSettingsForOffline(string strSettingType)
        {
            return DAAdvancedSettings.Instance.GetLayerColumnSettingsForOffline(strSettingType);
        }

        public bool SaveLayerColumnSettings(vwLayerColumnSettings objLayerColumnSettings)
        {
            return DAAdvancedSettings.Instance.SaveLayerColumnSettings(objLayerColumnSettings);
        }

        public List<Modules> GetModuleMasterList(bool isActive=false)
        {
            return DAAdvancedSettings.Instance.GetModuleMasterList(isActive);
        }

        public bool SaveModuleSettings(List<Modules> sett, int user_id)
        {
            return DAAdvancedSettings.Instance.SaveModuleSettings(sett, user_id);
        }

        public List<LayerActionDetails> GetLayerActionSettings(int layerId)
        {
            return DALayerActionSettings.Instance.GetLayerActionSettings(layerId);
        }

        public bool SaveLayerActionSettings(List<LayerActionDetails> actionDetails, int layer_id, int user_id)
        {
            return DALayerActionSettings.Instance.SaveLayerActionSettings(actionDetails, layer_id, user_id);
        }
    }
}
