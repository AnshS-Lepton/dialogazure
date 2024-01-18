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
using System.Web;
using System.Linq.Expressions;

namespace Models.Admin
{
    public class LayerGroupMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int group_id { get; set; }
        [Required]
        public string  group_name { get; set; }
        [Required]
        public string group_description { get; set; }
        public bool status { get; set; }
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
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public int chk_GrpStatus { get; set; }
        public LayerGroupMaster()
        {
            pageMsg = new PageMessage();
        }

    }

    public class LayerGroupMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? mapping_id { get; set; }

        //[Key]
        //[Column(Order = 0)]
        [Required]
        public int? layer_id { get; set; }
        [Required]
        public int? group_id { get; set; }
        public int? layer_seq { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        [NotMapped]
        public string modified_on_formated { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
       
        [NotMapped]
        public string group_name { get; set; }
        [NotMapped]
        public string group_description { get; set; }
        [NotMapped]
        public string layer_name { get; set; }
        [NotMapped]
        public string layer_title { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public List<LayerGroupMaster> lstGroupDetails { get; set; }
        [NotMapped]
        [Required]
        public List<layerDetail> lstLayerGrpMapping { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
            public LayerGroupMapping()
        {
            pageMsg = new PageMessage();
            lstGroupDetails = new List<LayerGroupMaster>();
            lstLayerGrpMapping = new List<layerDetail>();
           
        }
    }

    public class ViewLayerGroup
    {
        public List<LayerGroupMaster> lstLayerGroups { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewLayerGroup()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }
    }

    public class ViewLayerGroupMapping
    {
        public List<LayerGroupMaster> lstLayerGroups { get; set; }
        public List<LayerGroupMapping> lstLayerGroupsMapping { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public PageMessage pageMsg { get; set; }
        public ViewLayerGroupMapping()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
            pageMsg = new PageMessage();
        }
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }
    }

    public class LayerStyleMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int layer_id { get; set; }        
        public string color_code_hex { get; set; }
        public string outline_color_hex { get; set; }        
        public double? opacity { get; set; }        
        public int? label_font_size { get; set; }
        public string label_color_hex { get; set; }
        public string label_bg_color_hex { get; set; }
        public string icon_base_path { get; set; }
        public string icon_file_name { get; set; }        
        public int? line_width { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? layer_sequence { get; set; }
        [NotMapped]
        public bool status { get; set; }
        [NotMapped]
        public int user_id { get; set; }        
        [NotMapped]
        public int totalRecords { get; set; }        
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<LayerStyleMaster> lstLayerGroups { get; set; }        
        [NotMapped]
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public String message { get; set; }
        [NotMapped]
        public String LableExp1 { get; set; }
        [NotMapped]
        public String LableExp2 { get; set; }
        [NotMapped]
        public String lstlblAttributes { get; set; }
        [NotMapped]
        public String SeclblAttributes { get; set; }
        [NotMapped]
        public List<AttributeDetail> LabelExpression { get; set; } 
        public String label_expression { get; set; }
        [NotMapped]
        public String layer_name { get; set; }        
        public String entity_category { get; set; }
        [NotMapped]
        public String entity_sub_category { get; set; }
        [NotMapped]
        public String geom_type { get; set; }        
        [NotMapped]
        public int max_layer_sequence { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }
        [NotMapped]
        public List<AttributeDetail> calegorylistbylayername { get; set; }
        public LayerStyleMaster()
        {
            objGridAttributes = new CommonGridAttributes();
            LabelExpression = new List<AttributeDetail>();
            calegorylistbylayername = new List<AttributeDetail>();
        }
    }
    
    public class LableExp1
    {        
        public String type { get; set; }
        public String Value { get; set; }
    }
    public class LableExp2
    {
        public String type { get; set; }
        public String Value { get; set; }
    }
    public class LableExp3
    {
        public String type { get; set; }
        public String Value { get; set; }
    }
    public class LableExp4
    {
        public String type { get; set; }
        public String Value { get; set; }
    }    
}
