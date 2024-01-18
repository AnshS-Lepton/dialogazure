using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   
    public class DropdownMaster
    {
        public DropdownMasterSettingsFilter objDropdownMasterSettingsFilter { get; set; }

        public List<EntityDropDownMasterList> lstDropdownMasterGrid { get; set; }
        public List<layerDetail> lstLayers { get; set; }
        public int layer_id { get; set; }
        public string layer_name { get; set; }

        public string dropdown_type { get; set; }
       
        public string Value { get; set; }

        public bool IsVisible { get; set; }

    }

    public class ViewDropdownMasterSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int totalrecords { get; set; }
        public int s_no { get; set; }
        public string dropdown_type { get; set; }
        public string dropdown_value { get; set; }
        public bool dropdown_status { get; set; }
        public string layer_name { get; set; }
        public int id { get; set; }
        public bool isvisible { get; set; }
        public int layer_id { get; set; }
    }

    public class DropdownMasterSettingsFilter : CommonGridAttributes
    {
        public int totalrecords { get; set; }
        public int s_no { get; set; }
        public string dropdown_type { get; set; }
        public string dropdown_value { get; set; }
        public bool dropdown_status { get; set; }
        public string layer_name { get; set; }
        public int id { get; set; }
        public bool isvisible { get; set; }
        public int layer_id { get; set; }
    }


    //public class AddNewEntityUtilization
    //{

    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int system_id { get; set; }
    //    [Required]
    //    public int layer_id { get; set; }
    //    [Required]
    //    public string network_status { get; set; }
    //    [Required]
    //    public int region_id { get; set; }
    //    [Required]
    //    public int province_id { get; set; }
    //    [Required]
    //    public string utilization_range_low_from { get; set; }
    //    [Required]
    //    public string utilization_range_low_to { get; set; }
    //    [Required]
    //    public string utilization_range_moderate_from { get; set; }
    //    [Required]
    //    public string utilization_range_moderate_to { get; set; }
    //    [Required]
    //    public string utilization_range_high_from { get; set; }
    //    [Required]
    //    public string utilization_range_high_to { get; set; }
    //    [Required]
    //    public string utilization_range_over_from { get; set; }
    //    [Required]
    //    public string utilization_range_over_to { get; set; }
    //    public int created_by { get; set; }
    //    public DateTime created_on { get; set; }
    //    public int? modified_by { get; set; }
    //    public DateTime? modified_on { get; set; }
    //    [NotMapped]
    //    [Required]
    //    public List<Region> lstRegion { get; set; }
    //    [NotMapped]
    //    public List<Province> lstProvince { get; set; }
    //    [NotMapped]
    //    public int user_id { get; set; }
    //    [NotMapped]
    //    [Required]
    //    public List<layerDetail> lstLayers { get; set; }
    //    [NotMapped]
    //    public PageMessage pageMsg { get; set; }

    //    public AddNewEntityUtilization()
    //    {
    //        lstRegion = new List<Region>();
    //        lstProvince = new List<Province>();
    //        lstLayers = new List<layerDetail>();
    //        pageMsg = new PageMessage();
    //    }

    //}

    public class EntityDropDownMasterList
    {
        public string layer_name { get; set; }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }
        public bool IsVisible { get; set; }
    }

}
