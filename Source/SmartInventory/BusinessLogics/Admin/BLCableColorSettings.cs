using DataAccess.Admin;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLCableColorSettings
    {
        public List<CableMapColorSettingVw> GetCableMapColorSettingsList(VwCableMapColorSettingsFilter objCableColorSettingsFilter)
        {
            return new DACableColorSettings().GetCableMapColorSettingsList(objCableColorSettingsFilter);
        }
        public CableMapColorSettings GetCablMapColorDetailByID(int id)
        {
            return new DACableColorSettings().GetCablMapColorDetailByID(id);
        }
        public int DeleteCableColourSettingsById(int id)
        {
            return new DACableColorSettings().DeleteCableColourSettingsById(id);
        }
        public bool SaveCableMapColorSettingDetails(CableMapColorSettings objAddCableColrSetting)
        {
            return new DACableColorSettings().SaveCableMapColorSettingDetails(objAddCableColrSetting);
        }
    }
}
