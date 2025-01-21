using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DAADB : Repository<ADBMaster>
    {

        public ADBMaster SaveEntityADB(ADBMaster objADBMaster, int userId)
        {
            try
            {
                var objADB = repo.Get(x => x.system_id == objADBMaster.system_id);
                if (objADB != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objADBMaster.modified_on, objADB.modified_on, objADBMaster.modified_by, objADB.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objADBMaster.objPM = objPageValidate;
                        return objADBMaster;
                    }
                    objADB.adb_name = objADBMaster.adb_name.ToString();
                    objADB.pincode = objADBMaster.pincode;
                    objADB.address = objADBMaster.address;

                    objADB.specification = objADBMaster.specification;
                    objADB.category = objADBMaster.category;
                    objADB.subcategory1 = objADBMaster.subcategory1;
                    objADB.subcategory2 = objADBMaster.subcategory2;
                    objADB.subcategory3 = objADBMaster.subcategory3;
                    objADB.item_code = objADBMaster.item_code;
                    objADB.vendor_id = objADBMaster.vendor_id;
                    objADB.type = objADBMaster.type;
                    objADB.brand = objADBMaster.brand;
                    objADB.model = objADBMaster.model;
                    objADB.construction = objADBMaster.construction;
                    objADB.activation = objADBMaster.activation;
                    objADB.accessibility = objADBMaster.accessibility;
                    objADB.is_servingdb = objADBMaster.is_servingdb;
                    objADB.modified_by = userId;
                    objADB.modified_on = DateTimeHelper.Now;

                    objADB.project_id = objADBMaster.project_id ?? 0;
                    objADB.planning_id = objADBMaster.planning_id ?? 0;
                    objADB.workorder_id = objADBMaster.workorder_id ?? 0;
                    objADB.purpose_id = objADBMaster.purpose_id ?? 0;
                    

                    if (objADBMaster.no_of_input_port != 0 && objADBMaster.no_of_output_port != 0 && (objADB.no_of_input_port != objADBMaster.no_of_input_port || objADB.no_of_output_port != objADBMaster.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objADB.system_id, objADB.network_id, EntityType.ADB.ToString(), objADBMaster.no_of_input_port + ":" + objADBMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objADBMaster.no_of_input_port, objADBMaster.no_of_output_port, EntityType.ADB.ToString(), objADB.system_id, objADB.network_id, userId);
                    }
                    else if (objADB.no_of_port != objADBMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objADB.system_id, EntityType.ADB.ToString(), objADB.specification, objADB.vendor_id, objADB.item_code);
                        if (response.status)
                        {
                            objADB.isPortConnected = response.status;
                            objADB.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return objADB;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objADB.system_id, objADB.network_id, EntityType.ADB.ToString(), objADBMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objADBMaster.no_of_port, objADBMaster.no_of_port, EntityType.ADB.ToString(), objADB.system_id, objADB.network_id, userId);
                    }
                    objADB.no_of_input_port = objADBMaster.no_of_input_port;
                    objADB.no_of_output_port = objADBMaster.no_of_output_port;
                    objADB.no_of_port = objADBMaster.no_of_port;
                    objADB.entity_category = objADBMaster.entity_category;
                    objADB.acquire_from = objADBMaster.acquire_from;
                    objADB.ownership_type = objADBMaster.ownership_type;
                    objADB.third_party_vendor_id = objADBMaster.third_party_vendor_id;
                    objADB.audit_item_master_id = objADBMaster.audit_item_master_id;
                    objADB.primary_pod_system_id = objADBMaster.primary_pod_system_id;
                    objADB.secondary_pod_system_id = objADBMaster.secondary_pod_system_id;
                    objADB.status_remark = objADBMaster.status_remark;
                    objADB.remarks = objADBMaster.remarks;
                    objADB.is_acquire_from = objADBMaster.is_acquire_from;
                    objADB.other_info = objADBMaster.other_info; //additional-attributes
                    objADB.requested_by = objADBMaster.requested_by;
                    objADB.request_approved_by = objADBMaster.request_approved_by;
                    objADB.request_ref_id = objADBMaster.request_ref_id;
                    objADB.origin_ref_id = objADBMaster.origin_ref_id;
                    objADB.origin_ref_description = objADBMaster.origin_ref_description;
                    objADB.origin_from = objADBMaster.origin_from;
                    objADB.origin_ref_code = objADBMaster.origin_ref_code;
                    objADB.bom_sub_category = objADBMaster.bom_sub_category;
                    objADB.gis_design_id = objADBMaster.gis_design_id;
                    objADB.hierarchy_type = objADBMaster.hierarchy_type;
                    objADB.own_vendor_id = objADBMaster.own_vendor_id;
                    //objADB.served_by_ring = objADBMaster.served_by_ring;
                    var ADBResp = repo.Update(objADB);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(ADBResp.system_id, Models.EntityType.ADB.ToString(), ADBResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.ADB.ToString(), ADBResp.province_id);
                    return ADBResp;

                }
                else
                {
                    if (objADBMaster.objIspEntityMap.floor_id > 0 && objADBMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objADBMaster.system_id,
                            entity_type = EntityType.ADB.ToString(),
                            floor_id = objADBMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objADBMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objADBMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);// objMessage.message;
                            objADBMaster.objPM = objPageValidate;
                            return objADBMaster;
                        }
                    }

                    //ADBMaster objADBItem1 = new ADBMaster();
                    objADBMaster.created_by = userId;
                    objADBMaster.created_on = DateTimeHelper.Now;
                    //objADBMaster.status = "A";
                    //objADBMaster.network_status = "P";

                    objADBMaster.status = String.IsNullOrEmpty(objADBMaster.status) ? "A" : objADBMaster.status;
                    objADBMaster.network_status = String.IsNullOrEmpty(objADBMaster.network_status) ? "P" : objADBMaster.network_status;
                    objADBMaster.utilization = "L";
                    //if (objADBMaster.parent_entity_type != EntityType.Province.ToString())
                    //{
                    //    objADBMaster.parent_system_id = objADBMaster.pSystemId;
                    //    objADBMaster.parent_entity_type = objADBMaster.pEntityType;
                    //    objADBMaster.parent_network_id = objADBMaster.pNetworkId;
                    //}
                    var resultItem = repo.Insert(objADBMaster);
                    
                    //TRANSACTION NEED TO IMPLEMENT THERE...
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude; //resultItem.geom;
                    geom.userId = userId;
                    geom.entityType = EntityType.ADB.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.entity_category = objADBMaster.entity_category;
                    geom.project_id = resultItem.project_id;
                    if (objADBMaster.no_of_input_port != 0 && objADBMaster.no_of_output_port != 0)
                    { geom.ports = objADBMaster.no_of_input_port + ":" + objADBMaster.no_of_output_port; }
                    else if (objADBMaster.no_of_port != 0) { geom.ports = objADBMaster.no_of_port.ToString(); }
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.ADB.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.ADB.ToString(), resultItem.province_id);
                    if (resultItem != null && objADBMaster.no_of_input_port != 0 && objADBMaster.no_of_output_port != 0)
                    {
                        var inputPort = objADBMaster.no_of_input_port;
                        var outputPort = objADBMaster.no_of_output_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.ADB.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }
                    else
                    {
                        var inputPort = objADBMaster.no_of_port;
                        var outputPort = objADBMaster.no_of_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.ADB.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }
                    if (objADBMaster.pEntityType != null && objADBMaster.pSystemId != 0 && objADBMaster.pEntityType.ToUpper() != "STRUCTURE")
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.ADB.ToString();
                        objAsso.associated_system_id = resultItem.system_id;
                        objAsso.associated_network_id = resultItem.network_id;
                        objAsso.entity_network_id = objADBMaster.pNetworkId;
                        objAsso.entity_system_id = objADBMaster.pSystemId;
                        objAsso.entity_type = objADBMaster.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(objAsso);
                    }
                    else
                    {
                        var responseAsso = DAIspEntityMapping.Instance.associateEntityInStructure(objADBMaster.objIspEntityMap.shaft_id, objADBMaster.objIspEntityMap.floor_id, objADBMaster.system_id, EntityType.ADB.ToString(), objADBMaster.parent_system_id, objADBMaster.parent_entity_type, objADBMaster.longitude + " " + objADBMaster.latitude);
                        if (responseAsso.status)
                        {
                            objADBMaster.isPortConnected = responseAsso.status;
                            objADBMaster.message = Resources.Helper.MultilingualMessageConvert(responseAsso.message); //responseAsso.message;
                            return objADBMaster;
                        }
                    }
                    return resultItem;
                }
            }
            catch { throw; }
        }

        //public ADBMaster GetEntityItemTemplate(int user_id,int system_id)
        //{
        //    try
        //    {
        //        return repo.Get(u => u.created_by == user_id && u.system_id == system_id);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public ADBMaster GetADBDetailById(int systemid)
        //{
        //    try
        //    {
        //        var lstItems = repo.ExecuteProcedure<ADBMaster>("fn_get_entitydetail_by_id", new { p_systemid = systemid, p_entity_name = EntityType.ADB.ToString() }, true);
        //        return lstItems != null && lstItems.Count > 0 ? lstItems[0] : new ADBMaster();
        //    }
        //    catch { throw; }
        //}
        public List<ADBSubArea> GetADBSubArea(string geom)
        {
            try
            {
                return repo.ExecuteProcedure<ADBSubArea>("fn_get_subarea", new { p_geometry = geom, p_geomtype = GeometryType.Point.ToString() });
            }
            catch { throw; }
        }
        public int DeleteADBById(int systemId)
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

        public ADBMaster getADBDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
        #region Additional-Attributes
        public string GetOtherInfoADB(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }
    public class DACDB : Repository<CDBMaster>
    {

        public CDBMaster SaveEntityCDB(CDBMaster objCDBMaster, int userId)
        {
            try
            {
                var objCDB = repo.Get(x => x.system_id == objCDBMaster.system_id);
                if (objCDB != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objCDBMaster.modified_on, objCDB.modified_on, objCDBMaster.modified_by, objCDB.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objCDBMaster.objPM = objPageValidate;
                        return objCDBMaster;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(objCDB.system_id, objCDB.entityType, objCDB.latitude.ToString(), objCDB.longitude.ToString(), objCDB.region_id,objCDB.province_id);
                    if (geomresp.status != "OK")
                    {
                        objCDB.objPM = geomresp;
                        return objCDB;
                    }
                    // objCDB.network_id = objCDBMaster.network_id;
                    objCDB.cdb_name = objCDBMaster.cdb_name;
                    objCDB.pincode = objCDBMaster.pincode;
                    //objCDB.subarea_code = objCDBMaster.subarea_code;
                    objCDB.address = objCDBMaster.address;

                    objCDB.specification = objCDBMaster.specification;
                    objCDB.category = objCDBMaster.category;
                    objCDB.subcategory1 = objCDBMaster.subcategory1;
                    objCDB.subcategory2 = objCDBMaster.subcategory2;
                    objCDB.subcategory3 = objCDBMaster.subcategory3;
                    objCDB.item_code = objCDBMaster.item_code;
                    objCDB.vendor_id = objCDBMaster.vendor_id;
                    objCDB.type = objCDBMaster.type;
                    objCDB.brand = objCDBMaster.brand;
                    objCDB.model = objCDBMaster.model;
                    objCDB.construction = objCDBMaster.construction;
                    objCDB.activation = objCDBMaster.activation;
                    objCDB.accessibility = objCDBMaster.accessibility;
                    objCDB.is_servingdb = objCDBMaster.is_servingdb;
                    objCDB.modified_by = userId;
                    objCDB.modified_on = DateTimeHelper.Now;

                    objCDB.project_id = objCDBMaster.project_id ?? 0;
                    objCDB.planning_id = objCDBMaster.planning_id ?? 0;
                    objCDB.workorder_id = objCDBMaster.workorder_id ?? 0;
                    objCDB.purpose_id = objCDBMaster.purpose_id ?? 0;
                    

                    if (objCDBMaster.no_of_input_port != 0 && objCDBMaster.no_of_output_port != 0 && (objCDB.no_of_input_port != objCDBMaster.no_of_input_port || objCDB.no_of_output_port != objCDBMaster.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objCDB.system_id, objCDB.network_id, EntityType.CDB.ToString(), objCDBMaster.no_of_input_port + ":" + objCDBMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objCDBMaster.no_of_input_port, objCDBMaster.no_of_output_port, EntityType.CDB.ToString(), objCDB.system_id, objCDB.network_id, userId);
                    }
                    else if (objCDB.no_of_port != objCDBMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objCDB.system_id, EntityType.CDB.ToString(), objCDB.specification, objCDB.vendor_id, objCDB.item_code);
                        if (response.status)
                        {
                            objCDB.isPortConnected = response.status;
                            objCDB.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return objCDB;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objCDB.system_id, objCDB.network_id, EntityType.CDB.ToString(), objCDBMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objCDBMaster.no_of_port, objCDBMaster.no_of_port, EntityType.CDB.ToString(), objCDB.system_id, objCDB.network_id, userId);
                    }
                    objCDB.no_of_input_port = objCDBMaster.no_of_input_port;
                    objCDB.no_of_output_port = objCDBMaster.no_of_output_port;
                    objCDB.no_of_port = objCDBMaster.no_of_port;
                    objCDB.entity_category = objCDBMaster.entity_category;
                    objCDB.acquire_from = objCDBMaster.acquire_from;
                    objCDB.ownership_type = objCDBMaster.ownership_type;
                    objCDB.third_party_vendor_id = objCDBMaster.third_party_vendor_id;
                    objCDB.audit_item_master_id = objCDBMaster.audit_item_master_id;
                    objCDB.primary_pod_system_id = objCDBMaster.primary_pod_system_id;
                    objCDB.secondary_pod_system_id = objCDBMaster.secondary_pod_system_id;
                    objCDB.status_remark = objCDBMaster.status_remark;
                    objCDB.remarks = objCDBMaster.remarks;
                    objCDB.is_acquire_from = objCDBMaster.is_acquire_from;
                    objCDB.other_info = objCDBMaster.other_info; //for additional-attributes
                    objCDB.origin_from = objCDBMaster.origin_from;
                    objCDB.origin_ref_code = objCDBMaster.origin_ref_code;
                    objCDB.origin_ref_description = objCDBMaster.origin_ref_description;
                    objCDB.origin_ref_id = objCDBMaster.origin_ref_id;
                    objCDB.requested_by = objCDBMaster.requested_by;
                    objCDB.request_approved_by = objCDBMaster.request_approved_by;
                    objCDB.request_ref_id = objCDBMaster.request_ref_id;
                    objCDB.bom_sub_category = objCDBMaster.bom_sub_category;
                    // objCDB.served_by_ring= objCDBMaster.served_by_ring;
                    var CDBResp = repo.Update(objCDB);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(CDBResp.system_id, Models.EntityType.CDB.ToString(), CDBResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.CDB.ToString(), CDBResp.province_id);
                    return CDBResp;

                }
                else
                {
                    if (objCDBMaster.objIspEntityMap.floor_id > 0 && objCDBMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objCDBMaster.system_id,
                            entity_type = EntityType.CDB.ToString(),
                            floor_id = objCDBMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objCDBMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objCDBMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);// objMessage.message;
                            objCDBMaster.objPM = objPageValidate;
                            return objCDBMaster;
                        }
                    }

                    //CDBMaster objCDBItem1 = new CDBMaster();
                    objCDBMaster.created_by = userId;
                    objCDBMaster.created_on = DateTimeHelper.Now;
                    //objCDBMaster.status = "A";
                    //objCDBMaster.network_status = "P";
                    objCDBMaster.status = String.IsNullOrEmpty(objCDBMaster.status) ? "A" : objCDBMaster.status;
                    objCDBMaster.network_status = String.IsNullOrEmpty(objCDBMaster.network_status) ? "P" : objCDBMaster.network_status;
                    objCDBMaster.utilization = "L";
                    //if (objCDBMaster.parent_entity_type != EntityType.Province.ToString())
                    //{
                    //    objCDBMaster.parent_system_id = objCDBMaster.pSystemId;
                    //    objCDBMaster.parent_entity_type = objCDBMaster.pEntityType;
                    //    objCDBMaster.parent_network_id = objCDBMaster.pNetworkId;
                    //}
                    var resultItem = repo.Insert(objCDBMaster);

                    // transction code will be there
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.CDB.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.entity_category = objCDBMaster.entity_category;
                    geom.project_id = resultItem.project_id;
                    if (objCDBMaster.no_of_input_port != 0 && objCDBMaster.no_of_output_port != 0)
                    { geom.ports = objCDBMaster.no_of_input_port + ":" + objCDBMaster.no_of_output_port; }
                    else if (objCDBMaster.no_of_port != 0) { geom.ports = objCDBMaster.no_of_port.ToString(); }
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.CDB.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.CDB.ToString(), resultItem.province_id);
                    if (resultItem != null && objCDBMaster.no_of_input_port != 0 && objCDBMaster.no_of_output_port != 0)
                    {
                        var inputPort = objCDBMaster.no_of_input_port;
                        var outputPort = objCDBMaster.no_of_output_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.CDB.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }
                    else
                    {
                        var inputPort = objCDBMaster.no_of_port;
                        var outputPort = objCDBMaster.no_of_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.CDB.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }
                    if (objCDBMaster.pEntityType != null && objCDBMaster.pSystemId != 0 && objCDBMaster.pEntityType.ToUpper() != "STRUCTURE")
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.CDB.ToString();
                        objAsso.associated_system_id = resultItem.system_id;
                        objAsso.associated_network_id = resultItem.network_id;
                        objAsso.entity_network_id = objCDBMaster.pNetworkId;
                        objAsso.entity_system_id = objCDBMaster.pSystemId;
                        objAsso.entity_type = objCDBMaster.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(objAsso);
                    }
                    else
                    {
                        var responseAsso = DAIspEntityMapping.Instance.associateEntityInStructure(objCDBMaster.objIspEntityMap.shaft_id, objCDBMaster.objIspEntityMap.floor_id, objCDBMaster.system_id, EntityType.CDB.ToString(), objCDBMaster.parent_system_id, objCDBMaster.parent_entity_type, objCDBMaster.longitude + " " + objCDBMaster.latitude);
                        if (responseAsso.status)
                        {
                            objCDBMaster.isPortConnected = responseAsso.status;
                            objCDBMaster.message = Resources.Helper.MultilingualMessageConvert(responseAsso.message); //responseAsso.message;
                            return objCDBMaster;
                        }
                    }
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public int DeleteCDBById(int systemId)
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
        public CDBMaster getCDBDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }

        #region Additional-Attributes
        public string GetOtherInfoCDB(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }
    public class DABDB : Repository<BDBMaster>
    {

        public BDBMaster SaveEntityBDB(BDBMaster objBDBMaster, int userId)
        {
            try
            {
                var objBDB = repo.Get(x => x.system_id == objBDBMaster.system_id);
                if (objBDB != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objBDBMaster.modified_on, objBDB.modified_on, objBDBMaster.modified_by, objBDB.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objBDBMaster.objPM = objPageValidate;
                        return objBDBMaster;
                    }
                    var geomresp = new DAMisc().GetValidatePointGeometry(objBDB.system_id, objBDBMaster.entityType, objBDB.latitude.ToString(), objBDB.longitude.ToString(), objBDB.region_id, objBDB.province_id);
                    if (geomresp.status != "OK")
                    {
                        objBDB.objPM = geomresp;
                        return objBDB;
                    }
                    objBDB.bdb_name = objBDBMaster.bdb_name;
                    objBDB.pincode = objBDBMaster.pincode;
                    objBDB.address = objBDBMaster.address;
                    objBDB.specification = objBDBMaster.specification;
                    objBDB.category = objBDBMaster.category;
                    objBDB.subcategory1 = objBDBMaster.subcategory1;
                    objBDB.subcategory2 = objBDBMaster.subcategory2;
                    objBDB.subcategory3 = objBDBMaster.subcategory3;
                    objBDB.item_code = objBDBMaster.item_code;
                    objBDB.vendor_id = objBDBMaster.vendor_id;
                    objBDB.type = objBDBMaster.type;
                    objBDB.brand = objBDBMaster.brand;
                    objBDB.model = objBDBMaster.model;
                    objBDB.construction = objBDBMaster.construction;
                    objBDB.activation = objBDBMaster.activation;
                    objBDB.accessibility = objBDBMaster.accessibility;
                    objBDB.shaft_id = objBDBMaster.shaft_id ?? 0;
                    objBDB.floor_id = objBDBMaster.floor_id ?? 0;
                    objBDB.modified_by = userId;
                    objBDB.is_servingdb = objBDBMaster.is_servingdb;
                    objBDB.modified_on = DateTimeHelper.Now;
                    objBDB.project_id = objBDBMaster.project_id ?? 0;
                    objBDB.planning_id = objBDBMaster.planning_id ?? 0;
                    objBDB.workorder_id = objBDBMaster.workorder_id ?? 0;
                    objBDB.purpose_id = objBDBMaster.purpose_id ?? 0;
                    objBDB.parent_system_id = objBDBMaster.parent_system_id;
                    objBDB.parent_network_id = objBDBMaster.parent_network_id;
                    objBDB.parent_entity_type = objBDBMaster.parent_entity_type;
                    
                    if (objBDBMaster.no_of_input_port != 0 && objBDBMaster.no_of_output_port != 0 && (objBDB.no_of_input_port != objBDBMaster.no_of_input_port || objBDB.no_of_output_port != objBDBMaster.no_of_output_port))
                    {
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objBDB.system_id, objBDB.network_id, EntityType.BDB.ToString(), objBDBMaster.no_of_input_port + ":" + objBDBMaster.no_of_output_port);
                        new DAMisc().InsertPortInfo(objBDBMaster.no_of_input_port, objBDBMaster.no_of_output_port, EntityType.BDB.ToString(), objBDB.system_id, objBDB.network_id, userId);
                    }
                    else if (objBDB.no_of_port != objBDBMaster.no_of_port)
                    {
                        var response = new DAMisc().isPortConnected(objBDB.system_id, EntityType.BDB.ToString(), objBDB.specification, objBDB.vendor_id, objBDB.item_code);
                        if (response.status)
                        {
                            objBDB.isPortConnected = response.status;
                            objBDB.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                            return objBDB;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objBDB.system_id, objBDB.network_id, EntityType.BDB.ToString(), objBDBMaster.no_of_port.ToString());
                        new DAMisc().InsertPortInfo(objBDBMaster.no_of_port, objBDBMaster.no_of_port, EntityType.BDB.ToString(), objBDB.system_id, objBDB.network_id, userId);
                    }
                    objBDB.no_of_input_port = objBDBMaster.no_of_input_port;
                    objBDB.no_of_output_port = objBDBMaster.no_of_output_port;
                    objBDB.no_of_port = objBDBMaster.no_of_port;
                    objBDB.entity_category = objBDBMaster.entity_category;
                    objBDB.longitude = objBDBMaster.longitude;
                    objBDB.latitude = objBDBMaster.latitude;
                    objBDB.ownership_type = objBDBMaster.ownership_type;
                    objBDB.acquire_from = objBDBMaster.acquire_from;
                    objBDB.third_party_vendor_id = objBDBMaster.third_party_vendor_id;
                    objBDB.audit_item_master_id = objBDBMaster.audit_item_master_id;
                    objBDB.primary_pod_system_id = objBDBMaster.primary_pod_system_id;
                    objBDB.secondary_pod_system_id = objBDBMaster.secondary_pod_system_id;
                    objBDB.status_remark = objBDBMaster.status_remark;
                    objBDB.remarks = objBDBMaster.remarks;
                    objBDB.is_acquire_from = objBDBMaster.is_acquire_from;
                    var responseAsso = DAIspEntityMapping.Instance.associateEntityInStructure(objBDBMaster.objIspEntityMap.shaft_id, objBDBMaster.objIspEntityMap.floor_id, objBDBMaster.system_id, EntityType.BDB.ToString(), objBDBMaster.parent_system_id, objBDBMaster.parent_entity_type, objBDBMaster.longitude + " " + objBDBMaster.latitude);
                    if (responseAsso.status)
                    {
                        objBDB.isPortConnected = responseAsso.status;
                        objBDB.message = Resources.Helper.MultilingualMessageConvert(responseAsso.message); //responseAsso.message;
                        return objBDB;
                    }
                    objBDB.other_info = objBDBMaster.other_info; //for additional-attributes
                    objBDB.requested_by = objBDBMaster.requested_by;
                    objBDB.request_approved_by = objBDBMaster.request_approved_by;
                    objBDB.request_ref_id = objBDBMaster.request_ref_id;
                    objBDB.origin_ref_id = objBDBMaster.origin_ref_id;
                    objBDB.origin_ref_description = objBDBMaster.origin_ref_description;
                    objBDB.origin_from = objBDBMaster.origin_from;
                    objBDB.origin_ref_code = objBDBMaster.origin_ref_code;
                    objBDB.bom_sub_category=objBDBMaster.bom_sub_category;
                    ////  objBDB.served_by_ring=objBDBMaster.served_by_ring;
                    var result = repo.Update(objBDB);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.BDB.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.BDB.ToString(), result.province_id);

                    return result;
                }
                else
                {
                    if (objBDBMaster.objIspEntityMap.floor_id > 0 && objBDBMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objBDBMaster.system_id,
                            entity_type = EntityType.BDB.ToString(),
                            floor_id = objBDBMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objBDBMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objBDBMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objBDBMaster.objPM = objPageValidate;
                            return objBDBMaster;
                        }
                    }


                    objBDBMaster.created_by = userId;
                    objBDBMaster.created_on = DateTimeHelper.Now;
                    //objBDBMaster.status = "A";
                    //objBDBMaster.network_status = "P";
                    objBDBMaster.status = String.IsNullOrEmpty(objBDBMaster.status) ? "A" : objBDBMaster.status;
                    objBDBMaster.network_status = String.IsNullOrEmpty(objBDBMaster.network_status) ? "P" : objBDBMaster.network_status;
                    objBDBMaster.utilization = "L";
                    if (objBDBMaster.objIspEntityMap.structure_id != 0)
                    {
                        objBDBMaster.parent_system_id = Convert.ToInt32(objBDBMaster.objIspEntityMap.structure_id);
                        objBDBMaster.parent_entity_type = EntityType.Structure.ToString();
                    }
                    var resultItem = repo.Insert(objBDBMaster);
                  
                    // transaction handling code will be there...
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.BDB.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.entity_category = objBDBMaster.entity_category;
                    geom.project_id = resultItem.project_id;
                    if (objBDBMaster.no_of_input_port != 0 && objBDBMaster.no_of_output_port != 0)
                    { geom.ports = objBDBMaster.no_of_input_port + ":" + objBDBMaster.no_of_output_port; }
                    else if (objBDBMaster.no_of_port != 0) { geom.ports = objBDBMaster.no_of_port.ToString(); }
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.BDB.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.BDB.ToString(), resultItem.province_id);
                    if (objBDBMaster.pEntityType != null && objBDBMaster.pSystemId != 0 && objBDBMaster.pEntityType.ToUpper() != "STRUCTURE")
                    {
                        AssociateEntity objAsso = new AssociateEntity();
                        objAsso.associated_entity_type = EntityType.BDB.ToString();
                        objAsso.associated_system_id = resultItem.system_id;
                        objAsso.associated_network_id = resultItem.network_id;
                        objAsso.entity_network_id = objBDBMaster.pNetworkId;
                        objAsso.entity_system_id = objBDBMaster.pSystemId;
                        objAsso.entity_type = objBDBMaster.pEntityType;
                        objAsso.created_on = DateTimeHelper.Now;
                        objAsso.created_by = userId;
                        new DAAssociateEntity().SaveAssociation(objAsso);
                    }
                    else
                    {
                        DAIspEntityMapping.Instance.associateEntityInStructure(objBDBMaster.objIspEntityMap.shaft_id, objBDBMaster.objIspEntityMap.floor_id, objBDBMaster.system_id, EntityType.BDB.ToString(), objBDBMaster.parent_system_id, objBDBMaster.parent_entity_type, objBDBMaster.longitude + " " + objBDBMaster.latitude);
                    }
                    if (resultItem != null && objBDBMaster.no_of_input_port != 0 && objBDBMaster.no_of_output_port != 0)
                    {
                        var inputPort = objBDBMaster.no_of_input_port;
                        var outputPort = objBDBMaster.no_of_output_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.BDB.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }
                    else
                    {
                        var inputPort = objBDBMaster.no_of_port;
                        var outputPort = objBDBMaster.no_of_port;
                        new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.BDB.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public int DeleteBDBById(int systemId)
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

        public List<ShaftFloorList> GetShaftFloorByStrucId(int structureId)
        {
            try
            {
                return repo.ExecuteProcedure<ShaftFloorList>("fn_get_isp_records", new { structId = structureId }).ToList();
            }
            catch { throw; }
        }
        public BDBMaster getBDBDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
        
        #region Additional-Attributes
        public string GetOtherInfoBDB(int systemId)
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
