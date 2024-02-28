using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SplitMicroduct
    {
        public string split_entity_networkId { get; set; }
        public int split_entity_system_id { get; set; }
        public string split_entity_type { get; set; }
        public string split_entity_network_status { get; set; }
        public int split_microduct_system_id { get; set; }

        public string microduct_one_network_id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "SI_OSP_DUC_NET_FRM_016")]

        public string microduct_one_name { get; set; }
        [Required]
        public string microduct_one_a_location { get; set; }
        [Required]
        public string microduct_one_b_location { get; set; }
        [Required]
        public float? microduct_one_measured_length { get; set; }
        [Required]
        public float? microduct_one_calculated_length { get; set; }
        public int old_microduct_a_system_id { get; set; }
        public string old_microduct_a_entity_type { get; set; }

        [Required]
        public string old_microduct_a_location { get; set; }
        public int old_microduct_b_system_id { get; set; }
        public string old_microduct_b_entity_type { get; set; }
        [Required]
        public string old_microduct_b_location { get; set; }



        public string microduct_two_network_id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "SI_OSP_DUC_NET_FRM_016")]
        public string microduct_two_name { get; set; }
        [Required]
        public string microduct_two_a_location { get; set; }
        [Required]
        public string microduct_two_b_location { get; set; }
        [Required]
        public float? microduct_two_measured_length { get; set; }
        [Required]
        public float? microduct_two_calculated_length { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }

        public List<EntityDetail> microducts { get; set; }

        public PageMessage objPM { get; set; }
        public int userId { get; set; }

        public string network_status { get; set; }
     
        public SplitMicroduct()
        {
            objPM = new PageMessage();
            microducts = new List<EntityDetail>();
          
        }
    }
}
