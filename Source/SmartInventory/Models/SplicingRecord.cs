using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models
{
    public class SplicingRecord
    {

        public string splicing_source { get; set; }


        public string created_by { get; set; }


        public string created_on { get; set; } // Change to DateTime if appropriate


        public int source_system_id { get; set; }


        public string source_entity_type { get; set; }


        public string source_network_id { get; set; }


        public int source_port_no { get; set; }


        public int destination_system_id { get; set; }


        public string destination_entity_type { get; set; }


        public string destination_network_id { get; set; }


        public int destination_port_no { get; set; }
    }
}
