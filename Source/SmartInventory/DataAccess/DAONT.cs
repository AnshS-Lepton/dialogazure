using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DAONT : Repository<ONTMaster>
    {
        public ONTMaster SaveONTEntity(ONTMaster objONTMaster, int userId)
        {
            try
            {
                

                string oldPorts = string.Empty;
                int inputPort = 0;
                int outputPort = 0;
                var objONT = repo.Get(x => x.system_id == objONTMaster.system_id);
                if (objONT != null)
                {
                   PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objONTMaster.modified_on, objONT.modified_on,objONTMaster.modified_by, objONT.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objONTMaster.objPM = objPageValidate;
                        return objONTMaster;
                    }
                    objONT.network_id = objONTMaster.network_id;
                    objONT.ont_name = objONTMaster.ont_name;
                    objONT.serial_no = objONTMaster.serial_no;

                    objONT.specification = objONTMaster.specification;
                    objONT.category = objONTMaster.category;
                    objONT.subcategory1 = objONTMaster.subcategory1;
                    objONT.subcategory2 = objONTMaster.subcategory2;
                    objONT.subcategory3 = objONTMaster.subcategory3;
                    objONT.item_code = objONTMaster.item_code;
                    objONT.vendor_id = objONTMaster.vendor_id;
                    objONT.type = objONTMaster.type;
                    objONT.brand = objONTMaster.brand;
                    objONT.model = objONTMaster.model;
                    objONT.construction = objONTMaster.construction;
                    objONT.activation = objONTMaster.activation;
                    objONT.accessibility = objONTMaster.accessibility;
                    objONT.modified_by = userId;
                    objONT.modified_on = DateTimeHelper.Now;
                    objONT.longitude = objONTMaster.longitude;
                    objONT.latitude = objONTMaster.latitude;
                    objONT.parent_system_id = objONTMaster.parent_system_id;
                    objONT.parent_entity_type = objONTMaster.parent_entity_type;
                    objONT.parent_network_id = objONTMaster.parent_network_id;
                    objONT.ownership_type = objONTMaster.ownership_type;
                    objONT.third_party_vendor_id = objONTMaster.third_party_vendor_id;
                    objONT.audit_item_master_id = objONTMaster.audit_item_master_id;
                    objONT.acquire_from = objONTMaster.acquire_from;
                    objONT.primary_pod_system_id = objONTMaster.primary_pod_system_id;
                    objONT.secondary_pod_system_id = objONTMaster.secondary_pod_system_id;
                    objONT.status_remark = objONTMaster.status_remark;
                    objONT.remarks = objONTMaster.remarks;
                    objONT.cpe_type = objONTMaster.cpe_type ;
                    objONT.is_acquire_from = objONTMaster.is_acquire_from;
                    if (objONTMaster.no_of_input_port != 0 && objONTMaster.no_of_output_port != 0 && (objONT.no_of_input_port != objONTMaster.no_of_input_port || objONT.no_of_output_port != objONTMaster.no_of_output_port))
                    {
                        var response = new DAMisc().isPortConnected(objONT.system_id, EntityType.ONT.ToString(), objONT.specification, objONT.vendor_id, objONT.item_code);
                        if (response.status)
                        {
                            objONT.isPortConnected = response.status;
                            objONT.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return objONT;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objONT.system_id, objONT.network_id, EntityType.ONT.ToString(), objONTMaster.no_of_input_port + ":" + objONTMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objONTMaster.no_of_input_port, objONTMaster.no_of_output_port, EntityType.ONT.ToString(), objONT.system_id, objONT.network_id, userId);
                    }
                    else if (objONT.no_of_port != objONTMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objONT.system_id, EntityType.ONT.ToString(), objONT.specification, objONT.vendor_id, objONT.item_code);
                        if (response.status)
                        {
                            objONT.isPortConnected = response.status;
                            objONT.message = Resources.Helper.MultilingualMessageConvert(response.message);//response.message;
                            return objONT;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objONT.system_id, objONT.network_id, EntityType.ONT.ToString(), objONTMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objONTMaster.no_of_port, objONTMaster.no_of_port, EntityType.ONT.ToString(), objONT.system_id, objONT.network_id, userId);
                    }


                    objONT.project_id = objONTMaster.project_id ?? 0;
                    objONT.planning_id = objONTMaster.planning_id ?? 0;
                    objONT.workorder_id = objONTMaster.workorder_id ?? 0;
                    objONT.purpose_id = objONTMaster.purpose_id ?? 0;
                    //if (objONTMaster.objIspEntityMap.structure_id != 0)
                    //{
                    //    objONT.parent_system_id = Convert.ToInt32(objONTMaster.objIspEntityMap.structure_id);
                    //    objONT.parent_entity_type = EntityType.Structure.ToString();
                    //}
                    //else
                    //{
                    //    objONT.parent_system_id = 0;
                    //    objONT.parent_entity_type = "Province";
                    //}
                    objONT.longitude = objONTMaster.longitude;
                    objONT.latitude = objONTMaster.latitude;
                    var resp = DAIspEntityMapping.Instance.associateEntityInStructure(objONTMaster.objIspEntityMap.shaft_id, objONTMaster.objIspEntityMap.floor_id, objONTMaster.system_id, EntityType.ONT.ToString(), objONTMaster.parent_system_id, objONTMaster.parent_entity_type, objONTMaster.longitude + " " + objONTMaster.latitude);
                    if (resp.status)
                    {
                        objONT.isPortConnected = resp.status;
                        objONT.message = Resources.Helper.MultilingualMessageConvert(resp.message); //resp.message;
                        return objONT;
                    }

                    if (!(string.IsNullOrEmpty(objONTMaster.unitValue)) && objONTMaster.unitValue.Contains(":"))
                    {
                        oldPorts = objONT.no_of_input_port + ":" + objONT.no_of_output_port;
                        inputPort = Convert.ToInt32(objONTMaster.unitValue.Split(':')[0]);
                        outputPort = Convert.ToInt32(objONTMaster.unitValue.Split(':')[1]);
                        objONT.no_of_input_port = inputPort;
                        objONT.no_of_output_port = outputPort;
                        objONTMaster.no_of_input_port = inputPort;
                        objONTMaster.no_of_output_port = outputPort;

                    }
                    objONT.other_info = objONTMaster.other_info;    //for additional-attributes
                    objONT.requested_by = objONTMaster.requested_by;
                    objONT.request_approved_by = objONTMaster.request_approved_by;
                    objONT.request_ref_id = objONTMaster.request_ref_id;
                    objONT.origin_ref_id = objONTMaster.origin_ref_id;
                    objONT.origin_ref_description = objONTMaster.origin_ref_description;
                    objONT.origin_from = objONTMaster.origin_from;
                    objONT.origin_ref_code = objONTMaster.origin_ref_code;
                    objONT.bom_sub_category = objONTMaster.bom_sub_category;
                    objONT.gis_design_id = objONTMaster.gis_design_id;
                    // objONT.served_by_ring   = objONTMaster.served_by_ring;
                    var result = repo.Update(objONT);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.ONT.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.ONT.ToString(), result.province_id);
                    return result;
                }
                else
                {
                    PageMessage objPageValidate = new PageMessage();
                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = objONTMaster.system_id,
                        entity_type = EntityType.ONT.ToString(),
                        floor_id = objONTMaster.objIspEntityMap.floor_id ?? 0,
                        shaft_id = objONTMaster.objIspEntityMap.shaft_id ?? 0,
                        parent_system_id = objONTMaster.parent_system_id,
                        parent_entity_type = objONTMaster.parent_entity_type
                    }, objONTMaster.system_id == 0);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                        objONTMaster.objPM = objPageValidate;
                        return objONTMaster;
                    }

                    objONTMaster.created_by = userId;
                    objONTMaster.created_on = DateTimeHelper.Now;
                    //objONTMaster.status = "A";
                    //objONTMaster.network_status = "P";
                    objONTMaster.status = String.IsNullOrEmpty(objONTMaster.status) ? "A" : objONTMaster.status;
                    objONTMaster.network_status = String.IsNullOrEmpty(objONTMaster.network_status) ? "P" : objONTMaster.network_status;
                    objONTMaster.utilization = "L";
                    //if (objONTMaster.objIspEntityMap.structure_id != 0)
                    //{
                    //if (!string.IsNullOrEmpty(objONTMaster.pEntityType) && objONTMaster.pSystemId > 0)
                    //{
                    //    objONTMaster.parent_system_id = Convert.ToInt32(objONTMaster.pSystemId);
                    //    objONTMaster.parent_entity_type = objONTMaster.pEntityType;
                    //}
                    //else {
                    //    objONTMaster.parent_system_id = 0;
                    //    objONTMaster.parent_entity_type = "Province";
                    //}
                    // }

                    if (!(string.IsNullOrEmpty(objONTMaster.unitValue)) && objONTMaster.unitValue.Contains(":"))
                    {
                        objONTMaster.no_of_input_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[0]);
                        objONTMaster.no_of_output_port = Convert.ToInt32(objONTMaster.unitValue.Split(':')[1]);
                    }
             
                    var resultItem = repo.Insert(objONTMaster);
                    // Insert Port Info
                    new DAMisc().InsertPortInfo(objONTMaster.no_of_input_port, objONTMaster.no_of_output_port, EntityType.ONT.ToString(), objONTMaster.system_id, objONTMaster.network_id, userId);

                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.ONT.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = resultItem.project_id;
                    if (objONTMaster.no_of_input_port != 0 && objONTMaster.no_of_output_port != 0)
                    { geom.ports = objONTMaster.no_of_input_port + ":" + objONTMaster.no_of_output_port; }
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.ONT.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.ONT.ToString(), resultItem.province_id);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objONTMaster.objIspEntityMap.shaft_id, objONTMaster.objIspEntityMap.floor_id, objONTMaster.system_id, EntityType.ONT.ToString(), objONTMaster.parent_system_id, objONTMaster.parent_entity_type, objONTMaster.longitude + " " + objONTMaster.latitude);                                    
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteONTById(int systemId)
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
        public string GetOtherInfoONT(int systemId)
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
