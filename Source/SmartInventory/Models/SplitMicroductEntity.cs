using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class SplitMicroductEntity
    {
        public int system_id { get; set; }
        public string common_name { get; set; }
        public string geom_microduct1 { get; set; }
        public string geom_microduct2 { get; set; }
        public double microduct1Length { get; set; }
        public double microduct2Length { get; set; }
        public double microduct1CalculatedLength { get; set; }
        public double microduct2CalculatedLength { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public string parentmicroductNetworkId { get; set; }
        public string network_status { get; set; }

        public int? microduct_one_a_system_id { get; set; }
        public string microduct_one_a_entity_type { get; set; }
        public string microduct_one_a_location { get; set; }
        public int? microduct_one_b_system_id { get; set; }
        public string microduct_one_b_entity_type { get; set; }
        public string microduct_one_b_location { get; set; }
        public int? microduct_two_a_system_id { get; set; }
        public string microduct_two_a_entity_type { get; set; }
        public string microduct_two_a_location { get; set; }
        public int? microduct_two_b_system_id { get; set; }
        public string microduct_two_b_entity_type { get; set; }
        public string microduct_two_b_location { get; set; }

    }
}
