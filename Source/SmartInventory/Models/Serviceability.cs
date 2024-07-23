using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Models
{
    public class GPON
    {
        public string nap_id { get; set; }
        public string nap_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public decimal distance { get; set; }
        public string utilized_port { get; set; }
        //public GPON()
        //{
        //    detail = new ExpandoObject(); 
        //}
    }
    public class DOCSIS
    {
        public string node_id { get; set; }
        public decimal distance { get; set; }
    }
    public class GFAST
    {
        public string gfast_id { get; set; }
        public decimal distance { get; set; }
    }
    public class DeviceList
    {
        public List<GPON> GPON { get; set; }
        //public List<DOCSIS> DOCSIS { get; set; }
        //public List<GFAST> GFAST { get; set; }
        public DeviceList()
        {
            GPON = new List<GPON>();
            //DOCSIS = new List<DOCSIS>();
            //GFAST = new List<GFAST>();
        }
    }
    public class ServiceabilityRoot
    {
        public string requestid { get; set; }
        public DeviceList devicelist { get; set; }
        public ServiceabilityRoot()
        {
            devicelist = new DeviceList();
        }
    }
    public class ServiceabilityPort
    {
        public int system_id { get; set; }
        public string type { get; set; }
        public string port_type { get; set; }
        public string port_name { get; set; }
        public string port_status { get; set; }
        public string port_comment { get; set; }
        public int port_number { get; set; }
    }
    public class ServiceabilityPortIN
    {
        public string network_id { get; set; }
        public string entity_type { get; set; }
    }
    public class ServiceabilityModel
    {
        public string requestId { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string servicetypes { get; set; }
        public string segment { get; set; }
    }
    public class ApiSettings
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class apiresponse
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public string results { get; set; }
    }
    public class ReqInputs
    {
        public string data { get; set; }

    }
    public class ReqHelpers
    {
        public static T GetRequestData<T>(ReqInputs userData)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(userData.data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class UpdateDesignIDInputs
    {
        public string network_id { get; set; }
        public string new_design_id { get; set; }
        public string old_design_id { get; set; }
        public string entity_type { get; set; }
    }
    public class UpdateDesignIDInputsDetails
    {
        public List<UpdateDesignIDInputs> InputDetails { get; set; }
    }
    public class EntityLocationDetails
    {

        public string entity_id { get; set; }
        public string entity_type { get; set; }
        public string province { get; set; }
        public string region { get; set; }
        public string error { get; set; }

        public location location { get; set; }


    }
    public class location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
    public class IntermediateEntitiesDetails
    {
        public Entity source_entity { get; set; }
        public Entity destination_entity { get; set; }
        public List<Entity> intermediate_entities { get; set; }
        public double distance_meters { get; set; }
        public string error { get; set; }
        public IntermediateEntitiesDetails()
        {

            source_entity = new Entity();
            destination_entity = new Entity();
            intermediate_entities = new List<Entity>();

        }
    }
    public class IntermediateEntitiesRequest
    {
        [Required]
        public string source_entity_type { get; set; }
        [Required]
        public string source_id { get; set; }
        [Required]
        public string destination_entity_type { get; set; }
        [Required]
        public string destination_id { get; set; }
        public string port { get; set; }
        public IntermediateEntitiesRequest()
        {
            port = "1";
        }
    }
    public class EntityLocationRequest
    {
        public string entity_type { get; set; }
        public string entity_network_id { get; set; }
    }
    public class Entity
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }
    }
    public class DestinationEntity
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }
    }
    public class IntermediateEntities
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }
    }
    public class UpdateAlarmStatusetails
    {
        [Required]
        public string reference_id { get; set; }
        [Required]
        public string alarm_reason { get; set; }
        public List<impacted_entities> Impacted_entities { get; set; }
    }
    public class impacted_entities
    {
        [Required]
        public string entity_type { get; set; }
        [Required]
        public string entity_id { get; set; }
        
        public string port_number { get; set; }
        [Required]
        public string alarm_status { get; set; }
        public string comments { get; set; }
    }

    public class Serviceability
    {
        public string reference_id { get; set; }
        public string status { get; set; }
        public List<Devices> devices { get; set; }
        public Serviceability()
        {
            devices = new List<Devices>();
        }

    }
    public class ServiceabilityRequest
    {
        [Required]
        public string reference_id { get; set; }
        [Required]
        public string latitude { get; set; }
        [Required]
        public string longitude { get; set; }
    }
    public class Devices
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }
        public decimal distance { get; set; }


    }
    public class  ErrorResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    public class APIResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
}