using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

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

    }
}
