using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
    public class BLAccessories
    {
        public AccessoriesInfoModel SaveAccessories(AccessoriesInfoModel input, int userId)
        {
            return new DAAccessories().SaveAccessories(input, userId);
        }
        public bool ChkDuplicateAccessoriesBySpecification(AccessoriesInfoModel objAccessoriesInfoModel)
        {
            return new DAAccessories().ChkDuplicateAccessoriesBySpecification(objAccessoriesInfoModel);
        }
        public int DeleteAccessoriesById(int systemId)
        {
            return new DAAccessories().DeleteAccessoriesById(systemId);
        }
        public int UpdateAccessoriesNetworkStatus(int systemId, string NetworkStatus)
        {
            return new DAAccessories().UpdateAccessoriesNetworkStatus(systemId,NetworkStatus);
        }

        public AccessoriesInfoModel GeteAccessoriesById(int systemId)
        {
            return new DAAccessories().GeteAccessoriesById(systemId);
        }
        public AccessoriesInfoModel GetAccessories(int systemId,string entityType)
        {
            return new DAAccessories().GetAccessories(systemId, entityType);
        }

        public List<AccessoriesMaster> GetAccesoriesTypeByLayeKey(string key)
        {
            return new DAAccessories().GetAccesoriesTypeByLayeKey(key);
        }

        public List<AccessoriesMaster> GetAccessoriesList(AccessoriesViewModel objViewAccessories, int userId)
        {
            return new DAAccessoriesMaster().GetAccessoriesList(objViewAccessories, userId);
        }
        public AccessoriesMaster SaveAccessories(AccessoriesMaster objAccessoriesMst, int userId)
        {
            return new DAAccessoriesMaster().SaveAccessories(objAccessoriesMst, userId);
        }
        public AccessoriesMaster GetAccessoriesById(int id)
        {
            return new DAAccessoriesMaster().GetAccessoriesById(id);
        }
        public int DeleteAccessoriesById(int id, int userId)
        {
            return new DAAccessoriesMaster().DeleteAccessoriesById(id, userId);
        }
        public DbMessage verifyAccessories(int id, string entity_type,string accName)
        {
            return new DAAccessoriesMaster().verifyAccessories(id, entity_type, accName);
        }
        public List<AccessoriesMapping> GetAccessoriesMappingList(AccessoriesViewModel objViewAccessories, int userId)
        {
            return new DAAccessoriesMapping().GetAccessoriesMappingList(objViewAccessories, userId);
        }

        public List<AccessoriesMaster> GetAccessoriesDropdownList()
        {
            return new DAAccessoriesMaster().GetAccessoriesDropdownList();
        }
        public AccessoriesMapping SaveAccessoriesMapping(AccessoriesMapping objAccessoriesMapping, int userId)
        {
            return new DAAccessoriesMapping().SaveAccessoriesMapping(objAccessoriesMapping, userId);
        }
        public int DeleteAccessoriesMappingById(int id, int userId)
        {
            return new DAAccessoriesMapping().DeleteAccessoriesMappingById(id, userId);
        }

        public AccessoriesMapping GetAccessoriesMappingById(int id)
        {
            return new DAAccessoriesMapping().GetAccessoriesMappingById(id);
        }
        public List<AccessoriesMappingList> GetAccessoriesDropdownListByType(string type)
        {
            return new DAAccessoriesMapping().GetAccessoriesDropdownListByType(type);
        }
        public List<AccessoriesMapping> ChkDuplicateAccessoriesExist(AccessoriesMapping objAccessoriesExist)
        {
            return new DAAccessoriesMapping().ChkDuplicateAccessoriesExist(objAccessoriesExist);
        }
    }
}
