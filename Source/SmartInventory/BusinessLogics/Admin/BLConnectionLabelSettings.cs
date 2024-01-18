using DataAccess.Admin;
using Models.Admin;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Admin
{
    public class BLConnectionLabelSettings
    {
        public List<ConnectionLabelSettingsVw> GetConnectionLabelSettingsList(VwConnectionLabelSettingsFilter objConcLablStngFilter)
        {
            return new DAConnectionLabelSettings().GetConnectionLabelSettingsList(objConcLablStngFilter);
        }
        public List<KeyValueDropDown> GetConnectionLabel(int layerId)
        {
            return new DAConnectionLabelSettings().GetConnectionLabel(layerId);
        }
        public bool SaveConnectionLabelInfo(ConnectionLabelSettings corelst)
        {
            return new DAConnectionLabelSettings().SaveConnectionLabelInfo(corelst);
        }
        public ConnectionLabelSettings GetConnectionLabelSettingbyId(int id)
        {
            return new DAConnectionLabelSettings().GetConnectionLabelSettingbyId(id);
        }
        public bool SyncAllLabelColumn(int layerId)
        {
             return new DAConnectionLabelSettings().SyncAllLabelColumn(layerId);
        }
    }
}
