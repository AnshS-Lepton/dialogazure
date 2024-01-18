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

    public class DAISPModelInfo : Repository<ISPModelInfo>
    {
        DAISPModelInfo()
        {

        }

        private static DAISPModelInfo objISPModelMaster = null;
        private static readonly object lockObject = new object();
        public static DAISPModelInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objISPModelMaster == null)
                    {
                        objISPModelMaster = new DAISPModelInfo();
                    }
                }
                return objISPModelMaster;
            }
        }

        public List<AllEquipments> GetAllEquipments()
        {
            try
            {
                var res = repo.ExecuteProcedure<AllEquipments>("fn_isp_get_all_Equipments", new { }, true);//repo.GetAll().ToList();
                return res;
            }
            catch { throw; }
        }

        public List<ModelDetails> GetModelDetails(ISPModelInfo objISPModelMaster, int modelID = 0, string searchText = "", string searchBy = "")
        {
            try
            {
                var res = repo.ExecuteProcedure<ModelDetails>("fn_isp_get_model_details", new
                {
                    searchby = Convert.ToString(searchBy),
                    searchbyText = Convert.ToString(searchText),
                    P_PAGENO = objISPModelMaster.objOptionsEquipmentDetails.currentPage,
                    P_PAGERECORD = objISPModelMaster.objOptionsEquipmentDetails.pageSize,
                    P_SORTCOLNAME = objISPModelMaster.objOptionsEquipmentDetails.sort,
                    P_SORTTYPE = objISPModelMaster.objOptionsEquipmentDetails.orderBy,
                    P_TOTALRECORDS = objISPModelMaster.objOptionsEquipmentDetails.totalRecord,
                    P_RECORDLIST = 0,
                    P_ModelID = modelID
                }, true);//repo.GetAll().ToList();
                return res;
            }
            catch { throw; }
        }

        public List<ISPModelRule> GetRuleDetails(ISPModelRule objISPModelRule, string searchText = "", string searchBy = "")
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelRule>("fn_isp_get_rule_details", new
                {
                    searchby = Convert.ToString(searchBy),
                    searchbyText = Convert.ToString(searchText),
                    P_PAGENO = objISPModelRule.objOptionsRule.currentPage,
                    P_PAGERECORD = objISPModelRule.objOptionsRule.pageSize,
                    P_SORTCOLNAME = objISPModelRule.objOptionsRule.sort,
                    P_SORTTYPE = objISPModelRule.objOptionsRule.orderBy,
                    P_TOTALRECORDS = objISPModelRule.objOptionsRule.totalRecord,
                    P_RECORDLIST = 0,
                    p_parent_model = objISPModelRule.parent_model_id,
                    p_parent_type = objISPModelRule.parent_model_type_id
                }, true);//repo.GetAll().ToList();
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelTypeMaster> GetModalTypeDetails(ISPModelTypeMaster objISPModelTypeMaster, string searchText = "", string searchBy = "")
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelTypeMaster>("fn_isp_get_model_type_details", new
                {
                    searchby = Convert.ToString(searchBy),
                    searchbyText = Convert.ToString(searchText),
                    P_PAGENO = objISPModelTypeMaster.objOptionsModelType.currentPage,
                    P_PAGERECORD = objISPModelTypeMaster.objOptionsModelType.pageSize,
                    P_SORTCOLNAME = objISPModelTypeMaster.objOptionsModelType.sort,
                    P_SORTTYPE = objISPModelTypeMaster.objOptionsModelType.orderBy,
                    P_TOTALRECORDS = objISPModelTypeMaster.objOptionsModelType.totalRecord,
                    P_RECORDLIST = 0,
                    p_modelId = objISPModelTypeMaster.model_id,

                }, true);//repo.GetAll().ToList();
                return res;
            }
            catch { throw; }
        }


        public int SaveRule(ISPModelRule record)
        {
            try
            {
                var res = repo.ExecuteProcedure<int>("fn_isp_save_model_rule", new
                {
                    p_parent_model = record.parent_model_id,
                    p_parent_model_type = record.parent_model_type_id,
                    p_child_model = record.child_model_id,
                    p_child_model_type = record.child_model_type_id,
                    p_is_active = record.is_active,
                    p_user_id = record.created_by

                });
                return res[0];
            }
            catch { throw; }
        }

        public int DeleteRule(int id)
        {
            var lst = repo.ExecuteProcedure<int>("fn_isp_delete_rule_details", new { p_ruleid = id });
            return lst[0];
        }

        public int DeleteModalType(int id)
        {
            var lst = repo.ExecuteProcedure<int>("fn_isp_delete_modal_type_details", new { p_modaltypeid = id });
            return lst[0];
        }
        public List<ModelStatusCount> GetModelStatusCount(int modelID = 0)
        {
            try
            {
                var res = repo.ExecuteProcedure<ModelStatusCount>("fn_isp_get_model_count", new { modelID = modelID }, true);
                return res;
            }
            catch { throw; }
        }

        public List<CheckRulesExists> CheckModelTypeExists(int parent_model_type_id)
        {
            var res = repo.ExecuteProcedure<CheckRulesExists>("fn_check_modelType_exists", new { p_parent_model_type_id = parent_model_type_id },true);
            return res;
        }
        public List<CheckRulesExists> CheckModelTypeExists(string model_type_name)
        {
            var res = repo.ExecuteProcedure<CheckRulesExists>("fn_check_modelType_exists", new { p_model_type_name = model_type_name }, true);
            return res;
        }
        public List<CheckRulesExists> CheckRulesExists(int rule_id)
        {
            var res = repo.ExecuteProcedure<CheckRulesExists>("fn_check_rules_exists", new { p_rule_id = rule_id }, true);
            return res;
        }
        public List<CheckRulesExists> CheckRulesExists(int parent_model_id,int? parent_model_type_id,int child_model_id,int? child_model_type_id)
        {
            var res = repo.ExecuteProcedure<CheckRulesExists>("fn_check_rules_exists", new {
                p_parent_model_id = parent_model_id,
                p_parent_model_type_id= parent_model_type_id,
                p_child_model_id= child_model_id,
                p_child_model_type_id = child_model_type_id
            }, true);
            return res;
        }


        public int DeleteModelDetailsById(int id)
        {
            var lst = repo.ExecuteProcedure<int>("fn_isp_delete_model_details", new { p_modelid = id });
            return lst[0];
        }

        


        public ISPModelInfo SaveModelInfo(ISPModelInfo record, int userId)
        {
            try
            {
                var dataFound = repo.Get(u => u.id == record.id);
                if (dataFound != null)
                {
                    //PageMessage pageMessage = DAUtility.ValidateModifiedDate(record.modified_on, dataFound.modified_on, record.modified_by, dataFound.modified_by);
                    //if (pageMessage.message != null)
                    //{
                    //    record.page_message = pageMessage;
                    //    return record;
                    //}

                    dataFound.model_image_id = record.model_image_id;
                    dataFound.model_id = record.model_id;
                    dataFound.model_type_id = record.model_type_id;
                    dataFound.model_name = record.model_name;
                    dataFound.status_id = record.status_id;
                    if (record.item_template_id != 0)
                    {
                        dataFound.item_template_id = record.item_template_id;
                    }

                    dataFound.height = record.height;
                    dataFound.width = record.width;
                    dataFound.depth = record.depth;
                    dataFound.border_width = record.border_width;
                    dataFound.modified_on = DateTimeHelper.Now;
                    dataFound.modified_by = userId;
                    dataFound.unit_size = record.unit_size;
                    dataFound.rotation_angle = record.rotation_angle;
                    dataFound.is_multi_connection = record.is_multi_connection;
                    dataFound.border_color = record.border_color;                    
                    record = repo.Update(dataFound);

                }
                else
                {

                    record.created_on = DateTimeHelper.Now;
                    record.created_by = userId;
                    record = repo.Insert(record);

                    //return record;
                }
                //Save children

                return record;
            }
            catch
            {
                throw;
            }
        }


        public int SaveModelType(ISPModelTypeMaster record)
        {
            try
            {
                var res = repo.ExecuteProcedure<int>("fn_isp_save_model_type", new
                {
                    p_key = record.key,
                    p_value = record.value,
                    p_color_code = record.color_code,
                    p_model_id = record.model_id,
                    p_user_id = record.created_by,
                    p_stroke_code = record.stroke_code,
                    p_type_abbr = record.type_abbr,
                    p_model_color_id = record.model_color_id,
                    p_is_middleware = record.is_middleware_model_type
                });
                return res[0];
            }
            catch { throw; }
        }
        public List<ISPModelStatusMaster> GetModelStatus()
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelStatusMaster>("fn_isp_get_model_status", new { }, true);
                return res;
            }
            catch { throw; }
        }

        public List<ISPModelMaster> GetModels()
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelMaster>("fn_isp_get_model", new { }, true);
                return res;
            }
            catch { throw; }
        }

        public List<MiddleWareEntity> GetMiddleWareLayers()
        {
            try
            {
                var res = repo.ExecuteProcedure<MiddleWareEntity>("fn_isp_get_middleware_layer", new { }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelMaster> GetModelMaster()
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelMaster>("fn_isp_get_model_master", new { }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelTypeMaster> GetModelTypes(int model_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelTypeMaster>("   fn_isp_get_model_type", new { model_id }, true);
                return res;
            }
            catch { throw; }
        }

        public List<ISPModelInfo> GetModelsWithImage(int model_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_model_info_with_image", new { model_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetEquipmentWithImage(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_equipment_info_with_image", new { p_system_id= system_id }, true);
                
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetModelInfo(int model_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_model_info", new { model_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetEquipmentInfo(int system_id)
        {
            try
            {
                return repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_equipment_info", new { p_system_id= system_id }, true);
               
            }
            catch { throw; }
        }
        
        public List<ISPModelRule> EditRuleDetails(int rule_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelRule>("fn_isp_edit_update_rule_info", new { p_ruleID = rule_id, p_mode = "EDIT" }, true);
                return res;
            }
            catch { throw; }
        }

        public List<ISPModelRule> UpdateRuleDetails(ISPModelRule objISPModelRule,int userId)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelRule>("fn_isp_edit_update_rule_info",
                new
                {
                    p_ruleID = objISPModelRule.id,
                    p_mode = "UPDATE",
                    p_parent_model_id = objISPModelRule.parent_model_id,
                    p_parent_model_type_id = objISPModelRule.parent_model_type_id,
                    p_child_model_id = objISPModelRule.child_model_id,
                    p_child_model_type_id = objISPModelRule.child_model_type_id,
                    p_isActive = objISPModelRule.is_active,
                    p_modified_on = DateTimeHelper.Now,
                    p_modified_by = userId
                }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelTypeMaster> EditModelTypeDetails(int modeltype_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelTypeMaster>("fn_isp_edit_update_modeltype_info", new {
                    p_modeltypeid = modeltype_id,
                    p_mode = "EDIT",
                    p_model_id = 0,
                    p_value = "",
                    p_color_code = "",
                    p_stroke_code = "",
                    p_type_abbr = "",
                    p_modified_by = 0,
                    p_model_color_id = 0,
                    p_is_middleware = false
                }, true);
                return res;
            }
            catch { throw; }
        }

        public List<ISPModelTypeMaster> UpdateModelTypeDetails(ISPModelTypeMaster objISPModelTypeMaster, int userId)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelTypeMaster>("fn_isp_edit_update_modeltype_info", new
                {
                    p_modeltypeid = objISPModelTypeMaster.id,
                    p_mode = "UPDATE",
                    p_model_id = objISPModelTypeMaster.model_id,
                    p_value = objISPModelTypeMaster.value,
                    p_color_code = objISPModelTypeMaster.color_code??"transparent",
                    p_stroke_code = objISPModelTypeMaster.stroke_code ?? "transparent",
                    p_type_abbr = objISPModelTypeMaster.type_abbr,
                    p_modified_by = userId,
                    p_model_color_id= objISPModelTypeMaster.model_color_id,
                    p_is_middleware = objISPModelTypeMaster.is_middleware_model_type
                }, true);
                return res;
            }
            catch { throw; }
        }

        public List<ISPModelInfo> GetModelRules(int parent_id, int? parent_type)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_model_rules", new { parent_id, parent_type }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetEquipmentRules(int parent_id, int? parent_type)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_equipment_rules", new { parent_id, parent_type }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetModelSubTypes(int? parent_id, int? parent_type, int? child_model)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_model_sub_type", new { parent_id, parent_type, child_model }, true);
                return res;
            }
            catch { throw; }
        }

        public DbMessage SaveModelMapping(int modelId, string mappingData, int modelViewId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_isp_save_model_mapping", new { p_model_info_id = modelId, p_model_view_id = modelViewId, p_model_children = mappingData }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<ISPModelInfo> GetModelChildren(int parent_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_model_children", new { parent_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetEquipmentChildren(int system_id)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_Equipment_children", new { p_parent_id= system_id }, true);
                return res;
            }
            catch { throw; }
        }
        public List<ISPModelInfo> GetModelByType(string modelType)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelInfo>("fn_isp_get_model_by_type", new { modelType }, true);
                return res;
            }
            catch { throw; }
        }

        public ISPModelInfo GetById(int id)
        {

            try
            {
                return repo.Get(x => x.id == id);
            }
            catch { throw; }

        }
        public ISPModelInfo GetMidelImageById(int id)
        {

            try
            {
                return repo.Get(x => x.model_image_id == id);
            }
            catch { throw; }

        }
    }
    public class DAISPModelRule : Repository<ISPModelRule>
    {
        private static DAISPModelRule instance = null;
        private static readonly object lockObject = new object();
        public static DAISPModelRule Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAISPModelRule();
                    }
                }
                return instance;
            }
        }
        public List<ISPModelRule> GetAll()
        {
            try
            {
                //return repo.GetAll(x => x.is_active).ToList();
                var res = repo.ExecuteProcedure<ISPModelRule>("fn_isp_get_model_all_rules", new { }, true);
                return res;
            }
            catch { throw; }
        }
        
    }

    public class DAISPModelMaster : Repository<ISPModelMaster>
    {
        private static DAISPModelMaster instance = null;
        private static readonly object lockObject = new object();
        public static DAISPModelMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAISPModelMaster();
                    }
                }
                return instance;
            }
        }
        public List<ISPModelMaster> GetAll()
        {
            try
            {
                return repo.GetAll(x => x.is_active).ToList();
            }
            catch { throw; }
        }

        public ISPModelMaster Get(int modelId)
        {
            try
            {
                return repo.Get(x => x.id == modelId);
            }
            catch { throw; }
        }
        public ISPModelMaster GetByKey(string key)
        {
            try
            {
                return repo.Get(x => x.key.ToLower() == key.ToLower());
            }
            catch { throw; }
        }
    }

    public class DAISPModelMapping : Repository<ISPModelMapping>
    {
        private static DAISPModelMapping instance = null;
        private static readonly object lockObject = new object();
        public static DAISPModelMapping Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAISPModelMapping();
                    }
                }
                return instance;
            }
        }


        public ISPModelMapping Get(int modelId)
        {
            try
            {
                return repo.Get(x => x.id == modelId);
            }
            catch { throw; }
        }

        public bool isChildModel(int modelId)
        {
            try
            {
                var record = repo.Get(x => x.child_model_info_id == modelId);
                return record != null;
            }
            catch { throw; }
        }

        public List<ISPModelMapping> GetBySuperParent(int modelId)
        {
            try
            {
                return repo.GetAll(x => x.super_parent_model_info_id == modelId).ToList();
               
            }
            catch { throw; }
        }
        public bool DeleteBySuperParent(int id) {
            try
            {
                var result = repo.GetAll(x => x.super_parent_model_info_id == id).ToList();
                return repo.DeleteRange(result) > 0;

            }
            catch { throw; }
        }
    }

    public class DAISPModelTypeMaster : Repository<ISPModelTypeMaster>
    {
        private static DAISPModelTypeMaster instance = null;
        private static readonly object lockObject = new object();
        public static DAISPModelTypeMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAISPModelTypeMaster();
                    }
                }
                return instance;
            }
        }


        public ISPModelTypeMaster Get(int id)
        {
            try
            {
                return repo.Get(x => x.id == id);
            }
            catch { throw; }
        }

     
    }

    public class DAISPModelColorMaster : Repository<ISPModelColorMaster>
    {
        private static DAISPModelColorMaster instance = null;
        private static readonly object lockObject = new object();
        public static DAISPModelColorMaster Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new DAISPModelColorMaster();
                    }
                }
                return instance;
            }
        }


        public ISPModelColorMaster Get(int id)
        {
            try
            {
                return repo.Get(x => x.id == id);
            }
            catch { throw; }
        }
        public List< ISPModelColorMaster> GetAll(int model_id)
        {
            try
            {
                return repo.GetAll(x => x.model_id == model_id).ToList();
            }
            catch { throw; }
        }
        public List<ISPModelColorMaster> GetAllByKey(string key)
        {
            try
            {
                var res = repo.ExecuteProcedure<ISPModelColorMaster>("fn_isp_get_model_color", new { p_model_key = key }, true);//repo.GetAll().ToList();
                return res;
            }
            catch { throw; }
        }
        
        
    }
    public class DAISPModelImageMaster : Repository<ISPModelImageMaster>
    {
        public List<ISPModelImageMaster> GetIspModelImage(ViewIspModelImage objIspModelImage)
        {
            try
            {
                var lst = repo.ExecuteProcedure<ISPModelImageMaster>("fn_get_isp_model_image", new
                {
                    p_pageno = objIspModelImage.currentPage,
                    p_pagerecord = objIspModelImage.pageSize,
                    p_sortcolname = objIspModelImage.sort,
                    p_sorttype = objIspModelImage.orderBy,
                    p_searchBy = objIspModelImage.searchBy,
                    p_searchText = objIspModelImage.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public ISPModelImageMaster SaveModleImage(ISPModelImageMaster input)
        {
            try
            {
               
                var objFaqMst = repo.Get(x => x.id == input.id);
                if (input.id > 0)
                {

                    objFaqMst.is_active = input.is_active;
                    objFaqMst.modified_by = input.created_by;
                    objFaqMst.modified_on = DateTimeHelper.Now;
                    return repo.Update(objFaqMst);

                }
                else
                {
                  
                    input.created_by = input.created_by;
                    input.created_on = DateTimeHelper.Now;
                    return repo.Insert(input);


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string DeleteModleImageById(int id)
        {
            try
            {
                var result = "False";
              var validate=  DAISPModelInfo.Instance.GetMidelImageById(id);
                var objId = repo.Get(x => x.id == id);
                if (objId != null)
                {
                    if(validate==null)
                    {
                    repo.Delete(objId.id);
                    result = "DELETE";
                    }
                    else
                    {
                        result = "USED";
                    }
                }

                return result;

            }
            catch { throw; }

        }
        public ISPModelImageMaster GetModleImageById(int id)
        {
            try
            {
                return repo.Get(u => u.id == id);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
    }
