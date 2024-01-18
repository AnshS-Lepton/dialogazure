using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class DAAccessories : Repository<AccessoriesInfoModel>
    {
        public AccessoriesInfoModel SaveAccessories(AccessoriesInfoModel input, int userId)
        {
            try
            {
                var objAccessories = repo.Get(x => x.system_id == input.system_id);
                if (objAccessories != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(input.modified_on, objAccessories.modified_on, input.modified_by, objAccessories.modified_by);
                    if (objPageValidate.message != null)
                    {
                        input.objPM = objPageValidate;
                        return input;
                    }


                    objAccessories.accessories_id = input.accessories_id;
                    objAccessories.quantity = input.quantity;
                    objAccessories.remarks = input.remarks;

                    objAccessories.specification = input.accessories_template.specification;

                    objAccessories.vendor_id = input.accessories_template.vendor_id;
                    objAccessories.subcategory1 = input.accessories_template.subcategory1;
                    objAccessories.subcategory2 = input.accessories_template.subcategory2;
                    objAccessories.subcategory3 = input.accessories_template.subcategory3;
                    objAccessories.item_code = input.accessories_template.item_code;
                    objAccessories.audit_item_master_id = input.audit_item_master_id;

                    objAccessories.modified_by = userId;

                    objAccessories.modified_on = DateTimeHelper.Now;

                    var result = repo.Update(objAccessories);
                    return result;
                }
                else
                {
                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now;
                    input.status = "A";
                    input.network_status = "P";
                    input.specification = input.accessories_template.specification;

                    input.vendor_id = input.accessories_template.vendor_id;
                    input.subcategory1 = input.accessories_template.subcategory1;
                    input.subcategory2 = input.accessories_template.subcategory2;
                    input.subcategory3 = input.accessories_template.subcategory3;
                    input.item_code = input.accessories_template.item_code;
                    var resultItem = repo.Insert(input);

                    return resultItem;
                }
            }
            catch { throw; }
        }
        public bool ChkDuplicateAccessoriesBySpecification(AccessoriesInfoModel objAccessoriesInfoModel)
        {
            try
            {
                var obj = repo.GetAll(u => u.accessories_id == objAccessoriesInfoModel.accessories_id &&
                u.system_id!=objAccessoriesInfoModel.system_id &&
                u.parent_system_id == objAccessoriesInfoModel.parent_system_id && u.parent_entity_type == objAccessoriesInfoModel.parent_entity_type &&
                u.specification == objAccessoriesInfoModel.specification && u.vendor_id == objAccessoriesInfoModel.vendor_id).ToList();

                
                if (obj.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { throw; }
        }

        public int DeleteAccessoriesById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }


            }
            catch { throw; }
        }

        public AccessoriesInfoModel GeteAccessoriesById(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.system_id == systemId);
                return objSystmId;
            }
            catch { throw; }
        }

        public List<AccessoriesMaster> GetAccesoriesTypeByLayeKey(string key)
        {
            try
            {
                return repo.ExecuteProcedure<AccessoriesMaster>("fn_get_accessories_types_by_layer", new { p_layer_key = key }, true).ToList();
            }
            catch { throw; }
        }

        public AccessoriesInfoModel GetAccessories(int systemId ,string entityType)
        {
            try
            {
                var objSystmId = repo.Get(x => x.parent_system_id == systemId && x.parent_entity_type== entityType);
                return objSystmId;
            }
            catch { throw; }
        }

        public int UpdateAccessoriesNetworkStatus(int systemId, string NetworkStatus)
        {
            try
            {
                var objAccessories = repo.Get(x => x.system_id == systemId);
                if (objAccessories != null)
                {
                    objAccessories.network_status = NetworkStatus;
                    var obj = repo.Update(objAccessories);
                    return obj.system_id;
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }
        }


    }

    public class DAAccessoriesMaster : Repository<AccessoriesMaster>
    {
        public List<AccessoriesMaster> GetAccessoriesList(AccessoriesViewModel objViewAccessories, int user_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<AccessoriesMaster>("fn_accessories_get_list", new
                {
                    p_pageno = objViewAccessories.objFilterAttributes.currentPage,
                    p_pagerecord = objViewAccessories.objFilterAttributes.pageSize,
                    p_sortcolname = objViewAccessories.objFilterAttributes.sort,
                    p_sorttype = objViewAccessories.objFilterAttributes.orderBy,
                    p_userid = user_id,
                    p_isactive = objViewAccessories.objFilterAttributes.status,
                    p_searchBy = objViewAccessories.objFilterAttributes.searchBy,
                    p_searchText = objViewAccessories.objFilterAttributes.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public AccessoriesMaster SaveAccessories(AccessoriesMaster input, int userId)
        {
            try
            {
                var objAccessoriesMst = repo.Get(x => x.id == input.id);
                if (input.id > 0)
                {
                    objAccessoriesMst.is_active = input.is_active;
                    objAccessoriesMst.name = input.name;
                    objAccessoriesMst.display_name = input.display_name;
                    objAccessoriesMst.modified_by = userId;
                    objAccessoriesMst.modified_on = DateTimeHelper.Now;
                    return repo.Update(objAccessoriesMst);

                }
                else
                {
                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now; ;
                    return repo.Insert(input);


                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public AccessoriesMaster GetAccessoriesById(int id)
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

        public int DeleteAccessoriesById(int id, int userId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }

        }

        public DbMessage verifyAccessories(int Id, string entityType, string AccName)
        {
            try
            {
                try
                {
                    return repo.ExecuteProcedure<DbMessage>("fn_accessories_verify", new { p_system_id = Id, p_entity_type = entityType, p_acc_name = AccName }).FirstOrDefault();

                }
                catch { throw; }
            }
            catch { throw; }
        }
        public List<AccessoriesMaster> GetAccessoriesDropdownList()
        {
            try
            {
                return repo.GetAll().ToList();

            }
            catch { throw; }
        }




    }

    public class DAAccessoriesMapping : Repository<AccessoriesMapping>
    {
        public List<AccessoriesMapping> GetAccessoriesMappingList(AccessoriesViewModel objViewAccessories, int user_id)
        {
            try
            {
                var lst = repo.ExecuteProcedure<AccessoriesMapping>("fn_accessories_layer_mapping_get_list", new
                {
                    p_pageno = objViewAccessories.objFilterAttributes.currentPage,
                    p_pagerecord = objViewAccessories.objFilterAttributes.pageSize,
                    p_sortcolname = objViewAccessories.objFilterAttributes.sort,
                    p_sorttype = objViewAccessories.objFilterAttributes.orderBy,
                    p_userid = user_id,
                    p_isactive = objViewAccessories.objFilterAttributes.status,
                    p_searchBy = objViewAccessories.objFilterAttributes.searchBy,
                    p_searchText = objViewAccessories.objFilterAttributes.searchText,
                }, true);
                return lst;

            }
            catch { throw; }
        }

        public AccessoriesMapping GetAccessoriesMappingById(int id)
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
        public AccessoriesMapping SaveAccessoriesMapping(AccessoriesMapping input, int userId)
        {
            try
            {
                var objAccessories = repo.Get(x => x.id == input.id);
                if (input.id > 0)
                {
                    objAccessories.is_active = input.is_active;
                    objAccessories.min_quantity = input.min_quantity;
                    objAccessories.max_quantity = input.max_quantity;
                    objAccessories.modified_by = userId;
                    objAccessories.modified_on = DateTimeHelper.Now;
                    return repo.Update(objAccessories);

                }
                else
                {
                    input.created_by = userId;
                    input.created_on = DateTimeHelper.Now; ;
                    return repo.Insert(input);


                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public int DeleteAccessoriesMappingById(int id, int userId)
        {
            try
            {
                var objSystmId = repo.Get(x => x.id == id);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.id);
                }
                else
                {
                    return 0;
                }
            }
            catch { throw; }

        }
        public List<AccessoriesMappingList> GetAccessoriesDropdownListByType(string type)
        {
            try
            {

                return repo.ExecuteProcedure<AccessoriesMappingList>("fn_get_accessories_type_list", new { p_specifyType = type }).ToList();

            }
            catch { throw; }

        }
        public List<AccessoriesMapping> ChkDuplicateAccessoriesExist(AccessoriesMapping objAccessoriesExist)
        {
            try
            {
                return repo.GetAll(u => u.accessories_id == objAccessoriesExist.accessories_id && u.layer_id == objAccessoriesExist.layer_id).ToList();
            }
            catch
            {
                throw;
            }
        }
    }
}
