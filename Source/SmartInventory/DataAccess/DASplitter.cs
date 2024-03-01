using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DASplitter : Repository<SplitterMaster>
    {
        public SplitterMaster SaveSplitterEntity(SplitterMaster objSplitterMaster, int userId)
        {
            try
            {
                var resultItem = new SplitterMaster();
                var objSplitter = repo.Get(x => x.system_id == objSplitterMaster.system_id);
                if (objSplitter != null)
                {
                    PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objSplitterMaster.modified_on, objSplitter.modified_on, objSplitterMaster.modified_by,objSplitter.modified_by);
                    if (objPageValidate.message != null)
                    {
                        objSplitterMaster.objPM = objPageValidate;
                        return objSplitterMaster;
                    }

                    objSplitter.splitter_name = objSplitterMaster.splitter_name;
                    objSplitter.address = objSplitterMaster.address;
                    objSplitter.specification = objSplitterMaster.specification;
                    objSplitter.category = objSplitterMaster.category;
                    objSplitter.subcategory1 = objSplitterMaster.subcategory1;
                    objSplitter.subcategory2 = objSplitterMaster.subcategory2;
                    objSplitter.subcategory3 = objSplitterMaster.subcategory3;
                    objSplitter.item_code = objSplitterMaster.item_code;
                    objSplitter.vendor_id = objSplitterMaster.vendor_id;
                    objSplitter.type = objSplitterMaster.type;
                    objSplitter.brand = objSplitterMaster.brand;
                    objSplitter.model = objSplitterMaster.model;
                    objSplitter.construction = objSplitterMaster.construction;
                    objSplitter.activation = objSplitterMaster.activation;
                    objSplitter.accessibility = objSplitterMaster.accessibility;
                    objSplitter.modified_by = userId;
                    objSplitter.modified_on = DateTimeHelper.Now;

                    objSplitter.project_id = objSplitterMaster.project_id ?? 0;
                    objSplitter.planning_id = objSplitterMaster.planning_id ?? 0;
                    objSplitter.workorder_id = objSplitterMaster.workorder_id ?? 0;
                    objSplitter.purpose_id = objSplitterMaster.purpose_id ?? 0;
                    objSplitter.splitter_type = objSplitterMaster.splitter_type;
                    objSplitter.ownership_type = objSplitterMaster.ownership_type;
                    objSplitter.acquire_from = objSplitterMaster.acquire_from;
                    objSplitter.third_party_vendor_id = objSplitterMaster.third_party_vendor_id;
                    objSplitter.audit_item_master_id = objSplitterMaster.audit_item_master_id;
                    objSplitter.primary_pod_system_id = objSplitterMaster.primary_pod_system_id;
                    objSplitter.secondary_pod_system_id = objSplitterMaster.secondary_pod_system_id;
                    objSplitter.status_remark = objSplitterMaster.status_remark;
                    objSplitter.remarks = objSplitterMaster.remarks;
                    objSplitter.is_acquire_from = objSplitterMaster.is_acquire_from;
                    if (objSplitter.splitter_ratio != objSplitterMaster.splitter_ratio)
                    {
                        var response = new DAMisc().isPortConnected(objSplitter.system_id, Models.EntityType.Splitter.ToString(),objSplitter.specification, objSplitter.vendor_id, objSplitter.item_code);
                        if (response.status)
                        {
                            objSplitter.isPortConnected = response.status;
                            objSplitter.message = Resources.Helper.MultilingualMessageConvert(response.message);//response.message;
                            return objSplitter;
                        }
                        DASaveEntityGeometry.Instance.UpdatePortInGeom(objSplitterMaster.system_id, objSplitterMaster.network_id, Models.EntityType.Splitter.ToString(), objSplitterMaster.splitter_ratio);
                        var inputPort = Convert.ToInt32(objSplitterMaster.splitter_ratio.Split(':')[0]);
                        var outputPort = Convert.ToInt32(objSplitterMaster.splitter_ratio.Split(':')[1]);
                        new DAMisc().InsertPortInfo(inputPort, outputPort, Models.EntityType.Splitter.ToString(), objSplitter.system_id, objSplitter.network_id, userId);
                    }
                    objSplitter.splitter_ratio = objSplitterMaster.splitter_ratio;
                    objSplitter.other_info = objSplitterMaster.other_info;
                    objSplitter.requested_by = objSplitterMaster.requested_by;
                    objSplitter.request_approved_by = objSplitterMaster.request_approved_by;
                    objSplitter.request_ref_id = objSplitterMaster.request_ref_id;
                    objSplitter.origin_ref_id = objSplitterMaster.origin_ref_id;
                    objSplitter.origin_ref_description = objSplitterMaster.origin_ref_description;
                    objSplitter.origin_from = objSplitterMaster.origin_from;
                    objSplitter.origin_ref_code = objSplitterMaster.origin_ref_code;
                    //  objSplitter.served_by_ring = objSplitterMaster.served_by_ring;
                    objSplitter.bom_sub_category = objSplitterMaster.bom_sub_category;
                    var result = repo.Update(objSplitter);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Splitter.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Splitter.ToString(), result.province_id);
                    return result;


                }
                else
                {
                    if (objSplitterMaster.objIspEntityMap.floor_id > 0 && objSplitterMaster.objIspEntityMap.shaft_id > 0)
                    {
                        PageMessage objPageValidate = new PageMessage();
                        DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                        {
                            system_id = objSplitterMaster.system_id,
                            entity_type = Models.EntityType.Splitter.ToString(),
                            floor_id = objSplitterMaster.objIspEntityMap.floor_id ?? 0,
                            shaft_id = objSplitterMaster.objIspEntityMap.shaft_id ?? 0,
                            parent_entity_type = objSplitterMaster.parent_entity_type
                        }, true);

                        if (!string.IsNullOrEmpty(objMessage.message))
                        {
                            objPageValidate.status = ResponseStatus.FAILED.ToString();
                            objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                            objSplitterMaster.objPM = objPageValidate;
                            return objSplitterMaster;
                        }
                    }

                    objSplitterMaster.created_by = userId;
                    objSplitterMaster.created_on = DateTimeHelper.Now;
                    //objSplitterMaster.status = "A";
                    //objSplitterMaster.network_status = "P";
                    objSplitterMaster.status = String.IsNullOrEmpty(objSplitterMaster.status) ? "A" : objSplitterMaster.status;
                    objSplitterMaster.network_status = String.IsNullOrEmpty(objSplitterMaster.network_status) ? "P" : objSplitterMaster.network_status;
                    objSplitterMaster.utilization = "L";
                    resultItem = repo.Insert(objSplitterMaster);
                    // save grant parent details
                    
                    UpdateSplitterGrantDetails(resultItem.system_id, Models.EntityType.Splitter.ToString());

                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = Models.EntityType.Splitter.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.ports = objSplitterMaster.splitter_ratio.ToString();
                    geom.entity_category = objSplitterMaster.splitter_type;
                    geom.project_id = resultItem.project_id;
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Splitter.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Splitter.ToString(), resultItem.province_id);
                    DAIspEntityMapping.Instance.associateEntityInStructure(objSplitterMaster.objIspEntityMap.shaft_id, objSplitterMaster.objIspEntityMap.floor_id, objSplitterMaster.system_id, Models.EntityType.Splitter.ToString(), objSplitterMaster.parent_system_id, objSplitterMaster.parent_entity_type, objSplitterMaster.longitude + " " + objSplitterMaster.latitude);

                    //insert port information...
                    // NEED TO ADD A CONDITION NOT TO INSERT/UPPDATE PORT INFO IF STATUS IS USED..
                    if (resultItem != null && resultItem.splitter_ratio != null && resultItem.splitter_ratio != "")
                    {
                        var inputPort = Convert.ToInt32(objSplitterMaster.splitter_ratio.Split(':')[0]);
                        var outputPort = Convert.ToInt32(objSplitterMaster.splitter_ratio.Split(':')[1]);
                        new DAMisc().InsertPortInfo(inputPort, outputPort, Models.EntityType.Splitter.ToString(), resultItem.system_id, resultItem.network_id, userId);
                    }

                    //var bdbParentDetails = new DAMisc().GetEntityDetailById<BDBMaster>(objSplitterMaster.parent_system_id, EntityType.BDB);
                    //if (bdbParentDetails != null && bdbParentDetails.parent_entity_type == EntityType.Structure.ToString())
                    //{
                    //    var mappingDetails = DAIspEntityMapping.Instance.GetIspEntityMapByStrucId(bdbParentDetails.parent_system_id, bdbParentDetails.system_id, EntityType.BDB.ToString());
                    //    if (mappingDetails != null)
                    //    {
                    //        IspEntityMapping objMapping = new IspEntityMapping();
                    //        objMapping.id = 0;
                    //        objMapping.structure_id = mappingDetails.structure_id;
                    //        objMapping.floor_id = mappingDetails.floor_id ?? 0;
                    //        objMapping.shaft_id = mappingDetails.shaft_id ?? 0;
                    //        objMapping.entity_id = resultItem.system_id;
                    //        objMapping.entity_type = EntityType.Splitter.ToString();
                    //        objMapping.parent_id = mappingDetails.id;
                    //        var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //    }
                    //}
                }
                return resultItem;
            }
            catch { throw; }
        }
        public int DeleteSplitterById(int systemId)
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
        public SplitterMaster getSplitterDetails(int systemId)
        {
            try
            {
                return repo.GetById(m => m.system_id == systemId);
            }
            catch { throw; }
        }
		public bool VerifiedMeterReading(int systemId, double meterReading)
		{

			var objSplitter = repo.GetById(m => m.system_id == systemId);
			if (objSplitter != null)
			{
				objSplitter.power_meter_reading = meterReading;
				objSplitter.is_meter_reading_verified = true;
				repo.Update(objSplitter);
				return true;
			}
			else
			{
				return false;
			}
		}
		#region Additional-Attributes
		public string GetOtherInfoSplitter(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion

        public void UpdateSplitterGrantDetails(int systemId, string entityType)
        {
            try
            {

                 repo.ExecuteProcedure<string>("fn_update_splitter_grant_parent_details", new { p_system_id = systemId, p_entity_type = entityType }, true).FirstOrDefault();
            }
            catch { throw; }
        }
    }
}
