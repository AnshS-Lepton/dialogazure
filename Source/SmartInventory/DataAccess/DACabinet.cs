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
    public class DACabinet : Repository<CabinetMaster>
    {
        public CabinetMaster SaveEntityCabinet(CabinetMaster objCabinetMaster, int userId)
        {
            try
            {
                var objCabinet = repo.Get(x => x.system_id == objCabinetMaster.system_id);
                if (objCabinet != null)
                {
                        PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objCabinetMaster.modified_on, objCabinet.modified_on,objCabinetMaster.modified_by,objCabinet.modified_by);
                        if (objPageValidate.message != null)
                        {
                            objCabinetMaster.objPM = objPageValidate;
                            return objCabinetMaster;
                        }

                    objCabinet.network_id = objCabinetMaster.network_id;
                    objCabinet.cabinet_name = objCabinetMaster.cabinet_name;
                    objCabinet.pincode = objCabinetMaster.pincode;
                    objCabinet.address = objCabinetMaster.address;
                    objCabinet.remarks = objCabinetMaster.remarks;
                    objCabinet.specification = objCabinetMaster.specification;
                    objCabinet.category = objCabinetMaster.category;
                    objCabinet.subcategory1 = objCabinetMaster.subcategory1;
                    objCabinet.subcategory2 = objCabinetMaster.subcategory2;
                    objCabinet.subcategory3 = objCabinetMaster.subcategory3;
                    objCabinet.item_code = objCabinetMaster.item_code;
                    objCabinet.vendor_id = objCabinetMaster.vendor_id;
                    objCabinet.type = objCabinetMaster.type;
                    objCabinet.brand = objCabinetMaster.brand;
                    objCabinet.model = objCabinetMaster.model;
                    objCabinet.construction = objCabinetMaster.construction;
                    objCabinet.activation = objCabinetMaster.activation;
                    objCabinet.accessibility = objCabinetMaster.accessibility;
                    objCabinet.modified_by = userId;
                    objCabinet.modified_on = DateTimeHelper.Now;

                    objCabinet.project_id = objCabinetMaster.project_id ?? 0;
                    objCabinet.planning_id = objCabinetMaster.planning_id ?? 0;
                    objCabinet.workorder_id = objCabinetMaster.workorder_id ?? 0;
                    objCabinet.purpose_id = objCabinetMaster.purpose_id ?? 0;
                    objCabinet.parent_system_id = objCabinetMaster.parent_system_id;
                    objCabinet.parent_entity_type = objCabinetMaster.parent_entity_type;
                    objCabinet.parent_network_id = objCabinetMaster.parent_network_id;
                    objCabinet.longitude = objCabinetMaster.longitude;
                    objCabinet.latitude = objCabinetMaster.latitude;
                    objCabinet.ownership_type = objCabinetMaster.ownership_type;
                    objCabinet.acquire_from = objCabinetMaster.acquire_from;
                    objCabinet.third_party_vendor_id = objCabinetMaster.third_party_vendor_id;
                    objCabinet.cabinet_type = objCabinetMaster.cabinet_type;
                    objCabinet.audit_item_master_id = objCabinetMaster.audit_item_master_id;
                    objCabinet.primary_pod_system_id = objCabinetMaster.primary_pod_system_id;
                    objCabinet.secondary_pod_system_id = objCabinetMaster.secondary_pod_system_id;
                    objCabinet.length = objCabinetMaster.length;
                    objCabinet.width = objCabinetMaster.width;
                    objCabinet.height = objCabinetMaster.height;
                    objCabinet.is_acquire_from = objCabinetMaster.is_acquire_from;
                    objCabinet.gis_design_id = objCabinetMaster.gis_design_id;

                    if (!string.IsNullOrEmpty(objCabinetMaster.source_ref_type))
                        objCabinet.source_ref_type = objCabinetMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objCabinetMaster.source_ref_id))
                        objCabinet.source_ref_id = objCabinetMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objCabinetMaster.source_ref_description))
                        objCabinet.source_ref_description = objCabinetMaster.source_ref_description;

                    var response = DAIspEntityMapping.Instance.associateEntityInStructure(objCabinetMaster.objIspEntityMap.shaft_id, objCabinetMaster.objIspEntityMap.floor_id, objCabinetMaster.system_id, EntityType.Cabinet.ToString(), objCabinetMaster.parent_system_id, objCabinetMaster.parent_entity_type, objCabinetMaster.longitude + " " + objCabinetMaster.latitude);
                    if (response.status)
                    {
                        objCabinet.isPortConnected = response.status;
                        objCabinet.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                        return objCabinet;
                    }
                    objCabinet.requested_by = objCabinetMaster.requested_by;
                    objCabinet.request_approved_by = objCabinetMaster.request_approved_by;
                    objCabinet.request_ref_id = objCabinetMaster.request_ref_id;
                    objCabinet.origin_ref_id = objCabinetMaster.origin_ref_id;
                    objCabinet.origin_ref_description = objCabinetMaster.origin_ref_description;
                    objCabinet.origin_from = objCabinetMaster.origin_from;
                    objCabinet.origin_ref_code = objCabinetMaster.origin_ref_code;
                    objCabinet.bom_sub_category=objCabinetMaster.bom_sub_category;
                    objCabinet.hierarchy_type = objCabinetMaster.hierarchy_type;                    
                    //// objCabinet.served_by_ring = objCabinetMaster.served_by_ring;
                    var result = repo.Update(objCabinet);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Cabinet.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Cabinet.ToString(), result.province_id);
                    return result;
                }
                else
                {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objCabinetMaster.system_id,
                            entity_type = EntityType.Cabinet.ToString(),
                            floor_id = objCabinetMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objCabinetMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_system_id= objCabinetMaster.parent_system_id,
                            parent_entity_type = objCabinetMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objCabinetMaster.objPM = objPageValidate;
                            return objCabinetMaster;
                        }
                    //}

                    objCabinetMaster.created_by = userId;
                    objCabinetMaster.created_on = DateTimeHelper.Now;
                    objCabinetMaster.status = String.IsNullOrEmpty(objCabinetMaster.status) ? "A" : objCabinetMaster.status;
                    objCabinetMaster.network_status = String.IsNullOrEmpty(objCabinetMaster.network_status) ? "P" : objCabinetMaster.network_status;
                    var result = repo.Insert(objCabinetMaster);
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = result.system_id;
                    geom.longLat = result.longitude + " " + result.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Cabinet.ToString();
                    geom.commonName = result.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = result.project_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Cabinet.ToString(), result.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Cabinet.ToString(), result.province_id);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objCabinetMaster.objIspEntityMap.shaft_id, objCabinetMaster.objIspEntityMap.floor_id, objCabinetMaster.system_id, EntityType.Cabinet.ToString(), objCabinetMaster.parent_system_id, objCabinetMaster.parent_entity_type, objCabinetMaster.longitude + " " + objCabinetMaster.latitude);
                    return result;

                }
            }
            catch { throw; }
        }
        public int DeleteCabinetById(int systemId)
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
