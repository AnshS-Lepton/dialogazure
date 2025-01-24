using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class FiberLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string link_id { get; set; }

        public string link_type { get; set; }
        [Required]
        public string link_name { get; set; }
        public string network_id { get; set; }
        public string start_point_type { get; set; }
        public string start_point_network_id { get; set; }
        public string start_point_location { get; set; }
        public string end_point_type { get; set; }
        public string end_point_network_id { get; set; }
        public string end_point_location { get; set; }
        public int no_of_lmc { get; set; }
        public string each_lmc_length { get; set; }
        public double total_route_length { get; set; }
        public double gis_length { get; set; }
        public double otdr_distance { get; set; }

        public int no_of_pair { get; set; }
        public string tube_and_core_details { get; set; }
        public double existing_route_length_otdr { get; set; }
        public double new_building_route_length { get; set; }
        public double otm_length { get; set; }
        public double otl_length { get; set; }
        public bool any_row_portion { get; set; }
        public string row_authority { get; set; }

        public int total_row_segments { get; set; }
        public double total_row_length { get; set; }
        public double total_row_reccuring_charges { get; set; }
        public DateTime? handover_date { get; set; }
        public DateTime? hoto_signoff_date { get; set; }
        public string remarks { get; set; } 
        public string fiber_link_status { get; set; }
        public int created_by { get; set; }
        public string service_id { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string main_link_type { get; set; }
        public string redundant_link_type { get; set; }

        public string main_link_id { get; set; }
        public string redundant_link_id { get; set; }
        [NotMapped]
        // public List<fiberLinkColumnsMapping> lstFiberLinkColumnsMapping { get; set; }
        public List<string> lstFiberLinkColumnsMapping { get; set; }
         
        [NotMapped]  
        public List<userName> lstUserName { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; } 
        [NotMapped]
        public List<layerDetail> lstStartPointType { get; set; }
        [NotMapped]
        public List<layerDetail> lstEndPointType { get; set; }

        [NotMapped]
        public List<DropDownMaster> lstLinkType { get; set; }

        [NotMapped]
        public List<DropDownMaster> lstPrefixType { get; set; }

        [NotMapped]
        public FiberLinkPrefix FiberLinkPrefix { get; set; }


        [NotMapped]
        public List<LibraryAttachment> lstFiberLinkAttachments { get; set; }
        [NotMapped]
        public int CreateFL { get; set; }
        [Column("link_prefix")]
        [JsonProperty("link_prefix")]
        public string Link_Prefix { get; set; }
        [NotMapped]
        public string Lst_Link_Prefix { get; set; }
        public FiberLink()
        {
            pageMsg = new PageMessage(); 
            lstUserName = new List<Models.userName>();
            lstFiberLinkAttachments = new List<LibraryAttachment>();
            lstLinkType = new List<DropDownMaster>();
            lstPrefixType = new List<DropDownMaster>();
            CreateFL = 0;
            FiberLinkPrefix = new FiberLinkPrefix();
        } 
        [NotMapped]
        public int user_role_id { get; set; }
    }

    public class FiberLinkFilter
    {
        public int link_id { get; set; }
        public int system_id { get; set; }
        public string Searchtext { get; set; }
        public string SearchbyText { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int customDate { get; set; }
        public int userid { get; set; }
        public string orderBy { get; set; } 
        public string lstAssignedTo { get; set; } 
        public string fiber_link_status { get; set; }
        public List<status> lstStatus { get; set; }
        //public List<FiberLinkGrid> lstFiberLinkDetails { get; set; }
        public List<dynamic> lstFiberLinkDetails { get; set; }
        public List<fiberLinkStatus> lstFiberLinkStatus { get; set; }
        public FiberLink objFiberLink { get; set; }
        [NotMapped]
         public List<fiberLinkColumnsMapping> lstSearchByColumns { get; set; }
        public  List<string> lstFiberLinkColumnsMapping { get; set; } 
        [NotMapped]
        public List<userName> lstUserName { get; set; }
        public List<Dictionary<string, string>> lstAllFiberLinkDetails { get; set; }
        [NotMapped]
        public int associatedCount { get; set; }
        public FiberLinkFilter()
        {
            //lstFiberLinkDetails = new List<Models.FiberLinkGrid>();
            lstFiberLinkDetails = new List<dynamic>();
            lstStatus = new List<Models.status>(); 
            lstFiberLinkStatus = new List<Models.fiberLinkStatus>();
            objFiberLink = new FiberLink();
            lstUserName = new List<Models.userName>();
            lstAllFiberLinkDetails = new List<Dictionary<string, string>>();
        }
    }
    public class fiberLinkStatus
    {
        public string fiber_link_status { get; set; }
        public int fiber_link_count { get; set; }
        public string color_code { get; set; }
        public bool isFiberLinkStatusChecked { get; set; } 
    }
    public class FiberLinkPrefix
    {
        public string link_prefix { get; set; }
        
    }
    public class FiberLinkGrid
    {
        public int system_id { get; set; }
        public string link_id { get; set; } 
        public string link_name { get; set; }
        public string network_id { get; set; }
        public string start_point_type { get; set; }
        public string start_point_network_id { get; set; }
        public string start_point_location { get; set; }
        public string end_point_type { get; set; }
        public string end_point_network_id { get; set; }
        public string end_point_location { get; set; }
        public int no_of_lmc { get; set; }
        public string each_lmc_length { get; set; }
        public double total_route_length { get; set; }
        public double gis_length { get; set; }
        public double otdr_distance { get; set; } 
        public int no_of_pair { get; set; }
        public string tube_and_core_details { get; set; }
        public double existing_route_length_otdr { get; set; }
        public double new_building_route_length { get; set; }
        public double otm_length { get; set; }
        public double otl_length { get; set; }
        public bool any_row_portion { get; set; }
        public string row_authority { get; set; } 
        public int total_row_segments { get; set; }
        public double total_row_length { get; set; }
        public double total_row_reccuring_charges { get; set; }
        public DateTime? handover_date { get; set; }
        public DateTime? hoto_signoff_date { get; set; }
        public string remarks { get; set; }
        public string fiber_link_status { get; set; }
        public string created_by { get; set; } 
        public DateTime created_on { get; set; }
        public string modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int totalrecords { get; set; }
    }
    public class startEndEntityType
    {
        public int layer_id { get; set; }
        public string entity_type { get; set; }
    }
    public class fiberLinkColumnsMapping
    {
        public int id { get; set; }
      //  public string setting_type { get; set; }
        public string column_name { get; set; }
        public  string display_name { get; set; }
        public int column_sequence { get; set; }
      //  public string table_name { get; set; }
        public bool is_active { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; } 
    }
    public class startPointType
    {
        public int layer_id { get; set; }
        public string layer_name { get; set; }
    }
    public class endPointType
    {
        public int layer_id { get; set; }
        public string layer_name { get; set; }
    }
    public class FLNetworkCode
    {
        public string network_id { get; set; }
        public string sequence_id { get; set; }
    }


    public class FiberLinkCustomerFilter
    {
        public int link_system_id { get; set; }
        public int system_id { get; set; }
        public string Searchtext { get; set; }
        public string SearchbyText { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int userid { get; set; }
        public string orderBy { get; set; }
        public string lstAssignedTo { get; set; }
        public string fiber_link_status { get; set; }
        public List<status> lstStatus { get; set; }
        public List<dynamic> lstFiberLinkCustomer { get; set; }      
        public List<userName> lstUserName { get; set; }
        public FiberLinkCustomerFilter()
        {
            lstFiberLinkCustomer = new List<dynamic>();
            lstUserName = new List<Models.userName>();
        }
    }
  
    public class vmCustomerAssociation
    {
        [Required]
        public string customerName { get; set; }
        public List<Customer> lstCustomer { get; set; }
        public vmCustomerAssociation()
        {
            lstCustomer = new List<Models.Customer>();
        }
    }

    public class vmfiberLinkOnMap
    {
        public List<cableInfo> lstCableInfo { get; set; }
        public List<connectedElements> lstConnectedElements { get; set; }
        public vmfiberLinkOnMap()
        {
            lstConnectedElements = new List<Models.connectedElements>();
            lstCableInfo = new List<cableInfo>();
        }
    }
    public class cableInfo
    {
        public int link_system_id { get; set; }
        public string link_network_id { get; set; }
        public string link_id {get;set;}
        public string link_name {get;set;}
        public int cable_system_id {get;set;}
        public string cable_network_id {get;set;}
        public string cable_type {get;set;}
        public string cable_geom { get; set; }
        public int fiber_number {get;set;}
        public string core_status {get;set;}
        public string core_comment {get;set;}
        public int? connected_system_id { get; set; }
        public string connected_network_id { get; set; }
        public int? connected_port_no { get; set; }
        public string connected_entity_type { get; set; }


    }
    public class connectedElements
    {
        public int connected_system_id { get; set; }
        public string connected_network_id { get; set; }
        public int? connected_port_no { get; set; }
        public string connected_entity_type { get; set; }
        public string connected_entity_geom { get;set;} 
        public bool is_virtual_port_allowed { get; set; }
        public string connected_entity_category { get; set; }
        public bool is_virtual { get; set; }
    }


    public class vmLinkAssociation
    {

        public string link_id { get; set; }
        public int link_system_id { get; set; }
        public int cable_id { get; set; }
        public int fiber_number { get; set; }
        public fiberLinkAssociation lstFiberLink { get; set; }
        public vmLinkAssociation()
        {
            lstFiberLink = new Models.fiberLinkAssociation();

        }
    }

    public class fiberLinkAssociation
    {
        public string link_id { get; set; }
        public int link_system_id { get; set; }
        public int cable_id { get; set; }
        public int fiber_number { get; set; }  
    }
    public class TempFiberLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
       
        public string link_id { get; set; }

        public string link_type { get; set; }
        [Required]
        public string link_name { get; set; }
        public string network_id { get; set; }
        public string start_point_type { get; set; }
        public string start_point_network_id { get; set; }
        public string start_point_location { get; set; }
        public string end_point_type { get; set; }
        public string end_point_network_id { get; set; }
        public string end_point_location { get; set; }
        public int no_of_lmc { get; set; }
        public string each_lmc_length { get; set; }
        public double total_route_length { get; set; }
        public double gis_length { get; set; }
        public double otdr_distance { get; set; }

        public int no_of_pair { get; set; }
        public string tube_and_core_details { get; set; }
        public double existing_route_length_otdr { get; set; }
        public double new_building_route_length { get; set; }
        public double otm_length { get; set; }
        public double otl_length { get; set; }
        public bool any_row_portion { get; set; }
        public string row_authority { get; set; }

        public int total_row_segments { get; set; }
        public double total_row_length { get; set; }
        public double total_row_reccuring_charges { get; set; }
        public DateTime? handover_date { get; set; }
        public DateTime? hoto_signoff_date { get; set; }
        public string remarks { get; set; }
        public string fiber_link_status { get; set; }
        public int created_by { get; set; }
        public string service_id { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string main_link_type { get; set; }
        public string redundant_link_type { get; set; }

        public string main_link_id { get; set; }
        public string redundant_link_id { get; set; }
        public string link_prefix { get; set; }
        public TempFiberLink()
        {
            
        }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
    }
    public class count
    {
        public int count_id { get; set; }
    }
    public class FiberLinkDetail
    {
        public string system_id { get; set; }

        [JsonProperty("Network ID")]
        public string Network_Id { get; set; }

        [JsonProperty("Link/Route ID")]
        public string Link_Id { get; set; }

        [JsonProperty("Link/Route Name")]
        public string Link_Name { get; set; }
        [JsonProperty("Link Type")]
        public string Link_Type { get; set; }
    }

    public class FiberLinksFilter
    {
        public List<FiberLinkDetail> LstFiberLinkDetails { get; set; }
        public int TotalRecord { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public string OrderBy { get; set; }
        public string SearchText { get; set; }
        public string S_No { get; set; }
    }
}
