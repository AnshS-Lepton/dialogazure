using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FiberPath
    {
        public string equipment_id { get; set; }
        public int corePortNo { get; set; }
        public double tracingDistance { get; set; }
        public List<ConnectionInfo> lstConnectionInfo { get; set; }
    }

    public class FiberPathGeom
    {
        public string fiber_path_geom { get; set; }
        public double fiber_length { get; set; }
        public bool is_backword_path { get; set; }
        public string stream_type { get; set; }
    }
    public class FiberCutDetails
    {
        public string fault_lat { get; set; }
        public string fault_long { get; set; }
        public double physical_distance { get; set; }
        public double last_tp_distance { get; set; }
        public string last_tp_type { get; set; }
        public string last_tp_name { get; set; }
        public string last_tp_network_id { get; set; }
        public string last_tp_lat { get; set; }
        public string last_tp_long { get; set; }
        public string faulty_fiber_geom { get; set; }
        public int faulty_cable_id { get; set; }
       
    }
    public class FiberNodes
    {
        public string node_text { get; set; }
        public string node_value { get; set; }
        public string node_geom { get; set; }
    }
}
