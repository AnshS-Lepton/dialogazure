using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAGlobalSetting : Repository<GlobalSetting>
    {
        public List<GlobalSetting> GetGlobalSettings(string type = "")
        {        
            try
            {
                if (!string.IsNullOrWhiteSpace(type))
                {
                    if (type.ToUpper() == "WEB")
                    { 
                        return repo.GetAll(s => s.is_web_key == true).ToList();
                    }
                    else //Mobile
                    {
                        return repo.GetAll(s => s.is_mobile_key == true).ToList();
                    }
                }
                else
                {
                    return repo.GetAll().OrderByDescending(m => m.type).OrderByDescending(s => s.is_edit_allowed).ToList();
                }
            }
            catch { throw; }
        }               

        public string GetSignUpKeyForBuildingSurvey()   
        {
            try
            {
                return repo.Get(s => s.is_mobile_key == true && s.key.ToUpper() == "ISSIGNUPALLOWED").value;
            }
            catch { throw; }
        }
        public List<GlobalSetting> GetAllGlobalSettings(CommonGridAttributes objGridAttributes,int user_id)
        {
            string searchby = string.Empty;
            try
            {
                    
                
                return repo.ExecuteProcedure<GlobalSetting>("fn_get_global_settings", new
                {
                    p_searchby = searchby = objGridAttributes.searchBy,
                    p_searchtext = objGridAttributes.searchText,
                    P_PAGENO = objGridAttributes.currentPage,
                    P_PAGERECORD = objGridAttributes.pageSize,
                    P_SORTCOLNAME = objGridAttributes.sort,
                    P_SORTTYPE = objGridAttributes.orderBy,
                    P_USER_ID = user_id
                }, true);
            }
            catch (Exception ex) { throw ex; }
            // return repo.GetAll(m => m.is_active == true, "role_id" , "ASC", pagesize, pagenumber, out totalRecords).ToList(); 
            //  return repo.GetAll((x => x.cable_id == cableId), "cable_id", "DESC", pagesize, pagenumber, out totalRecords);
        }


        public GlobalSetting GetGlobalSettingbyId(int id)
        {
            try
            {
                return repo.ExecuteProcedure<GlobalSetting>("fn_get_global_settings_value", new{p_id = id}, true).FirstOrDefault();
            }
            catch (Exception ex) { throw ex; }
        }
        public GlobalSetting GetMobileAppVersionByKey(string Key)
        {
            try
            {
                return repo.Get(m => m.key == Key);
            }
            catch { throw; }
        }

        public bool SaveGlobalSetting(GlobalSetting objGlobalSettings,int userId)
        {


            try
            {

                if (objGlobalSettings.id != 0)
                {
                    repo.ExecuteProcedure<GlobalSetting>("fn_save_global_settings", new
                    {
                        p_key = objGlobalSettings.key,
                        p_value = objGlobalSettings.value,
                        p_is_web_key = objGlobalSettings.is_web_key,
                        P_is_mobile_key = objGlobalSettings.is_mobile_key,
                        P_modified_by = userId
                    }, true);
                }
                return true;
            }
            catch { throw; }
        }

        public GlobalSetting getValueFullText(string key)
        {
            try
            {
                return repo.Get(m => m.key == key);
            }
            catch { throw; }
        }
    }
}
