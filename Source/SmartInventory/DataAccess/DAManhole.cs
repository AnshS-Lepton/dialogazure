using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAManhole : Repository<ManholeMaster>
    {
     
        public ManholeMaster SaveEntityManhole(ManholeMaster objManholeMaster, int userId)
        {
            try
            {
                var objManholeItem = repo.Get(x => x.system_id == objManholeMaster.system_id);
                if (objManholeItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objManholeMaster.modified_on, objManholeItem.modified_on, objManholeMaster.modified_by,objManholeItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objManholeMaster.objPM = objPageValidate;
                        return objManholeMaster;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(objManholeMaster.system_id, objManholeMaster.entityType, objManholeMaster.latitude.ToString(), objManholeMaster.longitude.ToString(), objManholeMaster.region_id, objManholeMaster.province_id);
                    if (geomresp.status != "OK")
                    {
                        objManholeMaster.objPM = geomresp;
                        return objManholeMaster;
                    }

                    objManholeItem.mcgm_ward = objManholeMaster.mcgm_ward;
                    objManholeItem.manhole_name = objManholeMaster.manhole_name;                 
                    objManholeItem.address = objManholeMaster.address;
                    objManholeItem.is_virtual = objManholeMaster.is_virtual;
                    objManholeItem.specification = objManholeMaster.specification;
                    objManholeItem.category = objManholeMaster.category;
                    objManholeItem.subcategory1 = objManholeMaster.subcategory1;
                    objManholeItem.subcategory2 = objManholeMaster.subcategory2;
                    objManholeItem.subcategory3 = objManholeMaster.subcategory3;
                    objManholeItem.item_code = objManholeMaster.item_code;
                    objManholeItem.vendor_id = objManholeMaster.vendor_id;
                    objManholeItem.type = objManholeMaster.type;
                    objManholeItem.brand = objManholeMaster.brand;
                    objManholeItem.model = objManholeMaster.model;
                    objManholeItem.construction = objManholeMaster.construction;
                    objManholeItem.activation = objManholeMaster.activation;
                    objManholeItem.accessibility = objManholeMaster.accessibility;
                    objManholeItem.modified_by = userId;
                    objManholeItem.modified_on = DateTimeHelper.Now;

                    objManholeItem.project_id = objManholeMaster.project_id ?? 0;
                    objManholeItem.planning_id = objManholeMaster.planning_id ?? 0;
                    objManholeItem.workorder_id = objManholeMaster.workorder_id ?? 0;
                    objManholeItem.purpose_id = objManholeMaster.purpose_id ?? 0;
                    objManholeItem.construction_type = objManholeMaster.construction_type;
                    //-------------------changes by manoj------------------------------
                    objManholeItem.area = objManholeMaster.area;
                    objManholeItem.authority = objManholeMaster.authority;
                    objManholeItem.route_name = objManholeMaster.route_name;
                    //---------------------------------------------------------------------
                    objManholeItem.acquire_from = objManholeMaster.acquire_from;
                    objManholeItem.is_buried = objManholeMaster.is_buried;
                    objManholeItem.ownership_type = objManholeMaster.ownership_type;
                    objManholeItem.third_party_vendor_id = objManholeMaster.third_party_vendor_id;
                    objManholeItem.audit_item_master_id = objManholeMaster.audit_item_master_id;
                    objManholeItem.primary_pod_system_id = objManholeMaster.primary_pod_system_id;
                    objManholeItem.secondary_pod_system_id = objManholeMaster.secondary_pod_system_id;
                    objManholeItem.status_remark = objManholeMaster.status_remark;
                    objManholeItem.remarks = objManholeMaster.remarks;
                    objManholeItem.manhole_types = objManholeMaster.manhole_types;
                    objManholeItem.is_acquire_from = objManholeMaster.is_acquire_from;
                    objManholeItem.other_info = objManholeMaster.other_info;

                    if (!string.IsNullOrEmpty(objManholeMaster.source_ref_type))
                        objManholeItem.source_ref_type = objManholeMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objManholeMaster.source_ref_id))
                        objManholeItem.source_ref_id = objManholeMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objManholeMaster.source_ref_description))
                        objManholeItem.source_ref_description = objManholeMaster.source_ref_description;
                    objManholeItem.requested_by = objManholeMaster.requested_by;
                    objManholeItem.request_approved_by = objManholeMaster.request_approved_by;
                    objManholeItem.request_ref_id = objManholeMaster.request_ref_id;
                    objManholeItem.origin_ref_id = objManholeMaster.origin_ref_id;
                    objManholeItem.origin_ref_description = objManholeMaster.origin_ref_description;
                    objManholeItem.origin_from = objManholeMaster.origin_from;
                    objManholeItem.origin_ref_code = objManholeMaster.origin_ref_code;
                    //objManholeItem.served_by_ring = objManholeMaster.served_by_ring;
                    objManholeItem.bom_sub_category=objManholeMaster.bom_sub_category;
                    objManholeItem.gis_design_id = objManholeMaster.gis_design_id;
                    objManholeItem.own_vendor_id = objManholeMaster.own_vendor_id;
                    objManholeItem.hierarchy_type = objManholeMaster.hierarchy_type;
                    objManholeItem.aerial_location = objManholeMaster.aerial_location;
                    objManholeItem.section_name = objManholeMaster.section_name;
                    objManholeItem.generic_section_name = objManholeMaster.generic_section_name;
                    objManholeItem.subarea_id = objManholeMaster.subarea_id;
                    if (!string.IsNullOrEmpty(objManholeMaster.source_ref_type))
                        objManholeItem.source_ref_type = objManholeMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objManholeMaster.source_ref_id))
                        objManholeItem.source_ref_id = objManholeMaster.source_ref_id;
                    var ManholeResp = repo.Update(objManholeItem);
                    DbMessage entityObj =new DAMisc(). updateGeojsonEntityAttribute(ManholeResp.system_id, Models.EntityType.Manhole.ToString(), ManholeResp.province_id, 1);
                    //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.Manhole.ToString(), ManholeResp.province_id);
                    return ManholeResp;

                }
                else
                {
                    objManholeMaster.created_by = userId;
                    objManholeMaster.created_on = DateTimeHelper.Now;
                    //objManholeMaster.status = "A";
                    //objManholeMaster.network_status = "P";

                    objManholeMaster.status = String.IsNullOrEmpty(objManholeMaster.status) ? "A" : objManholeMaster.status;
                    objManholeMaster.network_status = String.IsNullOrEmpty(objManholeMaster.network_status) ? "P" : objManholeMaster.network_status;

                    var resultItem= repo.Insert(objManholeMaster);
                    // Save geometry
                    
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Manhole.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.entity_category = objManholeMaster.manhole_types;
                    geom.is_virtual = resultItem.is_virtual;
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj =new DAMisc(). updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Manhole.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = updateGeojsonMetadata(Models.EntityType.Manhole.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteManholeById(int systemId)
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
        public string GetOtherInfoManhole(int systemId)
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
