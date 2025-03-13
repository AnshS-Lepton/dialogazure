using DataAccess.DBHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DADeviceSearch : Repository<DeviceSearch>
    {
        DADeviceSearch()
        {

        }
        private static DADeviceSearch objDeviceSearch = null;
        private static readonly object lockObject = new object();
        public static DADeviceSearch Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objDeviceSearch == null)
                    {
                        objDeviceSearch = new DADeviceSearch();
                    }
                }
                return objDeviceSearch;
            }
        }

        public dbresponse GetNearbyDevices(DeviceSearch obj)
        {
            try
            {
                return repo.ExecuteProcedure<dbresponse>("fn_api_get_nearby_available_devices", new
                {
                    p_entity_type = obj.entity_type,
                    p_entity_column_name = obj.entity_column_name,
                    p_entity_column_value = obj.entity_column_value,
                    p_latitude = obj.latitude,
                    p_longitude = obj.longitude,
                    p_is_entity_geom_based = obj.is_entity_geom_based,
                    p_buffer_in_mtr = obj.buffer_in_mtr,
                    p_unit_name = obj.unit_name
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public dbresponse UpdatePortStatus(PortStatusUpdateInfo obj)
        {
            try
            {
                return repo.ExecuteProcedure<dbresponse>("fn_api_update_port_status", new
                {
                    p_system_id = obj.system_id,
                    p_port_status = obj.port_status,
                    p_comment = obj.comment,
                    p_user_id = obj.user_id,
                    p_source_ref_type = obj.source_ref_type
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }
        public dbresponse GetFiberCutDistance(FiberCutTracingInfo obj)
        {
            try
            {
                return repo.ExecuteProcedure<dbresponse>("fn_api_get_fiber_cut_details", new
                {
                    p_entity_system_id = obj.system_Id,

                    p_network_id = obj.network_Id,
                    p_entity_type = obj.entity_type,
                    p_entity_port_no = obj.core_port_no,
                    p_distance = obj.cut_distance,
                    p_is_a_end = obj.is_cable_A_End,
                    p_is_backword_path = obj.TraceDirection,
                    p_action_code = obj.actionCode 
                }, true).FirstOrDefault();
            }
            catch { throw; }
        }

        public string GetCableDetails(InputEntityInfo obj)
        {
            try
            {
                return repo.ExecuteProcedure<string>("fn_get_cable_FiberLink_detail", new
                {
                    p_entity_type = obj.entity_type,
                    p_entity_name = obj.entity_name,
                    p_network_id = obj.network_id,
                    p_route_buffer = obj.route_buffer

                }, false).FirstOrDefault();
            }
            catch { throw; }
        }

    }
}
