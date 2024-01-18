using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DataAccess.DBHelpers;

namespace DataAccess
{
    public sealed class DAIspEntityMapping : Repository<IspEntityMapping>
    {
        DAIspEntityMapping() { }

        private static readonly object lockObject = new object();
        private static DAIspEntityMapping objEntityMap = null;

        public static DAIspEntityMapping Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objEntityMap == null)
                    {
                        objEntityMap = new DAIspEntityMapping();
                    }
                }
                return objEntityMap;
            }
        }

        public IspEntityMapping GetIspEntityMapByCustomerId(int customer_system_id, string entity_type)
        {
            try
            {
                var result= repo.Get(u => u.entity_id == customer_system_id && u.entity_type==entity_type);
                return result != null ? result : new IspEntityMapping(); 
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IspEntityMapping SaveIspEntityMapping(IspEntityMapping objMapping)
        {
            try
            {
                var objIspMapping = repo.Get(u => u.id == objMapping.id);
                if (objIspMapping != null)
                {
                    objIspMapping.shaft_id = objMapping.shaft_id;
                    objIspMapping.floor_id = objMapping.floor_id;
                    objIspMapping.entity_type = objMapping.entity_type;
                    objIspMapping.entity_id = objMapping.entity_id;
                    objIspMapping.parent_id = objMapping.parent_id;
                    objIspMapping.structure_id = objMapping.structure_id;
                    return repo.Update(objIspMapping);
                }
                else
                {
                    return repo.Insert(objMapping);
                }
            }
            catch { throw; }
        }

        public IspEntityMapping GetIspEntityMapById(int mappingId)
        {
            IspEntityMapping result = new IspEntityMapping();
            try
            {
                var structureDetails = repo.Get(u => u.id == mappingId);
                if (structureDetails != null)
                {
                    result = structureDetails;
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public bool updateEntityMapping(int mappingId, int newMappingId)
        {
            var mappedDetails = repo.GetAll(u => u.parent_id == mappingId && mappingId != 0);
            if (mappedDetails != null && mappedDetails.Count() > 0)
            {
                foreach (var item in mappedDetails)
                {
                    item.parent_id = newMappingId;
                    repo.Update(item);
                }
                return true;
            }
            return false;
        }
        public bool updateEntityMapping(IspEntityMapping ispEntity)
        {
            var mappedDetails = repo.GetAll(u => u.parent_id == ispEntity.id);
            if (mappedDetails != null && mappedDetails.Count() > 0)
            {
                foreach (var item in mappedDetails)
                {
                    item.parent_id = ispEntity.id;
                    item.structure_id = ispEntity.structure_id;
                    item.shaft_id = ispEntity.shaft_id;
                    item.floor_id = ispEntity.floor_id;
                    repo.Update(item);
                }
                return true;
            }
            return false;
        }
        public bool InsertEntityMapping(IspEntityMapping ispEntity)
        {
            var childList = new DAFMS().GetFMSByParentId(ispEntity.entity_id, ispEntity.entity_type);
            if (childList != null && childList.Count() > 0)
            {
                foreach (var item in childList)
                {
                    var mappedDetails = repo.GetById(u => u.entity_id == item.system_id && u.entity_type == EntityType.FMS.ToString());
                    if (mappedDetails != null)
                    {
                        mappedDetails.parent_id = ispEntity.id;
                        mappedDetails.structure_id = ispEntity.structure_id;
                        mappedDetails.shaft_id = ispEntity.shaft_id;
                        mappedDetails.floor_id = ispEntity.floor_id;
                        repo.Update(mappedDetails);
                    }
                }
                return true;
            }
            return false;
        }
        public int DeleteIspEntityMap(int mappingId)
        {
            int result = 0;
            try
            {
                var objMapping = repo.Get(u => u.id == mappingId);
                if (objMapping != null)
                {
                    result = repo.Delete(objMapping.id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public IspEntityMapping GetIspEntityMapByStrucId(int structure_id, int entity_id, string entity_type)
        {
            IspEntityMapping result = new IspEntityMapping();
            try
            {
                var mappedDetails = repo.Get(u => u.entity_type == entity_type && u.entity_id == entity_id);
                if (mappedDetails != null)
                {
                    result = mappedDetails;
                    return result;
                }

            }
            catch
            {
                throw;
            }
            return result;
        }
        public IspEntityMapping GetStructureFloorbyEntityId(int entity_id, string entity_type)
        {
            IspEntityMapping result = new IspEntityMapping();
            try 
            {
                var mappedDetails = repo.Get(u => u.entity_type == entity_type && u.entity_id == entity_id);
                if (mappedDetails != null)
                {
                    result = mappedDetails;
                    return result;
                }

            }
            catch
            {
                throw;
            }
            return result;
        }
        public int DeleteIspEntityByStrucId(int structure_id, int entity_id, string entity_type, ref int mappingSysId)
        {
            int result = 0;
            try
            {
                var objMapping = repo.Get(u => u.structure_id == structure_id && u.entity_type == entity_type && u.entity_id == entity_id);
                if (objMapping != null)
                {
                    mappingSysId = objMapping.id;
                    result = repo.Delete(objMapping.id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public DbMessage associateEntityInStructure(int? shaftId, int? floorId, int systemId, string entityType, int parentSystemId, string parentEntityType,string longlat)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_associate_entity_to_structure", new { p_shaft_id = shaftId ?? 0, p_floor_id = floorId ?? 0, p_entity_system_id = systemId, p_entity_type = entityType, p_parent_system_id = parentSystemId, p_parent_entity_type = parentEntityType, p_longlat= longlat }).FirstOrDefault();
            }
            catch { throw; }
        }

        public bool IsShaftAssociated(int shaftId)
        {
            bool result = false;
            try
            {
                var shaftDetails = repo.Get(u => u.shaft_id == shaftId);
                if (shaftDetails != null)
                {
                    result = true;

                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public bool IsFloorAssociated(int floorId)
        {
            bool result = false;
            try
            {
                var floorDetails = repo.Get(u => u.floor_id == floorId);
                if (floorDetails != null)
                {
                    result = true;

                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public bool IsEntityExist(int structureId, int floorId, int shaftId, string entityType)
        {
            try
            {
                var entityDetails = repo.Get(u => u.floor_id == floorId && u.structure_id == structureId && u.shaft_id == shaftId && u.entity_type == entityType);
                if (entityDetails != null)
                { return true; }
                else { return false; }
            }
            catch { throw; }
        }
    }
}
