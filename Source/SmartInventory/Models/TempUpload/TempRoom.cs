using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TempUpload
{
   public class TempRoom:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string room_name { get; set; }
        public double room_width { get; set; }
        public double room_height { get; set; }
        public double room_length { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
       // public int floor_id { get; set; }
        public string floor_name { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public int parent_system_id { get; set; }
       // public int region_id { get; set; }
       // public string region_name { get; set; }
       // public int province_id { get; set; }
      //  public string province_name { get; set; }
        public string unit_type { get; set; }
        public string unitno { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
        public int row_order { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
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
