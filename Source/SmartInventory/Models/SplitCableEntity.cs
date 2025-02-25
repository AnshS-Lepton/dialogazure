using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;
using System.Web.Helpers;
using Models.ISP;

namespace Models
{
    public class SplitCableEntity
    {
        public int system_id{ get; set; }
        public string common_name { get; set; }
        public string geom_cable1 { get; set; }
        public string geom_cable2 { get; set; }
        public double cable1Length{ get; set; }
        public double cable2Length { get; set; }
        public double cable1CalculatedLength { get; set; }
        public double cable2CalculatedLength { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public string parentCableNetworkId { get; set; }
        public string network_status { get; set; }
        public int? cable1_a_system_id { get; set; }
        public string cable1_a_entity_type { get; set; }
        public int? cable1_b_system_id { get; set; }
        public string cable1_b_entity_type { get; set; }
        public string cable1_a_location { get; set; }
        public int? cable2_a_system_id { get; set; }
        public string cable2_a_entity_type { get; set; }
        public int? cable2_b_system_id { get; set; }
        public string cable2_b_entity_type { get; set; }
        public string cable2_b_location { get; set; }
        [NotMapped]
        public int split_entity_system_id { get; set; }
        [NotMapped]
        public string split_entity_type { get; set; }
        [NotMapped]
        public int split_cable_system_id { get; set; }
        [NotMapped]
        public int Split_entity_Core { get; set; }
        public string cable1_name { get; set; }
        public string cable2_name { get; set; }
    }
}
