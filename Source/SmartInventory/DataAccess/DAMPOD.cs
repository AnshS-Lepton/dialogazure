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
    public class DAMPOD : Repository<MPODMaster>
    {
        public MPODMaster SaveEntityMPOD(MPODMaster objMPODMaster, int userId)
        {
            try
            {
                var objMPOD = repo.Get(x => x.system_id == objMPODMaster.system_id);
                if (objMPOD != null)
                {
                        PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objMPODMaster.modified_on, objMPOD.modified_on,objMPODMaster.modified_by,objMPOD.modified_by);
                        if (objPageValidate.message != null)
                        {
                            objMPODMaster.objPM = objPageValidate;
                            return objMPODMaster;
                        }

                    objMPOD.network_id = objMPODMaster.network_id;
                    objMPOD.mpod_name = objMPODMaster.mpod_name;
                    objMPOD.pincode = objMPODMaster.pincode;
                    objMPOD.address = objMPODMaster.address;

                    objMPOD.specification = objMPODMaster.specification;
                    objMPOD.category = objMPODMaster.category;
                    objMPOD.subcategory1 = objMPODMaster.subcategory1;
                    objMPOD.subcategory2 = objMPODMaster.subcategory2;
                    objMPOD.subcategory3 = objMPODMaster.subcategory3;
                    objMPOD.item_code = objMPODMaster.item_code;
                    objMPOD.vendor_id = objMPODMaster.vendor_id;
                    objMPOD.type = objMPODMaster.type;
                    objMPOD.brand = objMPODMaster.brand;
                    objMPOD.model = objMPODMaster.model;
                    objMPOD.construction = objMPODMaster.construction;
                    objMPOD.activation = objMPODMaster.activation;
                    objMPOD.accessibility = objMPODMaster.accessibility;
                    objMPOD.modified_by = userId;
                    objMPOD.modified_on = DateTimeHelper.Now;

                    objMPOD.project_id = objMPODMaster.project_id ?? 0;
                    objMPOD.planning_id = objMPODMaster.planning_id ?? 0;
                    objMPOD.workorder_id = objMPODMaster.workorder_id ?? 0;
                    objMPOD.purpose_id = objMPODMaster.purpose_id ?? 0;
                    objMPOD.parent_system_id = objMPODMaster.parent_system_id;
                    objMPOD.parent_entity_type = objMPODMaster.parent_entity_type;
                    objMPOD.parent_network_id = objMPODMaster.parent_network_id;
                    objMPOD.longitude = objMPODMaster.longitude;
                    objMPOD.latitude = objMPODMaster.latitude;
                    objMPOD.ownership_type = objMPODMaster.ownership_type;
                    objMPOD.acquire_from = objMPODMaster.acquire_from;
                    objMPOD.third_party_vendor_id = objMPODMaster.third_party_vendor_id;
                    objMPOD.mpod_type = objMPODMaster.mpod_type;
                    objMPOD.audit_item_master_id = objMPODMaster.audit_item_master_id;
                    objMPOD.primary_pod_system_id = objMPODMaster.primary_pod_system_id;
                    objMPOD.secondary_pod_system_id = objMPODMaster.secondary_pod_system_id;
                    objMPOD.status_remark = objMPODMaster.status_remark;
                    objMPOD.remarks = objMPODMaster.remarks;
                    objMPOD.is_acquire_from = objMPODMaster.is_acquire_from;

                    if (!string.IsNullOrEmpty(objMPODMaster.source_ref_type))
                        objMPOD.source_ref_type = objMPODMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objMPODMaster.source_ref_id))
                        objMPOD.source_ref_id = objMPODMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objMPODMaster.source_ref_description))
                        objMPOD.source_ref_description = objMPODMaster.source_ref_description;

                    var response = DAIspEntityMapping.Instance.associateEntityInStructure(objMPODMaster.objIspEntityMap.shaft_id, objMPODMaster.objIspEntityMap.floor_id, objMPODMaster.system_id, EntityType.MPOD.ToString(), objMPODMaster.parent_system_id, objMPODMaster.parent_entity_type,objMPODMaster.longitude + " " + objMPODMaster.latitude);
                    if (response.status)
                    {
                        objMPOD.isPortConnected = response.status;
                        objMPOD.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                        return objMPOD;
                    }
                    objMPOD.other_info = objMPODMaster.other_info;  //for additional-attributes
                    objMPOD.requested_by = objMPODMaster.requested_by;
                    objMPOD.request_approved_by = objMPODMaster.request_approved_by;
                    objMPOD.request_ref_id = objMPODMaster.request_ref_id;
                    objMPOD.origin_ref_id = objMPODMaster.origin_ref_id;
                    objMPOD.origin_ref_description = objMPODMaster.origin_ref_description;
                    objMPOD.origin_from = objMPODMaster.origin_from;
                    objMPOD.origin_ref_code = objMPODMaster.origin_ref_code;
                    objMPOD.bom_sub_category = objMPODMaster.bom_sub_category;
                    objMPOD.gis_design_id = objMPODMaster.gis_design_id;
                    objMPOD.own_vendor_id = objMPODMaster.own_vendor_id;
                    objMPOD.hierarchy_type = objMPODMaster.hierarchy_type;
                    //objMPOD.served_by_ring=objMPODMaster.served_by_ring;
                    var result = repo.Update(objMPOD);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.MPOD.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.MPOD.ToString(), result.province_id);
                    return result;
                }
                else
                {
                    //if (objMPODMaster.objIspEntityMap.floor_id > 0 && objMPODMaster.objIspEntityMap.shaft_id > 0)
                    //{
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objMPODMaster.system_id,
                            entity_type = EntityType.MPOD.ToString(),
                            floor_id = objMPODMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objMPODMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_system_id=objMPODMaster.parent_system_id,
                            parent_entity_type = objMPODMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objMPODMaster.objPM = objPageValidate;
                            return objMPODMaster;
                        }
                    //}

                    objMPODMaster.created_by = userId;
                    objMPODMaster.created_on = DateTimeHelper.Now;
                    //objMPODMaster.status = "A";
                    //objMPODMaster.network_status = "P";                    
                    objMPODMaster.status = String.IsNullOrEmpty(objMPODMaster.status) ? "A" : objMPODMaster.status;
                    objMPODMaster.network_status = String.IsNullOrEmpty(objMPODMaster.network_status) ? "P" : objMPODMaster.network_status;
                    var result = repo.Insert(objMPODMaster);
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = result.system_id;
                    geom.longLat = result.longitude + " " + result.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.MPOD.ToString();
                    geom.commonName = result.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = result.project_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.MPOD.ToString(), result.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.MPOD.ToString(), result.province_id);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objMPODMaster.objIspEntityMap.shaft_id, objMPODMaster.objIspEntityMap.floor_id, objMPODMaster.system_id, EntityType.MPOD.ToString(), objMPODMaster.parent_system_id, objMPODMaster.parent_entity_type, objMPODMaster.longitude + " " + objMPODMaster.latitude);
                    return result;

                }
            }
            catch { throw; }
        }
        public int DeleteMPODById(int systemId)
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
        #region Additional-Attributes
        public string GetOtherInfoMPOD(int systemId)
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
