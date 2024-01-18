using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SplitConduitEntity
    {
        public int system_id { get; set; }
        public string common_name { get; set; }
        public string geom_conduit1 { get; set; }
        public string geom_conduit2 { get; set; }
        public double conduit1Length { get; set; }
        public double conduit2Length { get; set; }
        public double conduit1CalculatedLength { get; set; }
        public double conduit2CalculatedLength { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public string parentConduitNetworkId { get; set; }
        public string network_status { get; set; }

        public int? conduit_one_a_system_id { get; set; }
        public string conduit_one_a_entity_type { get; set; }
        public string conduit_one_a_location { get; set; }
        public int? conduit_one_b_system_id { get; set; }
        public string conduit_one_b_entity_type { get; set; }
        public string conduit_one_b_location { get; set; }
        public int? conduit_two_a_system_id { get; set; }
        public string conduit_two_a_entity_type { get; set; }
        public string conduit_two_a_location { get; set; }
        public int? conduit_two_b_system_id { get; set; }
        public string conduit_two_b_entity_type { get; set; }
        public string conduit_two_b_location { get; set; }

    }
}
