using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TempUpload
{
    public class TempRow : IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string row_name { get; set; }
        public string row_stage { get; set; }
        public string row_type { get; set; }       
        public int created_by { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; } 
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int upload_id { get; set; }
        public int batch_id { get; set; }
        public int row_order { get; set; }
        public string sp_geometry { get; set; }
        public string remarks { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string network_status { get; set; }
    }
}
