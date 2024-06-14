using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DADuct : Repository<DuctMaster>
    {
        private static DADuct objDuct = null;
        private static readonly object lockObject = new object();
        public static DADuct Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDuct == null)
                    {
                        objDuct = new DADuct();
                    }
                }
                return objDuct;
            }
        }
        public DuctMaster SaveDuct(DuctMaster DuctInfo, int userId)
        {
            try
            {
                var objDuct = repo.Get(u => u.system_id == DuctInfo.system_id);
                if (objDuct != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(DuctInfo.modified_on, objDuct.modified_on, DuctInfo.modified_by, objDuct.modified_by);
                    if (objPageValidate.message != null)
                    {
                        DuctInfo.objPM = objPageValidate;
                        return DuctInfo;
                    }
                    objDuct.a_location = DuctInfo.a_location;
                    objDuct.b_location = DuctInfo.b_location;
                    objDuct.pin_code = DuctInfo.pin_code;
                    objDuct.manual_length = DuctInfo.manual_length;
                    objDuct.duct_name = DuctInfo.duct_name;

                    objDuct.remarks = DuctInfo.remarks;
                    objDuct.specification = DuctInfo.specification;
                    objDuct.category = DuctInfo.category;
                    objDuct.subcategory1 = DuctInfo.subcategory1;
                    objDuct.subcategory2 = DuctInfo.subcategory2;
                    objDuct.subcategory3 = DuctInfo.subcategory3;
                    objDuct.item_code = DuctInfo.item_code;
                    objDuct.vendor_id = DuctInfo.vendor_id;
                    objDuct.type = DuctInfo.type;
                    objDuct.brand = DuctInfo.brand;
                    objDuct.model = DuctInfo.model;
                    objDuct.construction = DuctInfo.construction;
                    objDuct.activation = DuctInfo.activation;
                    objDuct.accessibility = DuctInfo.accessibility;
                    objDuct.duct_type = DuctInfo.cable_type;
                    objDuct.color_code = DuctInfo.color_code;
                    objDuct.inner_dimension = DuctInfo.inner_dimension;
                    objDuct.outer_dimension = DuctInfo.outer_dimension;

                    objDuct.modified_on = DateTimeHelper.Now;
                    objDuct.modified_by = userId;

                    objDuct.project_id = DuctInfo.project_id ?? 0;
                    objDuct.planning_id = DuctInfo.planning_id ?? 0;
                    objDuct.workorder_id = DuctInfo.workorder_id ?? 0;
                    objDuct.purpose_id = DuctInfo.purpose_id ?? 0;
                    objDuct.acquire_from = DuctInfo.acquire_from;
                    objDuct.ownership_type = DuctInfo.ownership_type;
                    objDuct.third_party_vendor_id = DuctInfo.third_party_vendor_id;
                    objDuct.audit_item_master_id = DuctInfo.audit_item_master_id;
                    objDuct.primary_pod_system_id = DuctInfo.primary_pod_system_id;
                    objDuct.secondary_pod_system_id = DuctInfo.secondary_pod_system_id;
                    objDuct.status_remark = DuctInfo.status_remark;
                    objDuct.is_acquire_from = DuctInfo.is_acquire_from;
                    objDuct.other_info = DuctInfo.other_info;   //for additional-attributes
                    objDuct.requested_by = DuctInfo.requested_by;
                    objDuct.request_approved_by = DuctInfo.request_approved_by;
                    objDuct.request_ref_id = DuctInfo.request_ref_id;
                    objDuct.origin_ref_id = DuctInfo.origin_ref_id;
                    objDuct.origin_ref_description = DuctInfo.origin_ref_description;
                    objDuct.origin_from = DuctInfo.origin_from;
                    objDuct.origin_ref_code = DuctInfo.origin_ref_code;
                    objDuct.bom_sub_category= DuctInfo.bom_sub_category;
                    objDuct.calculated_length = DuctInfo.calculated_length;
                    objDuct.gis_design_id = DuctInfo.gis_design_id;
                    objDuct.hierarchy_type = DuctInfo.hierarchy_type;
                    objDuct.own_vendor_id = DuctInfo.own_vendor_id;
                    //DuctInfo.served_by_ring = DuctInfo.served_by_ring;
                    var DuctResp =  repo.Update(objDuct);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(DuctResp.system_id, Models.EntityType.Duct.ToString(), DuctResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Duct.ToString(), DuctResp.province_id);
                    return DuctResp;
                }
                else
                {

                    var latLong = DuctInfo.geom;
                    //DuctInfo.status = "A";
                    DuctInfo.network_status = string.IsNullOrEmpty(DuctInfo.network_status) ? "P" : DuctInfo.network_status;
                    DuctInfo.status = String.IsNullOrEmpty(DuctInfo.status) ? "A" : DuctInfo.status;
                    DuctInfo.created_on = DateTimeHelper.Now;
                    DuctInfo.created_by = userId;
                    DuctInfo = repo.Insert(DuctInfo);
                    // Save geometry
                    if (DuctInfo.cable_type != "ISP" && latLong != "" && latLong != "0")
                    {
                        InputGeom geom = new InputGeom();
                        geom.networkStatus = string.IsNullOrEmpty(DuctInfo.network_status) ? "P" : DuctInfo.network_status;
                        geom.systemId = DuctInfo.system_id;
                        geom.longLat = latLong;
                        geom.userId = userId;
                        geom.entityType = EntityType.Duct.ToString();
                        geom.commonName = DuctInfo.network_id;
                        geom.geomType = GeometryType.Line.ToString();
                        geom.project_id = DuctInfo.project_id;
                        string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                        DAIspLine.Instance.CreateOSPCable(DuctInfo.system_id);
                        new DADuct().setEndPoint(DuctInfo.system_id);
                        DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(DuctInfo.system_id, Models.EntityType.Duct.ToString(), DuctInfo.province_id, 0);
                    }
                    else
                    {
                        IspLineMaster objLine = new IspLineMaster();
                        objLine.entity_id = DuctInfo.system_id;
                        objLine.entity_type = EntityType.Duct.ToString();
                        objLine.line_geom = DuctInfo.ispLineGeom;
                        objLine.structure_id = DuctInfo.structure_id;
                        objLine.created_by = userId;
                        objLine.created_on = DateTimeHelper.Now;
                        objLine.a_node_type = DuctInfo.a_node_type;
                        objLine.b_node_type = DuctInfo.b_node_type;
                        DAIspLine.Instance.saveLineGeom(objLine);
                    }

                    if (DuctInfo.a_system_id > 0)
                    {
                        AssociateEntity assStartPt = new AssociateEntity();
                        assStartPt.associated_entity_type = DuctInfo.a_entity_type;
                        assStartPt.associated_system_id = DuctInfo.a_system_id;
                        assStartPt.associated_network_id = DuctInfo.a_location;
                        assStartPt.entity_network_id = DuctInfo.network_id;
                        assStartPt.entity_system_id = DuctInfo.system_id;
                        assStartPt.entity_type = EntityType.Duct.ToString();
                        assStartPt.is_termination_point = true;
                        assStartPt.created_on = DateTimeHelper.Now;
                        assStartPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(assStartPt);
                    }
                    if (DuctInfo.b_system_id > 0)
                    {
                        AssociateEntity assEndPt = new AssociateEntity();
                        assEndPt.associated_entity_type = DuctInfo.b_entity_type;
                        assEndPt.associated_system_id = DuctInfo.b_system_id;
                        assEndPt.associated_network_id = DuctInfo.b_location;
                        assEndPt.entity_network_id = DuctInfo.network_id;
                        assEndPt.entity_system_id = DuctInfo.system_id;
                        assEndPt.entity_type = EntityType.Duct.ToString();
                        assEndPt.is_termination_point = true;
                        assEndPt.created_on = DateTimeHelper.Now;
                        assEndPt.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(assEndPt);
                    }
                    //old code to update number of ducts in trench id..
                    //if (DuctInfo.trench_id != 0)
                    //{
                    //    DAMisc objDAMisc = new DAMisc();
                    //    var objTrench = objDAMisc.GetEntityDetailById<TrenchMaster>(DuctInfo.trench_id, EntityType.Trench);
                    //    objTrench.no_of_ducts = (Convert.ToInt32(objTrench.no_of_ducts) + 1).ToString();
                    //    new DATrench().SaveTrench(objTrench, userId);
                    //}
                    //if (DuctInfo.trench_id != 0)
                    //{
                    //    AssociateEntity objAsso = new AssociateEntity();
                    //    objAsso.associated_entity_type = EntityType.Duct.ToString();
                    //    objAsso.associated_system_id = DuctInfo.system_id;
                    //    objAsso.associated_network_id = DuctInfo.network_id;
                    //    objAsso.entity_network_id = DuctInfo.pNetworkId;
                    //    objAsso.entity_system_id = DuctInfo.trench_id;
                    //    objAsso.entity_type = EntityType.Trench.ToString();
                    //    objAsso.created_on = DateTimeHelper.Now;
                    //    objAsso.created_by = userId;
                    //    new DAAssociateEntity().SaveAssociation(objAsso);
                    //}

                    if (DuctInfo.pSystemId != 0 && DuctInfo.pEntityType == EntityType.Trench.ToString())
                    {
                        DAMisc objDAMisc = new DAMisc();
                        var objTrench = objDAMisc.GetEntityDetailById<TrenchMaster>(DuctInfo.pSystemId, EntityType.Trench, DuctInfo.user_id);
                        objTrench.no_of_ducts = (Convert.ToInt32(objTrench.no_of_ducts) + 1).ToString();
                        new DATrench().SaveTrench(objTrench, userId);

                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.Duct.ToString();
                        objAsso.associated_system_id = DuctInfo.system_id;
                        objAsso.associated_network_id = DuctInfo.network_id;
                        objAsso.entity_network_id = DuctInfo.pNetworkId;
                        objAsso.entity_system_id = DuctInfo.pSystemId;
                        objAsso.entity_type = DuctInfo.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociationForEndToEnd(objAsso);
                    }


                    new DADuct().setEndPoint(DuctInfo.system_id);
                    return DuctInfo;
                }

            }
            catch
            {
                throw;
            }
        }

        public EditLineTP EditDuctTPDetail(EditLineTP objTPDetail, int userId)
        {
            try
            {
                var objDuct = repo.Get(u => u.system_id == objTPDetail.system_id);
                if (objDuct != null)
                {
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[0].network_id))
                    //{
                    objDuct.a_location = objTPDetail.tpDetail[0].network_id ?? "";
                    objDuct.a_system_id = objTPDetail.tpDetail[0].system_id;
                    objDuct.a_entity_type = objTPDetail.tpDetail[0].entity_type ?? "";
                    //}
                    //if (!string.IsNullOrEmpty(objTPDetail.tpDetail[1].network_id))
                    //{
                    objDuct.b_location = objTPDetail.tpDetail[1].network_id ?? "";
                    objDuct.b_system_id = objTPDetail.tpDetail[1].system_id;
                    objDuct.b_entity_type = objTPDetail.tpDetail[1].entity_type ?? "";
                    //}
                    //var networkCodeDetail = new DAMisc().GetLineNetworkCode(objDuct.a_location, objDuct.b_location, EntityType.Cable.ToString(), objTPDetail.entityGeom,"OSP");
                    //if (!string.IsNullOrEmpty(networkCodeDetail.network_code))
                    // objDuct.network_id = networkCodeDetail.network_code;

                    objDuct.modified_on = DateTimeHelper.Now;
                    objDuct.modified_by = userId;
                    repo.Update(objDuct);
                    new DAAssociateEntity().UpdateTPAssociation(objDuct.system_id, EntityType.Duct.ToString(), userId);
                }
                return objTPDetail;
            }
            catch { throw; }
        }

        public int DeleteDuctById(int duct_Id)
        {
            int result = 0;
            try
            {
                var ductDetails = repo.Get(u => u.system_id == duct_Id);
                if (ductDetails != null)
                {
                    result = repo.Delete(ductDetails.system_id);
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

            repo.ExecuteProcedure<object>("fn_duct_set_end_point", new
            {
                p_system_id = systemId
            });
        }

        #region Additional-Attributes
        public string GetOtherInfoDuct(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
        public int getDuctCount(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<int>("fn_get_duct_count", new
                {
                    p_system_id = systemId
                }).FirstOrDefault();
            }
            catch { throw; }
        }
        public DuctMaster GetDuctNameAndLengthForSlack(int DuctId)
        {
            try
            {
                var result = repo.GetById(m => m.system_id == DuctId);
                return result != null ? result : new DuctMaster();
            }
            catch
            {
                throw;
            }
        }
    }
}
