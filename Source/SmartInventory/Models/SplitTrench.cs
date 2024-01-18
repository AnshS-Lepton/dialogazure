using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{


    public class SplitTrench
    {
        public string split_entity_networkId { get; set; }
        public int split_entity_system_id { get; set; }
        public string split_entity_type { get; set; }
        public string split_entity_network_status { get; set; }
        public int split_trench_system_id { get; set; }

        public string trench_one_network_id { get; set; }

        public string trench_one_name { get; set; }

        public string trench_one_a_location { get; set; }

        public string trench_one_b_location { get; set; }

        public float? trench_one_measured_length { get; set; }

        public float? trench_one_calculated_length { get; set; }
        public int old_trench_a_system_id { get; set; }
        public string old_trench_a_entity_type { get; set; }

        public string old_trench_a_location { get; set; }
        public int old_trench_b_system_id { get; set; }
        public string old_trench_b_entity_type { get; set; }

        public string old_trench_b_location { get; set; }
        public string trench_two_network_id { get; set; }

        public string trench_two_name { get; set; }

        public string trench_two_a_location { get; set; }

        public string trench_two_b_location { get; set; }

        public float? trench_two_measured_length { get; set; }

        public float? trench_two_calculated_length { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }

        public List<EntityDetail> trenchs { get; set; }

        public PageMessage objPM { get; set; }
        public int userId { get; set; }

        public string network_status { get; set; }

        public SplitTrench()
        {
            objPM = new PageMessage();
            trenchs = new List<EntityDetail>();

        }
    }
}
