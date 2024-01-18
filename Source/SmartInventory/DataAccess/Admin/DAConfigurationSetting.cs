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
   public class DAEquipmentTypeMaster : Repository<EquipmentTypeMaster>
    {
        public List<EquipmentTypeMaster> getEquipmentTypeDetailByIdList(int entity_id = 0)
        {
            return repo.GetAll(m => m.entity_id == entity_id).ToList();

        }

        public EquipmentTypeMaster getEquipmentTypeDetailById(int id, int entity_id = 0)
        {
            var objgetEquipmentTypeDetailById = new EquipmentTypeMaster();

            if (entity_id != 0)
            {
                objgetEquipmentTypeDetailById = repo.Get(m => m.entity_id == entity_id);
            }

            else
            {
                objgetEquipmentTypeDetailById = repo.Get(m => m.id == id);
            }

            return objgetEquipmentTypeDetailById;




        }


        public EquipmentTypeMaster SaveEquipmentTypeDetails(EquipmentTypeMaster objAddEquipmentType, int userId)
        {
            try
            {
                if (objAddEquipmentType.id != 0)
                {

                    objAddEquipmentType.modified_by = userId;
                    objAddEquipmentType.modified_on = DateTimeHelper.Now;

                    return repo.Update(objAddEquipmentType);



                }

                else
                {
                    objAddEquipmentType.created_by = userId;
                    objAddEquipmentType.modified_by = userId;
                    objAddEquipmentType.modified_on = DateTimeHelper.Now;
                    return repo.Insert(objAddEquipmentType);


                }

            }
            catch { throw; }
        }

        public int DeleteEquipmentTypeDetailById(int Id)
        {
            try
            {
                var objId = repo.Get(x => x.id == Id);
                if (objId != null)
                {
                    return repo.Delete(objId.id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }


        public int IsValueExistsForConfigSettings(string type, string name, int parent_id)
        {
            try
            {
                var getResult = repo.ExecuteProcedure<object>("fn_uniquenameforconfigseetings", new { input_name = name, type = type, parent_id = parent_id }, true);

                if(getResult != null && getResult.Count > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }

        }

       


    }

   public class DABrandTypeMaster : Repository<BrandTypeMaster>
   {
       public List<BrandTypeMaster> getBrandTypeDetailByIdList(int equipment_id = 0)
       {
           return repo.GetAll(m => m.type_id == equipment_id).ToList();

       }

       public BrandTypeMaster getBrandTypeDetailById(int id, int equipment_id = 0)
       {
           var objgetBrandTypeDetailById = new BrandTypeMaster();

           if (equipment_id != 0)
           {
               objgetBrandTypeDetailById = repo.Get(m => m.type_id == equipment_id);
           }

           else
           {
               objgetBrandTypeDetailById = repo.Get(m => m.id == id);
           }

           return objgetBrandTypeDetailById;




       }

       public BrandTypeMaster SaveBrandTypeDetails(BrandTypeMaster objAddBrandType, int userId)
       {
           try
           {
               if (objAddBrandType.id != 0)
               {

                   objAddBrandType.modified_by = userId;
                   objAddBrandType.modified_on = DateTimeHelper.Now;

                   return repo.Update(objAddBrandType);



               }

               else
               {
                   objAddBrandType.created_by = userId;
                   objAddBrandType.modified_by = userId;
                   objAddBrandType.modified_on = DateTimeHelper.Now;
                   return repo.Insert(objAddBrandType);


               }

           }
           catch { throw; }
       }

       public int DeleteBrandTypeDetailById(int Id)
       {
           try
           {
               var objId = repo.Get(x => x.id == Id);
               if (objId != null)
               {
                   return repo.Delete(objId.id);
               }
               else
               {
                   return 0;
               }


           }
           catch { throw; }
       }
   }


   public class DAModelTypeMaster : Repository<ModelTypeMaster>
   {
         public List<ModelTypeMaster> getModelTypeDetailByIdList(int BrandType_id = 0)
       {   
           return repo.GetAll(m => m.brand_id == BrandType_id).ToList();
       }

         public ModelTypeMaster getModelTypeDetailById(int id, int brand_id = 0)
         {
             var objgetModelTypeDetailById = new ModelTypeMaster();

             if (brand_id != 0)
             {
                 objgetModelTypeDetailById = repo.Get(m => m.brand_id == brand_id);
             }

             else
             {
                 objgetModelTypeDetailById = repo.Get(m => m.id == id);
             }

             return objgetModelTypeDetailById;




         }


         public ModelTypeMaster SaveModelTypeDetails(ModelTypeMaster objAddModelType, int userId)
         {
             try
             {
                 if (objAddModelType.id != 0)
                 
                 {
                     objAddModelType.modified_by = userId;
                     objAddModelType.modified_on = DateTimeHelper.Now;

                     return repo.Update(objAddModelType);



                 }

                 else
                 {
                     objAddModelType.created_by = userId;
                     objAddModelType.modified_by = userId;
                     objAddModelType.modified_on = DateTimeHelper.Now;
                     return repo.Insert(objAddModelType);


                 }

             }
             catch { throw; }
         }




         public int DeleteModelTypeDetailById(int Id)
         {
             try
             {
                 var objId = repo.Get(x => x.id == Id);
                 if (objId != null)
                 {
                     return repo.Delete(objId.id);
                 }
                 else
                 {
                     return 0;
                 }


             }
             catch { throw; }
         }

   }


   public class DAPortMaster : Repository<PortInfo>
   {
       public PortInfo SavePortDetails(PortInfo objAddPortInfo, int userId)
       {
           try
           {
               if (objAddPortInfo.id != 0)
               {

                   objAddPortInfo.modified_by = userId;
                   objAddPortInfo.modified_on = DateTimeHelper.Now;

                   return repo.Update(objAddPortInfo);



               }

               else
               {
                   objAddPortInfo.created_by = userId;
                   objAddPortInfo.modified_by = userId;
                   objAddPortInfo.modified_on = DateTimeHelper.Now;
                   return repo.Insert(objAddPortInfo);


               }

           }
           catch { throw; }
       }

       public int DeleteInputOutputPortById(int Id)
         {
             try
             {
                 var objId = repo.Get(x => x.id == Id);
                 if (objId != null)
                 {
                     return repo.Delete(objId.id);
                 }
                 else
                 {
                     return 0;
                 }


             }
             catch { throw; }
         }


       public List<PortInfo> getInputPortInfoByIdList(int modelId = 0)
       {
            return repo.GetAll(m => m.model_id == modelId).ToList();

       }


      


   }


    
}
