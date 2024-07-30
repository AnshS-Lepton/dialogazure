using DataAccess;
using Models;
using System.Collections.Generic;

namespace BusinessLogics
{
    public class BLServiceability
    {
        DAServiceability objDAMisc = new DAServiceability();
        public List<EntityDetail> getServiceabilityResponse(double latitude, double longitude, string segment, int user_id = 0)
        {
            return objDAMisc.getServiceabilityResponse(latitude, longitude, segment, user_id);

        }
        public List<ServiceabilityPort> getServiceabilityPort(string network_id, string entity_type)
        {
            return objDAMisc.getServiceabilityPort(network_id, entity_type);

        }
        public apiresponse updateApiSettings(ApiSettings obj)
        {
            return objDAMisc.updateApiSettings(obj);

        }
        public apiresponse UpdateDesignIDInputs(UpdateDesignIDInputs obj)
        {
            return objDAMisc.UpdateDesignIDInputs(obj);

        }
        public apiresponse ResetXml(int System_id)
        {
            return objDAMisc.ResetXml(System_id);

        }
        public EntityLocationDetails GetEntityLocation(string entity_type, string entity_network_id)
        {
            return objDAMisc.GetEntityLocation(entity_type, entity_network_id);

        }

        public IntermediateEntitiesDetails GetIntermediateEntities(string source_entity_type, string source_id, string destination_entity_type, string destination_id, string port)
        {
            return objDAMisc.GetIntermediateEntities(source_entity_type, source_id, destination_entity_type, destination_id, port);
        }

        public APIResponse UpdateAlarmStatusetails(impacted_entities obj)
        {
            return objDAMisc.UpdateAlarmStatusetails(obj);
        }

        public List<EntityDetail> Serviceability(double latitude, double longitude)
        {
            return objDAMisc.Serviceability(latitude, longitude);
        }

        public APIResponse UpdateDiscoveredEntityDetails(UpdateDiscoveredEntityDetails obj)
        {
            return objDAMisc.UpdateDiscoveredEntityDetails(obj);
        }
    }
}
