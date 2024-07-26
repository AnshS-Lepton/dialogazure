using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAConduit : Repository<ConduitMaster>
    {
        private static DAConduit objConduit = null;
        private static readonly object lockObject = new object();
        public static DAConduit Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objConduit == null)
                    {
                        objConduit = new DAConduit();
                    }
                }
                return objConduit;
            }
        }
        public ConduitMaster SaveConduit(ConduitMaster ConduitInfo, int userId)
        {
            try
            {
                var objConduit = repo.Get(u => u.system_id == ConduitInfo.system_id);
                if (objConduit != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(ConduitInfo.modified_on, objConduit.modified_on, ConduitInfo.modified_by, objConduit.modified_by);
                    if (objPageValidate.message != null)
                    {
                        ConduitInfo.objPM = objPageValidate;
                        return ConduitInfo;
                    }
                    objConduit.a_location = ConduitInfo.a_location;
                    objConduit.b_location = ConduitInfo.b_location;
                    objConduit.pin_code = ConduitInfo.pin_code;
                    objConduit.manual_length = ConduitInfo.manual_length;
                    objConduit.network_name = ConduitInfo.network_name;

                    objConduit.remarks = ConduitInfo.remarks;
                    objConduit.specification = ConduitInfo.specification;
                    objConduit.category = ConduitInfo.category;
                    objConduit.subcategory1 = ConduitInfo.subcategory1;
                    objConduit.subcategory2 = ConduitInfo.subcategory2;
                    objConduit.subcategory3 = ConduitInfo.subcategory3;
                    objConduit.item_code = ConduitInfo.item_code;
                    objConduit.vendor_id = ConduitInfo.vendor_id;
                    objConduit.type = ConduitInfo.type;
                    objConduit.brand = ConduitInfo.brand;
                    objConduit.model = ConduitInfo.model;
                    objConduit.construction = ConduitInfo.construction;
                    objConduit.activation = ConduitInfo.activation;
                    objConduit.accessibility = ConduitInfo.accessibility;
                    objConduit.conduit_type = ConduitInfo.conduit_type;
                    objConduit.color_code = ConduitInfo.color_code;
                    objConduit.inner_dimension = ConduitInfo.inner_dimension;
                    objConduit.outer_dimension = ConduitInfo.outer_dimension;

                    objConduit.modified_on = DateTimeHelper.Now;
                    objConduit.modified_by = userId;

                    objConduit.project_id = ConduitInfo.project_id ?? 0;
                    objConduit.planning_id = ConduitInfo.planning_id ?? 0;
                    objConduit.workorder_id = ConduitInfo.workorder_id ?? 0;
                    objConduit.purpose_id = ConduitInfo.purpose_id ?? 0;
                    objConduit.acquire_from = ConduitInfo.acquire_from;
                    objConduit.ownership_type = ConduitInfo.ownership_type;
                    objConduit.third_party_vendor_id = ConduitInfo.third_party_vendor_id;
                    objConduit.audit_item_master_id = ConduitInfo.audit_item_master_id;
                    objConduit.is_acquire_from = ConduitInfo.is_acquire_from;

                    objConduit.other_info = ConduitInfo.other_info; //for additional-attributes
                    objConduit.requested_by = ConduitInfo.requested_by;
                    objConduit.request_approved_by = ConduitInfo.request_approved_by;
                    objConduit.request_ref_id = ConduitInfo.request_ref_id;
                    objConduit.origin_ref_id = ConduitInfo.origin_ref_id;
                    objConduit.origin_ref_description = ConduitInfo.origin_ref_description;
                    objConduit.origin_from = ConduitInfo.origin_from;
                    objConduit.origin_ref_code = ConduitInfo.origin_ref_code;
                    //objConduit.served_by_ring = ConduitInfo.served_by_ring;
                    objConduit.bom_sub_category = ConduitInfo.bom_sub_category;
                    objConduit.gis_design_id = ConduitInfo.gis_design_id;
                    objConduit.hierarchy_type = ConduitInfo.hierarchy_type;
                    objConduit.own_vendor_id = ConduitInfo.own_vendor_id;                
                    var ConduitResp= repo.Update(objConduit);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(ConduitResp.system_id, Models.EntityType.Conduit.ToString(), ConduitResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Conduit.ToString(), ConduitResp.province_id);
                    return ConduitResp;

                }
                else
                {

                    var latLong = ConduitInfo.geom;
                    //ConduitInfo.status = "A";
                    ConduitInfo.network_status = string.IsNullOrEmpty(ConduitInfo.network_status) ? "P" : ConduitInfo.network_status;
                    ConduitInfo.status = String.IsNullOrEmpty(ConduitInfo.status) ? "A" : ConduitInfo.status;
                    ConduitInfo.created_on = DateTimeHelper.Now;
                    ConduitInfo.created_by = userId;
                    ConduitInfo = repo.Insert(ConduitInfo);
                    // Save geometry
                    
                    InputGeom geom = new InputGeom();
                    geom.networkStatus = string.IsNullOrEmpty(ConduitInfo.network_status) ? "P" : ConduitInfo.network_status;
                    geom.systemId = ConduitInfo.system_id;
                    geom.longLat = latLong;
                    geom.userId = userId;
                    geom.entityType = EntityType.Conduit.ToString();
                    geom.commonName = ConduitInfo.network_id;
                    geom.geomType = GeometryType.Line.ToString();
                    geom.project_id = ConduitInfo.project_id;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(ConduitInfo.system_id, Models.EntityType.Conduit.ToString(), ConduitInfo.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Conduit.ToString(), ConduitInfo.province_id);
                    AssociateEntity assStartPt = new AssociateEntity();
                    assStartPt.associated_entity_type = ConduitInfo.a_entity_type;
                    assStartPt.associated_system_id = ConduitInfo.a_system_id;
                    assStartPt.associated_network_id = ConduitInfo.a_location;
                    assStartPt.entity_network_id = ConduitInfo.network_id;
                    assStartPt.entity_system_id = ConduitInfo.system_id;
                    assStartPt.entity_type = EntityType.Conduit.ToString();
                    assStartPt.is_termination_point = true;
                    assStartPt.created_on = DateTimeHelper.Now;
                    assStartPt.created_by = userId;
                    new DAAssociateEntity().SaveAssociation(assStartPt);
                    AssociateEntity assEndPt = new AssociateEntity();
                    assEndPt.associated_entity_type = ConduitInfo.b_entity_type;
                    assEndPt.associated_system_id = ConduitInfo.b_system_id;
                    assEndPt.associated_network_id = ConduitInfo.b_location;
                    assEndPt.entity_network_id = ConduitInfo.network_id;
                    assEndPt.entity_system_id = ConduitInfo.system_id;
                    assEndPt.entity_type = EntityType.Conduit.ToString();
                    assEndPt.is_termination_point = true;
                    assEndPt.created_on = DateTimeHelper.Now;
                    assEndPt.created_by = userId;
                    new DAAssociateEntity().SaveAssociation(assEndPt);
                    //old code to update number of Conduits in trench id..
                    //if (ConduitInfo.trench_id != 0)
                    //{
                    //    DAMisc objDAMisc = new DAMisc();
                    //    var objTrench = objDAMisc.GetEntityDetailById<TrenchMaster>(ConduitInfo.trench_id, EntityType.Trench);
                    //    objTrench.no_of_Conduits = (Convert.ToInt32(objTrench.no_of_Conduits) + 1).ToString();
                    //    new DATrench().SaveTrench(objTrench, userId);
                    //}
                    if (ConduitInfo.pSystemId != 0)
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.Conduit.ToString();
                        objAsso.associated_system_id = ConduitInfo.system_id;
                        objAsso.associated_network_id = ConduitInfo.network_id;
                        objAsso.entity_network_id = ConduitInfo.pNetworkId;
                        objAsso.entity_system_id = ConduitInfo.pSystemId;
                        objAsso.entity_type = ConduitInfo.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(objAsso);
                    }
                    new DAConduit().setEndPoint(ConduitInfo.system_id);
                    return ConduitInfo;
                }

            }
            catch
            {
                throw;
            }
        }

        public EditLineTP EditConduitTPDetail(EditLineTP objTPDetail, int userId)
        {
            try
            {
                var objConduit = repo.Get(u => u.system_id == objTPDetail.system_id);
                if (objConduit != null)
                {
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[0].network_id))
                    //{
                    objConduit.a_location = objTPDetail.tpDetail[0].network_id ?? "";
                    objConduit.a_system_id = objTPDetail.tpDetail[0].system_id;
                    objConduit.a_entity_type = objTPDetail.tpDetail[0].entity_type ?? "";
                    //}
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[1].network_id))
                    //{
                    objConduit.b_location = objTPDetail.tpDetail[1].network_id ?? "";
                    objConduit.b_system_id = objTPDetail.tpDetail[1].system_id;
                    objConduit.b_entity_type = objTPDetail.tpDetail[1].entity_type ?? "";
                    //}
                    //var networkCodeDetail = new DAMisc().GetLineNetworkCode(objConduit.a_location, objConduit.b_location, EntityType.Cable.ToString(), objTPDetail.entityGeom,"OSP");
                    //if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
                    // objConduit.network_id = networkCodeDetail.network_code;

                    objConduit.modified_on = DateTimeHelper.Now;
                    objConduit.modified_by = userId;
                    repo.Update(objConduit);
                    new DAAssociateEntity().UpdateTPAssociation(objConduit.system_id, EntityType.Conduit.ToString(), userId);
                }
                return objTPDetail;
            }
            catch { throw; }
        }

        public int DeleteConduitById(int Conduit_Id)
        {
            int result = 0;
            try
            {
                var ConduitDetails = repo.Get(u => u.system_id == Conduit_Id);
                if (ConduitDetails != null)
                {
                    result = repo.Delete(ConduitDetails.system_id);
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

            repo.ExecuteProcedure<object>("fn_Conduit_set_end_point", new
            {
                p_system_id = systemId
            });
        }
        #region Additional-Attributes
        public string GetOtherInfoConduit(int systemId)
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
