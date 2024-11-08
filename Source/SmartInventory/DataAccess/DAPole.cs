using DataAccess.DBHelpers;
using Models;
using Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAPole : Repository<PoleMaster>
    {

        public PoleMaster SaveEntityPole(PoleMaster objPoleMaster, int userId)
        {
            try
            {
                var objPoleItem = repo.Get(x => x.system_id == objPoleMaster.system_id);
                if (objPoleItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objPoleMaster.modified_on, objPoleItem.modified_on, objPoleMaster.modified_by,objPoleItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objPoleMaster.objPM = objPageValidate;
                        return objPoleMaster;
                    }
                    var geomresp =new DAMisc().GetValidatePointGeometry(objPoleMaster.system_id, objPoleMaster.entityType, objPoleMaster.latitude.ToString(), objPoleMaster.longitude.ToString(), objPoleMaster.region_id, objPoleMaster.province_id);
                    if (geomresp.status != "OK")
                    {
                        objPoleMaster.objPM = geomresp;
                        return objPoleMaster;
                    }

                    objPoleItem.pole_name = objPoleMaster.pole_name;
                    objPoleItem.pole_type = objPoleMaster.pole_type;
                    objPoleItem.pole_no = objPoleMaster.pole_no;
                    objPoleItem.pole_height = objPoleMaster.pole_height;
                    objPoleItem.address = objPoleMaster.address;

                    objPoleItem.specification = objPoleMaster.specification;
                    objPoleItem.category = objPoleMaster.category;
                    objPoleItem.subcategory1 = objPoleMaster.subcategory1;
                    objPoleItem.subcategory2 = objPoleMaster.subcategory2;
                    objPoleItem.subcategory3 = objPoleMaster.subcategory3;
                    objPoleItem.item_code = objPoleMaster.item_code;
                    objPoleItem.vendor_id = objPoleMaster.vendor_id;
                    objPoleItem.type = objPoleMaster.type;
                    objPoleItem.brand = objPoleMaster.brand;
                    objPoleItem.model = objPoleMaster.model;
                    objPoleItem.construction = objPoleMaster.construction;
                    objPoleItem.activation = objPoleMaster.activation;
                    objPoleItem.accessibility = objPoleMaster.accessibility;
                    objPoleItem.modified_by = userId;
                    objPoleItem.modified_on = DateTimeHelper.Now;

                    objPoleItem.project_id = objPoleMaster.project_id ?? 0;
                    objPoleItem.planning_id = objPoleMaster.planning_id ?? 0;
                    objPoleItem.workorder_id = objPoleMaster.workorder_id ?? 0;
                    objPoleItem.purpose_id = objPoleMaster.purpose_id ?? 0;
                    objPoleItem.acquire_from = objPoleMaster.acquire_from;
                    objPoleItem.ownership_type = objPoleMaster.ownership_type;
                    objPoleItem.third_party_vendor_id = objPoleMaster.third_party_vendor_id;
                    objPoleItem.remarks = objPoleMaster.remarks;

                    objPoleItem.audit_item_master_id = objPoleMaster.audit_item_master_id;
                    if(!string.IsNullOrEmpty(objPoleMaster.source_ref_type))
                        objPoleItem.source_ref_type = objPoleMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objPoleMaster.source_ref_id))
                        objPoleItem.source_ref_id = objPoleMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objPoleMaster.source_ref_description))
                        objPoleItem.source_ref_description = objPoleMaster.source_ref_description;
                    objPoleItem.primary_pod_system_id = objPoleMaster.primary_pod_system_id;
                    objPoleItem.secondary_pod_system_id = objPoleMaster.secondary_pod_system_id;
                    objPoleItem.status_remark = objPoleMaster.status_remark;
                    objPoleItem.is_acquire_from = objPoleMaster.is_acquire_from;
                    objPoleItem.other_info = objPoleMaster.other_info; //for Additional-Attributes
                    objPoleItem.requested_by = objPoleMaster.requested_by;
                    objPoleItem.request_approved_by = objPoleMaster.request_approved_by;
                    objPoleItem.request_ref_id = objPoleMaster.request_ref_id;
                    objPoleItem.origin_ref_id = objPoleMaster.origin_ref_id;
                    objPoleItem.origin_ref_description = objPoleMaster.origin_ref_description;
                    objPoleItem.origin_from = objPoleMaster.origin_from;
                    objPoleItem.origin_ref_code = objPoleMaster.origin_ref_code;
                    objPoleItem.bom_sub_category = objPoleMaster.bom_sub_category;
                    objPoleItem.gis_design_id = objPoleMaster.gis_design_id;
                    objPoleItem.own_vendor_id = objPoleMaster.own_vendor_id;
                    objPoleItem.hierarchy_type = objPoleMaster.hierarchy_type;
                    objPoleItem.subarea_id = objPoleMaster.subarea_id;
                    //objPoleItem.served_by_ring = objPoleMaster.served_by_ring;
                    var PoleResp = repo.Update(objPoleItem);
                    DbMessage entityObj = new DAMisc(). updateGeojsonEntityAttribute(PoleResp.system_id, Models.EntityType.Pole.ToString(), PoleResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Pole.ToString(), PoleResp.province_id);
                    return PoleResp;

                }
                else
                {
                    objPoleMaster.created_by = userId;
                    objPoleMaster.created_on = DateTimeHelper.Now;
                    //objPoleMaster.status = "A";
                    //objPoleMaster.network_status = "P";
                    objPoleMaster.status = String.IsNullOrEmpty(objPoleMaster.status) ? "A" :objPoleMaster.status;
                    objPoleMaster.network_status = String.IsNullOrEmpty(objPoleMaster.network_status) ? "P" : objPoleMaster.network_status;

                    var resultItem= repo.Insert(objPoleMaster);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Pole.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.entity_category = objPoleMaster.pole_type;
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Pole.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Pole.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeletePoleById(int systemId)
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
        public List<PoleArea> GetPoleArea(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<PoleArea>("fn_get_area", new { p_geometry = geom, p_geomtype = GeometryType.Point.ToString() });
            }
            catch { throw; }
        }

        #region Additional-Attributes
        public string GetOtherInfoPole(int systemId)
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
