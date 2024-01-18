using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class GisApiLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string api_url { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public string api_type { get; set; }
        public string latency { get; set; }
        public string transaction_id { get; set; }
        public string user_id { get; set; }
        public string gis_object_id { get; set; }
        public string request_time { get; set; }
        public string response_time { get; set; }
        public string gdb_version { get; set; }
        public string gis_design_id { get; set; }
        public string entity_type { get; set; }
        public string system_id { get; set; }
      
        [NotMapped]
        public int totalRecords { get; set; }
    }

  
}
