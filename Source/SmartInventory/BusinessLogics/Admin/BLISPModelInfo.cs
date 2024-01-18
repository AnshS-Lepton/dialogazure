using DataAccess.Admin;
using Models;
using Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogics.Admin
{
    public class BLISPModelInfo
    {
        BLISPModelInfo()
        {

        }

        private static BLISPModelInfo instance = null;
        private static readonly object lockObject = new object();
        public static BLISPModelInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new BLISPModelInfo();
                    }
                }
                return instance;
            }
        }



        public List<AllEquipments> GetAllEquipments()
        {
            return DAISPModelInfo.Instance.GetAllEquipments();
        }

        public List<ModelDetails> GetModelDetails(ISPModelInfo objISPModelMaster, int modelID = 0, string searchText = "", string searchBy = "")
        {
            return DAISPModelInfo.Instance.GetModelDetails(objISPModelMaster, modelID, searchText, searchBy);
        }

        public List<ISPModelRule> GetRuleDetails(ISPModelRule objISPModelRule, string searchText = "", string searchBy = "")
        {
            return DAISPModelInfo.Instance.GetRuleDetails(objISPModelRule, searchText, searchBy);
        }
        public List<ISPModelTypeMaster> GetModalTypeDetails(ISPModelTypeMaster objISPModelTypeMaster, string searchText = "", string searchBy = "")
        {
            return DAISPModelInfo.Instance.GetModalTypeDetails(objISPModelTypeMaster, searchText, searchBy);
        }
        public int SaveRule(ISPModelRule record)
        {
            return DAISPModelInfo.Instance.SaveRule(record);
        }
        public int DeleteModalType(int id)
        {
            return DAISPModelInfo.Instance.DeleteModalType(id);

        }
        public int DeleteRule(int id)
        {
            return DAISPModelInfo.Instance.DeleteRule(id);

        }

        public List<CheckRulesExists> CheckModelTypeExists(int parent_model_type_id)
        {
            return DAISPModelInfo.Instance.CheckModelTypeExists(parent_model_type_id);
        }
        public List<CheckRulesExists> CheckModelTypeExists(string model_type_name)
        {
            return DAISPModelInfo.Instance.CheckModelTypeExists(model_type_name);
        }
        public List<CheckRulesExists> CheckRulesExists(int rule_id)
        {
            return DAISPModelInfo.Instance.CheckRulesExists(rule_id);
        }
        public List<CheckRulesExists> CheckRulesExists(int parent_model_id, int? parent_model_type_id, int child_model_id, int? child_model_type_id)
        {
            return DAISPModelInfo.Instance.CheckRulesExists(parent_model_id, parent_model_type_id, child_model_id, child_model_type_id);
        }
        public int DeleteModelDetailsById(int id)
        {
            return DAISPModelInfo.Instance.DeleteModelDetailsById(id);

        }

        public int SaveModelType(ISPModelTypeMaster record)
        {
            return DAISPModelInfo.Instance.SaveModelType(record);
        }
        public List<ModelStatusCount> GetModelStatusCount(int modelID = 0)
        {
            return DAISPModelInfo.Instance.GetModelStatusCount(modelID);
        }

        public ISPModelInfo SaveModelInfo(ISPModelInfo input, int userId)
        {
            return DAISPModelInfo.Instance.SaveModelInfo(input, userId);
        }

        public List<ISPModelStatusMaster> GetModelStatus()
        {
            return DAISPModelInfo.Instance.GetModelStatus();
        }

        public List<ISPModelMaster> GetModels()
        {
            return DAISPModelInfo.Instance.GetModels();
        }

        public List<MiddleWareEntity> GetMiddleWareLayers()
        {
            return DAISPModelInfo.Instance.GetMiddleWareLayers();
        }
        public List<ISPModelMaster> GetModelMaster()
        {
            return DAISPModelInfo.Instance.GetModelMaster();
        }

        public List<ISPModelTypeMaster> GetModelTypes(int model_id)
        {
            return DAISPModelInfo.Instance.GetModelTypes(model_id);
        }

        public List<ISPModelInfo> GetModelsWithImage(int model_id)
        {
            return DAISPModelInfo.Instance.GetModelsWithImage(model_id);
        }

        public List<ISPModelInfo> GetEquipmentWithImage(int system_id)
        {
            return DAISPModelInfo.Instance.GetEquipmentWithImage(system_id);
        }
        
        public List<ISPModelInfo> GetModelInfo(int model_id)
        {
            return DAISPModelInfo.Instance.GetModelInfo(model_id);
        }
        public List<ISPModelInfo> GetEquipmentInfo(int system_id)
        {
            return DAISPModelInfo.Instance.GetEquipmentInfo(system_id);
        }
        
        public List<ISPModelRule> EditRuleDetails(int rule_id)
        {
            return DAISPModelInfo.Instance.EditRuleDetails(rule_id);
        }
        public List<ISPModelTypeMaster> EditModelTypeDetails(int modelType_id)
        {
            return DAISPModelInfo.Instance.EditModelTypeDetails(modelType_id);
        }
        public List<ISPModelTypeMaster> UpdateModelTypeDetails(ISPModelTypeMaster objISPModelTypeMaster, int userId)
        {
            return DAISPModelInfo.Instance.UpdateModelTypeDetails(objISPModelTypeMaster, userId);
        }
        public List<ISPModelRule> UpdateRuleDetails(ISPModelRule objISPModelRule, int userId)
        {
            return DAISPModelInfo.Instance.UpdateRuleDetails(objISPModelRule, userId);
        }

        public List<ISPModelInfo> GetModelRules(int parent_id, int? parent_type)
        {
            return DAISPModelInfo.Instance.GetModelRules(parent_id, parent_type);
        }
        public List<ISPModelInfo> GetEquipmentRules(int parent_id, int? parent_type)
        {
            return DAISPModelInfo.Instance.GetEquipmentRules(parent_id, parent_type);
        }
        
        public List<ISPModelInfo> GetModelSubTypes(int? parent_id, int? parent_type, int? child_model)
        {
            return DAISPModelInfo.Instance.GetModelSubTypes(parent_id, parent_type, child_model);
        }

        public DbMessage SaveModelMapping(int modelId, string mappingData, int modelViewId = 1)
        {
            return DAISPModelInfo.Instance.SaveModelMapping(modelId, mappingData, modelViewId);
        }

        public List<ISPModelInfo> GetModelChildren(int modelId)
        {
            return DAISPModelInfo.Instance.GetModelChildren(modelId);
        }
        public List<ISPModelInfo> GetEquipmentChildren(int systemId)
        {
            return DAISPModelInfo.Instance.GetEquipmentChildren(systemId);
        }
        
        public List<ISPModelRule> GetModelAllRules()
        {
            return DAISPModelRule.Instance.GetAll();
        }

        public bool ModelHasTypes(int id)
        {
            var model = DAISPModelMaster.Instance.Get(id);
            if (model != null)
            {
                return model.has_type;
            }
            return false;
        }

        public bool IsEditableModel(int id)
        {
            var model = (new BLRack()).GetByModelInfoId(id);
            bool isEditable = !DAISPModelMapping.Instance.isChildModel(id) && (model == null || model.Count == 0);
            return isEditable;
        }

        public List<ISPModelInfo> GetModelByType(string modelType)
        {
            return DAISPModelInfo.Instance.GetModelByType(modelType);
        }

        public ISPModelInfo GetById(int id)
        {
            return DAISPModelInfo.Instance.GetById(id);
        }
        public List<ISPModelMapping> GetMappingBySuperParent(int id)
        {
            return DAISPModelMapping.Instance.GetBySuperParent(id);
        }
        public bool DeleteMapBySuperParent(int id)
        {
            return DAISPModelMapping.Instance.DeleteBySuperParent(id);
        }

        public ISPModelTypeMaster GetModelType(int id)
        { return DAISPModelTypeMaster.Instance.Get(id); }

        public ISPModelMaster GetModelMaster(int id)
        { return DAISPModelMaster.Instance.Get(id); }

        public List<ISPModelColorMaster> GetColorByModelId(int model_id)
        {
            return DAISPModelColorMaster.Instance.GetAll(model_id);
        }

        public List<ISPModelColorMaster> GetColorByModelKey(string key)
        {
           
            return DAISPModelColorMaster.Instance.GetAllByKey(key);
        }
        public ISPModelMaster GetModelMasterByKey(string key)
        {
            return DAISPModelMaster.Instance.GetByKey(key);
        }
        public List<ISPModelImageMaster> GetIspModelImage(ViewIspModelImage objIspModelImage)
        {
            return new DAISPModelImageMaster().GetIspModelImage(objIspModelImage);
        }

        public List<ISPModelMaster> GetModelImage()
        {
            return DAISPModelMaster.Instance.GetAll();
        }
        public ISPModelImageMaster SaveModleImage(ISPModelImageMaster objModelImage)
        {
            return new DAISPModelImageMaster().SaveModleImage(objModelImage);
        }
        public string DeleteModleImageById(int id)
        {
            return new DAISPModelImageMaster().DeleteModleImageById(id);
        }
        public ISPModelImageMaster GetModleImageById(int id)
        {
            return new DAISPModelImageMaster().GetModleImageById(id);
        }
    }

}
