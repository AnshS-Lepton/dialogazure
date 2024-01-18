using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DistributionBox
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string entity_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int no_of_port { get; set; }
        public string entity_type { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
    }
}
