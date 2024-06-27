using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DASC : Repository<SCMaster>
    {
        
        public SCMaster SaveEntitySC(SCMaster objSCMaster, int userId)
        {
            try
            {
                var objSCItem = repo.Get(x => x.system_id == objSCMaster.system_id);
                if (objSCItem != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objSCMaster.modified_on, objSCItem.modified_on, objSCMaster.modified_by, objSCItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objSCMaster.objPM = objPageValidate;
                        return objSCMaster;
                    }

                    objSCItem.spliceclosure_name = objSCMaster.spliceclosure_name;
                    objSCItem.address = objSCMaster.address;

                    objSCItem.specification = objSCMaster.specification;
                    objSCItem.category = objSCMaster.category;
                    objSCItem.subcategory1 = objSCMaster.subcategory1;
                    objSCItem.subcategory2 = objSCMaster.subcategory2;
                    objSCItem.subcategory3 = objSCMaster.subcategory3;
                    objSCItem.item_code = objSCMaster.item_code;
                    objSCItem.vendor_id = objSCMaster.vendor_id;
                    objSCItem.type = objSCMaster.type;
                    objSCItem.brand = objSCMaster.brand;
                    objSCItem.model = objSCMaster.model;
                    objSCItem.construction = objSCMaster.construction;
                    objSCItem.activation = objSCMaster.activation;
                    objSCItem.accessibility = objSCMaster.accessibility;
                    objSCItem.modified_by = userId;
                    objSCItem.modified_on = DateTimeHelper.Now;
                    objSCItem.is_virtual = objSCMaster.is_virtual;
                    objSCItem.project_id = objSCMaster.project_id ?? 0;
                    objSCItem.planning_id = objSCMaster.planning_id ?? 0;
                    objSCItem.workorder_id = objSCMaster.workorder_id ?? 0;
                    objSCItem.purpose_id = objSCMaster.purpose_id ?? 0;
                    objSCItem.pincode = objSCMaster.pincode;
                    objSCItem.remarks = objSCMaster.remarks;
                    objSCItem.is_acquire_from = objSCMaster.is_acquire_from;
                    if (objSCMaster.no_of_input_port != 0 && objSCMaster.no_of_output_port != 0 && (objSCItem.no_of_input_port != objSCMaster.no_of_input_port || objSCItem.no_of_output_port != objSCMaster.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objSCItem.system_id, objSCItem.network_id, EntityType.SpliceClosure.ToString(), objSCMaster.no_of_input_port + ":" + objSCMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objSCMaster.no_of_input_port, objSCMaster.no_of_output_port, EntityType.SpliceClosure.ToString(), objSCMaster.system_id, objSCMaster.network_id, userId);
                    }
                    else if (objSCItem.no_of_ports != objSCMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objSCItem.system_id, EntityType.SpliceClosure.ToString(), objSCItem.specification, objSCItem.vendor_id, objSCItem.item_code);
                        if (response.status)
                        {
                            objSCItem.isPortConnected = response.status;
                            objSCItem.message = Resources.Helper.MultilingualMessageConvert(response.message);// response.message;
                            return objSCItem;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objSCItem.system_id, objSCItem.network_id, EntityType.SpliceClosure.ToString(), objSCMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objSCMaster.no_of_port, objSCMaster.no_of_port, EntityType.SpliceClosure.ToString(), objSCMaster.system_id, objSCMaster.network_id, userId);
                    }
                    objSCItem.no_of_input_port = objSCMaster.no_of_input_port;
                    objSCItem.no_of_output_port = objSCMaster.no_of_output_port;
                    objSCItem.no_of_ports = objSCMaster.no_of_port;
                    objSCItem.acquire_from = objSCMaster.acquire_from;
                    objSCItem.is_buried = objSCMaster.is_buried;
                    objSCItem.ownership_type = objSCMaster.ownership_type;
                    objSCItem.third_party_vendor_id = objSCMaster.third_party_vendor_id;
                    objSCItem.audit_item_master_id = objSCMaster.audit_item_master_id;
                    objSCItem.primary_pod_system_id = objSCMaster.primary_pod_system_id;
                    objSCItem.secondary_pod_system_id = objSCMaster.secondary_pod_system_id;
                    objSCItem.status_remark = objSCMaster.status_remark;
                    objSCItem.other_info = objSCMaster.other_info;
                    objSCItem.requested_by = objSCMaster.requested_by;
                    objSCItem.request_approved_by = objSCMaster.request_approved_by;
                    objSCItem.request_ref_id = objSCMaster.request_ref_id;
                    objSCItem.origin_ref_id = objSCMaster.origin_ref_id;
                    objSCItem.origin_ref_description = objSCMaster.origin_ref_description;
                    objSCItem.origin_from = objSCMaster.origin_from;
                    objSCItem.origin_ref_code = objSCMaster.origin_ref_code;
                    // objSCItem.served_by_ring = objSCMaster.served_by_ring;
                    objSCItem.bom_sub_category = objSCMaster.bom_sub_category;
                    objSCItem.spliceclosure_type = objSCMaster.spliceclosure_type;
                    var SCResp = repo.Update(objSCItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(SCResp.system_id, Models.EntityType.SpliceClosure.ToString(), SCResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.SpliceClosure.ToString(), SCResp.province_id);
                    return SCResp;
                }
                else
                {
                    if (objSCMaster.objIspEntityMap.floor_id > 0 && objSCMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objSCMaster.system_id,
                            entity_type = EntityType.SpliceClosure.ToString(),
                            floor_id = objSCMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objSCMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objSCMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);// objMessage.message;
                            objSCMaster.objPM = objPageValidate;
                            return objSCMaster;
                        }
                    }

                    objSCMaster.created_by = userId;
                    objSCMaster.created_on = DateTimeHelper.Now;
                    //objSCMaster.status = "A";
                    //objSCMaster.network_status = "P";
                    objSCMaster.status = String.IsNullOrEmpty(objSCMaster.status) ? "A" : objSCMaster.status;
                    objSCMaster.network_status = String.IsNullOrEmpty(objSCMaster.network_status) ? "P" : objSCMaster.network_status;

                    objSCMaster.no_of_ports = objSCMaster.no_of_port;
                    objSCMaster.utilization = "L";
                    if(objSCMaster.parent_entity_type!=EntityType.Province.ToString())
                    {
                        objSCMaster.parent_system_id = objSCMaster.pSystemId;
                        objSCMaster.parent_entity_type = objSCMaster.pEntityType;
                        objSCMaster.parent_network_id = objSCMaster.pNetworkId;
                    }
                    
                    var response = repo.Insert(objSCMaster);
                    
                    new DAAssociateEntity().SaveEntityAssociation(objSCMaster.associated_entity_type, objSCMaster.associated_system_id, response.network_id,response.system_id, EntityType.SpliceClosure.ToString(), userId);
                
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = response.system_id;
                    geom.longLat = response.longitude + " " + response.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.SpliceClosure.ToString();
                    geom.commonName = response.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.is_virtual = response.is_virtual;
                    geom.project_id = response.project_id;
                    if (objSCMaster.no_of_input_port != 0 && objSCMaster.no_of_output_port != 0)
                    { geom.ports = objSCMaster.no_of_input_port + ":" + objSCMaster.no_of_output_port; }
                    else if (objSCMaster.no_of_ports != 0) { geom.ports = objSCMaster.no_of_ports.ToString(); }
                    //string chkGeomInsert = BASaveEntityGeometry.Instance.SaveEntityGeometry(geom);
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.SpliceClosure.ToString(), response.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.SpliceClosure.ToString(), response.province_id);
                    //DAIspEntityMapping.Instance.associateEntityInStructure(objSCMaster.objIspEntityMap.shaft_id, objSCMaster.objIspEntityMap.floor_id, objSCMaster.system_id, EntityType.Spliceclosure.ToString(), objSCMaster.parent_system_id, objSCMaster.parent_entity_type);
                    //if (objSCMaster.objIspEntityMap.floor_id != 0 || objSCMaster.objIspEntityMap.shaft_id != 0)
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.id = 0;// objSplitterMaster.objIspEntityMap.id;
                    //    objMapping.structure_id = objSCMaster.objIspEntityMap.structure_id!=0?objSCMaster.objIspEntityMap.structure_id:objSCMaster.parent_system_id;
                    //    objMapping.floor_id = objSCMaster.objIspEntityMap.floor_id ?? 0;
                    //    objMapping.shaft_id = objSCMaster.objIspEntityMap.shaft_id ?? 0;
                    //    objMapping.entity_id = objSCMaster.system_id;
                    //    objMapping.entity_type = EntityType.Spliceclosure.ToString();
                    //    objMapping.parent_id = objSCMaster.objIspEntityMap.id;
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    if (response != null && objSCMaster.no_of_input_port != 0 && objSCMaster.no_of_output_port != 0)
                    {
                        var inputPort = objSCMaster.no_of_input_port;
                        var outputPort = objSCMaster.no_of_output_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.SpliceClosure.ToString(), response.system_id, response.network_id, userId);
                    }
                    else
                    {
                        var inputPort = objSCMaster.no_of_ports;
                        var outputPort = objSCMaster.no_of_ports;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.SpliceClosure.ToString(), response.system_id, response.network_id, userId);
                    }
                    if (objSCMaster.pEntityType != null && objSCMaster.pSystemId != 0 && objSCMaster.pEntityType.ToUpper() != "STRUCTURE")
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.SpliceClosure.ToString();
                        objAsso.associated_system_id = response.system_id;
                        objAsso.associated_network_id = response.network_id;
                        objAsso.entity_network_id = objSCMaster.pNetworkId;
                        objAsso.entity_system_id = objSCMaster.pSystemId;
                        objAsso.entity_type = objSCMaster.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(objAsso);
                    }
                    else
                    {
                        var responseAsso = DAIspEntityMapping.Instance.associateEntityInStructure(objSCMaster.objIspEntityMap.shaft_id, objSCMaster.objIspEntityMap.floor_id, objSCMaster.system_id, EntityType.SpliceClosure.ToString(), objSCMaster.pSystemId, objSCMaster.parent_entity_type, objSCMaster.longitude + " " + objSCMaster.latitude);
                        if (responseAsso.status)
                        {
                            objSCMaster.isPortConnected = responseAsso.status;
                            objSCMaster.message = Resources.Helper.MultilingualMessageConvert(responseAsso.message); //responseAsso.message;
                            return objSCMaster;
                        }
                    }
                    return response;
                }
            }
            catch { throw; }
        }
        public int DeleteSCById(int systemId)
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
        public SCMaster getSCDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
        #region Additional-Attributes
        public string GetOtherInfoSpliceClosure(int systemId)
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
