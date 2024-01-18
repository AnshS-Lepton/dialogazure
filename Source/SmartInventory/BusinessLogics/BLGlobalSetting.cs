using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLGlobalSetting
    {        
        public List<GlobalSetting> GetGlobalSettings(string type="")
        {
            return new DAGlobalSetting().GetGlobalSettings(type);
        }
        
        public string GetSignUpKeyForBuildingSurvey()
        {
            return new DAGlobalSetting().GetSignUpKeyForBuildingSurvey();
        }

        public GlobalSetting GetGlobalSettingbyId(int id)
        {
            return new DAGlobalSetting().GetGlobalSettingbyId(id);        
        }
        public GlobalSetting GetMobileAppVersionByKey(string Key)
        {
            return new DAGlobalSetting().GetMobileAppVersionByKey(Key);
        }

        public bool SaveGlobalSetting(GlobalSetting objGlobalSettings, int userId)
        {
            return new DAGlobalSetting().SaveGlobalSetting(objGlobalSettings, userId);        
        }
        public List<GlobalSetting> GetAllGlobalSettings(CommonGridAttributes objGridAttributes,int user_id)
        {
            return new DAGlobalSetting().GetAllGlobalSettings(objGridAttributes, user_id);
        }
        public GlobalSetting getValueFullText(string key)
        {
            return new DAGlobalSetting().getValueFullText(key);
        }
    }
}
