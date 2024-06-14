using DataAccess.DBHelpers;
using DataAccess.ISP;
using Models;
using Models.WFM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DAFMS : Repository<FMSMaster>
    {
       
        public FMSMaster SaveEntityFMS(FMSMaster objFMSMaster, int userId)
        {
            try
            {
                var objFMSItem = repo.Get(x => x.system_id == objFMSMaster.system_id);
                if (objFMSItem != null)
                {
                    PageMessage objPageValidate = new PageMessage();
                    objPageValidate = DAUtility.ValidateModifiedDate(objFMSMaster.modified_on, objFMSItem.modified_on, objFMSMaster.modified_by, objFMSItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objFMSMaster.objPM = objPageValidate;
                        return objFMSMaster;
                    }

                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = objFMSMaster.system_id,
                        entity_type = EntityType.FMS.ToString(),
                        network_id = objFMSMaster.network_id,
                        vendor_id = objFMSMaster.vendor_id,
                        specification = objFMSMaster.specification,
                        item_code = objFMSMaster.item_code,
                        no_of_input_port=objFMSMaster.no_of_input_port,
                        no_of_output_port= objFMSMaster.no_of_output_port,
                        no_of_port= objFMSMaster.no_of_port
                    }, false);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);//objMessage.message;
                        objFMSMaster.objPM = objPageValidate;
                        return objFMSMaster;
                    }

                    objFMSItem.fms_name = objFMSMaster.fms_name;
                    objFMSItem.address = objFMSMaster.address;

                    objFMSItem.specification = objFMSMaster.specification;
                    objFMSItem.category = objFMSMaster.category;
                    objFMSItem.subcategory1 = objFMSMaster.subcategory1;
                    objFMSItem.subcategory2 = objFMSMaster.subcategory2;
                    objFMSItem.subcategory3 = objFMSMaster.subcategory3;
                    objFMSItem.item_code = objFMSMaster.item_code;
                    objFMSItem.vendor_id = objFMSMaster.vendor_id;
                    objFMSItem.type = objFMSMaster.type;
                    objFMSItem.brand = objFMSMaster.brand;
                    objFMSItem.model = objFMSMaster.model;
                    objFMSItem.construction = objFMSMaster.construction;
                    objFMSItem.activation = objFMSMaster.activation;
                    objFMSItem.accessibility = objFMSMaster.accessibility;
                    objFMSItem.modified_by = userId;
                    objFMSItem.modified_on = DateTimeHelper.Now;

                    objFMSItem.project_id = objFMSMaster.project_id ?? 0;
                    objFMSItem.planning_id = objFMSMaster.planning_id ?? 0;
                    objFMSItem.workorder_id = objFMSMaster.workorder_id ?? 0;
                    objFMSItem.purpose_id = objFMSMaster.purpose_id ?? 0;
                    objFMSItem.pincode = objFMSMaster.pincode;
                    objFMSItem.ownership_type = objFMSMaster.ownership_type;
                    objFMSItem.third_party_vendor_id = objFMSMaster.third_party_vendor_id;
                    objFMSItem.audit_item_master_id = objFMSMaster.audit_item_master_id;
                    objFMSItem.acquire_from = objFMSMaster.acquire_from;
                    objFMSItem.secondary_pod_system_id = objFMSMaster.secondary_pod_system_id;
                    objFMSItem.primary_pod_system_id = objFMSMaster.primary_pod_system_id;
                    objFMSItem.status_remark = objFMSMaster.status_remark;
                    objFMSItem.remarks = objFMSMaster.remarks;
                    objFMSItem.is_acquire_from = objFMSMaster.is_acquire_from;
                    if (objFMSMaster.no_of_input_port != 0 && objFMSMaster.no_of_output_port != 0 && (objFMSItem.no_of_input_port != objFMSMaster.no_of_input_port || objFMSItem.no_of_output_port != objFMSMaster.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objFMSItem.system_id, objFMSItem.network_id, EntityType.FMS.ToString(), objFMSMaster.no_of_input_port + ":" + objFMSMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objFMSMaster.no_of_input_port, objFMSMaster.no_of_output_port, EntityType.FMS.ToString(), objFMSMaster.system_id, objFMSMaster.network_id, userId);
                    }
                    else if (objFMSItem.no_of_port != objFMSMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objFMSItem.system_id, EntityType.FMS.ToString(), objFMSItem.specification, objFMSItem.vendor_id, objFMSItem.item_code);
                        if (response.status)
                        {
                            objFMSItem.isPortConnected = response.status;
                            objFMSItem.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return objFMSItem;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objFMSItem.system_id, objFMSItem.network_id, EntityType.FMS.ToString(), objFMSMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objFMSMaster.no_of_port, objFMSMaster.no_of_port, EntityType.FMS.ToString(), objFMSMaster.system_id, objFMSMaster.network_id, userId);
                    }
                    objFMSItem.no_of_input_port = objFMSMaster.no_of_input_port;
                    objFMSItem.no_of_output_port = objFMSMaster.no_of_output_port;
                    objFMSItem.no_of_port = objFMSMaster.no_of_port;

                    objFMSItem.other_info = objFMSMaster.other_info;    //for additional-attributes
                    objFMSItem.requested_by = objFMSMaster.requested_by;
                    objFMSItem.request_approved_by = objFMSMaster.request_approved_by;
                    objFMSItem.request_ref_id = objFMSMaster.request_ref_id;
                    objFMSItem.origin_ref_id = objFMSMaster.origin_ref_id;
                    objFMSItem.origin_ref_description = objFMSMaster.origin_ref_description;
                    objFMSItem.origin_from = objFMSMaster.origin_from;
                    objFMSItem.origin_ref_code = objFMSMaster.origin_ref_code;
                    objFMSItem.bom_sub_category = objFMSMaster.bom_sub_category;
                    objFMSItem.gis_design_id = objFMSMaster.gis_design_id;
                    objFMSItem.own_vendor_id = objFMSMaster.own_vendor_id;
                    objFMSItem.hierarchy_type = objFMSMaster.hierarchy_type;
                    //objFMSItem.served_by_ring=objFMSMaster.served_by_ring;
                    var FMSResp = repo.Update(objFMSItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(FMSResp.system_id, Models.EntityType.FMS.ToString(), FMSResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.FMS.ToString(), FMSResp.province_id);
                    return FMSResp;



                }
                else
                {
                    if (objFMSMaster.objIspEntityMap.floor_id > 0 && objFMSMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objFMSMaster.system_id,
                            entity_type = EntityType.FMS.ToString(),
                            floor_id = objFMSMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objFMSMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objFMSMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objFMSMaster.objPM = objPageValidate;
                            return objFMSMaster;
                        }
                    }

                    objFMSMaster.created_by = userId;
                    objFMSMaster.created_on = DateTimeHelper.Now;
                    //objFMSMaster.status = "A";
                    //objFMSMaster.network_status = "P";
                    objFMSMaster.status = String.IsNullOrEmpty(objFMSMaster.status) ? "A" : objFMSMaster.status;
                    objFMSMaster.network_status = String.IsNullOrEmpty(objFMSMaster.network_status) ? "P" : objFMSMaster.network_status;
                    objFMSMaster.utilization = "L";
                    var response = repo.Insert(objFMSMaster);
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = response.system_id;
                    geom.longLat = response.longitude + " " + response.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.FMS.ToString();
                    geom.commonName = response.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = response.project_id;
                    if (objFMSMaster.no_of_input_port != 0 && objFMSMaster.no_of_output_port != 0)
                    { geom.ports = objFMSMaster.no_of_input_port + ":" + objFMSMaster.no_of_output_port; }
                    else if (objFMSMaster.no_of_port != 0) { geom.ports = objFMSMaster.no_of_port.ToString(); }
                    //string chkGeomInsert = BASaveEntityGeometry.Instance.SaveEntityGeometry(geom);
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.FMS.ToString(), response.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.FMS.ToString(), response.province_id);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objFMSMaster.objIspEntityMap.shaft_id, objFMSMaster.objIspEntityMap.floor_id, objFMSMaster.system_id, EntityType.FMS.ToString(), objFMSMaster.pSystemId, objFMSMaster.pEntityType, objFMSMaster.longitude + " " + objFMSMaster.latitude);
                    //var ispMapping = DAISPEntityMapping.Instance.getMappingByEntityId(objFMSMaster.parent_system_id, objFMSMaster.parent_entity_type);
                    //if (ispMapping != null && ispMapping.structure_id != 0 && ispMapping.floor_id != 0)
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.structure_id = ispMapping.structure_id;
                    //    objMapping.floor_id = ispMapping.floor_id ?? 0;
                    //    objMapping.shaft_id = ispMapping.shaft_id ?? 0;
                    //    objMapping.entity_id = objFMSMaster.system_id;
                    //    objMapping.entity_type = EntityType.FMS.ToString();
                    //    objMapping.parent_id = ispMapping.id;
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    //else
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.entity_id = objFMSMaster.system_id;
                    //    objMapping.entity_type = EntityType.FMS.ToString();
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    if (response != null && objFMSMaster.no_of_input_port != 0 && objFMSMaster.no_of_output_port != 0)
                    {
                        var inputPort = objFMSMaster.no_of_input_port;
                        var outputPort = objFMSMaster.no_of_output_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.FMS.ToString(), response.system_id, response.network_id, userId);
                    }
                    else
                    {
                        var inputPort = objFMSMaster.no_of_port;
                        var outputPort = objFMSMaster.no_of_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.FMS.ToString(), response.system_id, response.network_id, userId);
                    }
                    return response;
                }
            }
            catch { throw; }
        }
        public int DeleteFMSById(int systemId)
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
        public List<FMSMaster> GetFMSByParentId(int systemid, string entityType)
        {
            try
            {
                return repo.GetAll(m => m.parent_system_id == systemid && m.parent_entity_type == entityType).ToList();
            }
            catch { throw; }
        }
        #region Additional-Attributes
        public string GetOtherInfoFMS(int systemId)
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

