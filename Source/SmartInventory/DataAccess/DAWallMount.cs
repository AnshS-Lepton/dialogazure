using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAWallMount : Repository<WallMountMaster>
    {
      
        public WallMountMaster SaveEntityWallMount(WallMountMaster objWallMountMaster, int userId)
        {
            try
            {
                var objWallMountItem = repo.Get(x => x.system_id == objWallMountMaster.system_id);
                if (objWallMountItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objWallMountMaster.modified_on, objWallMountItem.modified_on, objWallMountMaster.modified_by,objWallMountItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objWallMountMaster.objPM = objPageValidate;
                        return objWallMountMaster;
                    }
                    objWallMountItem.wallmount_name = objWallMountMaster.wallmount_name;               
                    objWallMountItem.wallmount_no = objWallMountMaster.wallmount_no;
                    objWallMountItem.wallmount_height = objWallMountMaster.wallmount_height;
                    objWallMountItem.address = objWallMountMaster.address;
                    
                    objWallMountItem.specification = objWallMountMaster.specification;
                    objWallMountItem.category = objWallMountMaster.category;
                    objWallMountItem.subcategory1 = objWallMountMaster.subcategory1;
                    objWallMountItem.subcategory2 = objWallMountMaster.subcategory2;
                    objWallMountItem.subcategory3 = objWallMountMaster.subcategory3;
                    objWallMountItem.item_code = objWallMountMaster.item_code;
                    objWallMountItem.vendor_id = objWallMountMaster.vendor_id;
                    objWallMountItem.type = objWallMountMaster.type;
                    objWallMountItem.brand = objWallMountMaster.brand;
                    objWallMountItem.model = objWallMountMaster.model;
                    objWallMountItem.construction = objWallMountMaster.construction;
                    objWallMountItem.activation = objWallMountMaster.activation;
                    objWallMountItem.accessibility = objWallMountMaster.accessibility;
                    objWallMountItem.modified_by = userId;
                    objWallMountItem.modified_on = DateTimeHelper.Now;

                    objWallMountItem.project_id = objWallMountMaster.project_id ?? 0;
                    objWallMountItem.planning_id = objWallMountMaster.planning_id ?? 0;
                    objWallMountItem.workorder_id = objWallMountMaster.workorder_id ?? 0;
                    objWallMountItem.purpose_id = objWallMountMaster.purpose_id ?? 0;
                    objWallMountItem.acquire_from = objWallMountMaster.acquire_from;
                    objWallMountItem.ownership_type = objWallMountMaster.ownership_type;
                    objWallMountItem.third_party_vendor_id = objWallMountMaster.third_party_vendor_id;
                    objWallMountItem.audit_item_master_id = objWallMountMaster.audit_item_master_id;
                    objWallMountItem.primary_pod_system_id = objWallMountMaster.primary_pod_system_id;
                    objWallMountItem.secondary_pod_system_id = objWallMountMaster.secondary_pod_system_id;
                    objWallMountItem.status_remark = objWallMountMaster.status_remark;
                    objWallMountItem.remarks = objWallMountMaster.remarks;
                    objWallMountItem.is_acquire_from = objWallMountMaster.is_acquire_from;

                    if (!string.IsNullOrEmpty(objWallMountMaster.source_ref_type))
                        objWallMountItem.source_ref_type = objWallMountMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objWallMountMaster.source_ref_id))
                        objWallMountItem.source_ref_id = objWallMountMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objWallMountMaster.source_ref_description))
                        objWallMountItem.source_ref_description = objWallMountMaster.source_ref_description;
                    
                    objWallMountItem.other_info = objWallMountMaster.other_info;    //for additional-attributes
                    objWallMountItem.requested_by = objWallMountMaster.requested_by;
                    objWallMountItem.request_approved_by = objWallMountMaster.request_approved_by;
                    objWallMountItem.request_ref_id = objWallMountMaster.request_ref_id;
                    objWallMountItem.origin_ref_id = objWallMountMaster.origin_ref_id;
                    objWallMountItem.origin_ref_description = objWallMountMaster.origin_ref_description;
                    objWallMountItem.origin_from = objWallMountMaster.origin_from;
                    objWallMountItem.origin_ref_code = objWallMountMaster.origin_ref_code;
                    objWallMountItem.bom_sub_category = objWallMountMaster.bom_sub_category;
                    //  objWallMountItem.served_by_ring = objWallMountMaster.served_by_ring;
                    var WallMountResp = repo.Update(objWallMountItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(WallMountResp.system_id, Models.EntityType.WallMount.ToString(), WallMountResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.WallMount.ToString(), WallMountResp.province_id);
                    return WallMountResp;

                }
                else
                {
                    objWallMountMaster.created_by = userId;
                    objWallMountMaster.created_on = DateTimeHelper.Now;
                    //objWallMountMaster.status = "A";
                    //objWallMountMaster.network_status = "P";
                    objWallMountMaster.status = String.IsNullOrEmpty(objWallMountMaster.status) ? "A" : objWallMountMaster.status;
                    objWallMountMaster.network_status = String.IsNullOrEmpty(objWallMountMaster.network_status) ? "P" : objWallMountMaster.network_status;
                    var resultItem=repo.Insert(objWallMountMaster);
                  
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.WallMount.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.WallMount.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.WallMount.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteWallMountById(int systemId)
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
        public string GetOtherInfoWallMount(int systemId)
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
