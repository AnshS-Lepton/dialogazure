using DataAccess;
using DataAccess.Admin;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLResources
    {
        private static BLResources objResourcManager = null;
        private static readonly object lockObject = new object();
        public static BLResources Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objResourcManager == null)
                    {
                        objResourcManager = new BLResources();
                    }
                }
                return objResourcManager;
            }
        }

        public List<ResourcesList> GetResourceList(ManageResources objVMResources)
        {
            return new DAResources().GetResourceList(objVMResources);
        }

        public string EditResource(List<ManageResources> manageResource)
        {
            return new DAResources().EditResource(manageResource);
        }

        public List<ResourceLangugeList> GetResourceDropdown(string p_type)
        {
            return new DAResources().GetResourceDropdown(p_type);
        }
        public void GenerateResource(string newculture, int created_by,bool p_is_mobile_key)
        {
            new DAResources().GenerateResource(newculture, created_by, p_is_mobile_key);
        }

        public List<ResourceLangugeList> GetResourceCultureList()
        {
            return new DAResources().GetResourceCultureList();
        }
        public List<ResourceLangugeList> GetResourcelanguageList(string p_type)
        {
            return new DAResources().GetResourcelanguageList(p_type);
        }
        
        public List<res_dropdown_master> GetDropDownList()
        {
            return new DAdropdown_master().GetDropDownList();
        }

        public string AddNewKey(ResourceKeyMaster reskeyMaster, int created_by)
        {
            return new DAResources().AddNewKey(reskeyMaster, created_by);
        }
        public List<AuditResourcelist> GetResourceAuditList(string culture)
        {
            return new DAResources().GetResourceAuditList(culture);
        }
        public List<ManageResources> chkResourceKeys(string key)
        {
            return new DAResources().chkResourceKeys(key);
        }
        public List<ManageResources> GetAllKeys()
        {
            return new DAResources().GetAllKeys();
        }
        public List<ResourcesScriptList> GetResourceScriptList(string p_key)
        {
            return new DAResources().GetResourceScriptList(p_key);
        }
        public string UpdetisUsedjquery(string key, Int32 modified_by)
        {
            return new DAResources().UpdetisUsedjquery(key, modified_by);
        }
        public List<ExportResourceTemplate> DownloadResourcesTemplate(bool p_is_mobile_key)
        {
            return new DAResources().DownloadResourcesTemplate(p_is_mobile_key);
        }

        public void BulkUploadTempResource(List<TempResources> BulkUploadTempResource)
        {
            DATempResources.Instance.BulkUploadTempResource(BulkUploadTempResource);
        }
        public DbMessage UploadResources(int UserId)
        {
            return DATempResources.Instance.UploadResources(UserId);
        }
        public void DeleteTempResourcesData(int UserId)
        {
            DATempResources.Instance.DeleteTempResourcesData(UserId);
        }
        public Tuple<int, int> getTotalUploadResourcesfailureAndSuccess(int UserId)
        {
            return DATempResources.Instance.getTotalUploadResourcesfailureAndSuccess(UserId);
        }

        public List<T> DownloadResourcesLogs<T>(int userId)where T : new()
        {
            return DATempResources.Instance.DownloadResourcesLogs<T>(userId);
        }
        public string GetCultureFont(string culture)
        {
            return new DAResources().GetCultureFont(culture);
        }
        public ResLangKeyListInfo GetResLangKeyListInfo(ResLangKeyListParam objRes)
        {
            return new DAResources().GetResLangKeyListInfo(objRes);
        }

        public List<string> GetResourceKeyList() 
        {
            return new DAResources().GetResourceKeyList();
        }

        public void CheckJunkKeys(List<ResourcesKeyStatus> objResKeyStatus)
        {
            DARes_Key_Status.Instance.CheckJunkKeys(objResKeyStatus);
        }
        public void checkFunctionJunk_Key()
        {
            DARes_Key_Status.Instance.checkFunctionJunk_Key();
        }
        public void Delete_Junk_Key()
        {
            DARes_Key_Status.Instance.Delete_Junk_Key();
        }
    }
}

