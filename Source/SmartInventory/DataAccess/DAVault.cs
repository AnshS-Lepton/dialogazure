using DataAccess.DBHelpers;
using DataAccess.ISP;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DAVault : Repository<VaultMaster>
    {
        public VaultMaster SaveEntityVault(VaultMaster objVaultMaster, int userId)
        {
            try
            {
                var objVault = repo.Get(x => x.system_id == objVaultMaster.system_id);
                if (objVault != null)
                {
                        PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objVaultMaster.modified_on, objVault.modified_on,objVaultMaster.modified_by,objVault.modified_by);
                        if (objPageValidate.message != null)
                        {
                            objVaultMaster.objPM = objPageValidate;
                            return objVaultMaster;
                        }

                    objVault.network_id = objVaultMaster.network_id;
                    objVault.vault_name = objVaultMaster.vault_name;
                    objVault.pincode = objVaultMaster.pincode;
                    objVault.address = objVaultMaster.address;
                    objVault.remarks = objVaultMaster.remarks;
                    objVault.no_of_entry_exit_points = objVaultMaster.no_of_entry_exit_points;
                    objVault.specification = objVaultMaster.specification;
                    objVault.category = objVaultMaster.category;
                    objVault.subcategory1 = objVaultMaster.subcategory1;
                    objVault.subcategory2 = objVaultMaster.subcategory2;
                    objVault.subcategory3 = objVaultMaster.subcategory3;
                    objVault.item_code = objVaultMaster.item_code;
                    objVault.vendor_id = objVaultMaster.vendor_id;
                    objVault.type = objVaultMaster.type;
                    objVault.brand = objVaultMaster.brand;
                    objVault.model = objVaultMaster.model;
                    objVault.construction = objVaultMaster.construction;
                    objVault.activation = objVaultMaster.activation;
                    objVault.accessibility = objVaultMaster.accessibility;
                    objVault.modified_by = userId;
                    objVault.modified_on = DateTimeHelper.Now;

                    objVault.project_id = objVaultMaster.project_id ?? 0;
                    objVault.planning_id = objVaultMaster.planning_id ?? 0;
                    objVault.workorder_id = objVaultMaster.workorder_id ?? 0;
                    objVault.purpose_id = objVaultMaster.purpose_id ?? 0;
                    objVault.parent_system_id = objVaultMaster.parent_system_id;
                    objVault.parent_entity_type = objVaultMaster.parent_entity_type;
                    objVault.parent_network_id = objVaultMaster.parent_network_id;
                    objVault.longitude = objVaultMaster.longitude;
                    objVault.latitude = objVaultMaster.latitude;
                    objVault.ownership_type = objVaultMaster.ownership_type;
                    objVault.acquire_from = objVaultMaster.acquire_from;
                    objVault.third_party_vendor_id = objVaultMaster.third_party_vendor_id;
                    objVault.vault_type = objVaultMaster.vault_type;
                    objVault.audit_item_master_id = objVaultMaster.audit_item_master_id;
                    objVault.is_acquire_from = objVaultMaster.is_acquire_from;

                    if (!string.IsNullOrEmpty(objVaultMaster.source_ref_type))
                        objVault.source_ref_type = objVaultMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objVaultMaster.source_ref_id))
                        objVault.source_ref_id = objVaultMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objVaultMaster.source_ref_description))
                        objVault.source_ref_description = objVaultMaster.source_ref_description;

                    var response = DAIspEntityMapping.Instance.associateEntityInStructure(objVaultMaster.objIspEntityMap.shaft_id, objVaultMaster.objIspEntityMap.floor_id, objVaultMaster.system_id, EntityType.Vault.ToString(), objVaultMaster.parent_system_id, objVaultMaster.parent_entity_type, objVaultMaster.longitude + " " + objVaultMaster.latitude);
                    if (response.status)
                    {
                        objVault.isPortConnected = response.status;
                        objVault.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                        return objVault;
                    }
                    objVault.requested_by = objVaultMaster.requested_by;
                    objVault.request_approved_by = objVaultMaster.request_approved_by;
                    objVault.request_ref_id = objVaultMaster.request_ref_id;
                    objVault.origin_ref_id = objVaultMaster.origin_ref_id;
                    objVault.origin_ref_description = objVaultMaster.origin_ref_description;
                    objVault.origin_from = objVaultMaster.origin_from;
                    objVault.origin_ref_code = objVaultMaster.origin_ref_code;
                    objVault.bom_sub_category = objVaultMaster.bom_sub_category;
                    // objVault.served_by_ring = objVaultMaster.served_by_ring;
                    var result = repo.Update(objVault);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Vault.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Vault.ToString(), result.province_id);
                    return result;
                }
                else
                {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objVaultMaster.system_id,
                            entity_type = EntityType.Vault.ToString(),
                            floor_id = objVaultMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objVaultMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_system_id= objVaultMaster.parent_system_id,
                            parent_entity_type = objVaultMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objVaultMaster.objPM = objPageValidate;
                            return objVaultMaster;
                        }
                    //}

                    objVaultMaster.created_by = userId;
                    objVaultMaster.created_on = DateTimeHelper.Now;
                    objVaultMaster.status = String.IsNullOrEmpty(objVaultMaster.status) ? "A" : objVaultMaster.status;
                    objVaultMaster.network_status = String.IsNullOrEmpty(objVaultMaster.network_status) ? "P" : objVaultMaster.network_status;
                    var result = repo.Insert(objVaultMaster);

                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = result.system_id;
                    geom.longLat = result.longitude + " " + result.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Vault.ToString();
                    geom.commonName = result.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = result.project_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Vault.ToString(), result.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Vault.ToString(), result.province_id);
                    //DAIspEntityMapping.Instance.associateEntityInStructure(objVaultMaster.objIspEntityMap.shaft_id, objVaultMaster.objIspEntityMap.floor_id, objVaultMaster.system_id, EntityType.Vault.ToString(), objVaultMaster.parent_system_id, objVaultMaster.parent_entity_type, objVaultMaster.longitude + " " + objVaultMaster.latitude);
                    if (objVaultMaster.pEntityType != null && objVaultMaster.pSystemId != 0 && objVaultMaster.pEntityType.ToUpper() != "STRUCTURE")
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.Vault.ToString();
                        objAsso.associated_system_id = result.system_id;
                        objAsso.associated_network_id = result.network_id;
                        objAsso.entity_network_id = objVaultMaster.pNetworkId;
                        objAsso.entity_system_id = objVaultMaster.pSystemId;
                        objAsso.entity_type = objVaultMaster.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(objAsso);
                    }
                    else
                    {
                        DAIspEntityMapping.Instance.associateEntityInStructure(objVaultMaster.objIspEntityMap.shaft_id, objVaultMaster.objIspEntityMap.floor_id, objVaultMaster.system_id, EntityType.Vault.ToString(), objVaultMaster.parent_system_id, objVaultMaster.parent_entity_type, objVaultMaster.longitude + " " + objVaultMaster.latitude);
                    }
                    return result;

                }
            }
            catch { throw; }
        }
        public int DeleteVaultById(int systemId)
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
        
    }
}
