using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
   public class DAFault : Repository<Fault>
    {
        DAFault()
        {

        }
        private static DAFault objFault = null;
        private static readonly object lockObject = new object();
        public static DAFault Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objFault == null)
                    {
                        objFault = new DAFault();
                    }
                }
                return objFault;
            }
        }

        public Fault SaveFault(Fault faultInfo, int userId)
        {
            try 
            {   
                var objFault = repo.Get(u => u.system_id == faultInfo.system_id);
                PageMessage objPageValidate = new PageMessage();
                if (objFault != null)
                {
                    
                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = faultInfo.system_id,
                        entity_type = EntityType.Fault.ToString(), 
                    }, faultInfo.system_id == 0);
                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message); //objMessage.message;
                        faultInfo.objPM = objPageValidate;
                        return faultInfo;
                    }
                    objFault.address = faultInfo.address;
                    objFault.fault_reason = faultInfo.fault_reason;
                    objFault.fault_ticket_id = faultInfo.fault_ticket_id;
                    objFault.fault_ticket_type = faultInfo.fault_ticket_type;
                    objFault.fault_type = faultInfo.fault_type;
                    objFault.remarks = faultInfo.remarks;
                    objFault.fault_entity_network_id = faultInfo.fault_entity_network_id;
                    objFault.fault_entity_system_id = faultInfo.fault_entity_system_id;
                    objFault.fault_entity_type = faultInfo.fault_entity_type;
                    objFault.select_entity = faultInfo.select_entity;
                    objFault.business_type = faultInfo.business_type;
                    objFault.latitude = faultInfo.latitude;
                    objFault.longitude = faultInfo.longitude;
                   // objFault.status = faultInfo.status;
                    objFault.fault_status = faultInfo.fault_status;
                    objFault.network_status = NetworkStatus.P.ToString();
                    objFault.modified_on = DateTimeHelper.Now;
                    objFault.modified_by = userId;
                    objFault.source_ref_description = faultInfo.source_ref_description;
                    objFault.source_ref_id = faultInfo.source_ref_id;
                    objFault.source_ref_type = faultInfo.source_ref_type;
                    objFault.primary_pod_system_id = faultInfo.primary_pod_system_id;
                    objFault.secondary_pod_system_id = faultInfo.secondary_pod_system_id;
                    objFault.status_remark = faultInfo.status_remark;
                    objFault.requested_by = faultInfo.requested_by;
                    objFault.request_approved_by = faultInfo.request_approved_by;
                    objFault.request_ref_id = faultInfo.request_ref_id;
                    objFault.origin_ref_id = faultInfo.origin_ref_id;
                    objFault.origin_ref_description = faultInfo.origin_ref_description;
                    objFault.origin_from = faultInfo.origin_from;
                    objFault.origin_ref_code = faultInfo.origin_ref_code;
                    var response = repo.Update(objFault);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.Fault.ToString(), response.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Fault.ToString(), response.province_id);
                    /* Geom update is now happening via trigger*/
                    // Update Geom  
                    //EditGeomIn geomObj = new EditGeomIn();
                    //geomObj.entityType = EntityType.Fault.ToString();
                    //geomObj.geomType = GeometryType.Point.ToString();
                    //geomObj.isExisting = true;
                    //geomObj.longLat = response.longitude + " " + response.latitude;
                    //geomObj.systemId = response.system_id;
                    //geomObj.networkStatus = response.network_status;
                    //geomObj.userId = userId; 
                    //DASaveEntityGeometry.Instance.EditEntityGeometry(geomObj);
                    return response;
                }
                else
                {
                    //faultInfo.status = "A";
                    //faultInfo.network_status = NetworkStatus.P.ToString();
                    faultInfo.status = String.IsNullOrEmpty(faultInfo.status) ? "A" : faultInfo.status;
                    faultInfo.network_status = String.IsNullOrEmpty(faultInfo.network_status) ? NetworkStatus.P.ToString() : faultInfo.network_status;
                    faultInfo.created_on = DateTimeHelper.Now;
                    faultInfo.created_by = userId; 
                    var response = repo.Insert(faultInfo);
                    //  Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = response.system_id;
                    geom.longLat = response.longitude + " " + response.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Fault.ToString();
                    geom.commonName = response.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(response.system_id, Models.EntityType.Fault.ToString(), response.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Fault.ToString(), response.province_id);
                    return response;
                }
            }
            catch { throw; }
           
    }
        public List<Fault> GetFaultTypeList(string searchText)
        {
            try
            {
                //return repo.GetAll(x => x.site_vendor.ToUpper().Contains(searchText.ToUpper())).Take(10).Distinct().ToList();
                return repo.ExecuteProcedure<Fault>("fn_get_fault_type_list", new { searchtext = searchText }, true);
            }
            catch { throw; } 
        }
        public Fault GetFaultById(int systemId)
        {
            try
            {
                return repo.Get(x => x.system_id == systemId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public FaultDetails GetFaultID(string parentCode)
        {
            try
            {
                try
                {
                    var result = repo.ExecuteProcedure<FaultDetails>("fn_get_fault_id", new { p_parentCode = parentCode});
                    return result != null && result.Count > 0 ? result[0] : new FaultDetails();
                }
                catch { throw; }
            }
            catch (Exception)
            {

                throw;
            }
        }
        }
     
}
