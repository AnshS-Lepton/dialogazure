using DataAccess;
using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess
{
    public class DACustomer : Repository<Customer>
    {
        public Customer SaveCustomer(Customer objCustomer, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objCustomer.system_id);
                if (objitem != null)
                {
                    //PageMessage objPageValidate = DAUtility.ValidateModifiedDate(objCustomer.modified_on, objitem.modified_on, objCustomer.modified_by, objitem.modified_by);
                    //if (objPageValidate.message != null)
                    //{
                    //    objCustomer.objPM = objPageValidate;
                    //    return objCustomer;
                    //}
                    if (!string.IsNullOrEmpty(objCustomer.lmc_type))
                    {
                        objitem.agl = objCustomer.agl;
                        objitem.cable_entry_point = objCustomer.cable_entry_point;
                        objitem.lmc_type = objCustomer.lmc_type;
                        objitem.customer_site_id = objCustomer.customer_site_id;
                        objitem.customer_name = objCustomer.customer_name;
                        objitem.modified_by = userId;
                        objitem.modified_on = DateTimeHelper.Now;
                        objitem.region_id = objCustomer.region_id;
                        objitem.province_id = objCustomer.province_id;
                        objitem.parent_system_id = objCustomer.parent_system_id;
                        objitem.parent_entity_type = objCustomer.parent_entity_type;
                        objitem.parent_network_id = objCustomer.parent_network_id;
                        objitem.floor_id = objCustomer.floor_id;
                        objitem.structure_id = objCustomer.structure_id;
                        objitem.structure_name = objCustomer.structure_name;
                        objitem.cluster_id = objCustomer.cluster_id;
                        objitem.cluster_name = objCustomer.cluster_name; 
                        objitem.customer_name = objCustomer.customer_name;
                        objitem.customer_site_id = objCustomer.customer_site_id;
                        objitem.customer_area = objCustomer.customer_area; 
                        objitem.opco = objCustomer.opco;
                        objitem.electrical_meter_type = objCustomer.electrical_meter_type;
                        objitem.rtn_name = objCustomer.rtn_name;
                        objitem.small_cell_installed = objCustomer.small_cell_installed; 
                        objitem.is_power_back_up_available = objCustomer.is_power_back_up_available;
                        objitem.power_back_up_capacity = objCustomer.power_back_up_capacity; 
                        objitem.paf_no = objCustomer.paf_no;
                        objitem.paf_signing_date = objCustomer.paf_signing_date;
                        objitem.paf_expiry_date = objCustomer.paf_expiry_date; 
                        objitem.remote_pop = objCustomer.remote_pop;
                        objitem.order_tenure = objCustomer.order_tenure;
                        objitem.site_details = objCustomer.site_details;
                        objitem.po_number = objCustomer.po_number;
                        objitem.po_issue_date = objCustomer.po_issue_date;
                        objitem.po_expiry_date = objCustomer.po_expiry_date;
                        objitem.rfai_date = objCustomer.rfai_date;
                        objitem.rfs_date = objCustomer.rfs_date;
                        objitem.contract_end_date = objCustomer.contract_end_date;
                        objitem.email_id = objCustomer.email_id;
                        objitem.mobile_no = objCustomer.mobile_no;
                        objitem.phone_no = objCustomer.phone_no;
                        objitem.gis_design_id = objCustomer.gis_design_id;
                    }
                    else
                    {
                        objitem.customer_name = objCustomer.customer_name;
                        objitem.pin_code = objCustomer.pin_code;
                        objitem.address = objCustomer.address;
                        objitem.activation_date = objCustomer.activation_date;
                        objitem.deactivation_date = objCustomer.deactivation_date;
                        objitem.activation_status = objCustomer.activation_status;
                        objitem.customer_type = objCustomer.customer_type;
                        objitem.service_type = objCustomer.service_type;
                        objitem.remarks = objCustomer.remarks;
                        objitem.modified_by = userId;
                        objitem.modified_on = DateTimeHelper.Now;
                        objitem.rfstype = objCustomer.rfstype;
                        //objitem.building_code = objCustomer.building_code;
                        objitem.buildcode = objCustomer.buildcode;
                        objitem.region_id = objCustomer.region_id;
                        objitem.province_id = objCustomer.province_id;
                        objitem.parent_system_id = objCustomer.parent_system_id;
                        objitem.parent_entity_type = objCustomer.parent_entity_type;
                        objitem.parent_network_id = objCustomer.parent_network_id;
                        objitem.longitude = objCustomer.longitude;
                        objitem.latitude = objCustomer.latitude;
                        objitem.floor_id = objCustomer.floor_id;
                        objitem.structure_id = objCustomer.structure_id;
                        objitem.email_id = objCustomer.email_id;
                        objitem.mobile_no = objCustomer.mobile_no;
                        objitem.phone_no = objCustomer.phone_no;
                        
                    }
                    
                    // update mapping to isp_entity_mapping                   
                    var resp = DAIspEntityMapping.Instance.associateEntityInStructure(objCustomer.objIspEntityMap.shaft_id, objCustomer.objIspEntityMap.floor_id, objCustomer.system_id, EntityType.Customer.ToString(), objCustomer.parent_system_id == 0 ? objCustomer.objIspEntityMap.structure_id : objCustomer.parent_system_id, string.IsNullOrEmpty(objCustomer.parent_entity_type) == true ? EntityType.Structure.ToString() : objCustomer.parent_entity_type, objCustomer.longitude + " " + objCustomer.latitude);

                    

                    if (resp != null && resp.status)
                    {
                        objitem.isPortConnected = resp.status;
                        objitem.message = Resources.Helper.MultilingualMessageConvert(resp.message);// resp.message;
                        return objitem;
                    }
                    objitem.project_id = objCustomer.project_id;
                    objitem.planning_id = objCustomer.project_id;
                    objitem.workorder_id = objCustomer.workorder_id;
                    objitem.purpose_id = objCustomer.purpose_id;
                    objitem.primary_pod_system_id = objCustomer.primary_pod_system_id;
                    objitem.secondary_pod_system_id = objCustomer.secondary_pod_system_id;
                    objitem.status_remark = objCustomer.status_remark;
                    objitem.other_info = objCustomer.other_info;    //for additional-attributes
                    objitem.requested_by = objCustomer.requested_by;
                    objitem.request_approved_by = objCustomer.request_approved_by;
                    objitem.request_ref_id = objCustomer.request_ref_id;
                    objitem.origin_ref_id = objCustomer.origin_ref_id;
                    objitem.origin_ref_description = objCustomer.origin_ref_description;
                    objitem.origin_from = objCustomer.origin_from;
                    objitem.origin_ref_code = objCustomer.origin_ref_code;
                    var result = repo.Update(objitem);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(result.system_id, Models.EntityType.Customer.ToString(), result.province_id, 1);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Customer.ToString(), result.province_id);
                    return result;
                }
                else
                {
                    PageMessage objPageValidate = new PageMessage();
                    DbMessage objMessage = new DAMisc().validateEntity(new validateEntity
                    {
                        system_id = objCustomer.system_id,
                        entity_type = EntityType.Customer.ToString(),
                        floor_id = objCustomer.objIspEntityMap.floor_id ?? 0,
                        shaft_id = objCustomer.objIspEntityMap.shaft_id ?? 0,
                        parent_system_id = objCustomer.parent_system_id,
                        parent_entity_type = objCustomer.parent_entity_type
                    }, true);

                    if (!string.IsNullOrEmpty(objMessage.message))
                    {
                        objPageValidate.status = ResponseStatus.FAILED.ToString();
                        objPageValidate.message = Resources.Helper.MultilingualMessageConvert(objMessage.message);//objMessage.message;
                        objCustomer.objPM = objPageValidate;
                        return objCustomer;
                    }
					objCustomer.status = String.IsNullOrEmpty(objCustomer.status) ? "A" : objCustomer.status;
					//objCustomer.status = "A";
                    objCustomer.created_by = userId;
                    objCustomer.created_on = DateTimeHelper.Now;  
                    var resultItem = repo.Insert(objCustomer);
                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Customer.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.networkStatus = NetworkStatus.A.ToString();
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);
                    DbMessage entityObj = new DAMisc().updateGeojsonEntityAttribute(resultItem.system_id, Models.EntityType.Customer.ToString(), resultItem.province_id, 0);
                    //DbMessage geojsonObj = new DAMisc().updateGeojsonMetadata(Models.EntityType.Customer.ToString(), resultItem.province_id);
                    // update mapping to isp_entity_mapping  
                    DAIspEntityMapping.Instance.associateEntityInStructure(objCustomer.objIspEntityMap.shaft_id, objCustomer.objIspEntityMap.floor_id, objCustomer.system_id, EntityType.Customer.ToString(), objCustomer.parent_system_id == 0 ? objCustomer.objIspEntityMap.structure_id : objCustomer.parent_system_id, string.IsNullOrEmpty(objCustomer.parent_entity_type) == true ? EntityType.Structure.ToString() : objCustomer.parent_entity_type, objCustomer.longitude + " " + objCustomer.latitude);
                    return resultItem;
                }
            }
            catch { throw; }
        }

        public bool IsCustomerCodeExists(string customre_code, int userId)
        {
            var objitem = repo.Get(x => x.network_id.ToLower() == customre_code.ToLower());
            if (objitem != null)
            {
                return true;
            }

            return false;

        }
        public List<Customer> getCustomers(int parentSystemId, string parentEntityType)
        {
            try
            {
                return repo.GetAll(m => m.parent_system_id == parentSystemId && m.parent_entity_type == parentEntityType).ToList();
            }
            catch
            {
                throw;
            }
        }

        public Customer GetCustomerByCanId(string canId)
        {
            try
            {
                  return repo.Get(m => m.network_id.ToUpper() == canId.ToUpper());
                //return objCustomer != null ? objCustomer : new Customer(); 
                
            }
            catch { throw; }
        } 
        public Customer getCustomerbyId(int systemId)
        {
            try
            {
                var result= repo.Get(m => m.system_id == systemId);
                return result != null ? result : new Customer();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Customer getSiteCustomerId(string siteCustomerId)
        {
            try
            {
                var result = repo.GetById(m => m.customer_site_id == siteCustomerId);
                return result != null ? result : new Customer();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Customer getSiteCustomerPO(string PONumber)
        {
            try
            {
                var result = repo.GetById(m => m.po_number == PONumber);
                return result != null ? result : new Customer();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Customer getSiteCustomerPAF(string PAFNO)
        {
            try
            {
                var result = repo.GetById(m => m.paf_no == PAFNO);
                return result != null ? result : new Customer();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DbMessage SaveCustomerInfo(int system_id, string can_id, string customer_name, string address, string building_code, string rfs_type, int floor_id, double latitude, double longitude, int structure_id, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<DbMessage>("fn_api_save_customer_info", new { p_system_id = system_id, p_can_id = can_id, p_customer_name = customer_name, p_address = address, p_building_code = building_code, p_building_rfs_type = rfs_type, p_floor_id = floor_id, p_latitude = latitude, p_longitude = longitude, p_structure_id = structure_id, p_user_id = user_id }).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        public List<Dictionary<string, string>> GetSiteCustomerList(int siteId, string lmcType)
        {
            try
            {

                return repo.ExecuteProcedure<Dictionary<string, string>>("fn_get_site_customer_by_id", new { p_siteId = siteId, p_lmcType = lmcType }, true);
                 
            }
            catch (Exception)
            {

                throw;
            }
            // return repo.GetAll(u => u.site_id == siteId).ToList();

        }

        public int deleteCustomerbyId(int systemId)
        {
            try
            {
                var objSystmId = repo.Get(m => m.system_id == systemId);
                if (objSystmId != null)
                {
                    return repo.Delete(objSystmId.system_id);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Customer> getDarkFiberCustomers()
        {
            try
            {
                //return repo.GetAll().Where(x=> x.lmc_type=="Dark Fiber").ToList();
                return repo.ExecuteProcedure<Customer>("fn_get_fiber_link_customers", new { }, true).ToList();
            }
            catch
            {
                throw;
            }
        }
        #region Additional-Attributes
        public string GetOtherInfoCustomer(int systemId)
        {
            try
            {
                return repo.GetById(systemId).other_info;
            }
            catch { throw; }
        }
        #endregion
    }

    public class DACustomerInfo: Repository<CustomerInfo> 
    {
        public CustomerInfo GetCustomerInfoByCanId(string canId, string entity_type,int ticket_id)
        {
            try
            {
                // return repo.Get(m => m.network_id.ToUpper() == canId.ToUpper());
                //return objCustomer != null ? objCustomer : new Customer(); 
                return repo.ExecuteProcedure<CustomerInfo>("fn_api_get_customer_info", new { p_can_id = canId, p_entity_type = entity_type,p_ticket_id= ticket_id } ,true).FirstOrDefault();
            }
            catch { throw; }
        }
    }
    public class DAticketMaster : Repository<TicketMaster>
    {
        public TicketMaster UpdateTicketStatus(int ticket_id, string reference_type, int step_id, string address)
        {
            TicketMaster result = new TicketMaster(); 
            try
            { 
                var objitem = repo.Get(u => u.ticket_id == ticket_id && u.reference_type == reference_type);
                if (objitem != null)
                { 
                    objitem.ticket_status_id = 1;
                    objitem.ticket_status = "InProgress";
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.last_step_id = step_id;
                    objitem.address = address;
                  //  objitem.stepsMaster.is_processed = true;
                    result = repo.Update(objitem); 
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }

        public TicketMaster UpdateTicketMaster(int ticket_id, string reference_type,int step_id, int user_id)
        {
            TicketMaster result = new TicketMaster(); 
            try
            {
                var objitem = repo.Get(u => u.ticket_id == ticket_id && u.reference_type == reference_type);
                if (objitem != null)
                {
                    objitem.ticket_status = "Completed";
                    objitem.ticket_status_id = 3;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.completed_on = DateTimeHelper.Now;
                    objitem.completed_by = user_id;
                    objitem.last_step_id = step_id;
                    result = repo.Update(objitem);
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }

    }

    public class DATicketStepMaster : Repository<TicketStepsMaster>
    {
        public TicketStepsMaster UpdateTicketIsProcessed(int ticket_id, int ticket_type_id )
        {
            TicketStepsMaster result = new TicketStepsMaster(); 
            try
            {
                var objitem = repo.Get(u =>u.ticket_type_id == ticket_type_id && u.step_name=="Customer Information");
                if (objitem != null)
                {
                    objitem.is_processed = true;
                    objitem.modified_on = DateTimeHelper.Now; 
                    result = repo.Update(objitem);
                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }
    }

    
}
