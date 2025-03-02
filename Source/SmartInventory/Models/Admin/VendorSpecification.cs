using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class ItemVendorCostMaster 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }       
        public int layer_id { get; set; }         
        public string specification { get; set; }       
        public string item_code { get; set; }     
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string uom { get; set; }
        public int user_id { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Item Cost must be a positive number.")]
         // Or use decimal instead of double
        public decimal item_cost { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
    
        public ItemVendorCostMaster()
        {
            pageMsg = new PageMessage();
            
        }

    }
    public class AuditItemVendorCostMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int item_cost_id { get; set; }
        public int layer_id { get; set; }
        public string specification { get; set; }
        public string item_code { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string uom { get; set; }
        public int user_id { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Item Cost must be a positive number.")]
        // Or use decimal instead of double
        public decimal item_cost { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }

        public AuditItemVendorCostMaster()
        {
            pageMsg = new PageMessage();

        }

    }
    public class SiteAwardDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string item_code { get; set; }
        public string specification { get; set; }
        public string uom { get; set; }
        public int user_id { get; set; }
        public int item_cost_audit_id { get; set; }
        public int site_plan_id { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }

        public SiteAwardDetails()
        {
            pageMsg = new PageMessage();

        }


    }
    public class VendorSpecificationMaster : TemplateForDropDownVendorSpec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int layer_id { get; set; }
        public string category_reference { get; set; }
        public string subcategory_1 { get; set; }
        public string subcategory_2 { get; set; }
        public string subcategory_3 { get; set; }
        [Required]
        public string specification { get; set; }
        [Required]
        public string code { get; set; }
        [Required]
        public int vendor_id { get; set; }
        public Boolean is_active { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public int key { get; set; }
        [NotMapped]
        public string unit_input_type { get; set; }
        public int no_of_port { get; set; }
        public int no_of_input_port { get; set; }
        public int no_of_output_port { get; set; }
        public string other { get; set; }
        public string unit { get; set; }
        public string iop_value { get; set; }
        public string unit_measurement { get; set; }
        public Boolean is_arfs { get; set; }
        public Boolean is_brfs { get; set; }
        public Boolean is_crfs { get; set; }
        public string item_type { get; set; }
        public double cable_length { get; set; }
        public string cable_type { get; set; }
        public string specify_type { get; set; }
        public double? length { get; set; }
        public double? width { get; set; }
        public double? height { get; set; }
        public double? border_width { get; set; }
        public string border_color { get; set; }

        public int? no_of_units { get; set; }

        public int accessories_id { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayerDetails { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstIOPDetails { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal cost_per_unit { get; set; }
        [NotMapped]
        public string layer_name { get; set; }
        [Required]
        public double service_cost_per_unit { get; set; }
        [NotMapped]
        public List<VendorSpecificationCategory> categories { get; set; }
        [NotMapped]
        public List<VendorSpecificationCategory> entity_categories { get { return categories.Where(x => x.layer_id > 0).OrderBy(x => x.display_name).ToList(); } }
        [NotMapped]
        public List<VendorSpecificationCategory> other_categories { get { return categories.Where(x => x.layer_id == 0).OrderBy(x => x.display_name).ToList(); } }

        [NotMapped]
        public string category_key { get { return string.Format("{0}-{1}", layer_id, accessories_id); } }
        [NotMapped]
        public int audit_id { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listItemType { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listUnitMeasurement { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSpecifyType { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstItemSpec { get; set; }
        [NotMapped]
        public string is_specification_allowed { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstNoOfWays { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstSplitterType { get; set; }
        //[NotMapped]
        //public IList<DropDownMaster> lstCableCategory { get; set; }
        public Boolean is_master { get; set; }
        public string item_master_code { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listItemMasterCode { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> listItemCode { get; set; }
        [NotMapped]
        public List<IvcKeyValueDropDown> listItemCategory { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal extra_percentage { get; set; }
        public int? no_of_tube { get; set; }
        public int? no_of_core_per_tube { get; set; }
        [NotMapped]
        public List<FormInputSettings> formInputSettings { get; set; }
        public bool is_default { get; set; }
        [NotMapped]
        public int totalRecord { get; set; }
            [NotMapped]
            public string user_name { get; set; }
        [NotMapped]
        public List<User> lstUserDetails { get; set; }
        [NotMapped]
        public string ItemSpec { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public decimal totalqty { get; set; }
        public VendorSpecificationMaster()
        {
            pageMsg = new PageMessage();
            lstLayerDetails = new List<layerDetail>();
            lstIOPDetails = new List<DropDownMaster>();
            is_active = true;
            is_master = true;
            categories = new List<Admin.VendorSpecificationCategory>();
            listItemType = new List<KeyValueDropDown>();
            listUnitMeasurement = new List<KeyValueDropDown>();
            lstSpecifyType = new List<KeyValueDropDown>();
            listItemMasterCode = new List<KeyValueDropDown>();
            formInputSettings = new List<FormInputSettings>();
            extra_percentage = 0;
            lstItemSpec = new List<KeyValueDropDown>();
            listItemCode = new List<KeyValueDropDown>();
            lstUserDetails = new List<User>();
        }

    }

    public class ViewItemVendorCost
    {
        public List<VendorSpecificationMaster> lstItem { get; set; }
        public List<MergedItem> lstItem1 { get; set; }
        public CommonGridAttr objGridAttributes { get; set; }
        [NotMapped]
        public List<parentuser> lstUserDetails { get; set; }
        [NotMapped]
        public string user_name { get; set; }
        [NotMapped]
        public string user_id { get; set; }
        public ViewItemVendorCost()
        {
            objGridAttributes = new CommonGridAttr();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
            lstUserDetails = new List<parentuser>();
        }
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }
    }

    public class parentuser
    {
        public string user_name { get; set; }
        public int user_id { get; set; }
    }
    public class CombineCableGeom
    {
        public bool status { get; set; }
        public string combine_geom { get; set; }
        public string network_id { get; set; }
        public int system_id { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
    }
    
    public class MergedItem
    {
        //[Range(0, double.MaxValue)]
        //public decimal item_cost { get; set; }
        public string code { get; set; }
        public string uam { get; set; }
        public string specification { get; set; }
        public string entity_type { get; set; }
        public int user { get; set; }
        public int cost { get; set; }



    }
    public class CommonGridAttr
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public Boolean is_active { get; set; }
        public string application_access { get; set; }
        public string fileType { get; set; }
        public int customDate { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int history_id { get; set; }
        public string log_type { get; set; }
        public string SelectedRegionIds { get; set; }
        public CommonGridAttr()
        {
            sort = "";
            orderBy = "";
        }
    }


    public class ViewVendorSpecificationDetailsList : TemplateForDropDownVendorSpec
    {
        public IList<ViewVendorSpecificationList> VendorDetailSpecificationList { get; set; }
        public ViewVendorSpecficationDetail viewVendorSpecificationDetail { get; set; }


        public ViewVendorSpecificationDetailsList()
        {
            viewVendorSpecificationDetail = new ViewVendorSpecficationDetail();

            viewVendorSpecificationDetail.searchText = string.Empty;
            viewVendorSpecificationDetail.searchBy = string.Empty;
            viewVendorSpecificationDetail.is_active = true;
        }

    }

    public class ViewSpecificationServiceDetailsList : TemplateForDropDownVendorSpec
    {
        public IList<ViewSpecificationServiceList> DetailSpecificationServiceList { get; set; }
        public ViewSpecficationServiceDetail viewSpecificationServiceDetail { get; set; }


        public ViewSpecificationServiceDetailsList()
        {
            viewSpecificationServiceDetail = new ViewSpecficationServiceDetail();

            viewSpecificationServiceDetail.searchText = string.Empty;
            viewSpecificationServiceDetail.searchBy = string.Empty;
            viewSpecificationServiceDetail.is_active = true;
        }

    }



    //[Serializable]
    public class ViewVendorSpecficationDetail
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


    public class ViewSpecficationServiceDetail
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public bool is_active { get; set; }
    }

    public class ViewSpecificationServiceList
    {
        public ViewSpecificationServiceList()
        {
            pageMsg = new PageMessage();
            categories = new List<Admin.VendorSpecificationCategory>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public int layer_id { get; set; }
        public int item_template_id { get; set; }
        public string service_name { get; set; }
        public decimal? service_cost { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool is_active { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        public string entity_type { get; set; }
        [NotMapped]
        public string specification { get; set; }
        public string code { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstspecification { get; set; }
        [NotMapped]
        public List<VendorSpecificationCategory> categories { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<VendorSpecificationCategory> entity_categories { get { return categories.Where(x => x.layer_id > 0).OrderBy(x => x.display_name).ToList(); } }
        [NotMapped]
        public string category_key { get { return string.Format("{0}-{1}", layer_id, accessories_id); } }

        [NotMapped]
        public List<VendorSpecificationCategory> other_categories { get { return categories.Where(x => x.layer_id == 0).OrderBy(x => x.display_name).ToList(); } }
        [NotMapped]
        public string layer_name { get; set; }
        [NotMapped]
        public int accessories_id { get; set; }
    }

    public class ViewVendorSpecificationList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string entity_type { get; set; }
        public string specify_type { get; set; }
        public string subcategory_1 { get; set; }
        public string subcategory_2 { get; set; }
        public string subcategory_3 { get; set; }
        public string specification { get; set; }
        public string code { get; set; }
        public int vendor_id { get; set; }
        public decimal cost_per_unit { get; set; }
        public string vendor_name { get; set; }
        public bool is_active { get; set; }
        public int totalRecords { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string created_by_text { get; set; }
        public string modified_by_text { get; set; }
        //public string port_ratio { get; set; }
        public string iop_value { get; set; }
        public double service_cost_per_unit { get; set; }
        public string Item_type { get; set; }
        public string unit_measurement { get; set; }
        public bool is_arfs { get; set; }
        public bool is_brfs { get; set; }
        public bool is_crfs { get; set; }


    }

    public class TemplateForDropDownVendorSpec
    {
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstEntityDetails { get; set; }


        [NotMapped]
        public List<KeyValueDropDown> lstAllVendor { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchByServices { get; set; }

    }

    public class dropdown_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int layer_id { get; set; }
        public string dropdown_type { get; set; }
        public string dropdown_value { get; set; }
        public int? created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string dropdown_key { get; set; }
        public bool dropdown_status { get; set; }

        public string db_column_name { get; set; }
        public bool is_action_allowed { get; set; }


        public bool is_active { get; set; }


        [NotMapped]
        public int inputport { get; set; }

        [NotMapped]
        public int outputport { get; set; }

        [NotMapped]
        public string entity_name { get; set; }


        [NotMapped]
        public PageMessage pageMsg { get; set; }
        public string parent_value { get; set; }
        public int parent_id { get; set; }
        public dropdown_master()
        {
            pageMsg = new PageMessage();
        }

    }
    public class RCA_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string RCA { get; set; }
        //[Required(ErrorMessage = "Please enter RCA")]
        public string status { get; set; }
        //[Required(ErrorMessage = "Please enter RC")]
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        public int totalRecords { get; set; }
        [NotMapped]
        public DateTime? modified_on { get; set; }
        public PageMessage pageMsg { get; set; }

        public RCA_master()
        {
            pageMsg = new PageMessage();

        }

    }

    public class VendorSpecificationCategory
    {
        public int layer_id { get; set; }
        public int accessories_id { get; set; }
        public string display_name { get; set; }
        public string category_key { get { return string.Format("{0}-{1}", layer_id, accessories_id); } }
        public string unit_input_type { get; set; }
    }

    public class TempVendorSpecificationMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string subcategory_1 { get; set; }
        public string subcategory_2 { get; set; }
        public string subcategory_3 { get; set; }
        public string specification { get; set; }
        public string code { get; set; }
        public string is_active { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public List<VendorSpecificationCategory> categories { get; set; }
        [NotMapped]
        public string unit_input_type { get; set; }
        public string no_of_port { get; set; }
        public string no_of_input_port { get; set; }
        public string no_of_output_port { get; set; }
        public string other { get; set; }
        public string unit { get; set; }
        public string iop_value { get; set; }
        public string unit_measurement { get; set; }
        public string is_arfs { get; set; }
        public string is_brfs { get; set; }
        public string is_crfs { get; set; }
        public string item_type { get; set; }
        public double? cable_length { get; set; }
        public string cable_type { get; set; }
        public int vendor_id { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string border_width { get; set; }
        public string border_color { get; set; }
        public int layer_id { get; set; }

        public string no_of_units { get; set; }
        public string cost_per_unit { get; set; }
        public string entity_type { get; set; }
        public string layer_name { get; set; }
        public string service_cost_per_unit { get; set; }
        public Boolean is_valid { get; set; }
        public string error_msg { get; set; }
        public string vendor { get; set; }
        public string specify_type { get; set; }
        public TempVendorSpecificationMaster()
        {
            categories = new List<Admin.VendorSpecificationCategory>();

        }


    }

    public class UploadVendorSpecificationImageDocVM
    {
        public int specification_id { get; set; }
        public List<UploadVendorSpecificationImageDoc> lstImages { get; set; }
        public List<UploadVendorSpecificationImageDoc> lstDocuments { get; set; }
        public List<UploadVendorSpecificationImageDoc> lstRefLinks { get; set; }
        public UploadVendorSpecificationImageDocVM()
        {
            lstImages = new List<UploadVendorSpecificationImageDoc>();
            lstDocuments = new List<UploadVendorSpecificationImageDoc>();
            lstRefLinks = new List<UploadVendorSpecificationImageDoc>();
        }

    }

    public class UploadVendorSpecificationImageDoc
    {
        public int id { get; set; }
        public int specification_id { get; set; }
        [NotMapped]
        public string ImgSrc { get; set; }
        public string org_file_name { get; set; }
        public int uploaded_by { get; set; }
        public string file_name { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public int file_size { get; set; }
        [NotMapped]
        public string file_size_converted { get; set; }
        [NotMapped]
        public Nullable<double> longitude { get; set; }
        public DateTime uploaded_on { get; set; }
        [NotMapped]
        public string created_on { get; set; }
        [NotMapped]
        public string uploaded_by_name { get; set; }
        public string file_extension { get; set; }
        [NotMapped]
        public int item_cost { get; set; }

    }
    public class SpecicifationImgDocDownload
    {
        public int systemId { get; set; }
        public string name { get; set; }
        public string location { get; set; }
    }
}
