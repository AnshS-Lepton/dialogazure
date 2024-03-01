using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DAMicroduct : Repository<MicroductMaster>
    {
        private readonly DAAssociateEntity daAssociate;
        public DAMicroduct()
        {
            daAssociate = new DAAssociateEntity();
        }
        public MicroductMaster Save(MicroductMaster objMicroductMaster, int userId)
        {
            try
            {
                var resultItem = new MicroductMaster();
                var objMicroduct = repo.Get(x => x.system_id == objMicroductMaster.system_id);
                if (objMicroduct != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objMicroductMaster.modified_on, objMicroduct.modified_on, objMicroductMaster.modified_by, objMicroduct.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objMicroductMaster.objPM = objPageValidate;
                        return objMicroductMaster;
                    }
                    objMicroduct.network_name = objMicroductMaster.network_name;
                    objMicroduct.accessibility = objMicroductMaster.accessibility;
                    objMicroduct.activation = objMicroductMaster.activation;
                    objMicroduct.audit_item_master_id = objMicroductMaster.audit_item_master_id;
                    objMicroduct.brand = objMicroductMaster.brand;
                    objMicroduct.calculated_length = objMicroductMaster.calculated_length;
                    objMicroduct.category = objMicroductMaster.category;
                    objMicroduct.circuit_id = objMicroductMaster.circuit_id;
                    objMicroduct.construction = objMicroductMaster.construction;


                    objMicroduct.item_code = objMicroductMaster.item_code;
                    objMicroduct.manual_length = objMicroductMaster.manual_length;
                    objMicroduct.modified_by = userId;
                    objMicroduct.modified_on = DateTimeHelper.Now;
                    objMicroduct.network_id = objMicroductMaster.network_id;
                    objMicroduct.network_name = objMicroductMaster.network_name;
                    //objMicroduct.network_status = objMicroductMaster.network_status;

                    objMicroduct.ownership_type = objMicroductMaster.ownership_type;
                    //objMicroduct.parent_entity_type = objMicroductMaster.parent_entity_type;
                    //objMicroduct.parent_network_id = objMicroductMaster.parent_network_id;
                    //objMicroduct.parent_system_id = objMicroductMaster.parent_system_id;
                    objMicroduct.planning_id = objMicroductMaster.planning_id ?? 0;

                    objMicroduct.project_id = objMicroductMaster.project_id ?? 0;
                    objMicroduct.province_id = objMicroductMaster.province_id;
                    objMicroduct.purpose_id = objMicroductMaster.purpose_id ?? 0;
                    objMicroduct.region_id = objMicroductMaster.region_id;
                    //objMicroduct.sequence_id = objMicroductMaster.sequence_id;

                    objMicroduct.specification = objMicroductMaster.specification;
                    //objMicroduct.status = objMicroductMaster.status;

                    objMicroduct.subcategory1 = objMicroductMaster.subcategory1;
                    objMicroduct.subcategory2 = objMicroductMaster.subcategory2;
                    objMicroduct.subcategory3 = objMicroductMaster.subcategory3;
                    objMicroduct.thirdparty_circuit_id = objMicroductMaster.thirdparty_circuit_id;
                    objMicroduct.third_party_vendor_id = objMicroductMaster.third_party_vendor_id;
                    objMicroduct.type = objMicroductMaster.type;
                    objMicroduct.vendor_id = objMicroductMaster.vendor_id;
                    //objMicroduct.width = objMicroductMaster.width;
                    objMicroduct.workorder_id = objMicroductMaster.workorder_id ?? 0;
                    //objMicroduct.x_position = objMicroductMaster.x_position;
                    //objMicroduct.y_position = objMicroductMaster.y_position;
                    objMicroduct.inner_dimension = objMicroductMaster.inner_dimension;
                    objMicroduct.outer_dimension = objMicroductMaster.outer_dimension;
                    objMicroduct.no_of_cables = objMicroductMaster.no_of_cables;
                    objMicroduct.color_code = objMicroductMaster.color_code;
                    objMicroduct.microduct_type = objMicroductMaster.microduct_type;
                    objMicroduct.acquire_from = objMicroductMaster.acquire_from;
                    objMicroduct.pin_code = objMicroductMaster.pin_code;
                    objMicroduct.remarks = objMicroductMaster.remarks;
                    objMicroduct.primary_pod_system_id = objMicroductMaster.primary_pod_system_id;
                    objMicroduct.secondary_pod_system_id = objMicroductMaster.secondary_pod_system_id;
                    objMicroduct.status_remark = objMicroductMaster.status_remark;
                    objMicroduct.no_of_ways = objMicroductMaster.no_of_ways;
                    objMicroduct.internal_diameter = objMicroductMaster.internal_diameter;
                    objMicroduct.external_diameter = objMicroductMaster.external_diameter;
                    objMicroduct.material_type = objMicroductMaster.material_type;                    
                    objMicroduct.is_acquire_from = objMicroductMaster.is_acquire_from;
                    objMicroduct.other_info = objMicroductMaster.other_info;	//for additional-attributes
                    objMicroduct.requested_by = objMicroductMaster.requested_by;
                    objMicroduct.request_approved_by = objMicroductMaster.request_approved_by;
                    objMicroduct.request_ref_id = objMicroductMaster.request_ref_id;
                    objMicroduct.origin_ref_id = objMicroductMaster.origin_ref_id;
                    objMicroduct.origin_ref_description = objMicroductMaster.origin_ref_description;
                    objMicroduct.origin_from = objMicroductMaster.origin_from;
                    objMicroduct.origin_ref_code = objMicroductMaster.origin_ref_code;
                    objMicroduct.bom_sub_category=objMicroductMaster.bom_sub_category;
                    //objMicroduct.served_by_ring=objMicroductMaster.served_by_ring;
                    resultItem = repo.Update(objMicroduct);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Microduct.ToString(), resultItem.province_id, 1);
                   // DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Microduct.ToString(), resultItem.province_id);
                }
                else
                {
                    objMicroductMaster.created_by = userId;
                    objMicroductMaster.created_on = DateTimeHelper.Now;
                    objMicroductMaster.status = "A";
                    objMicroductMaster.network_status = "P";
                    resultItem = repo.Insert(objMicroductMaster);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.networkStatus = string.IsNullOrEmpty(objMicroductMaster.network_status) ? "P" : objMicroductMaster.network_status;
                    geom.systemId = resultItem.system_id;
                    geom.longLat = objMicroductMaster.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.Microduct.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Line.ToString();
                    geom.project_id = resultItem.project_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Microduct.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Microduct.ToString(), resultItem.province_id);

                    AssociateEntity assStartPt = new AssociateEntity();
                    assStartPt.associated_entity_type = objMicroductMaster.a_entity_type;
                    assStartPt.associated_system_id = objMicroductMaster.a_system_id;
                    assStartPt.associated_network_id = objMicroductMaster.a_location;
                    assStartPt.entity_network_id = objMicroductMaster.network_id;
                    assStartPt.entity_system_id = objMicroductMaster.system_id;
                    assStartPt.entity_type = EntityType.Microduct.ToString();
                    assStartPt.is_termination_point = true;
                    assStartPt.created_on = DateTimeHelper.Now;
                    assStartPt.created_by = userId;
                    daAssociate.SaveAssociation(assStartPt);
                    AssociateEntity assEndPt = new AssociateEntity();
                    assEndPt.associated_entity_type = objMicroductMaster.b_entity_type;
                    assEndPt.associated_system_id = objMicroductMaster.b_system_id;
                    assEndPt.associated_network_id = objMicroductMaster.b_location;
                    assEndPt.entity_network_id = objMicroductMaster.network_id;
                    assEndPt.entity_system_id = objMicroductMaster.system_id;
                    assEndPt.entity_type = EntityType.Microduct.ToString();
                    assEndPt.is_termination_point = true;
                    assEndPt.created_on = DateTimeHelper.Now;
                    assEndPt.created_by = userId;
                    daAssociate.SaveAssociation(assEndPt);

                }
                if (resultItem.pSystemId != 0)
                {
                    AssociateEntity objAsso = new AssociateEntity();
                    objAsso.associated_entity_type = EntityType.Microduct.ToString();
                    objAsso.associated_system_id = resultItem.system_id;
                    objAsso.associated_network_id = resultItem.network_id;
                    objAsso.entity_network_id = resultItem.pNetworkId;
                    objAsso.entity_system_id = resultItem.pSystemId;
                    objAsso.entity_type = resultItem.pEntityType;
                    objAsso.created_on = DateTimeHelper.Now;
                    objAsso.created_by = userId;
                    daAssociate.SaveAssociationForEndToEnd(objAsso);
                }
                new DAMicroduct().setEndPoint(resultItem.system_id);
                return resultItem;
            }
            catch (Exception ex) { throw; }
        }
        public int Delete(int systemId)
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
        public MicroductMaster Get(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
        public DbMessage Validate(int systemId)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_validate_noofcables", new{p_system_id = systemId}).FirstOrDefault();
            }
            catch { throw; }
        }
        public void setEndPoint(int systemId)
        {

            repo.ExecuteProcedure<object>("fn_duct_set_end_point", new
            {
                p_system_id = systemId
            });
        }
        #region Additional-Attributes
        public string GetOtherInfoMicroduct(int systemId)
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
