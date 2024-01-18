using Models.ISP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SplitCable
    {
        public string split_entity_networkId { get; set; }
        public int split_entity_system_id { get; set; }
        public string split_entity_type { get; set; }
        public string split_entity_network_status { get; set; }
        public int splitCore { get; set; }

        public int split_cable_system_id { get; set; }

        public string cable_one_network_id { get; set; }
        [Required(ErrorMessage = "Cable name is required.")]
        public string cable_one_name { get; set; }
        [Required]
        public string cable_one_a_location { get; set; }
        [Required]
        public string cable_one_b_location { get; set; }
        [Required]
        public float? cable_one_measured_length { get; set; }
        [Required]
        public float? cable_one_calculated_length { get; set; }
        public int old_cable_a_system_id { get; set; }
        public string old_cable_a_entity_type { get; set; }
       
        [Required]
        public string old_cable_a_location { get; set; }
        public int old_cable_b_system_id { get; set; }
        public string old_cable_b_entity_type { get; set; }
        [Required]
        public string old_cable_b_location { get; set; }
    


        public string cable_two_network_id { get; set; }
        [Required(ErrorMessage = "Cable name is required.")]
        public string cable_two_name { get; set; }
        [Required]
        public string cable_two_a_location { get; set; }
        [Required]
        public string cable_two_b_location { get; set; }
        [Required]
        public float? cable_two_measured_length { get; set; }
        [Required]
        public float? cable_two_calculated_length { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }

        public List<EntityDetail> cables { get; set; }

        public PageMessage objPM { get; set; }
        public int userId{ get; set; }       
        public string splicing_source { get; set; }
        public string network_status { get; set; }
        public float? cable_one_start_reading { get; set; }
        public float? cable_one_end_reading { get; set; }
       
        public float? cable_two_start_reading { get; set; }
        public float? cable_two_end_reading { get; set; }
        public List<FormInputSettings> formInputSettings { get; set; }
        public List<LineEntityInfo> listLineEntityInfo { get; set; }
        public SplitCable() {
            objPM = new PageMessage();
            cables = new List<EntityDetail>();
            splicing_source = "OSP_Split";
        }
    }

    public class SplitCableMDU {
        public string split_entity_networkId { get; set; }
        public int split_entity_system_id { get; set; }
        public string split_entity_type { get; set; }
        public string split_entity_network_status { get; set; }

        public int split_cable_system_id { get; set; }

        public string cable_one_network_id { get; set; }
        [Required(ErrorMessage = "Cable name is required.")]
        public string cable_one_name { get; set; }
        [Required]
        public string cable_one_a_location { get; set; }
        [Required]
        public string cable_one_b_location { get; set; }
        [Required]
        public float? cable_one_measured_length { get; set; }
        [Required]
        public float? cable_one_calculated_length { get; set; }
        public string ispLineGeomCableOne { get; set; }

        public string cable_two_network_id { get; set; }
        [Required(ErrorMessage = "Cable name is required.")]
        public string cable_two_name { get; set; }
        [Required]
        public string cable_two_a_location { get; set; }
        [Required]
        public string cable_two_b_location { get; set; }
        [Required]
        public float? cable_two_measured_length { get; set; }
        [Required]
        public float? cable_two_calculated_length { get; set; }
        public string ispLineGeomCableTwo { get; set; }


        public int old_cable_a_system_id { get; set; }
        public string old_cable_a_entity_type { get; set; }

        [Required]
        public string old_cable_a_location { get; set; }
        public int old_cable_b_system_id { get; set; }
        public string old_cable_b_entity_type { get; set; }
        [Required]
        public string old_cable_b_location { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public List<StructureCable> cables { get; set; }

        public PageMessage objPM { get; set; }

        public string splicing_source { get; set; }
        public int userId { get; set; }
        public float? cable_one_start_reading { get; set; }
        public float? cable_one_end_reading { get; set; }
        public float? cable_two_start_reading { get; set; }
        public float? cable_two_end_reading { get; set; }
        public List<FormInputSettings> formInputSettings { get; set; }
        public int split_entity_x { get; set; }
        public int split_entity_y { get; set; }
        public string cable_one_a_node_type { get; set; }
        public string cable_one_b_node_type { get; set; }
        public string cable_two_a_node_type { get; set; }
        public string cable_two_b_node_type { get; set; }
        public SplitCableMDU() {
            objPM = new PageMessage();
            cables = new List<StructureCable>();
            splicing_source = "ISP_Split";
        }
    
    }
}
