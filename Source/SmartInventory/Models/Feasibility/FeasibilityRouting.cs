using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Feasibility
{
    public class DirectionsDetail
    {
        public int seq { get; set; }
        public int path_Seq { get; set; }
        public int? start_VID { get; set; }
        public int? end_VID { get; set; }
        public int node_SourceID { get; set; }
        public int edge_TargetID { get; set; }
        public decimal cost { get; set; }
        public decimal agg_Cost { get; set; }
        public string roadLine_GeomText { get; set; }
        public decimal len_Km { get; set; }
        public decimal speed_Kph { get; set; }
        public string waypoint_Seq { get; set; }
    }

    public class RoutingDetail
    {
        public int seq { get; set; }
        public int path_Seq { get; set; }
        public int edge_TargetID { get; set; }
        public string roadLine_GeomText { get; set; }
        public string start_point { get; set; }
        public string end_point { get; set; }
        public string network_id { get; set; }
        public string cable_name { get; set; }
        public string cable_status { get; set; }
        public int available_cores { get; set; }
        public int total_cores { get; set; }
    }
}
