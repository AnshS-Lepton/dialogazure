using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAHandhole : Repository<HandholeMaster>
    {

        public HandholeMaster SaveEntityHandhole(HandholeMaster objHandholeMaster, int userId)
        {
            try
            {
                var objHandholeItem = repo.Get(x => x.system_id == objHandholeMaster.system_id);
                if (objHandholeItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objHandholeMaster.modified_on, objHandholeItem.modified_on, objHandholeMaster.modified_by, objHandholeItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objHandholeMaster.objPM = objPageValidate;
                        return objHandholeMaster;
                    }

                    objHandholeItem.handhole_name = objHandholeMaster.handhole_name;
                    objHandholeItem.address = objHandholeMaster.address;
                    objHandholeItem.is_virtual = objHandholeMaster.is_virtual;
                    objHandholeItem.specification = objHandholeMaster.specification;
                    objHandholeItem.category = objHandholeMaster.category;
                    objHandholeItem.subcategory1 = objHandholeMaster.subcategory1;
                    objHandholeItem.subcategory2 = objHandholeMaster.subcategory2;
                    objHandholeItem.subcategory3 = objHandholeMaster.subcategory3;
                    objHandholeItem.item_code = objHandholeMaster.item_code;
                    objHandholeItem.vendor_id = objHandholeMaster.vendor_id;
                    objHandholeItem.type = objHandholeMaster.type;
                    objHandholeItem.brand = objHandholeMaster.brand;
                    objHandholeItem.model = objHandholeMaster.model;
                    objHandholeItem.construction = objHandholeMaster.construction;
                    objHandholeItem.activation = objHandholeMaster.activation;
                    objHandholeItem.accessibility = objHandholeMaster.accessibility;
                    objHandholeItem.modified_by = userId;
                    objHandholeItem.modified_on = DateTimeHelper.Now;

                    objHandholeItem.project_id = objHandholeMaster.project_id ?? 0;
                    objHandholeItem.planning_id = objHandholeMaster.planning_id ?? 0;
                    objHandholeItem.workorder_id = objHandholeMaster.workorder_id ?? 0;
                    objHandholeItem.purpose_id = objHandholeMaster.purpose_id ?? 0;
                    objHandholeItem.construction_type = objHandholeMaster.construction_type;
                    objHandholeItem.acquire_from = objHandholeMaster.acquire_from;
                    objHandholeItem.is_buried = objHandholeMaster.is_buried;
                    objHandholeItem.ownership_type = objHandholeMaster.ownership_type;
                    objHandholeItem.third_party_vendor_id = objHandholeMaster.third_party_vendor_id;
                    objHandholeItem.audit_item_master_id = objHandholeMaster.audit_item_master_id;
                    objHandholeItem.primary_pod_system_id = objHandholeMaster.primary_pod_system_id;
                    objHandholeItem.secondary_pod_system_id = objHandholeMaster.secondary_pod_system_id;
                    objHandholeItem.status_remark = objHandholeMaster.status_remark;
                    objHandholeItem.remarks = objHandholeMaster.remarks;
                    objHandholeItem.is_acquire_from = objHandholeMaster.is_acquire_from;

                    if (!string.IsNullOrEmpty(objHandholeMaster.source_ref_type))
                        objHandholeItem.source_ref_type = objHandholeMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objHandholeMaster.source_ref_id))
                        objHandholeItem.source_ref_id = objHandholeMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objHandholeMaster.source_ref_description))
                        objHandholeItem.source_ref_description = objHandholeMaster.source_ref_description;
                    objHandholeItem.requested_by = objHandholeMaster.requested_by;
                    objHandholeItem.request_approved_by = objHandholeMaster.request_approved_by;
                    objHandholeItem.request_ref_id = objHandholeMaster.request_ref_id;
                    objHandholeItem.origin_ref_id = objHandholeMaster.origin_ref_id;
                    objHandholeItem.origin_ref_description = objHandholeMaster.origin_ref_description;
                    objHandholeItem.origin_from = objHandholeMaster.origin_from;
                    objHandholeItem.origin_ref_code = objHandholeMaster.origin_ref_code;
                    objHandholeItem.bom_sub_category= objHandholeMaster.bom_sub_category;
                    objHandholeItem.other_info = objHandholeMaster.other_info; //for Additional-Attributes
                    //objHandholeItem.served_by_ring= objHandholeMaster.served_by_ring;
                    objHandholeItem.gis_design_id = objHandholeMaster.gis_design_id;
                    var HandholeResp = repo.Update(objHandholeItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(HandholeResp.system_id, Models.EntityType.Handhole.ToString(), HandholeResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Handhole.ToString(), HandholeResp.province_id);
                    return HandholeResp;
                }
                else
                {
                    objHandholeMaster.created_by = userId;
                    objHandholeMaster.created_on = DateTimeHelper.Now;
                    //objHandholeMaster.status = "A";
                    //objHandholeMaster.network_status = "P";

                    objHandholeMaster.status = String.IsNullOrEmpty(objHandholeMaster.status) ? "A" : objHandholeMaster.status;
                    objHandholeMaster.network_status = String.IsNullOrEmpty(objHandholeMaster.network_status) ? "P" : objHandholeMaster.network_status;

                    var resultItem = repo.Insert(objHandholeMaster);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Handhole.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.is_virtual = resultItem.is_virtual;
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Handhole.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Handhole.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteHandholeById(int systemId)
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
        public string GetOtherInfoHandhole(int systemId)
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
