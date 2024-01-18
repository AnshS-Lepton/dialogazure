using DataAccess.DBHelpers;
using Models;
using Models.Admin;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAResources : Repository<ManageResources>
    {
        private static DAResources objResourcesManager = null;
        private static readonly object lockObject = new object();
        public static DAResources Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objResourcesManager == null)
                    {
                        objResourcesManager = new DAResources();
                    }
                }
                return objResourcesManager;
            }
        }
        public List<ResourcesList> GetResourceList(ManageResources objVMResources)
        {
            try
            {
                if (objVMResources.culture == null || objVMResources.culture == "")
                {
                    objVMResources.culture = "en";
                }
                return repo.ExecuteProcedure<ResourcesList>("fn_res_get_Resource_list", new
                {
                    p_searchtext = objVMResources.key,
                    p_searchvalue = objVMResources.value,
                    selectlang = objVMResources.culture,
                    p_type = objVMResources.Type,
                    p_pageno = objVMResources.objGridAttributes.currentPage,
                    p_pagerecord = objVMResources.objGridAttributes.pageSize,
                    p_sortcolname = objVMResources.objGridAttributes.sort,
                    p_sorttype = objVMResources.objGridAttributes.orderBy
                }, true);
            }
            catch
            {
                throw;
            }
        }

        public string EditResource(List<ManageResources> manageResource)
        {

            try
            {
                var result = repo.Update(manageResource);
                return result.ToString();
            }
            catch
            {
                throw;
            }
        }



        public List<ResourceLangugeList> GetResourceDropdown(string p_type)
        {
            try
            {

                return repo.ExecuteProcedure<ResourceLangugeList>("fn_res_get_resource_dropdown", new { p_type = p_type });
            }
            catch
            {
                throw;
            }
        }

        public void GenerateResource(string newculture, int created_by, bool p_is_mobile_key)
        {
            try
            {

                repo.ExecuteProcedure("fn_res_insert_new_resource", new { p_newculture = newculture, p_created_by = created_by, p_is_mobile_key });
            }
            catch
            {
                throw;
            }
        }

        public List<ResourceLangugeList> GetResourceCultureList()
        {
            try
            {

                return repo.ExecuteProcedure<ResourceLangugeList>("fn_res_get_resource_culture_dropdown", new { });
            }
            catch
            {
                throw;
            }
        }

        public List<ResourceLangugeList> GetResourcelanguageList(string p_type)
        {
            try
            {

                return repo.ExecuteProcedure<ResourceLangugeList>("fn_res_get_resource_language_dropdown", new { p_type = p_type });
            }
            catch
            {
                throw;
            }
        }

        public string AddNewKey(ResourceKeyMaster reskeyMaster, int created_by)
        {
            try
            {
                //var NewKey = reskeyMaster.Projectkey + "_" + reskeyMaster.Modulekey + "_" + reskeyMaster.Sub_Modulekey + "_" + reskeyMaster.DotNet_JQuerykey + "_" + reskeyMaster.Purpose_Typekey + "_" + reskeyMaster.Value;
                var description = reskeyMaster.Projectvalue + "_" + reskeyMaster.Modulevalue + "_" + reskeyMaster.Sub_Modulevalue + "_" + reskeyMaster.DotNet_JQueryvalue + "_" + reskeyMaster.Purpose_Typevalue;

                var newKey = repo.ExecuteProcedure<string>("fn_res_insert_resource_key", new { p_newkey = reskeyMaster.key, p_newdescription = description, p_value = reskeyMaster.Value, p_created_by = created_by, p_is_used_jquery = reskeyMaster.is_jquery_used, p_is_mobile_key = reskeyMaster.is_mobile_key });
                return newKey.Last();
            }
            catch
            {
                throw;
            }
        }


        public List<AuditResourcelist> GetResourceAuditList(string p_culture)
        {
            try
            {

                return repo.ExecuteProcedure<AuditResourcelist>("fn_get_res_audit_resource_list", new { p_culture = p_culture });
            }
            catch
            {
                throw;
            }
        }

        public List<ManageResources> chkResourceKeys(string key)
        {

            try
            {
                var res = repo.GetAll(m => m.key == key).ToList();
                return res;
            }
            catch { throw; }
        }
        public List<ManageResources> GetAllKeys()
        {

            try
            {
                var res = repo.GetAll().ToList();
                return res;
            }
            catch { throw; }
        }

        public List<ResourcesScriptList> GetResourceScriptList(string p_key)
        {
            try
            {

                return repo.ExecuteProcedure<ResourcesScriptList>("fn_res_get_resource_list_by_key", new { p_key = p_key });
            }
            catch
            {
                throw;
            }
        }

        public string UpdetisUsedjquery(string key, Int32 modified_by)
        {

            try
            {

                var result = repo.ExecuteProcedure<string>("fn_res_update_is_used_jquery", new { p_key = key, p_modified_by = modified_by });

                return result.ToString();

            }
            catch
            {
                throw;
            }
        }

        public List<ExportResourceTemplate> DownloadResourcesTemplate(bool p_is_mobile_key)
        {
            try
            {

                return repo.ExecuteProcedure<ExportResourceTemplate>("fn_res_download_resource_template", new { p_is_mobile_key = p_is_mobile_key });
            }
            catch
            {
                throw;
            }
        }
        public string GetCultureFont(string p_culture)
        {

            try
            {
                var result = repo.ExecuteProcedure<string>("fn_res_get_culture_font", new { p_culture = p_culture });
                return result.Last();
            }
            catch
            {
                throw;
            }
        }
        public ResLangKeyListInfo GetResLangKeyListInfo(ResLangKeyListParam objRes)
        {
            try
            {
                var lstResLangKey = repo.ExecuteProcedure<ResLangKeyListInfo>("fn_get_res_lang_key_list", new { p_language = objRes.language.ToUpper(), p_purpose = objRes.purpose.ToUpper() }, true).FirstOrDefault();
                return lstResLangKey != null ? lstResLangKey : new ResLangKeyListInfo();
            }
            catch { throw; }
        }

        public List<string> GetResourceKeyList()
        {

            try
            {
                var result = repo.ExecuteProcedure<string>("fn_res_get_resource_key_list", new { });
                return result;
            }
            catch
            {
                throw;
            }

        }
       
    }

    public class DATempResources : Repository<TempResources>
    {
        private static DATempResources objResourcesManager = null;
        private static readonly object lockObject = new object();
        public static DATempResources Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objResourcesManager == null)
                    {
                        objResourcesManager = new DATempResources();
                    }
                }
                return objResourcesManager;
            }
        }
        public void BulkUploadTempResource(List<TempResources> TempResources)
        {
            try
            {
                repo.Insert(TempResources);
            }
            catch { throw; }
        }
        public DbMessage UploadResources(int userID)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_res_bulk_upload_resources_insert", new { P_UserId = userID }).FirstOrDefault();
            }
            catch { throw; }
        }

        public void DeleteTempResourcesData(int user_id)
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_delete_temp_resources_manager", new { P_Userid = user_id });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Tuple<int, int> getTotalUploadResourcesfailureAndSuccess(int UserId)
        {
            try
            {
                var getTotalUploadResourefailure = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == false).Count();
                var getTotalUploadResourceSuccess = repo.GetAll().Where(x => x.uploaded_by == UserId & x.is_valid == true).Count();
                return Tuple.Create(getTotalUploadResourceSuccess, getTotalUploadResourefailure);
            }
            catch { throw; }
        }

        public List<T> DownloadResourcesLogs<T>(int userId) where T : new()
        {
            try
            {

                var lstItems= repo.ExecuteProcedure<T>("fn_res_download_resource_logs", new { p_userId= userId },true);
               
                return lstItems != null ? lstItems : new List<T>();
            }
            catch
            {
                throw;
            }
        }

        
    }

    public class DAdropdown_master : Repository<res_dropdown_master>
    {
        public List<res_dropdown_master> GetDropDownList()
        {
            return (List<res_dropdown_master>)repo.GetAll(m => m.dropdown_status == true);
        }
    }

    public class DARes_Key_Status : Repository<ResourcesKeyStatus>
    {
         private static DARes_Key_Status objResourcesManager = null;
        private static readonly object lockObject = new object();
        public static DARes_Key_Status Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objResourcesManager == null)
                    {
                        objResourcesManager = new DARes_Key_Status();
                    }
                }
                return objResourcesManager;
            }
        }
        public void CheckJunkKeys(List<ResourcesKeyStatus> objreskeyStatus)
        {
            try
            {
                repo.Insert(objreskeyStatus);
            }
            catch { throw; }
        }

        public void checkFunctionJunk_Key()
        {
            try
            {
                repo.ExecuteProcedure<string>("fn_res_check_junk_key", new {});
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Delete_Junk_Key()
        {
            try
            {
                repo.ExecuteProcedure<bool>("fn_res_delete_resource_junk_key", new { });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
