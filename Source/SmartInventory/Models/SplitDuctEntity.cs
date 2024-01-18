using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class SplitDuctEntity
    {
        public int system_id { get; set; }
        public string common_name { get; set; }
        public string geom_duct1 { get; set; }
        public string geom_duct2 { get; set; }
        public double duct1Length { get; set; }
        public double duct2Length { get; set; }
        public double duct1CalculatedLength { get; set; }
        public double duct2CalculatedLength { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public string parentDuctNetworkId { get; set; }
        public string network_status { get; set; }

        public int? duct_one_a_system_id { get; set; }
        public string duct_one_a_entity_type { get; set; }
        public string duct_one_a_location { get; set; }
        public int? duct_one_b_system_id { get; set; }
        public string duct_one_b_entity_type { get; set; }
        public string duct_one_b_location { get; set; }
        public int? duct_two_a_system_id { get; set; }
        public string duct_two_a_entity_type { get; set; }
        public string duct_two_a_location { get; set; }
        public int? duct_two_b_system_id { get; set; }
        public string duct_two_b_entity_type { get; set; }
        public string duct_two_b_location { get; set; }

    }
}
