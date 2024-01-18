using DataAccess.DBHelpers;
using DataAccess.ISP;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DAPatchPanel : Repository<PatchPanelMaster>
    {

        public PatchPanelMaster SaveEntityPatchPanel(PatchPanelMaster objPatchPanelMaster, int userId)
        {
            try
            {
                var objPatchPanelItem = repo.Get(x => x.system_id == objPatchPanelMaster.system_id);
                if (objPatchPanelItem != null)
                {
                    PageMessage objPageValidate = new PageMessage();
                    objPageValidate = DAUtility.ValidateModifiedDate(objPatchPanelMaster.modified_on, objPatchPanelItem.modified_on, objPatchPanelMaster.modified_by, objPatchPanelItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objPatchPanelMaster.objPM = objPageValidate;
                        return objPatchPanelMaster;
                    }

                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = objPatchPanelMaster.system_id,
                        entity_type = EntityType.PatchPanel.ToString(),
                        network_id = objPatchPanelMaster.network_id,
                        vendor_id = objPatchPanelMaster.vendor_id,
                        specification = objPatchPanelMaster.specification,
                        item_code = objPatchPanelMaster.item_code,
                        no_of_input_port = objPatchPanelMaster.no_of_input_port,
                        no_of_output_port = objPatchPanelMaster.no_of_output_port,
                        no_of_port = objPatchPanelMaster.no_of_port
                    }, false);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);//objMessage.message;
                        objPatchPanelMaster.objPM = objPageValidate;
                        return objPatchPanelMaster;
                    }

                    objPatchPanelItem.patchpanel_name = objPatchPanelMaster.patchpanel_name;
                    objPatchPanelItem.address = objPatchPanelMaster.address;

                    objPatchPanelItem.specification = objPatchPanelMaster.specification;
                    objPatchPanelItem.category = objPatchPanelMaster.category;
                    objPatchPanelItem.subcategory1 = objPatchPanelMaster.subcategory1;
                    objPatchPanelItem.subcategory2 = objPatchPanelMaster.subcategory2;
                    objPatchPanelItem.subcategory3 = objPatchPanelMaster.subcategory3;
                    objPatchPanelItem.item_code = objPatchPanelMaster.item_code;
                    objPatchPanelItem.vendor_id = objPatchPanelMaster.vendor_id;
                    objPatchPanelItem.type = objPatchPanelMaster.type;
                    objPatchPanelItem.brand = objPatchPanelMaster.brand;
                    objPatchPanelItem.model = objPatchPanelMaster.model;
                    objPatchPanelItem.construction = objPatchPanelMaster.construction;
                    objPatchPanelItem.activation = objPatchPanelMaster.activation;
                    objPatchPanelItem.accessibility = objPatchPanelMaster.accessibility;
                    objPatchPanelItem.modified_by = userId;
                    objPatchPanelItem.modified_on = DateTimeHelper.Now;

                    objPatchPanelItem.project_id = objPatchPanelMaster.project_id ?? 0;
                    objPatchPanelItem.planning_id = objPatchPanelMaster.planning_id ?? 0;
                    objPatchPanelItem.workorder_id = objPatchPanelMaster.workorder_id ?? 0;
                    objPatchPanelItem.purpose_id = objPatchPanelMaster.purpose_id ?? 0;
                    objPatchPanelItem.pincode = objPatchPanelMaster.pincode;
                    objPatchPanelItem.ownership_type = objPatchPanelMaster.ownership_type;
                    objPatchPanelItem.third_party_vendor_id = objPatchPanelMaster.third_party_vendor_id;
                    objPatchPanelItem.audit_item_master_id = objPatchPanelMaster.audit_item_master_id;
                    objPatchPanelItem.acquire_from = objPatchPanelMaster.acquire_from;
                    objPatchPanelItem.secondary_pod_system_id = objPatchPanelMaster.secondary_pod_system_id;
                    objPatchPanelItem.primary_pod_system_id = objPatchPanelMaster.primary_pod_system_id;
                    objPatchPanelItem.status_remark = objPatchPanelMaster.status_remark;
                    objPatchPanelItem.remarks = objPatchPanelMaster.remarks;
                    objPatchPanelItem.patchpanel_type = objPatchPanelMaster.patchpanel_type;
                    objPatchPanelItem.is_acquire_from = objPatchPanelMaster.is_acquire_from;

                    if (objPatchPanelMaster.no_of_input_port != 0 && objPatchPanelMaster.no_of_output_port != 0 && (objPatchPanelItem.no_of_input_port != objPatchPanelMaster.no_of_input_port || objPatchPanelItem.no_of_output_port != objPatchPanelMaster.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objPatchPanelItem.system_id, objPatchPanelItem.network_id, EntityType.PatchPanel.ToString(), objPatchPanelMaster.no_of_input_port + ":" + objPatchPanelMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objPatchPanelMaster.no_of_input_port, objPatchPanelMaster.no_of_output_port, EntityType.PatchPanel.ToString(), objPatchPanelMaster.system_id, objPatchPanelMaster.network_id, userId);
                    }
                    else if (objPatchPanelItem.no_of_port != objPatchPanelMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objPatchPanelItem.system_id, EntityType.PatchPanel.ToString(), objPatchPanelItem.specification, objPatchPanelItem.vendor_id, objPatchPanelItem.item_code);
                        if (response.status)
                        {
                            objPatchPanelItem.isPortConnected = response.status;
                            objPatchPanelItem.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return objPatchPanelItem;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objPatchPanelItem.system_id, objPatchPanelItem.network_id, EntityType.PatchPanel.ToString(), objPatchPanelMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objPatchPanelMaster.no_of_port, objPatchPanelMaster.no_of_port, EntityType.PatchPanel.ToString(), objPatchPanelMaster.system_id, objPatchPanelMaster.network_id, userId);
                    }
                    objPatchPanelItem.no_of_input_port = objPatchPanelMaster.no_of_input_port;
                    objPatchPanelItem.no_of_output_port = objPatchPanelMaster.no_of_output_port;
                    objPatchPanelItem.no_of_port = objPatchPanelMaster.no_of_port;
                    objPatchPanelItem.requested_by = objPatchPanelMaster.requested_by;
                    objPatchPanelItem.request_approved_by = objPatchPanelMaster.request_approved_by;
                    objPatchPanelItem.request_ref_id = objPatchPanelMaster.request_ref_id;
                    objPatchPanelItem.origin_ref_id = objPatchPanelMaster.origin_ref_id;
                    objPatchPanelItem.origin_ref_description = objPatchPanelMaster.origin_ref_description;
                    objPatchPanelItem.origin_from = objPatchPanelMaster.origin_from;
                    objPatchPanelItem.origin_ref_code = objPatchPanelMaster.origin_ref_code;
                    objPatchPanelItem.bom_sub_category=objPatchPanelMaster.bom_sub_category;
                    // objPatchPanelItem.served_by_ring=objPatchPanelMaster.served_by_ring;
                    var PPResp = repo.Update(objPatchPanelItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(PPResp.system_id, Models.EntityType.PatchPanel.ToString(), PPResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.PatchPanel.ToString(), PPResp.province_id);
                    return PPResp;

                }
                else
                {
                    if (objPatchPanelMaster.objIspEntityMap.floor_id > 0 && objPatchPanelMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objPatchPanelMaster.system_id,
                            entity_type = EntityType.PatchPanel.ToString(),
                            floor_id = objPatchPanelMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objPatchPanelMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objPatchPanelMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objPatchPanelMaster.objPM = objPageValidate;
                            return objPatchPanelMaster;
                        }
                    }

                    objPatchPanelMaster.created_by = userId;
                    objPatchPanelMaster.created_on = DateTimeHelper.Now;
                    //objPatchPanelMaster.status = "A";
                    //objPatchPanelMaster.network_status = "P";
                    objPatchPanelMaster.status = String.IsNullOrEmpty(objPatchPanelMaster.status) ? "A" : objPatchPanelMaster.status;
                    objPatchPanelMaster.network_status = String.IsNullOrEmpty(objPatchPanelMaster.network_status) ? "P" : objPatchPanelMaster.network_status;
                    objPatchPanelMaster.utilization = "L";
                    var response = repo.Insert(objPatchPanelMaster);
                    
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = response.system_id;
                    geom.longLat = response.longitude + " " + response.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.PatchPanel.ToString();
                    geom.commonName = response.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = response.project_id;
                    if (objPatchPanelMaster.no_of_input_port != 0 && objPatchPanelMaster.no_of_output_port != 0)
                    { geom.ports = objPatchPanelMaster.no_of_input_port + ":" + objPatchPanelMaster.no_of_output_port; }
                    else if (objPatchPanelMaster.no_of_port != 0) { geom.ports = objPatchPanelMaster.no_of_port.ToString(); }
                    //string chkGeomInsert = BASaveEntityGeometry.Instance.SaveEntityGeometry(geom);
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objPatchPanelMaster.objIspEntityMap.shaft_id, objPatchPanelMaster.objIspEntityMap.floor_id, objPatchPanelMaster.system_id, EntityType.PatchPanel.ToString(), objPatchPanelMaster.pSystemId, objPatchPanelMaster.pEntityType, objPatchPanelMaster.longitude + " " + objPatchPanelMaster.latitude);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.PatchPanel.ToString(), response.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.PatchPanel.ToString(), response.province_id);
                    //var ispMapping = DAISPEntityMapping.Instance.getMappingByEntityId(objPatchPanelMaster.parent_system_id, objPatchPanelMaster.parent_entity_type);
                    //if (ispMapping != null && ispMapping.structure_id != 0 && ispMapping.floor_id != 0)
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.structure_id = ispMapping.structure_id;
                    //    objMapping.floor_id = ispMapping.floor_id ?? 0;
                    //    objMapping.shaft_id = ispMapping.shaft_id ?? 0;
                    //    objMapping.entity_id = objPatchPanelMaster.system_id;
                    //    objMapping.entity_type = EntityType.PatchPanel.ToString();
                    //    objMapping.parent_id = ispMapping.id;
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    //else
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.entity_id = objPatchPanelMaster.system_id;
                    //    objMapping.entity_type = EntityType.PatchPanel.ToString();
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    if (response != null && objPatchPanelMaster.no_of_input_port != 0 && objPatchPanelMaster.no_of_output_port != 0)
                    {
                        var inputPort = objPatchPanelMaster.no_of_input_port;
                        var outputPort = objPatchPanelMaster.no_of_output_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.PatchPanel.ToString(), response.system_id, response.network_id, userId);
                    }
                    else
                    {
                        var inputPort = objPatchPanelMaster.no_of_port;
                        var outputPort = objPatchPanelMaster.no_of_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.PatchPanel.ToString(), response.system_id, response.network_id, userId);
                    }
                    return response;
                }
            }
            catch(Exception ex) { throw; }
        }
        public int DeletePatchPanelById(int systemId)
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
        public List<PatchPanelMaster> GetPatchPanelByParentId(int systemid, string entityType)
        {
            try
            {
                return repo.GetAll(m => m.parent_system_id == systemid && m.parent_entity_type == entityType).ToList();
            }
            catch { throw; }
        }
    }
}
