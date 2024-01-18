using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class ManageDropdownValues
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please enter Layer Name")]
        public int landbase_layer_id { get; set; }
        [NotMapped]
        public string layer_name { get; set; }
        public string type { get; set; }
        [NotMapped]
        public string OldValue { get; set; }
        public string value { get; set; }

        [NotMapped]
        public int category_id { get; set; }

        public int parent_id { get; set; }

        [Required]
        public bool is_active { get; set; }

        public DateTime created_on { get; set; }
        public Nullable<int> created_by { get; set; }

        public DateTime? modified_on { get; set; }
        public Nullable<int> modified_by { get; set; }

        [NotMapped]
        public bool IsSubmit { get; set; } = false;
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public int totalrecords { get; set; }
        [NotMapped]
        public List<LandBaseMaster> lstLayers { get; set; }
        [NotMapped]
        public List<LandbaseDropdownMaster> landbaseCategoryList { get; set; }
        [NotMapped]
        public string status { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public int s_no { get; set; }
         
        public ManageDropdownValues()
        {
            pageMsg = new PageMessage();
        }
    }
    public class ViewLandBaseDropdownMasterSettingsFilter : CommonGridAttributes
    {
        public int totalrecords { get; set; }
        public int s_no { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public string status { get; set; }
        public string layer_name { get; set; }
        public int id { get; set; }
        public int layer_id { get; set; }
    }
    public class ViewLandBaseDropdownMasterSettings
    {
        public int id { get; set; }
        public int landbase_layer_id { get; set; }
        public string type { get; set; }
        [NotMapped]
        public string OldValue { get; set; }
        
        public string value { get; set; }

        [NotMapped]
        public int category_id { get; set; }

        public int parent_id { get; set; }
        public bool is_active { get; set; }
        [NotMapped]
        public bool status { get; set; } = true;
        public bool? is_action_allowed { get; set; }
        public DateTime created_on { get; set; }
        public Nullable<int> created_by { get; set; }

        public DateTime? modified_on { get; set; }
        public Nullable<int> modified_by { get; set; }

        [NotMapped]
        public bool IsSubmit { get; set; } = false;
        [NotMapped]
        public PageMessage pageMsg { get; set; }

        [NotMapped]
        public List<LandBaseMaster> lstLayers { get; set; }
        [NotMapped]
        public List<LandbaseDropdownMaster> landbaseCategoryList { get; set; }
        [NotMapped]
        public ViewLandBaseDropdownMasterSettingsFilter objdrpFilter { get; set; }
        [NotMapped] 
        public int totalrecords { get; set; }
        [NotMapped]
        public List<ManageDropdownValues> lstViewDropdownMasterSetting { get; set; }

        public ViewLandBaseDropdownMasterSettings()
        {
            pageMsg = new PageMessage();
            objdrpFilter = new ViewLandBaseDropdownMasterSettingsFilter();
            lstViewDropdownMasterSetting = new List<ManageDropdownValues>();
            pageMsg = new PageMessage(); 
        }
    }

}
