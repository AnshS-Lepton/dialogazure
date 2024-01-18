
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempWallMount:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string wallmount_name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string wallmount_no { get; set; }
        public string wallmount_height { get; set; }
        public string address { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string specification { get; set; }
        public string vendor_name { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public int batch_id { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public int row_order { get; set; }
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
