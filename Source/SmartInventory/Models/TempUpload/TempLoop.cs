
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.TempUpload
{
    public class TempLoop
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double loop_length { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int cable_system_id { get; set; }
        public string cable_network_id { get; set; }
        public int associated_system_id { get; set; }
        public string associated_network_id { get; set; }
        public string associated_entity_type { get; set; }
        public string network_status { get; set; }
        public double start_reading { get; set; }
        public double end_reading { get; set; }        
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
        public int row_order { get; set; }
    }
}
