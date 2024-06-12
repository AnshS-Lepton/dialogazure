using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Admin;

namespace Models
{
    public class Customer:IReference, IProjectSpecification,IGeographicDetails, ICustomCoordinate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required (ErrorMessage ="Can Id is required!")] 
        public string network_id { get; set; }
       
        public string customer_name { get; set; }
        public string structure_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }        
        public string address { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
       // [Required]
        public string pin_code { get; set; }
        public DateTime? activation_date { get; set; }
        public DateTime? deactivation_date { get; set; }
        public string activation_status { get; set; }      
        public int province_id { get; set; }
        public int region_id { get; set; }
        //[NotMapped]
        public string status { get; set; }
        public string remarks { get; set; }
		public bool is_new_entity { get; set; }
		public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; } 
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]      
        public string province_name { get; set; }
        [NotMapped]       
        public string region_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstActivationStatus { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public int templateId { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
         
        //[NotMapped]
        //public string building_code { get; set; }

        public string buildcode { get; set; } 
        [NotMapped]
        public int structure_id { get; set; }
        [NotMapped]
        public int floor_id { get; set; }
        [NotMapped]
        public bool isPortConnected { get; set; }
        [NotMapped]
        public string message { get; set; }

        public string rfstype { get; set; }

        public string customer_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstCustomerType { get; set; }
        public string service_type { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstServiceType { get; set; }
        [NotMapped]
        public List<string> SelectedServiceType { get; set; }
        public int site_id { get; set; }
        //[Required(ErrorMessage ="Customer Site Id is Required!")]
        public string customer_site_id { get; set; }

        public string lmc_type { get; set; } 
        //[Required(ErrorMessage = "Customer area is Required!")]
        public string customer_area { get; set; }
        //[Required(ErrorMessage = "OPCO is Required!")]
        public string opco { get; set; }
        //[Required(ErrorMessage = "Electrical Meter Type is Required!")]
        public string electrical_meter_type { get; set; }
        //[Required(ErrorMessage = "RTN Name is Required!")]
        public string rtn_name { get; set; }
        //[Required(ErrorMessage = "Small Cell Installed is Required!")]
        public int small_cell_installed { get; set; }
        //[Required(ErrorMessage = "AGL is Required!")]
        public int agl { get; set; }
        public bool is_power_back_up_available { get; set; }
        //[Required(ErrorMessage = "Power bakup capacity is Required!")]
        public int power_back_up_capacity { get; set; }
        //[Required(ErrorMessage = "Cluster Name is Required!")]
        public string cluster_name { get; set; }
         
        //[Required(ErrorMessage = "Cluster ID is Required!")]
        public string cluster_id { get; set; }
        //[Required(ErrorMessage = "PAF No is Required!")]
        public string paf_no { get; set; }
        //[Required(ErrorMessage = "PAF Signing Date is Required!")]
        public DateTime? paf_signing_date { get; set; }
        //[Required(ErrorMessage = "PAF Expiry Date is Required!")]
        public DateTime? paf_expiry_date { get; set; }
        //[Required(ErrorMessage = "Cable Entry Point is Required!")]
        public string cable_entry_point { get; set; }
        //[Required(ErrorMessage = "Remote POP is Required!")]
        public string remote_pop { get; set; }
        //[Required(ErrorMessage = "Order Tenure is Required!")]
        public int order_tenure { get; set; }
        //[Required(ErrorMessage = "Site Details is Required!")]
        public string site_details { get; set; }
        //[Required(ErrorMessage = "PO Number is Required!")]
        public string po_number { get; set; }
        //[Required(ErrorMessage = "PO Issue Date is Required!")]
        public DateTime? po_issue_date { get; set; }
        //[Required(ErrorMessage = "PO Expiry Date is Required!")]
        public DateTime? po_expiry_date { get; set; }
        //[Required(ErrorMessage = "RFAI Date is Required!")]
        public DateTime? rfai_date { get; set; }
        //[Required(ErrorMessage = "RFS Date is Required!")]
        public DateTime? rfs_date { get; set; }
        //[Required(ErrorMessage = "Contract Ends Date is Required!")]
        public DateTime? contract_end_date { get; set; }
        [NotMapped]
        public string Floor_Name { get; set; }
        [NotMapped]
        public string childModel { get; set; }
       
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Invalid Email Address!")]
        public string email_id { get; set; }
        [Required]
        [RegularExpression("^[0-9]{10,10}$", ErrorMessage = "Contact Number should have 10 digit!")]
        public string mobile_no { get; set; }
        public string phone_no { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstElectricalmeter { get; set; }

        [NotMapped]
        public List<DropDownMaster> lstCableEntryPoints { get; set; }
        [NotMapped]
        public List<LibraryAttachment> lstSiteCustomerAttachment { get; set; }
        [NotMapped]
        public List<StructureFloorInfo> lstFloorInfo { get; set; } 
        public List<dynamic> lstData { get; set; }

        public int? primary_pod_system_id { get; set; }
        public int? secondary_pod_system_id { get; set; }
        public string status_remark { get; set; }
        public string other_info { get; set; }
        [NotMapped]
        public vm_dynamic_form objDynamicControls { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string subarea_id { get; set; }
        public string area_id { get; set; }
        public string dsa_id { get; set; }
        public string csa_id { get; set; }
        public string gis_design_id { get; set; }
        [NotMapped]
        public string geographic_id { get; set; }
        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
        public string st_x { get; set; }
        public string st_y { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }

        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? workorder_id { get; set; }
        public int? purpose_id { get; set; }

        [NotMapped]
        public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
        [NotMapped]
        public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
        [NotMapped]
        public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
        [NotMapped]
        public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        [NotMapped]
        public List<RouteInfo> lstRouteInfo { get; set; }
        public Customer()
        {
            objPM = new PageMessage();
            objIspEntityMap = new IspEntityMapping();
            parent_entity_type = string.Empty;
            lstSiteCustomerAttachment = new List<LibraryAttachment>();
            lstData = new List<dynamic>();
            lstFloorInfo = new List<Models.StructureFloorInfo>();
            entityType = "Customer";
            lstUserModule = new List<string>();
            lstRouteInfo = new List<RouteInfo>();

        }
    } 
}
