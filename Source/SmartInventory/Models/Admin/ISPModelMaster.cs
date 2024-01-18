using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models.Admin
{
    public enum ModelType
    {
        Equipment
        , Slot
        , Card
        , Tray
        , Port
        , Chassis
    }
    public class ISPModelMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string description { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool is_active { get; set; }
        public bool has_type { get; set; }
      
        public string network_id_type { get; set; }
        public string network_code_format { get; set; }
        public string network_code_seperator { get; set; }
        public string model_abbr { get; set; }

        public bool is_rotation_enabled { get; set; }
        public int min_height { get; set; }
        public int min_width { get; set; }
        public int? model_color_id { get; set; }
        public bool is_model_image_allowed { get; set; }

        [NotMapped]
        public List<AllEquipments> lstEquipments { get; set; }

        [NotMapped]
        public List<ModelDetails> lstModelDetails { get; set; }


        [NotMapped]
        public OptionsEquipmentDetails objOptionsEquipmentDetails { get; set; }


        [NotMapped]
        public List<ModelStatusCount> lstModelStatusCount { get; set; }

        public ISPModelMaster()
        {
            lstEquipments = new List<AllEquipments>();
            lstModelDetails = new List<ModelDetails>();

            objOptionsEquipmentDetails = new OptionsEquipmentDetails();
            objOptionsEquipmentDetails.searchText = string.Empty;
            objOptionsEquipmentDetails.searchBy = string.Empty;
            objOptionsEquipmentDetails.is_active = true;

            lstModelStatusCount = new List<ModelStatusCount>();
        }
    }

    public class MiddleWareEntity
    {
        public string Layer_Title { get; set; }
        public string Layer_Name { get; set; }
        public string Layer_Abbr { get; set; }
    }

    public class AllEquipments
    {
        public int Modelcount { get; set; }
        public string ModelTypeName { get; set; }
        public int model_id { get; set; }
        public string model { get; set; }
    }

    public class ModelDetails
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int? manufacturer { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }

        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? status { get; set; }
        public int totalRecords { get; set; }
        public string manufacturer_name { get; set; }
    }


    public class OptionsEquipmentDetails
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public Boolean is_active { get; set; }

    }

    public class OptionsModelType
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public Boolean is_active { get; set; }

    }

    public class OptionsRule
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public Boolean is_active { get; set; }

    }


    public class ModelStatusCount
    {
        public int? statuscount { get; set; }
        public string statusname { get; set; }


    }

    public class ISPModelInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        [DefaultValue(true)]
        public bool is_active { get; set; } 
        [Required]
        [MaxLength(50)]
        public string model_name { get; set; }
        [Required]
        public int model_id { get; set; }

        public int? model_type_id { get; set; }
        public int? model_image_id { get; set; }
        [Required]
        public double height { get; set; }
        [Required]
        public double width { get; set; }
        public double depth { get; set; }
        public double border_width { get; set; }
        public int item_template_id { get; set; }
        public double rotation_angle { get; set; }
        [Required]
        [DefaultValue(1)]
        public int status_id { get; set; }
        public double unit_size { get; set; }        
        public bool is_multi_connection { get; set; }
        public string border_color { get; set; }
        [NotMapped]
        public string model_master_name { get; set; }
        [NotMapped]
        public string model_type_master_name { get; set; }
        [NotMapped]
        public string image_data { get; set; }
        [NotMapped]
        public string color_code { get; set; }
        [NotMapped]
        public string stroke_code { get; set; }
        [NotMapped]
        public double child_x_pos { get; set; }
        [NotMapped]
        public double child_y_pos { get; set; }
        //[NotMapped]
        //public double rotation_angle { get; set; }
        [NotMapped]
        public int parent { get; set; }
        [NotMapped]
        public int map_id { get; set; }
        [NotMapped]
        public int model_view_id { get; set; }
        [NotMapped]
        public string alt_name { get; set; }
        [NotMapped]
        public bool isChangeMapping { get; set; }
        [NotMapped]
        public PageMessage page_message { get; set; }

        [NotMapped]
        public ISPModelTemplate model_template { get; set; }

        [NotMapped]
        public List<ISPModelStatusMaster> lstModelStatus { get; set; }

        [NotMapped]
        public List<ISPModelMaster> lstModel { get; set; }
        [NotMapped]
        public List<ISPModelTypeMaster> lstModelType { get; set; }


        [NotMapped]
        public List<AllEquipments> lstEquipments { get; set; }

        [NotMapped]
        public List<ModelDetails> lstModelDetails { get; set; }


        [NotMapped]
        public OptionsEquipmentDetails objOptionsEquipmentDetails { get; set; }


        [NotMapped]
        public List<ModelStatusCount> lstModelStatusCount { get; set; }

        [NotMapped]
        public List<ISPModelMapping> lstModelChildren { get; set; }

        [NotMapped]
        public string modeltypevalue { get; set; }

        [NotMapped]
        public string model { get; set; }

        [NotMapped]
        public string value { get; set; }

        [NotMapped] [DefaultValue(true)]
        public bool is_editable { get; set; }
        [NotMapped]
        public string specification { get; set; }

        [NotMapped]
        public string code { get; set; }
        [NotMapped]
        public int? no_of_port { get; set; }
        [NotMapped]
        public int? vendor_id { get; set; }
        [NotMapped]
        public string entity_type { get; set; }
        [NotMapped]
        public string font_size { get; set; }
        [NotMapped]
        public string font_color { get; set; }
        [NotMapped]
        public string text_orientation { get; set; }
        [NotMapped]
        public string background_image { get; set; }
        [NotMapped]
        public bool is_view_enabled { get; set; }
        [NotMapped]
        public string port_status { get; set; }
        [NotMapped]
        public int? model_color_id { get; set; }
        [NotMapped]
        public List<ISPModelColorMaster> lstLabelColor { get; set; }
        [NotMapped]
        public string font_weight { get; set; }
        [NotMapped]
        public int system_id { get; set; }
        public ISPModelInfo()
        {
            model_template = new ISPModelTemplate();
            lstModelStatus = new List<ISPModelStatusMaster>();
            lstModel = new List<ISPModelMaster>();
            lstModelType = new List<ISPModelTypeMaster>();

            lstEquipments = new List<AllEquipments>();
            lstModelDetails = new List<ModelDetails>();

            objOptionsEquipmentDetails = new OptionsEquipmentDetails();
            objOptionsEquipmentDetails.searchText = string.Empty;
            objOptionsEquipmentDetails.searchBy = string.Empty;
            objOptionsEquipmentDetails.is_active = true;

            lstModelStatusCount = new List<ModelStatusCount>();
            status_id = 1;
            //lstModelChildren = new List<ISPModelMapping>();
        }
    }

    public class ISPModelMapping
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        //public bool is_active { get; set; }
        public int parent_model_info_id { get; set; }
        public int model_view_id { get; set; }
        public int child_model_info_id { get; set; }
        public double child_x_pos { get; set; }
        public double child_y_pos { get; set; }
        public double rotation_angle { get; set; }
        public int parent_model_mapping_id { get; set; }
        public int super_parent_model_info_id { get; set; }
        public string model_name { get; set; }
        [NotMapped]
        public int parent_id { get; set; }
        [NotMapped]
        public double height { get; set; }
        [NotMapped]
        public double width { get; set; }
        [NotMapped]
        public string font_size { get; set; }
        [NotMapped]
        public string font_color { get; set; }
        [NotMapped]
        public string background_color { get; set; }
        [NotMapped]
        public string stroke_color { get; set; }
        [NotMapped]
        public string text_orientation { get; set; }
        [NotMapped]
        public string background_image { get; set; }
        [NotMapped]
        public int? model_color_id { get; set; }
        [NotMapped]
        public string font_weight { get; set; }
        [NotMapped]
        public bool isNewEquipment { get; set; }
        [NotMapped]
        public int ref_id { get; set; }
    }
    public class ISPModelImageMaster
    {
       
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }
        public int model_id { get; set; }
        public string image_data { get; set; }
        [NotMapped]
        public string model_name { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public List<ISPModelMaster> lstTypeModel { get; set; }
        public ISPModelImageMaster()
        {
            lstTypeModel = new List<ISPModelMaster>();
        }
    }
    public class ISPModelTypeMaster
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }
        public int? model_id { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public string color_code { get; set; }
        public string stroke_code { get; set; }
        public bool is_middleware_model_type { get; set; }
        public string type_abbr { get; set; }
        public int? model_color_id { get; set; }
        [NotMapped]
        public List<ISPModelTypeMaster> lstModelType { get; set; }

        [NotMapped]
        public bool has_type { get; set; }

        [NotMapped]
        public int totalRecords { get; set; }

        [NotMapped]
        public string modal_name { get; set; }

        [NotMapped]
        public OptionsModelType objOptionsModelType { get; set; }

        public ISPModelTypeMaster()
        {
            lstModelType = new List<ISPModelTypeMaster>();
            objOptionsModelType = new OptionsModelType();
            objOptionsModelType.searchText = string.Empty;
            objOptionsModelType.searchBy = string.Empty;
        }
    }

    public class CheckRulesExists
    {
        public string model_type_name { get; set; }
        public int rule_id { get; set; }
    }

    public class ISPModelStatusMaster
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }
        public string name { get; set; }
    }

    public class ISPModelRule
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }

        public int parent_model_id { get; set; }
        public int? parent_model_type_id { get; set; }
        public int child_model_id { get; set; }
        public int? child_model_type_id { get; set; }

        [NotMapped]
        public int totalRecords { get; set; }

        [NotMapped]
        public List<ISPModelMaster> lstModel { get; set; }
        [NotMapped]
        public List<MiddleWareEntity> lstMiddleWare { get; set; }

        [NotMapped]
        public List<ISPModelMaster> lstHasTypeModel { get; set; }

        [NotMapped]
        public List<ISPModelTypeMaster> lstModelType { get; set; }

        [NotMapped]
        public OptionsRule objOptionsRule { get; set; }

        [NotMapped]
        public List<ISPModelRule> lstModelRule { get; set; }

        [NotMapped]
        public string parent_model { get; set; }
        [NotMapped]
        public string parent_model_type { get; set; }
        [NotMapped]
        public string child_model { get; set; }
        [NotMapped]
        public string child_model_type { get; set; }


        public ISPModelRule()
        {
            lstModel = new List<ISPModelMaster>();
            lstModelType = new List<ISPModelTypeMaster>();
            objOptionsRule = new OptionsRule();
            objOptionsRule.searchText = string.Empty;
            objOptionsRule.searchBy = string.Empty;
            
            lstModelRule = new List<ISPModelRule>();
            is_active = true;
        }
    }

    public class ISPModelViewMaster
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }
        public string name { get; set; }

    }

    public class ISPModelLabelInfo
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }
        public double height { get; set; }
        public double width { get; set; }
        public string font_size { get; set; }
        public string font_color { get; set; }
        public string background_color { get; set; }
        public string stroke_color { get; set; }
        public string text_orientation { get; set; }
        public string background_image { get; set; }
      
        public string font_weight { get; set; }

    }

    public class ISPModelColorMaster
    {
        public int id { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public bool is_active { get; set; }
        public int model_id { get; set; }
        public string color_name { get; set; }
        public string fill_color_code { get; set; }
        public string outline_color_code { get; set; }

    }

    public class ViewIspModelImage : CommonGridAttributes
    {
        public List<ISPModelImageMaster> listISPModelImage { get; set; }
        public ViewIspModelImage()
        {
            listISPModelImage = new List<ISPModelImageMaster>();
        }

    }
}
