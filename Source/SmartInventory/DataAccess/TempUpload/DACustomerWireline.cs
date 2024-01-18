using DataAccess.DBHelpers;
using Models;
using Models.TempUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DACustomerWireline : Repository<Customer>
    {
        public Customer SaveCustomer(Customer objCustomer, int userId)
        {
            try
            {
                var objitem = repo.Get(x => x.system_id == objCustomer.system_id);
                if (objitem != null)
                {
                    objitem.activation_date = objCustomer.activation_date;
                    objitem.activation_status = objCustomer.activation_status;
                    objitem.address = objCustomer.address;
                    objitem.buildcode = objCustomer.buildcode;
     //               objitem.building_height = objCustomer.building_height;
     //               objitem.building_type1 = objCustomer.building_type1;
					//objitem.building_type2 = objCustomer.building_type2;
     //               objitem.commissioning_date = objCustomer.commissioning_date;
     //               objitem.connected_sw_ip = objCustomer.connected_sw_ip;
     //               objitem.connected_sw_port = objCustomer.connected_sw_port;
     //               objitem.customer_code = objCustomer.customer_code;
     //               objitem.customer_connected_through = objCustomer.customer_connected_through;
     //               objitem.customer_link_id = objCustomer.customer_link_id;
     //               objitem.customer_name = objCustomer.customer_name;
     //               objitem.customer_po_id1 = objCustomer.customer_po_id1;
     //               objitem.customer_po_id2 = objCustomer.customer_po_id2;
     //               objitem.customer_service_address = objCustomer.customer_service_address;
     //               objitem.customer_subscribed_bw_in_kbps = objCustomer.customer_subscribed_bw_in_kbps;
     //               objitem.deactivation_date = objCustomer.deactivation_date;
     //               objitem.entityType = objCustomer.entityType;
     //               objitem.fiber_length = objCustomer.fiber_length;
     //               objitem.kml_length = objCustomer.kml_length;
     //               objitem.lms_id = objCustomer.lms_id;
                    objitem.modified_by = userId;
                    objitem.modified_on = DateTimeHelper.Now;
                    objitem.networkIdType = objCustomer.networkIdType;
                    objitem.parent_entity_type = objCustomer.parent_entity_type;
                    objitem.parent_network_id = objCustomer.parent_network_id;
                    objitem.parent_system_id = objCustomer.parent_system_id;
                    //objitem.path_type = objCustomer.path_type;
                    objitem.pEntityType = objCustomer.pEntityType;
                    objitem.pin_code = objCustomer.pin_code;
                    objitem.pNetworkId = objCustomer.pNetworkId;
                    //objitem.po_date = objCustomer.po_date;
                    //objitem.primary_bstn_name = objCustomer.primary_bstn_name;
                    objitem.remarks = objCustomer.remarks;
                    objitem.rfstype = objCustomer.rfstype;
                    //objitem.secondary_bstn_name = objCustomer.secondary_bstn_name;
                    //objitem.site_address = objCustomer.site_address;
                    objitem.site_id = objCustomer.site_id;
                    //objitem.site_name = objCustomer.site_name;
                    //objitem.status = objCustomer.status;
                    //objitem.structure_name = objCustomer.structure_name;
                    //objitem.templateId = objCustomer.templateId;
                    //objitem.vendor_name = objCustomer.vendor_name;
                    //objitem.collector_ring_name = objCustomer.collector_ring_name;
                    //objitem.physical_status = objCustomer.physical_status;
                    //objitem.fusion_status = objCustomer.fusion_status;
                    //objitem.commercial_status = objCustomer.commercial_status;
                    //objitem.terminate_pe_ip = objCustomer.terminate_pe_ip;
                    //objitem.pe_interface = objCustomer.pe_interface;
                    //objitem.bstn_sw_port = objCustomer.bstn_sw_port;
                    //objitem.bstn_sw_ip = objCustomer.bstn_sw_ip;
                    //objitem.customer_connecting_port_from_bstn_sw = objCustomer.customer_connecting_port_from_bstn_sw;
                    //objitem.fms_port_no = objCustomer.fms_port_no;

                    //objitem.primary_site_switch_ip = objCustomer.primary_site_switch_ip;
                    //objitem.primary_switch_port_no = objCustomer.primary_switch_port_no;
                    //objitem.secondary_site_id = objCustomer.secondary_site_id;
                    //objitem.secondary_site_name = objCustomer.secondary_site_name;
                    //objitem.secondary_site_address = objCustomer.secondary_site_address;
                    //objitem.secondary_site_switch_ip = objCustomer.secondary_site_switch_ip;
                    //objitem.secondary_site_switch_port_no = objCustomer.secondary_site_switch_port_no;
                    //objitem.network_status = objCustomer.network_status;
                    //objitem.otdr_length = objCustomer.otdr_length;
                    //objitem.po_length = objCustomer.po_length;
                    //if (objCustomer.objIspEntityMap.structure_id != 0)
                    //{
                    //    objitem.parent_system_id = Convert.ToInt32(objCustomer.objIspEntityMap.structure_id);
                    //    objitem.parent_entity_type = EntityType.Structure.ToString();
                    //}
                    var result = repo.Update(objitem);


                    //if (objCustomer.objIspEntityMap.floor_id != 0 || objCustomer.objIspEntityMap.shaft_id != 0)
                    //{
                    //    int mappingSysId = 0;
                    //    DAIspEntityMapping.Instance.DeleteIspEntityByStrucId(objitem.parent_system_id, objitem.system_id, EntityType.Customer.ToString(), ref mappingSysId);
                    //    IspEntityMapping objMapping = new IspEntityMapping();
                    //    objMapping.id = objCustomer.objIspEntityMap.id;
                    //    objMapping.structure_id = objitem.parent_system_id;
                    //    objMapping.floor_id = objCustomer.objIspEntityMap.floor_id ?? 0;
                    //    objMapping.shaft_id = objCustomer.objIspEntityMap.shaft_id ?? 0;
                    //    objMapping.entity_id = objitem.system_id;
                    //    objMapping.entity_type = EntityType.Customer.ToString();
                    //    var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //    DAIspEntityMapping.Instance.updateEntityMapping(mappingSysId, insertMap.id);
                    //}

                    return result;
                }
                else
                {
                    objCustomer.created_by = userId;
                    objCustomer.created_on = DateTimeHelper.Now;
                    //if (objCustomer.objIspEntityMap.structure_id != 0)
                    //{
                    //    objCustomer.parent_system_id = Convert.ToInt32(objCustomer.objIspEntityMap.structure_id);
                    //    objCustomer.parent_entity_type = EntityType.Structure.ToString();
                    //}
                    var resultItem = repo.Insert(objCustomer);


                    // update network_id
                    //resultItem.network_id = new DAMisc().setElocID(EntityType.Customer.ToString(), resultItem.geom, resultItem.system_id);



                    // Save geometry
                    InputGeom geom = new InputGeom();
                    geom.systemId = resultItem.system_id;
                    geom.longLat = resultItem.longitude + " " + resultItem.latitude;
                    geom.userId = userId;
                    geom.entityType = EntityType.Customer.ToString();
                    geom.commonName = resultItem.network_id;
                    geom.geomType = GeometryType.Point.ToString();
                    geom.ports = "0";
                    //geom.networkStatus = resultItem.network_status;
                    string chkGeomInsert = DASaveEntityGeometry.Instance.SaveEntityGeom(geom);

                    //var customerParentDetails = new DAMisc().GetEntityDetailById<Customer>(objCustomer.parent_system_id, EntityType.ONT);
                    //if (customerParentDetails != null && customerParentDetails.parent_entity_type == EntityType.Structure.ToString())
                    //{
                    //    var mappingDetails = DAIspEntityMapping.Instance.GetIspEntityMapByStrucId(customerParentDetails.parent_system_id, customerParentDetails.system_id, EntityType.ONT.ToString());
                    //    if (mappingDetails!=null)
                    //    {
                    //        IspEntityMapping objMapping = new IspEntityMapping();
                    //        objMapping.id = 0;
                    //        objMapping.structure_id = mappingDetails.structure_id;
                    //        objMapping.floor_id = mappingDetails.floor_id ?? 0;
                    //        objMapping.shaft_id = mappingDetails.shaft_id ?? 0;
                    //        objMapping.entity_id = resultItem.system_id;
                    //        objMapping.entity_type = EntityType.Customer.ToString();
                    //        objMapping.parent_id = mappingDetails.id;
                    //        var insertMap = DAIspEntityMapping.Instance.SaveIspEntityMapping(objMapping);
                    //    }
                    //}                                        
                    return resultItem;
                }
            }
            catch { throw; }
        }
        public int DeleteCustomerById(int systemId)
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

        public bool IsCustomerCodeExists(string customre_code, int userId)
        {
            var objitem = repo.Get(x => x.network_id.ToLower() == customre_code.ToLower());
            if(objitem != null)
            {
                return true;
            }

            return false;

        }
        public List<Customer> getCustomers(int parentSystemId, string parentEntityType)
        {
            try
            {
                return repo.GetAll(m => m.parent_system_id == parentSystemId && m.parent_entity_type==parentEntityType).ToList();
            }
            catch {
                throw;
            }
        }

        public ErrorLog SaveBulkCustomer(List<Customer> lstCustomer)
        {
            repo.Insert(lstCustomer);
            ErrorLog error = new ErrorLog();
            //error.is_valid = true;

            return error;

        }

        
    }
    public class DATempCustomer : Repository<TempWirelineCustomer>
    {
        public ErrorLog SaveBulkCustomer(List<TempWirelineCustomer> lstCustomer)
        {

            repo.Insert(lstCustomer);
            ErrorLog error = new ErrorLog();
            //error.is_valid = true;

            return error;

        }

        public void InsertWirelineCustomerIntoMainTable(UploadSummary summary)
        {
            repo.ExecuteSQLCommand(string.Format("select * from fn_bulkupload_customer({0},{1})", summary.id, "''"));
        }
    }
}
