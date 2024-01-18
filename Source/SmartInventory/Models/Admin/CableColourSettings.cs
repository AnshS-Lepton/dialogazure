using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Admin
{
    public class CableMapColorSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string cable_type { get; set; }
        public string cable_category { get; set; }
        public int fiber_count { get; set; }
        [Required]
        public string color_code { get; set; }
        [NotMapped]
        [Required]
        public List<DropDownMaster> lstCableType { get; set; }
        [NotMapped]
        [Required]
        public List<DropDownMaster> lstCableCategory { get; set; }
        [NotMapped]
        [Required]
        public List<DropDownMaster> lstFiberCount { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<string> lstfiberCounts { get; set; }
        [NotMapped]
        public List<string> lstCableCategorys { get; set; }

        public CableMapColorSettings()
        {
            lstFiberCount = new List<DropDownMaster>();
            lstCableType = new List<DropDownMaster>();
            lstCableCategory = new List<DropDownMaster>();
            color_code = "#ffffff";
            pageMsg = new PageMessage();
        }

    }

    public class CableMapColorSettingVw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string cable_type { get; set; }
        public string cable_category { get; set; }
        public int fiber_count { get; set; }
        public string color_code { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
    }

    public class VwCableMapColorSettings
    {
        public VwCableMapColorSettingsFilter objCableColorSettingsFilter { get; set; }
        public List<DropDownMaster> lstCableType { get; set; }
        public List<DropDownMaster> lstCableCategory { get; set; }
        public List<DropDownMaster> lstFiberCount { get; set; }
        public List<CableMapColorSettingVw> lstViwCableColorSetting { get; set; }

        public VwCableMapColorSettings()
        {
            lstCableCategory = new List<DropDownMaster>();
            lstCableType = new List<DropDownMaster>();
            lstFiberCount = new List<DropDownMaster>();
            objCableColorSettingsFilter = new VwCableMapColorSettingsFilter();
            lstViwCableColorSetting = new List<CableMapColorSettingVw>();
        }
    }
    public class VwCableMapColorSettingsFilter : CommonGridAttributes
    {
        public string fiber_count { get; set; }
        public string cable_type { get; set; }
        public string cable_category { get; set; }
       
    }
}
