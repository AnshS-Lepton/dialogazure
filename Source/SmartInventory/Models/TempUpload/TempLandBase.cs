using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.TempUpload
{
  public  class TempLandBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string  layer_type {get;set;}
        public string network_id { get; set; }
        public string name { get; set; }
        public int landbase_layer_id { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string sp_geometry { get; set; }
        public string attribute_1 { get; set; }
        public string attribute_2 { get; set; }
        public string attribute_3 { get; set; }
        public string attribute_4 { get; set; }
        public string attribute_5 { get; set; }
        public string attribute_6 { get; set; }
        public string attribute_7 { get; set; }
        public string attribute_8 { get; set; }
        public string attribute_9 { get; set; }
        public string attribute_10 { get; set; }
        public string address { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public string buffer_geom { get; set; }
        public string category { get; set; }
        public string sub_category { get; set; } 
        public string classification { get; set; }
        public double buffer_width { get; set; } 
        public int upload_id { get; set; }
        public int batch_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int row_order { get; set; }
    }
}
