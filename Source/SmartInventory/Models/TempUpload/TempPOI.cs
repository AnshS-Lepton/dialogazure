using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempPOI
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int du_history_id { get; set; }
        public string poi_id { get; set; }
        public string data_source { get; set; }
        public string site_branch_building_name { get; set; }
        public string site_address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double tower_height { get; set; }
        public double bldg_height { get; set; }
        public double total_height { get; set; }
        public string bldg_type { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public DateTime created_on { get; set; }
        public string nearest_fiber_pop { get; set; }
        public string requested_bw { get; set; }
        public string fiber_length { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string city_name { get; set; }
        public string district { get; set; }
        public string state { get; set; }
        //public int accessibility { get; set; }
        //public int activation { get; set; }
        //public string network_status { get; set; }
        //public int construction { get; set; }
        public int upload_id { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
    }
}
