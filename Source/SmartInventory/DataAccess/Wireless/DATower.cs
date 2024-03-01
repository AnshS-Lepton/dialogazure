using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
namespace DataAccess
{
    public class DATower : Repository<TowerMaster>
    {
        public TowerMaster Save(TowerMaster objTowerMaster, int UserId)
        {
            try
            {
                var objTowerItem = repo.Get(x => x.system_id == objTowerMaster.system_id);
                if (objTowerItem != null)
                {
                    PageMessage objPageValidate = new PageMessage();
                    objPageValidate = DAUtility.ValidateModifiedDate(objTowerMaster.modified_on, objTowerItem.modified_on, objTowerMaster.modified_by, objTowerItem.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objTowerMaster.objPM = objPageValidate;
                        return objTowerMaster;
                    }

                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = objTowerMaster.system_id,
                        entity_type = EntityType.Tower.ToString(),
                        network_id = objTowerMaster.network_id,
                        vendor_id = objTowerMaster.vendor_id,
                        specification = objTowerMaster.specification,
                        item_code = objTowerMaster.item_code,
                        //no_of_input_port = objTowerMaster.no_of_input_port,
                        //no_of_output_port = objTowerMaster.no_of_output_port,
                        //no_of_port = objTowerMaster.no_of_port
                    }, false);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);//objMessage.message;
                        objTowerMaster.objPM = objPageValidate;
                        return objTowerMaster;
                    }
                    objTowerItem.network_name = objTowerMaster.network_name;

                    objTowerItem.elevation = objTowerMaster.elevation;
                    objTowerItem.tower_height = objTowerMaster.tower_height;
                    objTowerItem.tenancy = objTowerMaster.tenancy;
                    objTowerItem.no_of_sectors = objTowerMaster.no_of_sectors;
                    objTowerItem.ownership_type = objTowerMaster.ownership_type;
                    objTowerItem.operator_name = objTowerMaster.operator_name;
                    objTowerItem.address = objTowerMaster.address;

                    objTowerItem.specification = objTowerMaster.specification;
                    objTowerItem.category = objTowerMaster.category;
                    objTowerItem.subcategory1 = objTowerMaster.subcategory1;
                    objTowerItem.subcategory2 = objTowerMaster.subcategory2;
                    objTowerItem.subcategory3 = objTowerMaster.subcategory3;
                    objTowerItem.item_code = objTowerMaster.item_code;
                    objTowerItem.vendor_id = objTowerMaster.vendor_id;
                    objTowerItem.type = objTowerMaster.type;
                    objTowerItem.brand = objTowerMaster.brand;
                    objTowerItem.model = objTowerMaster.model;
                    objTowerItem.construction = objTowerMaster.construction;
                    objTowerItem.activation = objTowerMaster.activation;
                    objTowerItem.accessibility = objTowerMaster.accessibility;
                    objTowerItem.modified_by = UserId;
                    objTowerItem.modified_on = DateTimeHelper.Now;

                    objTowerItem.project_id = objTowerMaster.project_id ?? 0;
                    objTowerItem.planning_id = objTowerMaster.planning_id ?? 0;
                    objTowerItem.workorder_id = objTowerMaster.workorder_id ?? 0;
                    objTowerItem.purpose_id = objTowerMaster.purpose_id ?? 0;

                    objTowerItem.ownership_type = objTowerMaster.ownership_type;
                    objTowerItem.third_party_vendor_id = objTowerMaster.third_party_vendor_id;
                    objTowerItem.acquire_from = objTowerMaster.acquire_from;

                    objTowerItem.network_type = objTowerMaster.network_type; 
                    objTowerItem.remark = objTowerMaster.remark;

					objTowerItem.installation = objTowerMaster.installation;
					objTowerItem.installation_company = objTowerMaster.installation_company;
					objTowerItem.installation_number = objTowerMaster.installation_number;
					objTowerItem.installation_technician = objTowerMaster.installation_technician;
					objTowerItem.installation_year = objTowerMaster.installation_year;
					objTowerItem.production_year = objTowerMaster.production_year;
                    objTowerItem.status_remark = objTowerMaster.status_remark;
                    objTowerItem.requested_by = objTowerMaster.requested_by;
                    objTowerItem.request_approved_by = objTowerMaster.request_approved_by;
                    objTowerItem.request_ref_id = objTowerMaster.request_ref_id;
                    objTowerItem.origin_ref_id = objTowerMaster.origin_ref_id;
                    objTowerItem.origin_ref_description = objTowerMaster.origin_ref_description;
                    objTowerItem.origin_from = objTowerMaster.origin_from;
                    objTowerItem.origin_ref_code = objTowerMaster.origin_ref_code;
                    objTowerItem.bom_sub_category=objTowerMaster.bom_sub_category;
                    //  objTowerItem.served_by_ring = objTowerMaster.served_by_ring;

                    //if (objTowerMaster.no_of_input_port != 0 && objTowerMaster.no_of_output_port != 0 && (objTowerItem.no_of_input_port != objTowerMaster.no_of_input_port || objTowerItem.no_of_output_port != objTowerMaster.no_of_output_port))
                    //{
                    //    DASaveEntityGeometry.Instance.UpdatePortInGeom(objTowerItem.system_id, objTowerItem.network_id, EntityType.FMS.ToString(), objTowerMaster.no_of_input_port + ":" + objTowerMaster.no_of_output_port);
                    //    new DAMisc().InsertPortInfo(objTowerMaster.no_of_input_port, objTowerMaster.no_of_output_port, EntityType.FMS.ToString(), objTowerMaster.system_id, objTowerMaster.network_id, userId);
                    //}
                    //else if (objTowerItem.no_of_port != objTowerMaster.no_of_port)
                    //{
                    //    var response = new DAMisc().isPortConnected(objTowerItem.system_id, EntityType.FMS.ToString());
                    //    if (response.status)
                    //    {
                    //        objTowerItem.isPortConnected = response.status;
                    //        objTowerItem.message = Resources.Helper.MultilingualMessageConvert(response.message); //response.message;
                    //        return objTowerItem;
                    //    }
                    //    DASaveEntityGeometry.Instance.UpdatePortInGeom(objTowerItem.system_id, objTowerItem.network_id, EntityType.FMS.ToString(), objTowerMaster.no_of_port.ToString());
                    //    new DAMisc().InsertPortInfo(objTowerMaster.no_of_port, objTowerMaster.no_of_port, EntityType.FMS.ToString(), objTowerMaster.system_id, objTowerMaster.network_id, userId);
                    //}
                    //objTowerItem.no_of_input_port = objTowerMaster.no_of_input_port;
                    //objTowerItem.no_of_output_port = objTowerMaster.no_of_output_port;
                    //objTowerItem.no_of_port = objTowerMaster.no_of_port;


                    var TowerResp =  repo.Update(objTowerItem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(TowerResp.system_id, Models.EntityType.Tower.ToString(), TowerResp.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Tower.ToString(), TowerResp.province_id);
                    return TowerResp;

                }
                else
                {
                    //if (objTowerMaster.objIspEntityMap.floor_id > 0 && objTowerMaster.objIspEntityMap.shaft_id > 0)
                    //{
                    //    PageMessage objPageValidate = new PageMessage();
                    //    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    //    {
                    //        system_id = objTowerMaster.system_id,
                    //        entity_type = EntityType.FMS.ToString(),
                    //        floor_id = objTowerMaster.objIspEntityMap.floor_id ?? 0,
                    //        shaft_id = objTowerMaster.objIspEntityMap.shaft_id ?? 0,
                    //        parent_entity_type = objTowerMaster.parent_entity_type
                    //    }, true);

                    //    if (!string.IsNullOrEmpty(objMessage.message))
                    //    {
                    //        objPageValidate.status = ResponseStatus.FAILED.ToString();
                    //        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                    //        objTowerMaster.objPM = objPageValidate;
                    //        return objTowerMaster;
                    //    }
                    //}

                    objTowerMaster.created_by = UserId;
                    objTowerMaster.created_on = DateTimeHelper.Now;
                    objTowerMaster.status = "A";
                    objTowerMaster.network_status = "P";
                    //objTowerMaster.utilization = "L";
                    var response = repo.Insert(objTowerMaster);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.Tower.ToString(), response.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Tower.ToString(), response.province_id);
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = response.system_id;
                    geom.longLat = response.longitude + " " + response.latitude;
                    geom.userId = UserId;
                    geom.entityType = EntityType.Tower.ToString();
                    geom.commonName = response.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.project_id = response.project_id;
                    //if (objTowerMaster.no_of_input_port != 0 && objTowerMaster.no_of_output_port != 0)
                    //{ geom.ports = objTowerMaster.no_of_input_port + ":" + objTowerMaster.no_of_output_port; }
                    //else if (objTowerMaster.no_of_port != 0) { geom.ports = objTowerMaster.no_of_port.ToString(); }
                    //string chkGeomInsert = BASaveEntityGeometry.Instance.SaveEntityGeometry(geom);
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    //DAIspEntityMapping.Instance.associateEntityInStructure(objTowerMaster.objIspEntityMap.shaft_id, objTowerMaster.objIspEntityMap.floor_id, objTowerMaster.system_id, EntityType.FMS.ToString(), objTowerMaster.pSystemId, objTowerMaster.pEntityType, objTowerMaster.longitude + " " + objTowerMaster.latitude);
                    //var ispMapping = DAISPEntityMapping.Instance.getMappingByEntityId(objTowerMaster.parent_system_id, objTowerMaster.parent_entity_type);
                    //if (ispMapping != null && ispMapping.structure_id != 0 && ispMapping.floor_id != 0)
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.structure_id = ispMapping.structure_id;
                    //    objMapping.floor_id = ispMapping.floor_id ?? 0;
                    //    objMapping.shaft_id = ispMapping.shaft_id ?? 0;
                    //    objMapping.entity_id = objTowerMaster.system_id;
                    //    objMapping.entity_type = EntityType.FMS.ToString();
                    //    objMapping.parent_id = ispMapping.id;
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    //else
                    //{
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.entity_id = objTowerMaster.system_id;
                    //    objMapping.entity_type = EntityType.FMS.ToString();
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //}
                    //if (response != null && objTowerMaster.no_of_input_port != 0 && objTowerMaster.no_of_output_port != 0)
                    //{
                    //    var inputPort = objTowerMaster.no_of_input_port;
                    //    var outputPort = objTowerMaster.no_of_output_port;
                    //    new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.FMS.ToString(), response.system_id, response.network_id, UserId);
                    //}
                    //else
                    //{
                    //    var inputPort = objTowerMaster.no_of_port;
                    //    var outputPort = objTowerMaster.no_of_port;
                    //    new DAMisc().InsertPortInfo(inputPort, outputPort, EntityType.FMS.ToString(), response.system_id, response.network_id, UserId);
                    //}
                    return response;
                }
            }
            catch { throw; }
        }
        public List<AssociatedPop> GetPopInBuffer(int towerId, int distance)
        {
            try
            {
                return repo.ExecuteProcedure<AssociatedPop>("fn_get_pop_in_buffer", new { p_systemid = towerId, p_buffer = distance });
            }
            catch { throw; }
        }

    }

    public class DATowerAssociatedPop : Repository<TowerAssociatedPop>
    {

        public TowerAssociatedPop SaveAssociatedPop(int popId, int towerId, int UserId)
        {
            try
            {
                TowerAssociatedPop objTowerAssPop = new TowerAssociatedPop();
                objTowerAssPop.tower_id = towerId;
                objTowerAssPop.pop_id = popId;
                objTowerAssPop.created_by = UserId;
                objTowerAssPop.created_on = DateTimeHelper.Now;
                var response = repo.Insert(objTowerAssPop);
                return response;
            }
            catch { throw; }
        }
        public ErrorMessage CheckDuplicate(int popId, int towerId)
        {
            ErrorMessage error = new ErrorMessage();
            var data = repo.Get(m => m.pop_id == popId && m.tower_id == towerId);
            if (data == null)
            {
                error.status = StatusCodes.OK.ToString();
                error.is_valid = true;
                error.error_msg =Resources.Resources.SI_OSP_GBL_GBL_FRM_087;
            }
            else
            {
                error.status = StatusCodes.DUPLICATE_EXIST.ToString();
                error.error_msg = Resources.Resources.SI_OSP_GBL_GBL_FRM_093;
            }
            return error;
        }
        public List<TowerAssociatedPopView> GetAssociatedPop(int towerId)
        {
            try
            {
                return repo.ExecuteProcedure<TowerAssociatedPopView>("fn_get_associated_pop_list", new { towerId = towerId });
            }
            catch { throw; }
        }

        public ErrorMessage DeAssociatePop(int popId, int towerId)
        {
            try
            {
                ErrorMessage error = new ErrorMessage();
                var objAssociatePop = repo.Get(x => x.pop_id == popId&&x.tower_id==towerId);
                if (objAssociatePop != null)
                {
                    var result = repo.Delete(objAssociatePop.system_id);
                    if (result == 1)
                    {
                        error.status = StatusCodes.OK.ToString();
                        error.is_valid = true;
                        error.error_msg = Resources.Resources.SI_OSP_GBL_GBL_FRM_095;
                    }
                }
                else
                {
                    error.status = StatusCodes.FAILED.ToString();
                    error.error_msg = Resources.Resources.SI_OSP_GBL_GBL_RPT_001;
                }
                return error;
            }
            catch { throw; }
        }
    }
}
