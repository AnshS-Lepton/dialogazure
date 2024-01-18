using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using Models;

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
    }
}
