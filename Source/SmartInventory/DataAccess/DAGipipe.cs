using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAGipipe : Repository<GipipeMaster>
    {
        private static DAGipipe objGipipe = null;
        private static readonly object lockObject = new object();
        public static DAGipipe Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objGipipe == null)
                    {
                        objGipipe = new DAGipipe();
                    }
                }
                return objGipipe;
            }
        }
        public GipipeMaster SaveGipipe(GipipeMaster GipipeInfo, int userId)
        {
            try
            {
                var objGipipe = repo.Get(u => u.system_id == GipipeInfo.system_id);
                if (objGipipe != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(GipipeInfo.modified_on, objGipipe.modified_on, GipipeInfo.modified_by, objGipipe.modified_by);
                    if (objPageValidate.message != null)
                    {
                        GipipeInfo.objPM = objPageValidate;
                        return GipipeInfo;
                    }
                    objGipipe.project_id = GipipeInfo.project_id ?? 0;
                    objGipipe.a_location = GipipeInfo.a_location;
                    objGipipe.b_location = GipipeInfo.b_location;
                    objGipipe.pin_code = GipipeInfo.pin_code;
                    objGipipe.manual_length = GipipeInfo.manual_length;
                    objGipipe.gipipe_name = GipipeInfo.gipipe_name;

                    objGipipe.remarks = GipipeInfo.remarks;
                    objGipipe.specification = GipipeInfo.specification;
                    objGipipe.category = GipipeInfo.category;
                    objGipipe.subcategory1 = GipipeInfo.subcategory1;
                    objGipipe.subcategory2 = GipipeInfo.subcategory2;
                    objGipipe.subcategory3 = GipipeInfo.subcategory3;
                    objGipipe.item_code = GipipeInfo.item_code;
                    objGipipe.vendor_id = GipipeInfo.vendor_id;
                    objGipipe.type = GipipeInfo.type;
                    objGipipe.brand = GipipeInfo.brand;
                    objGipipe.model = GipipeInfo.model;
                    objGipipe.construction = GipipeInfo.construction;
                    objGipipe.activation = GipipeInfo.activation;
                    objGipipe.accessibility = GipipeInfo.accessibility;
                    objGipipe.gipipe_type = GipipeInfo.gipipe_type;
                    objGipipe.color_code = GipipeInfo.color_code;
                    objGipipe.inner_dimension = GipipeInfo.inner_dimension;
                    objGipipe.outer_dimension = GipipeInfo.outer_dimension;

                    objGipipe.modified_on = DateTimeHelper.Now;
                    objGipipe.modified_by = userId;

                    objGipipe.project_id = GipipeInfo.project_id ?? 0;
                    objGipipe.planning_id = GipipeInfo.planning_id ?? 0;
                    objGipipe.workorder_id = GipipeInfo.workorder_id ?? 0;
                    objGipipe.purpose_id = GipipeInfo.purpose_id ?? 0;
                    objGipipe.acquire_from = GipipeInfo.acquire_from;
                    objGipipe.ownership_type = GipipeInfo.ownership_type;
                    objGipipe.third_party_vendor_id = GipipeInfo.third_party_vendor_id;
                    objGipipe.audit_item_master_id = GipipeInfo.audit_item_master_id;
                    objGipipe.primary_pod_system_id = GipipeInfo.primary_pod_system_id;
                    objGipipe.secondary_pod_system_id = GipipeInfo.secondary_pod_system_id;
                    objGipipe.status_remark = GipipeInfo.status_remark;
                    objGipipe.is_acquire_from = GipipeInfo.is_acquire_from;
                    objGipipe.other_info = GipipeInfo.other_info;   //for additional-attributes
                    objGipipe.requested_by = GipipeInfo.requested_by;
                    objGipipe.request_approved_by = GipipeInfo.request_approved_by;
                    objGipipe.request_ref_id = GipipeInfo.request_ref_id;
                    objGipipe.origin_ref_id = GipipeInfo.origin_ref_id;
                    objGipipe.origin_ref_description = GipipeInfo.origin_ref_description;
                    objGipipe.origin_from = GipipeInfo.origin_from;
                    objGipipe.origin_ref_code = GipipeInfo.origin_ref_code;
                    objGipipe.bom_sub_category= GipipeInfo.bom_sub_category;
                    objGipipe.calculated_length = GipipeInfo.calculated_length;
                    objGipipe.gis_design_id = GipipeInfo.gis_design_id;
                    objGipipe.hierarchy_type = GipipeInfo.hierarchy_type;
                    objGipipe.own_vendor_id = GipipeInfo.own_vendor_id;
                    var GipipeResp = repo.Update(objGipipe);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(GipipeResp.system_id, Models.EntityType.Gipipe.ToString(), GipipeResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Gipipe.ToString(), GipipeResp.province_id);
                    return GipipeResp;
                }
                else
                {

                    var latLong = GipipeInfo.geom;
                    GipipeInfo.network_status = string.IsNullOrEmpty(GipipeInfo.network_status) ? "P" : GipipeInfo.network_status;
                    GipipeInfo.status = String.IsNullOrEmpty(GipipeInfo.status) ? "A" : GipipeInfo.status;
                    GipipeInfo.created_on = DateTimeHelper.Now;
                    GipipeInfo.created_by = userId;
                    GipipeInfo = repo.Insert(GipipeInfo);
                    // Save geometry
                   
                    InputGeom geom = new InputGeom();
                    geom.networkStatus = string.IsNullOrEmpty(GipipeInfo.network_status) ? "P" : GipipeInfo.network_status;
                    geom.systemId = GipipeInfo.system_id;
                    geom.longLat = latLong;
                    geom.userId = userId;
                    geom.entityType = EntityType.Gipipe.ToString();
                    geom.commonName = GipipeInfo.network_id;
                    geom.geomType = GeometryType.Line.ToString();
                    geom.project_id = GipipeInfo.project_id ?? 0;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(GipipeInfo.system_id, Models.EntityType.Gipipe.ToString(), GipipeInfo.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Gipipe.ToString(), GipipeInfo.province_id);
                    if (GipipeInfo.a_system_id > 0)
                    {
                        AssociateEntity assStartPt = new AssociateEntity();
                        assStartPt.associated_entity_type = GipipeInfo.a_entity_type;
                        assStartPt.associated_system_id = GipipeInfo.a_system_id;
                        assStartPt.associated_network_id = GipipeInfo.a_location;
                        assStartPt.entity_network_id = GipipeInfo.network_id;
                        assStartPt.entity_system_id = GipipeInfo.system_id;
                        assStartPt.entity_type = EntityType.Gipipe.ToString();
                        assStartPt.is_termination_point = true;
                        assStartPt.created_on = DateTimeHelper.Now;
                        assStartPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(assStartPt);
                    }
                    if (GipipeInfo.b_system_id > 0)
                    {
                        AssociateEntity assEndPt = new AssociateEntity();
                        assEndPt.associated_entity_type = GipipeInfo.b_entity_type;
                        assEndPt.associated_system_id = GipipeInfo.b_system_id;
                        assEndPt.associated_network_id = GipipeInfo.b_location;
                        assEndPt.entity_network_id = GipipeInfo.network_id;
                        assEndPt.entity_system_id = GipipeInfo.system_id;
                        assEndPt.entity_type = EntityType.Gipipe.ToString();
                        assEndPt.is_termination_point = true;
                        assEndPt.created_on = DateTimeHelper.Now;
                        assEndPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(assEndPt);
                    }
                    

                    if (GipipeInfo.pSystemId != 0 && GipipeInfo.pEntityType == EntityType.Trench.ToString())
                    {
                        DAMisc objDAMisc = new DAMisc();
                        var objTrench = objDAMisc.GetEntityDetailById<TrenchMaster>(GipipeInfo.pSystemId, EntityType.Trench, GipipeInfo.user_id);
                        objTrench.no_of_gipipes = (Convert.ToInt32(objTrench.no_of_gipipes) + 1).ToString();
                        new DATrench().SaveTrench(objTrench, userId);

                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.Gipipe.ToString();
                        objAsso.associated_system_id = GipipeInfo.system_id;
                        objAsso.associated_network_id = GipipeInfo.network_id;
                        objAsso.entity_network_id = GipipeInfo.pNetworkId;
                        objAsso.entity_system_id = GipipeInfo.pSystemId;
                        objAsso.entity_type = GipipeInfo.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociationForEndToEnd(objAsso);
                    }
                  

                    new DAGipipe().setEndPoint(GipipeInfo.system_id);
                    return GipipeInfo;
                }

            }
            catch
            {
                throw;
            }
        }

        public EditLineTP EditGipipeTPDetail(EditLineTP objTPDetail, int userId)
        {
            try
            {
                var objGipipe = repo.Get(u => u.system_id == objTPDetail.system_id);
                if (objGipipe != null)
                {

                    objGipipe.a_location = objTPDetail.tpDetail[0].network_id ?? "";
                    objGipipe.a_system_id = objTPDetail.tpDetail[0].system_id;
                    objGipipe.a_entity_type = objTPDetail.tpDetail[0].entity_type ?? "";

                    objGipipe.b_location = objTPDetail.tpDetail[1].network_id ?? "";
                    objGipipe.b_system_id = objTPDetail.tpDetail[1].system_id;
                    objGipipe.b_entity_type = objTPDetail.tpDetail[1].entity_type ?? "";

                    objGipipe.modified_on = DateTimeHelper.Now;
                    objGipipe.modified_by = userId;
                    repo.Update(objGipipe);
                    new DAAssociateEntity().UpdateTPAssociation(objGipipe.system_id, EntityType.Gipipe.ToString(), userId);
                }
                return objTPDetail;
            }
            catch { throw; }
        }

        public int DeleteGipipeById(int gipipe_Id)
        {
            int result = 0;
            try
            {
                var gipipeDetails = repo.Get(u => u.system_id == gipipe_Id);
                if (gipipeDetails != null)
                {
                    result = repo.Delete(gipipeDetails.system_id);
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

            repo.ExecuteProcedure<object>("fn_gipipe_set_end_point", new
            {
                p_system_id = systemId
            });
        }

        #region Additional-Attributes
        public string GetOtherInfoGipipe(int systemId)
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
