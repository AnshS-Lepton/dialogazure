using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.POIFS.Properties;
using Org.BouncyCastle.Asn1.Ocsp;

namespace DataAccess
{
    public class DAServiceability : Repository<DropDownMaster>
    {
        public List<EntityDetail> getServiceabilityResponse(double latitude, double longitude, string segment, int user_id)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_api_getserviceability", new { p_lat = latitude, p_lng = longitude, p_segment = segment, p_user_id = user_id });
            }
            catch { throw; }
        }
        public List<ServiceabilityPort> getServiceabilityPort(string network_id, string entity_type)
        {
            try
            {
                return repo.ExecuteProcedure<ServiceabilityPort>("fn_api_getserviceability_portinfo", new { network_id = network_id, p_lng = entity_type });
            }
            catch { throw; }
        }
        public apiresponse updateApiSettings(ApiSettings obj)
        {
            try
            {
                return repo.ExecuteProcedure<apiresponse>("fn_api_updateApiSettings", new { p_key = obj.key, p_value = obj.value }).FirstOrDefault();
            }
            catch { throw; }
        }

        public apiresponse UpdateDesignIDInputs(UpdateDesignIDInputs obj)
        {
            try
            {
                return repo.ExecuteProcedure<apiresponse>("fn_api_UpdateNewDesignId", new { v_network_id = obj.network_id, v_new_design_id = obj.new_design_id, v_old_design_id = obj.old_design_id, v_entity_type = obj.entity_type }).FirstOrDefault();
            }
            catch { throw; }
        }

        public apiresponse ResetXml(int System_id)
        {
            try
            {
                return repo.ExecuteProcedure<apiresponse>("fn_api_Reset_Xml", new { System_id = System_id }).FirstOrDefault();
            }
            catch { throw; }
        }

        public EntityLocationDetails GetEntityLocation(string entity_type, string entity_network_id)
        {
            try
            {
                return repo.ExecuteProcedure<EntityLocationDetails>("fn_api_get_entitylocation", new { p_entity_type = entity_type, p_entity_network_id = entity_network_id }, true).FirstOrDefault();

            }
            catch (Exception )
            {
                throw ;
            }
        }
        public IntermediateEntitiesDetails GetIntermediateEntities(string source_entity_type, string source_id, string destination_entity_type, string destination_id, string port)
        {
            
            try
            {

                return repo.ExecuteProcedure<IntermediateEntitiesDetails>("fn_api_get_intermediateentities", new
                {
                    p_source_entity_type = source_entity_type,
                    p_source_id = source_id,
                    p_destination_entity_type = destination_entity_type,
                    p_destination_id = destination_id,
                    p_port = Convert.ToInt32(port)
                }, true).FirstOrDefault();

            }

            catch (Exception )
            {
                throw ;
            }
        }
        public APIResponse UpdateAlarmStatusetails(impacted_entities obj)
        {
            try
            {
                return repo.ExecuteProcedure<APIResponse>("fn_api_Update_AlarmStatusdetails", new
                {
                    p_entity_type = obj.entity_type,
                    p_network_id = obj.entity_id,
                    p_port_number=!string.IsNullOrEmpty(obj.port_number)?Convert.ToInt32( obj.port_number):0,
                    p_alarm_status=obj.alarm_status,
                    p_comments=obj.comments


                }).FirstOrDefault();
            }
            catch { throw; }
        }

        public List<EntityDetail> Serviceability(double latitude, double longitude)
        {
            try
            {
                return repo.ExecuteProcedure<EntityDetail>("fn_api_get_ossserviceability", new { p_lng = longitude, p_lat = latitude });
            }
            catch { throw; }
        }

        public APIResponse UpdateDiscoveredEntityDetails(UpdateDiscoveredEntityDetails obj)
        {
           
            try
            {
                
                    
                
                

                return repo.ExecuteProcedure<APIResponse>("fn_api_Update_DiscoveredEntity", new
                    {
                        p_entity_type = obj.entity_type,
                        p_network_id = obj.entity_id,
                        p_serial_no = obj.serial_no,
                        p_ip_address = obj.ip_address,
                        p_ports = JsonConvert.SerializeObject(obj.ports)


                }).FirstOrDefault();
                
            }
            catch { throw; }
        }
    }
}
