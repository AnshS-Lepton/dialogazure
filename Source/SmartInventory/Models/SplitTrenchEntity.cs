using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class SplitTrenchEntity
    {
        public int system_id { get; set; }
        public string common_name { get; set; }
        public string geom_trench1 { get; set; }
        public string geom_trench2 { get; set; }
        public double trench1Length { get; set; }
        public double trench2Length { get; set; }
        public double trench1CalculatedLength { get; set; }
        public double trench2CalculatedLength { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public string parentTrenchNetworkId { get; set; }
        public string network_status { get; set; }

        public int? trench_one_a_system_id { get; set; }
        public string trench_one_a_entity_type { get; set; }
        public string trench_one_a_location { get; set; }
        public int? trench_one_b_system_id { get; set; }
        public string trench_one_b_entity_type { get; set; }
        public string trench_one_b_location { get; set; }
        public int? trench_two_a_system_id { get; set; }
        public string trench_two_a_entity_type { get; set; }
        public string trench_two_a_location { get; set; }
        public int? trench_two_b_system_id { get; set; }
        public string trench_two_b_entity_type { get; set; }
        public string trench_two_b_location { get; set; }

    }
}
