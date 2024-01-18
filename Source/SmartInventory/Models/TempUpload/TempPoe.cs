using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempPoe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string poe_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string poe_chamber_type { get; set; }
        public string region { get; set; }
        public string state { get; set; }
        public string city { get; set; }

        public string type_of_road { get; set; }
        public string road_name { get; set; }
        public string land_mark { get; set; }


        public string referance_point_1 { get; set; }
        public double latitude_1 { get; set; }
        public double longitude_1 { get; set; }
        public double distance_in_mtrs_1 { get; set; }

        public string referance_point_2 { get; set; }
        public double latitude_2 { get; set; }
        public double longitude_2 { get; set; }
        public double distance_in_mtrs_2 { get; set; }

        public string referance_point_3 { get; set; }
        public double latitude_3 { get; set; }
        public double longitude_3 { get; set; }
        public double distance_in_mtrs_3 { get; set; }

        public string remark_1 { get; set; }
        public string remark_2 { get; set; }
        public string remark_3 { get; set; }
        public int upload_id { get; set; }

        public int created_by { get; set; }
        public string network_status { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
    }

}
