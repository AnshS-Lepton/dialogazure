using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Models.Admin;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Admin
{
    public class layerColumnSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int layer_id { get; set; }
        public string setting_type { get; set; }
        [Required]
        public string column_name { get; set; }
        [Required]
        public int column_sequence { get; set; }
        [Required(ErrorMessage ="Display Name field is required!")]
        //[RegularExpression(@"^[^<>,-?;:'!~@#*""]+$", ErrorMessage = @"Following special characters are not allowed: ^<>,_-?;:'!~@#*""")]                    
        public string display_name { get; set; }
        public string table_name { get; set; }
        public bool is_active { get; set; }
        public bool is_required { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool is_kml_column_required { get; set; }
        public string res_field_key { get; set; }
    }
    public class vwLayerColumnSettings
    {
        public int layer_id { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayers { get; set; }
        [NotMapped]
        public List<layerColumnSettings> lstLayerColumns { get; set; }
        public int user_id { get; set; }
        public string settingType { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public vwLayerColumnSettings()
        {
            lstLayers = new List<layerDetail>();
            lstLayerColumns = new List<layerColumnSettings>();
            pageMsg = new PageMessage();
        }
    }


    public class ModuleSettings
    {
        public List<Modules> lstUserModule { get; set; }
        public PageMessage pageMsg { get; set; }
    }

    public class LayerActionSettings : ViewLayerActionList
    {
        public int layer_id { get; set; }
        public List<LayerActionDetails> actionDetails { get; set; }
        public List<layerDetail> layerDetails { get; set; }
        public PageMessage pageMsg { get; set; }
        public ViewLayerActionList ViewLayerActionList { get; set; }
    }
    public class ViewLayerActionList
    {
        public ViewLayerActionList()
        {
            ViewLayerAction = new ViewLayerAction();

            ViewLayerAction.searchText = string.Empty;
        }
        public IList<ViewVendorList> VendorDetailList { get; set; }
        public ViewLayerAction ViewLayerAction { get; set; }
    }
    public class ViewLayerAction
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }


    }
}
