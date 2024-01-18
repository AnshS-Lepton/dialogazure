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
   public class BLConfigurationSetting
    {


        #region BrandType

            public List<BrandTypeMaster> getBrandTypeDetailByIdList(int equipment_id = 0)
        {

            return new DABrandTypeMaster().getBrandTypeDetailByIdList(equipment_id);
        }

            public BrandTypeMaster getBrandTypeDetailById(int id, int equipment_id = 0)
        {
            return new DABrandTypeMaster().getBrandTypeDetailById(id, equipment_id);

        }


            public BrandTypeMaster SaveBrandTypeDetails(BrandTypeMaster objBrandType, int userId)
            {
                return new DABrandTypeMaster().SaveBrandTypeDetails(objBrandType, userId);
            }

            public int DeleteBrandTypeDetailById(int id)
            {
                return new DABrandTypeMaster().DeleteBrandTypeDetailById(id);

            }

    
        #endregion

        #region ModelTypeMaster

            public List<ModelTypeMaster> getModelTypeDetailByIdList(int BrandType_id = 0)
        {

            return new DAModelTypeMaster().getModelTypeDetailByIdList(BrandType_id);
        }


           public List<PortInfo> GetModelTypePortInforDetails(int model_id)
            {
                return new DAPortMaster().getInputPortInfoByIdList(model_id);

            }
           
            public ModelTypeMaster getModelTypeDetailById(int id, int BrandType_id = 0)
            {
                return new DAModelTypeMaster().getModelTypeDetailById(id, BrandType_id);

            }

             

            public ModelTypeMaster SaveModelTypeDetails(ModelTypeMaster objModelType, int userId)
            {  
                  return new DAModelTypeMaster().SaveModelTypeDetails(objModelType, userId);
            }

            public PortInfo SavePortInfoDetails(PortInfo objModelType, int userId)
            {
                return new DAPortMaster().SavePortDetails(objModelType, userId);

            }

            public int DeleteModelTypeDetailById(int id)
            {
                return new DAModelTypeMaster().DeleteModelTypeDetailById(id);

            }

            public int DeleteInputOutputPortById(int id)
            {
                return new DAPortMaster().DeleteInputOutputPortById(id);

            }

       

       

        #endregion

        #region Equipment Type

            public List<EquipmentTypeMaster> getEquipmentTypeDetailByIdList(int entity_id = 0)
              {
                  return new DAEquipmentTypeMaster().getEquipmentTypeDetailByIdList(entity_id);
              }

            public EquipmentTypeMaster getEquipmentTypeDetailById(int id, int entity_id = 0)
              {
                  return new DAEquipmentTypeMaster().getEquipmentTypeDetailById(id, entity_id);    
              }
            
            public EquipmentTypeMaster SaveEquipmentTypeDetails(EquipmentTypeMaster objEquipmentType, int userId)
             {
                 return new DAEquipmentTypeMaster().SaveEquipmentTypeDetails(objEquipmentType, userId);
             }

            public int DeleteEquipmentTypeDetailById(int id)
            {
                return new DAEquipmentTypeMaster().DeleteEquipmentTypeDetailById(id);

            }


            public int IsValueExistsForConfigSettings(string type, string name, int parent_id)
            {

                return new DAEquipmentTypeMaster().IsValueExistsForConfigSettings(type, name, parent_id);
            }



        #endregion


        public bool SaveConfigSetting(ChangeConfigurationSetting obj, int userId)
        {
            return new DAChangeConfigurationSetting().SavConfigSetting(obj,userId);
        }
    }

    
}
