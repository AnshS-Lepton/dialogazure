using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SplitConduit
    {
        public string split_entity_networkId { get; set; }
        public int split_entity_system_id { get; set; }
        public string split_entity_type { get; set; }
        public string split_entity_network_status { get; set; }
        public int split_conduit_system_id { get; set; }

        public string conduit_one_network_id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "SI_OSP_DUC_NET_FRM_016")]

        public string conduit_one_name { get; set; }
        [Required]
        public string conduit_one_a_location { get; set; }
        [Required]
        public string conduit_one_b_location { get; set; }
        [Required]
        public float? conduit_one_measured_length { get; set; }
        [Required]
        public float? conduit_one_calculated_length { get; set; }
        public int old_conduit_a_system_id { get; set; }
        public string old_conduit_a_entity_type { get; set; }

        [Required]
        public string old_conduit_a_location { get; set; }
        public int old_conduit_b_system_id { get; set; }
        public string old_conduit_b_entity_type { get; set; }
        [Required]
        public string old_conduit_b_location { get; set; }



        public string conduit_two_network_id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "SI_OSP_DUC_NET_FRM_016")]
        public string conduit_two_name { get; set; }
        [Required]
        public string conduit_two_a_location { get; set; }
        [Required]
        public string conduit_two_b_location { get; set; }
        [Required]
        public float? conduit_two_measured_length { get; set; }
        [Required]
        public float? conduit_two_calculated_length { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }

        public List<EntityDetail> conduits { get; set; }

        public PageMessage objPM { get; set; }
        public int userId { get; set; }

        public string network_status { get; set; }

        public SplitConduit()
        {
            objPM = new PageMessage();
            conduits = new List<EntityDetail>();

        }
    }
}
