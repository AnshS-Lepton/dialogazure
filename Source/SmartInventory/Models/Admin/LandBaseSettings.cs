using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Admin
{
    public class LandBaseSettings
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please enter Layer Name")]
        public string layer_name { get; set; }
        [Required(ErrorMessage = "Please select Geom Type")]
        public string geom_type { get; set; }
       
        [Required(ErrorMessage = "Please enter Layer Abbreviation")]
        public string layer_abbr { get; set; }
        
        [Required(ErrorMessage = "Please enter Map Abbreviation")]
        public string map_abbr { get; set; }
        [Required(ErrorMessage = "Please enter Map Border Color")]
        public string map_border_color { get; set; }
        [Required(ErrorMessage = "Please enter Map Sequence")]
        public int map_seq { get; set; }
        [Required(ErrorMessage = "Please enter Map border Thickness")]
        public int map_border_thickness { get; set; }
        [Required(ErrorMessage = "Please enter Map Background Color")]
        public string map_bg_color { get; set; }
        [Range(1, 100)]
        [Required(ErrorMessage = "Please enter Map Background Opacity")]
        public int map_bg_opacity { get; set; }
        [Required]
        public bool is_active { get; set; }
        public bool is_center_line_enable { get; set; }
        [Required(ErrorMessage = "Please Select Network Id Type")]
        public string network_id_type { get; set; }
        [Required(ErrorMessage = "Please enter Network Code Seperator")]
        public string network_code_seperator { get; set; }

        public DateTime created_on { get; set; }
        public int created_by { get; set; }

        public DateTime modified_on { get; set; }
        public int modified_by { get; set; } 
        public string report_view_name { get; set; }
        public string audit_view_name { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public bool IsSubmit { get; set; } = false;
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public int S_No { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public List<LandBaseSettings> lstLayers { get; set; }
        [NotMapped]
        public CommonGridAttributes objGridAttributes { get; set; }
        public LandBaseSettings()
        {
            map_border_color = "#ffffff";
            map_bg_color = "#ffffff";
            pageMsg = new PageMessage();
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
    } 
}
