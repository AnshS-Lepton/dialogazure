using System;
using System.Linq;
using Models;
using DataAccess.DBHelpers;

namespace DataAccess
{
    public class DACoupler : Repository<CouplerMaster>
    {
       
        public CouplerMaster SaveEntityCoupler(CouplerMaster objCouplerMaster, int userId)
        {
            try
            {
                var objCouplerItem = repo.Get(x => x.system_id == objCouplerMaster.system_id);
                if (objCouplerItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objCouplerMaster.modified_on, objCouplerItem.modified_on, objCouplerMaster.modified_by,objCouplerItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objCouplerMaster.objPM = objPageValidate;
                        return objCouplerMaster;
                    }

                    objCouplerItem.coupler_name = objCouplerMaster.coupler_name;
                    objCouplerItem.address = objCouplerMaster.address;
                    objCouplerItem.is_virtual = objCouplerMaster.is_virtual;
                    objCouplerItem.specification = objCouplerMaster.specification;
                    objCouplerItem.category = objCouplerMaster.category;
                    objCouplerItem.subcategory1 = objCouplerMaster.subcategory1;
                    objCouplerItem.subcategory2 = objCouplerMaster.subcategory2;
                    objCouplerItem.subcategory3 = objCouplerMaster.subcategory3;
                    objCouplerItem.item_code = objCouplerMaster.item_code;
                    objCouplerItem.vendor_id = objCouplerMaster.vendor_id;
                    objCouplerItem.type = objCouplerMaster.type;
                    objCouplerItem.brand = objCouplerMaster.brand;
                    objCouplerItem.model = objCouplerMaster.model;
                    objCouplerItem.construction = objCouplerMaster.construction;
                    objCouplerItem.activation = objCouplerMaster.activation;
                    objCouplerItem.accessibility = objCouplerMaster.accessibility;
                    objCouplerItem.coupler_type = objCouplerMaster.coupler_type;
                    objCouplerItem.inner_dimention = objCouplerMaster.inner_dimention;
                    objCouplerItem.outer_dimention = objCouplerMaster.outer_dimention;
                    objCouplerItem.modified_by = userId;
                    objCouplerItem.modified_on = DateTimeHelper.Now;

                    objCouplerItem.project_id = objCouplerMaster.project_id ?? 0;
                    objCouplerItem.planning_id = objCouplerMaster.planning_id ?? 0;
                    objCouplerItem.workorder_id = objCouplerMaster.workorder_id ?? 0;
                    objCouplerItem.purpose_id = objCouplerMaster.purpose_id ?? 0;
                    objCouplerItem.construction_type = objCouplerMaster.construction_type;
                    objCouplerItem.acquire_from = objCouplerMaster.acquire_from;
                    objCouplerItem.ownership_type = objCouplerMaster.ownership_type;
                    objCouplerItem.third_party_vendor_id = objCouplerMaster.third_party_vendor_id;
                    objCouplerItem.audit_item_master_id = objCouplerMaster.audit_item_master_id;
                    objCouplerItem.primary_pod_system_id = objCouplerMaster.primary_pod_system_id;
                    objCouplerItem.secondary_pod_system_id = objCouplerMaster.secondary_pod_system_id;
                    objCouplerItem.status_remark = objCouplerMaster.status_remark;
                    objCouplerItem.remarks = objCouplerMaster.remarks;
                    objCouplerItem.is_acquire_from = objCouplerMaster.is_acquire_from;
                    objCouplerItem.other_info = objCouplerMaster.other_info;    //for additional-attributes
                    objCouplerItem.requested_by = objCouplerMaster.requested_by;
                    objCouplerItem.request_approved_by = objCouplerMaster.request_approved_by;
                    objCouplerItem.request_ref_id = objCouplerMaster.request_ref_id;
                    objCouplerItem.origin_ref_id = objCouplerMaster.origin_ref_id;
                    objCouplerItem.origin_ref_description = objCouplerMaster.origin_ref_description;
                    objCouplerItem.origin_from = objCouplerMaster.origin_from;
                    objCouplerItem.origin_ref_code = objCouplerMaster.origin_ref_code;
                    objCouplerItem.bom_sub_category=objCouplerMaster.bom_sub_category;
                    objCouplerItem.gis_design_id = objCouplerMaster.gis_design_id;
                    //objCouplerItem.served_by_ring=objCouplerMaster.served_by_ring;
                    var CouplerResp =  repo.Update(objCouplerItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(CouplerResp.system_id, Models.EntityType.Coupler.ToString(), CouplerResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Coupler.ToString(), CouplerResp.province_id);
                    return CouplerResp;

                }
                else
                {
                    objCouplerMaster.created_by = userId;
                    objCouplerMaster.created_on = DateTimeHelper.Now;
                    //objCouplerMaster.status = "A";
                    //objCouplerMaster.network_status = "P";
                    objCouplerMaster.status = String.IsNullOrEmpty(objCouplerMaster.status) ? "A" : objCouplerMaster.status;
                    objCouplerMaster.network_status = String.IsNullOrEmpty(objCouplerMaster.network_status) ? "P" : objCouplerMaster.network_status;
                    var resultItem = repo.Insert(objCouplerMaster);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Coupler.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Coupler.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Coupler.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteCouplerById(int systemId)
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
        public string GetOtherInfoCoupler(int systemId)
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
