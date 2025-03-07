using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public  class DeviceSearch
    {
        public string entity_type { get; set; }
        public string entity_column_name { get; set; }
        public string entity_column_value { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public bool? is_entity_geom_based { get; set; }
        public double? buffer_in_mtr { get; set; }
        public string unit_name { get; set; }
    }
    public class PortStatusUpdateInfo
    {
        public int? system_id { get; set; }
        public string port_status { get; set; }
        public string comment { get; set; }
        public string user_id { get; set; }
        public string source_ref_type { get; set; }
    }
    public class FiberCutTracingInfo
    {
        public int? system_Id { get; set; }
        public string network_Id { get; set; }
        public string entity_type { get; set; }
        public int? core_port_no { get; set; }
        public double? cut_distance { get; set; }
        public bool? is_cable_A_End { get; set; }
        public string TraceDirection { get; set; }
        public int? actionCode { get; set; }
    }
     public class dbresponse
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public dynamic results { get; set; }
    }

    #region PNO Integration

    public class InputEntityInfo
    {
        public string entity_type { get; set; }

        public string entity_name { get; set; }

        public string network_id { get; set; }

        public int route_buffer { get; set; } = 5;
    }

    #endregion

}
