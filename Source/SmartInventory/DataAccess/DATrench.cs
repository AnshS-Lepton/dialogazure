using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DATrench : Repository<TrenchMaster>
    {
        private static DATrench objTrench = null;
        private static readonly object lockObject = new object();
        public static DATrench Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objTrench == null)
                    {
                        objTrench = new DATrench();
                    }
                }
                return objTrench;
            }
        }
        public TrenchMaster SaveTrench(TrenchMaster TrenchInfo, int userId)
        {
            try
            {
                TrenchInfo.no_of_ducts = TrenchInfo.no_of_ducts == null ? "0" : TrenchInfo.no_of_ducts;
                var objTrench = repo.Get(u => u.system_id == TrenchInfo.system_id);
                if (objTrench != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(TrenchInfo.modified_on, objTrench.modified_on, TrenchInfo.modified_by,objTrench.modified_by);
                    if (objPageValidate.message != null)
                    {
                        TrenchInfo.objPM = objPageValidate;
                        return TrenchInfo;
                    }
                    objTrench.a_location = TrenchInfo.a_location;
                    objTrench.b_location = TrenchInfo.b_location;
                    objTrench.pin_code = TrenchInfo.pin_code;
                    objTrench.trench_length = TrenchInfo.trench_length;
                    objTrench.trench_width = TrenchInfo.trench_width;
                    objTrench.trench_height = TrenchInfo.trench_height;
                    objTrench.trench_type = TrenchInfo.trench_type;
                    objTrench.trench_name = TrenchInfo.trench_name;


                    objTrench.remarks = TrenchInfo.remarks;
                    objTrench.specification = TrenchInfo.specification;
                    objTrench.category = TrenchInfo.category;
                    objTrench.subcategory1 = TrenchInfo.subcategory1;
                    objTrench.subcategory2 = TrenchInfo.subcategory2;
                    objTrench.subcategory3 = TrenchInfo.subcategory3;
                    objTrench.item_code = TrenchInfo.item_code;
                    objTrench.vendor_id = TrenchInfo.vendor_id;
                    objTrench.type = TrenchInfo.type;
                    objTrench.brand = TrenchInfo.brand;
                    objTrench.model = TrenchInfo.model;
                    objTrench.construction = TrenchInfo.construction;
                    objTrench.activation = TrenchInfo.activation;
                    objTrench.accessibility = TrenchInfo.accessibility;
                    objTrench.no_of_ducts = TrenchInfo.no_of_ducts;
                    objTrench.modified_on = DateTimeHelper.Now;
                    objTrench.modified_by = userId;
                    objTrench.mcgm_ward = TrenchInfo.mcgm_ward;
                    objTrench.strata_type = TrenchInfo.strata_type;
                    objTrench.manufacture_year = TrenchInfo.manufacture_year;
                    objTrench.surface_type = TrenchInfo.surface_type;
                    objTrench.project_id = TrenchInfo.project_id ?? 0;
                    objTrench.planning_id = TrenchInfo.planning_id ?? 0;
                    objTrench.workorder_id = TrenchInfo.workorder_id ?? 0;
                    objTrench.purpose_id = TrenchInfo.purpose_id ?? 0;
                    objTrench.acquire_from = TrenchInfo.acquire_from;
                    objTrench.ownership_type = TrenchInfo.ownership_type;
                    objTrench.third_party_vendor_id = TrenchInfo.third_party_vendor_id;
                    objTrench.audit_item_master_id = TrenchInfo.audit_item_master_id;
                    objTrench.primary_pod_system_id = TrenchInfo.primary_pod_system_id;
                    objTrench.secondary_pod_system_id = TrenchInfo.secondary_pod_system_id;
                    objTrench.status_remark = TrenchInfo.status_remark;
                    objTrench.is_acquire_from = TrenchInfo.is_acquire_from;
                    objTrench.other_info = TrenchInfo.other_info;   //for additional-attributes
                    objTrench.requested_by = TrenchInfo.requested_by;
                    objTrench.request_approved_by = TrenchInfo.request_approved_by;
                    objTrench.request_ref_id = TrenchInfo.request_ref_id;
                    objTrench.origin_ref_id = TrenchInfo.origin_ref_id;
                    objTrench.origin_ref_description = TrenchInfo.origin_ref_description;
                    objTrench.origin_from = TrenchInfo.origin_from;
                    objTrench.origin_ref_code = TrenchInfo.origin_ref_code;
                    objTrench.bom_sub_category = TrenchInfo.bom_sub_category;
                    // objTrench.served_by_ring= TrenchInfo.served_by_ring;
                    objTrench.trench_serving_type= TrenchInfo.trench_serving_type;
                    objTrench.gis_design_id = TrenchInfo.gis_design_id;
                    objTrench.own_vendor_id = TrenchInfo.own_vendor_id;
                    objTrench.hierarchy_type = TrenchInfo.hierarchy_type;
					objTrench.a_location_code = TrenchInfo.a_location_code;
					objTrench.b_location_code = TrenchInfo.b_location_code;
					var TrenchResp = repo.Update(objTrench);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(TrenchResp.system_id, Models.EntityType.Trench.ToString(), TrenchResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Trench.ToString(), TrenchResp.province_id);
                    return TrenchResp;
                }
                else
                {
                    var latLong = TrenchInfo.geom;
                    //TrenchInfo.status = "A";
                    //TrenchInfo.network_status = "P";
                    TrenchInfo.status = String.IsNullOrEmpty(TrenchInfo.status) ? "A" : TrenchInfo.status;
                    TrenchInfo.network_status = String.IsNullOrEmpty(TrenchInfo.network_status) ? "P" : TrenchInfo.network_status;
                    TrenchInfo.created_on = DateTimeHelper.Now;
                    TrenchInfo.created_by = userId;
                    TrenchInfo= repo.Insert(TrenchInfo);
                    // Save geometry
                    
                    InputGeom geom = new InputGeom();
                    geom.systemId = TrenchInfo.system_id;
                    geom.longLat = latLong;
                    geom.userId = userId;
                    geom.entityType = EntityType.Trench.ToString();
                    geom.commonName = TrenchInfo.network_id;
                    geom.geomType = GeometryType.Line.ToString();
                    geom.project_id = TrenchInfo.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(TrenchInfo.system_id, Models.EntityType.Trench.ToString(), TrenchInfo.province_id, 0);
                   // DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Trench.ToString(), TrenchInfo.province_id);
                    if (TrenchInfo.a_system_id > 0)
                    {
                        AssociateEntity assStartPt = new AssociateEntity();
                        assStartPt.associated_entity_type = TrenchInfo.a_entity_type;
                        assStartPt.associated_system_id = TrenchInfo.a_system_id;
                        assStartPt.associated_network_id = TrenchInfo.a_location;
                        assStartPt.entity_network_id = TrenchInfo.network_id;
                        assStartPt.entity_system_id = TrenchInfo.system_id;
                        assStartPt.entity_type = EntityType.Trench.ToString();
                        assStartPt.is_termination_point = true;
                        assStartPt.created_on = DateTimeHelper.Now;
                        assStartPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(assStartPt);
                    }
                    if (TrenchInfo.b_system_id > 0)
                    {
                        AssociateEntity assEndPt = new AssociateEntity();
                        assEndPt.associated_entity_type = TrenchInfo.b_entity_type;
                        assEndPt.associated_system_id = TrenchInfo.b_system_id;
                        assEndPt.associated_network_id = TrenchInfo.b_location;
                        assEndPt.entity_network_id = TrenchInfo.network_id;
                        assEndPt.entity_system_id = TrenchInfo.system_id;
                        assEndPt.entity_type = EntityType.Trench.ToString();
                        assEndPt.is_termination_point = true;
                        assEndPt.created_on = DateTimeHelper.Now;
                        assEndPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(assEndPt);
                        new DATrench().setEndPoint(TrenchInfo.system_id);
                    }
                    return TrenchInfo;
                }

            }
            catch
            {
                throw;
            }
        }

        public EditLineTP EditTrenchTPDetail(EditLineTP objTPDetail, int userId)
        {
            try
            {               
                var objTrench = repo.Get(u => u.system_id == objTPDetail.system_id);
                if (objTrench != null)
                {
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[0].network_id))
                    //{
                        objTrench.a_location = objTPDetail.tpDetail[0].network_id ?? "";
                        objTrench.a_system_id = objTPDetail.tpDetail[0].system_id;
                        objTrench.a_entity_type = objTPDetail.tpDetail[0].entity_type ?? "";
                    //}
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[1].network_id))
                    //{
                        objTrench.b_location = objTPDetail.tpDetail[1].network_id ?? "";
                        objTrench.b_system_id = objTPDetail.tpDetail[1].system_id;
                        objTrench.b_entity_type = objTPDetail.tpDetail[1].entity_type ?? "";
                    objTrench.a_location_code = "A";
                    objTrench.b_location_code = "B";
                    //}
                    //var networkCodeDetail = new DAMisc().GetLineNetworkCode(objTrench.a_location, objTrench.b_location, objTPDetail.entity_type, objTPDetail.entityGeom,"OSP");
                    //if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
                    //objTrench.network_id = networkCodeDetail.network_code;

                    objTrench.modified_on = DateTimeHelper.Now;
                    objTrench.modified_by = userId;
                    repo.Update(objTrench);
                    new DAAssociateEntity().UpdateTPAssociation(objTrench.system_id, EntityType.Trench.ToString(), userId);
                }
                return objTPDetail;
            }
            catch { throw; }
        }

        public int DeleteTrenchById(int trench_Id)
        {
            int result = 0;
            try
            {
                var trenchDetails = repo.Get(u => u.system_id == trench_Id && u.network_status == "P");
                if (trenchDetails != null)
                {
                    result = repo.Delete(trenchDetails.system_id);
                    return result;
                }
            }
            catch
            {
                throw;
            }
            return result;
        }
        public void setEndPoint(int systemId)
        {

            repo.ExecuteProcedure<object>("fn_trench_set_end_point", new
            {
                p_system_id = systemId
            });
        }

        #region Additional-Attributes
        public string GetOtherInfoTrench(int systemId)
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
