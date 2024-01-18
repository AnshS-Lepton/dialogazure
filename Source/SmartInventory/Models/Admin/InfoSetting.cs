using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class InfoSetting
    {
        [Key]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string info_attributes { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayers { get; set; }
        [NotMapped]
        public List<AttributeDetail> lstInfoAttributes { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public int ? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
         [NotMapped]
        public int user_id { get; set; }
        public InfoSetting()
        {
            lstLayers = new List<layerDetail>();
            lstInfoAttributes = new List<AttributeDetail>();
            pageMsg = new PageMessage();
            info_attributes = string.Empty;
        }
    }

    public class AttributeDetail
    {
        public string column_name { get; set; }
        public string display_name { get; set; }
        public bool is_selected { get; set; }
        public bool is_disabled { get; set; }
    }
   

    public class downloadbckupfile
    {
        public string status { get; set; }
        public string file_type { get; set; }
      
    }
}
