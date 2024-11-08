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
    public class DAPOD : Repository<PODMaster>
    {
        public PODMaster SaveEntityPOD(PODMaster objPODMaster, int userId)
        {


            try
            {
                var objPOD = repo.Get(x => x.system_id == objPODMaster.system_id);

                if (objPOD != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objPODMaster.modified_on, objPOD.modified_on, objPODMaster.modified_by, objPOD.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objPODMaster.objPM = objPageValidate;
                        return objPODMaster;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(objPOD.system_id, objPOD.entityType, objPOD.latitude.ToString(), objPOD.longitude.ToString(), objPOD.region_id, objPOD.province_id);
                    if (geomresp.status != "OK")
                    {
                        objPOD.objPM = geomresp;
                        return objPOD;
                    }
                    objPOD.network_id = objPODMaster.network_id;
                    objPOD.pod_name = objPODMaster.pod_name;
                    objPOD.pincode = objPODMaster.pincode;
                    objPOD.address = objPODMaster.address;

                    objPOD.specification = objPODMaster.specification;
                    objPOD.category = objPODMaster.category;
                    objPOD.subcategory1 = objPODMaster.subcategory1;
                    objPOD.subcategory2 = objPODMaster.subcategory2;
                    objPOD.subcategory3 = objPODMaster.subcategory3;
                    objPOD.item_code = objPODMaster.item_code;
                    objPOD.vendor_id = objPODMaster.vendor_id;
                    objPOD.type = objPODMaster.type;
                    objPOD.brand = objPODMaster.brand;
                    objPOD.model = objPODMaster.model;
                    objPOD.construction = objPODMaster.construction;
                    objPOD.activation = objPODMaster.activation;
                    objPOD.accessibility = objPODMaster.accessibility;
                    objPOD.modified_by = userId;
                    objPOD.modified_on = DateTimeHelper.Now;

                    objPOD.project_id = objPODMaster.project_id ?? 0;
                    objPOD.planning_id = objPODMaster.planning_id ?? 0;
                    objPOD.workorder_id = objPODMaster.workorder_id ?? 0;
                    objPOD.purpose_id = objPODMaster.purpose_id ?? 0;
                    objPOD.parent_system_id = objPODMaster.parent_system_id;
                    objPOD.parent_entity_type = objPODMaster.parent_entity_type;
                    objPOD.parent_network_id = objPODMaster.parent_network_id;
                    objPOD.longitude = objPODMaster.longitude;
                    objPOD.latitude = objPODMaster.latitude;
                    objPOD.acquire_from = objPODMaster.acquire_from;
                    objPOD.pod_type = objPODMaster.pod_type;
                    objPOD.ownership_type = objPODMaster.ownership_type;
                    objPOD.third_party_vendor_id = objPODMaster.third_party_vendor_id;
                    objPOD.status_remark = objPODMaster.status_remark;
                    objPOD.remarks = objPODMaster.remarks;
                    objPOD.is_acquire_from = objPODMaster.is_acquire_from;
                    objPOD.audit_item_master_id = objPODMaster.audit_item_master_id;
                    if (!string.IsNullOrEmpty(objPODMaster.source_ref_type))
                        objPOD.source_ref_type = objPODMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objPODMaster.source_ref_id))
                        objPOD.source_ref_id = objPODMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objPODMaster.source_ref_description))
                        objPOD.source_ref_description = objPODMaster.source_ref_description;
                    var response = DAIspEntityMapping.Instance.associateEntityInStructure(objPODMaster.objIspEntityMap.shaft_id, objPODMaster.objIspEntityMap.floor_id, objPODMaster.system_id, EntityType.POD.ToString(), objPODMaster.parent_system_id, objPODMaster.parent_entity_type, objPODMaster.longitude + " " + objPODMaster.latitude);
                    if (response.status)
                    {
                        objPOD.isPortConnected = response.status;
                        objPOD.message = Resources.Helper.MultilingualMessageConvert(response.message);//response.message;
                        return objPOD;
                    }
                    objPOD.other_info = objPODMaster.other_info;    //for additional-attributes
                    objPOD.requested_by = objPODMaster.requested_by;
                    objPOD.request_approved_by = objPODMaster.request_approved_by;
                    objPOD.request_ref_id = objPODMaster.request_ref_id;
                    objPOD.origin_ref_id = objPODMaster.origin_ref_id;
                    objPOD.origin_ref_description = objPODMaster.origin_ref_description;
                    objPOD.origin_from = objPODMaster.origin_from;
                    objPOD.origin_ref_code = objPODMaster.origin_ref_code;
                    //objPOD.served_by_ring = objPODMaster.served_by_ring;
                    objPOD.bom_sub_category = objPODMaster.bom_sub_category;
                    objPOD.gis_design_id = objPODMaster.gis_design_id;
                    objPOD.hierarchy_type = objPODMaster.hierarchy_type;
                    objPOD.own_vendor_id = objPODMaster.own_vendor_id;
                    var result = repo.Update(objPOD);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.POD.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.POD.ToString(), result.province_id);
                    return result;
                }
                else
                {
                    //if (objPODMaster.objIspEntityMap.floor_id > 0 && objPODMaster.objIspEntityMap.shaft_id > 0)
                    //{
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objPODMaster.system_id,
                            entity_type = EntityType.POD.ToString(),
                            floor_id = objPODMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objPODMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_system_id= objPODMaster.parent_system_id,
                            parent_entity_type = objPODMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objPODMaster.objPM = objPageValidate;
                            return objPODMaster;
                        }
                    //}
                    objPODMaster.created_by = userId;
                    objPODMaster.created_on = DateTimeHelper.Now;
                    //objPODMaster.status = "A";
                    //objPODMaster.network_status = "P";                    
                    objPODMaster.status = String.IsNullOrEmpty(objPODMaster.status) ? "A" : objPODMaster.status;
                    objPODMaster.network_status = String.IsNullOrEmpty(objPODMaster.network_status) ? "P" : objPODMaster.network_status;
                    var result = repo.Insert(objPODMaster);
                    //  Save geometry
                   
                    InputGeom geom = new InputGeom();
                    geom.systemId = result.system_id;
                    geom.longLat = result.longitude + " " + result.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.POD.ToString();
                    geom.commonName = result.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = result.project_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.POD.ToString(), result.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.POD.ToString(), result.province_id);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objPODMaster.objIspEntityMap.shaft_id, objPODMaster.objIspEntityMap.floor_id, objPODMaster.system_id, EntityType.POD.ToString(), objPODMaster.parent_system_id, objPODMaster.parent_entity_type, objPODMaster.longitude + " " + objPODMaster.latitude);                    
                    return result;
                }
            }
            catch { throw; }
        }
        public int DeletePODById(int systemId)
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

        public List<PODDetail> GetPODAssociationDetail(string geom, int associated_SystemId, string associated_entity_Type)
        {
            try
            {
                return repo.ExecuteProcedure<PODDetail>("fn_get_pod_association_details", new { p_geom= geom, p_associated_system_id = associated_SystemId, p_associated_entity_type = associated_entity_Type }, true);
            }
            catch { throw; }
        }

        public List<KeyValueDropDown> GetPODDetailForFilter()
        {
            try
            {
                return repo.ExecuteProcedure<KeyValueDropDown>("fn_get_pod_details",null);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<PODDetail> GetPodDetailsInBulk(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<PODDetail>("fn_bulk_get_pod_details", new { p_geom = geom }, true);
            }
            catch { throw; }
        }
        #region Additional-Attributes
        public string GetOtherInfoPOD(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
        
    }
}
