using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAPEP : Repository<PEPMaster>
    {
      
        public PEPMaster SaveEntityPEP(PEPMaster objPEPMaster, int userId)
        {
            try
            {
                var objPEPItem = repo.Get(x => x.system_id == objPEPMaster.system_id);
                if (objPEPItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objPEPMaster.modified_on, objPEPItem.modified_on, objPEPMaster.modified_by, objPEPItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objPEPMaster.objPM = objPageValidate;
                        return objPEPMaster;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(objPEPMaster.system_id, objPEPMaster.entityType, objPEPMaster.latitude.ToString(), objPEPMaster.longitude.ToString(), objPEPMaster.region_id, objPEPMaster.province_id);
                    if (geomresp.status != "OK")
                    {
                        objPEPMaster.objPM = geomresp;
                        return objPEPMaster;
                    }
                    objPEPItem.pep_name = objPEPMaster.pep_name;
                    objPEPItem.pep_no = objPEPMaster.pep_no;
                    objPEPItem.pep_height = objPEPMaster.pep_height;
                    objPEPItem.address = objPEPMaster.address;

                    objPEPItem.specification = objPEPMaster.specification;
                    objPEPItem.category = objPEPMaster.category;
                    objPEPItem.subcategory1 = objPEPMaster.subcategory1;
                    objPEPItem.subcategory2 = objPEPMaster.subcategory2;
                    objPEPItem.subcategory3 = objPEPMaster.subcategory3;
                    objPEPItem.item_code = objPEPMaster.item_code;
                    objPEPItem.vendor_id = objPEPMaster.vendor_id;
                    objPEPItem.type = objPEPMaster.type;
                    objPEPItem.brand = objPEPMaster.brand;
                    objPEPItem.model = objPEPMaster.model;
                    objPEPItem.construction = objPEPMaster.construction;
                    objPEPItem.activation = objPEPMaster.activation;
                    objPEPItem.accessibility = objPEPMaster.accessibility;
                    objPEPItem.modified_by = userId;
                    objPEPItem.modified_on = DateTimeHelper.Now;

                    objPEPItem.project_id = objPEPMaster.project_id ?? 0;
                    objPEPItem.planning_id = objPEPMaster.planning_id ?? 0;
                    objPEPItem.workorder_id = objPEPMaster.workorder_id ?? 0;
                    objPEPItem.purpose_id = objPEPMaster.purpose_id ?? 0;
                    objPEPItem.acquire_from = objPEPMaster.acquire_from;
                    objPEPItem.ownership_type = objPEPMaster.ownership_type;
                    objPEPItem.third_party_vendor_id = objPEPMaster.third_party_vendor_id;
                    objPEPItem.audit_item_master_id = objPEPMaster.audit_item_master_id;
                    objPEPItem.primary_pod_system_id = objPEPMaster.primary_pod_system_id;
                    objPEPItem.secondary_pod_system_id = objPEPMaster.secondary_pod_system_id;
                    objPEPItem.status_remark = objPEPMaster.status_remark;
                    objPEPItem.remarks = objPEPMaster.remarks;
                    objPEPItem.is_acquire_from = objPEPMaster.is_acquire_from;

                    if (!string.IsNullOrEmpty(objPEPMaster.source_ref_type))
                        objPEPItem.source_ref_type = objPEPMaster.source_ref_type;
                    if (!string.IsNullOrEmpty(objPEPMaster.source_ref_id))
                        objPEPItem.source_ref_id = objPEPMaster.source_ref_id;
                    if (!string.IsNullOrEmpty(objPEPMaster.source_ref_description))
                        objPEPItem.source_ref_description = objPEPMaster.source_ref_description;
                    
                    objPEPItem.other_info = objPEPMaster.other_info;    //for additional-attributes
                    objPEPItem.requested_by = objPEPMaster.requested_by;
                    objPEPItem.request_approved_by = objPEPMaster.request_approved_by;
                    objPEPItem.request_ref_id = objPEPMaster.request_ref_id;
                    objPEPItem.origin_ref_id = objPEPMaster.origin_ref_id;
                    objPEPItem.origin_ref_description = objPEPMaster.origin_ref_description;
                    objPEPItem.origin_from = objPEPMaster.origin_from;
                    objPEPItem.origin_ref_code = objPEPMaster.origin_ref_code;
                    objPEPItem.bom_sub_category = objPEPMaster.bom_sub_category;
                    objPEPItem.gis_design_id = objPEPMaster.gis_design_id;
                    objPEPItem.own_vendor_id = objPEPMaster.own_vendor_id;
                    objPEPItem.hierarchy_type = objPEPMaster.hierarchy_type;
                    var PEPResp = repo.Update(objPEPItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(PEPResp.system_id, Models.EntityType.PEP.ToString(), PEPResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.PEP.ToString(), PEPResp.province_id);
                    return PEPResp;

                }
                else
                {
                    objPEPMaster.created_by = userId;
                    objPEPMaster.created_on = DateTimeHelper.Now;
                    objPEPMaster.status = String.IsNullOrEmpty(objPEPMaster.status) ? "A" : objPEPMaster.status;
                    objPEPMaster.network_status = String.IsNullOrEmpty(objPEPMaster.network_status) ? "P" : objPEPMaster.network_status;
                    var resultItem=repo.Insert(objPEPMaster);
                  
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.PEP.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = resultItem.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.PEP.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.PEP.ToString(), resultItem.province_id);
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeletePEPById(int systemId)
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
        public string GetOtherInfoPEP(int systemId)
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
