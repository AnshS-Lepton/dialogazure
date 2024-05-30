using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Models
{
    //public class Roles
    //{
    //    public int LayerID { get; set; }
    //    public string Action { get; set; }
    //    public string Network { get; set; }
    //    public string Type { get; set; }
    //}

    //public class RoleLayersMapping
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    public int role_id { get; set; }
    //    public int layer_id { get; set; }
    //    public bool add_planned { get; set; }
    //    public bool update_planned { get; set; }
    //    public bool delete_planned { get; set; }
    //    public bool view_planned { get; set; }
    //    public bool add_asbuilt { get; set; }
    //    public bool update_asbuilt { get; set; }
    //    public bool delete_asbuilt { get; set; }
    //    public bool view_asbuilt { get; set; }
    //    public bool add_dormant { get; set; }
    //    public bool update_dormant { get; set; }
    //    public bool delete_dormant { get; set; }
    //    public bool view_dormant { get; set; }
    //    public int created_by { get; set; }
    //    public DateTime? created_on { get; set; }
    //    public int modified_by { get; set; }
    //    public DateTime? modified_on { get; set; }
    //    [NotMapped]
    //    public string layer_name { get; set; }
    //}
    public class RoleMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int role_id { get; set; }
        [Required]
        public string role_name { get; set; }

        public int reporting_role_id { get; set; }
        [NotMapped]
        public string reporting_role_name { get; set; }
        public Boolean is_active { get; set; }
        [Required]
        public string remarks { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public List<LayerRightsTemplatePermission> lstTemplatePermission { get; set; }
        [NotMapped]
        public bool isLayerSelected { get; set; }
        [NotMapped]
        public bool isDisabled { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstRM { get; set; }
        [NotMapped]
        public List<TicketTypeRoleMapping> lstTemplateTicketTypePermission { get; set; }

        public RoleMaster()
        {
            is_active = true;
            lstTemplatePermission = new List<LayerRightsTemplatePermission>();
           lstTemplateTicketTypePermission = new List<TicketTypeRoleMapping>();
        }

    }

    public class RoleModuleMapping
    {
        [Key]
        public int id { get; set; }
        public int role_id { get; set; }
        public int module_id { get; set; }
        [NotMapped]
        public List<UserModule> lstUserModule { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstRole { get; set; }

        [NotMapped]
        public List<RoleModuleMapping> lstRoleModuleMapping { get; set; }
        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstRoleServiceFacilityMapping { get; set; }
        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstRoleJobOrderMapping { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstRoleJobCategoryMapping { get; set; }
    }

    public class RoleSeviceFacilityMapping
    {
        [Key]
        public int id { get; set; }
        public int role_id { get; set; }
        public int service_facility_id { get; set; }
       
    }

    public class RoleJoTypeMapping
    {
        [Key]
        public int id { get; set; }
        public int role_id { get; set; }
        public int jo_type_id { get; set; }
        
    }


    public class UserSeviceFacilityMapping
    {
        [Key]
        public int id { get; set; }
        public int service_facility_id { get; set; }
        public int user_id { get; set; }
    }

    public class UserJoTypeMapping
    {
        [Key]
        public int id { get; set; }
        public int jo_type_id { get; set; }
        public int user_id { get; set; }
    }
    public class UserReportDetail
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string name { get; set; }
        public string user_email { get; set; }
        public string role_name { get; set; }
        public string mobile_number { get; set; }
        public string groupUser { get; set; }
        public bool isSelected { get; set; }
        [NotMapped]
        public int role_id { get; set; }
        [NotMapped]
        public string parentUser_ids { get; set; }
    }
    public class UserReportMapping
    {
        [Key]
        public int id { get; set; }
        public int parent_user_id { get; set; }
        public int child_user_id { get; set; }
    }
    public class UserReportMappingVw
    {
        public int parent_user_id { get; set; }
        public int child_user_id { get; set; }
    }

    public class UserReportMappingVM
    {
        public int user_id { get; set; }
        [NotMapped]
        public List<UserDetail> lstUser { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<UserReportDetail> UserReportMappingList { get; set; }
        [NotMapped]
        public bool is_all_users_mapped { get; set; }
        public UserReportMappingVM()
        {
            lstUser = new List<UserDetail>();
            pageMsg = new PageMessage();
            UserReportMappingList = new List<UserReportDetail>();
        }
    }

    [Serializable]
    public class RoleViewModel
    {

        [Required]
        public int template_id { get; set; }
        public RoleMaster objRoleMaster { get; set; }
        public bool isLayerSelected { get; set; }
        public List<SelectListItem> templateList { get; set; }
        public List<SelectListItem> lstRoles { get; set; }
        public List<RoleMaster> lstRoleMaster { get; set; }
        public List<NetworkLayer> lstNetworkLayers { get; set; }
        public List<RolePermissionEntity> lstRoleLayersMapping { get; set; }
        public List<LayerRightsTemplatePermission> lstTemplatePermission { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }

        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        [NotMapped]
        public string multi_reporting_role_ids { get; set; }
        [NotMapped]
        public bool is_multi_role_allowed { get; set; }
        public List<TicketTypeRoleMapping> lstTemplateTicketTypePermission { get; set; }
        [NotMapped]
        public bool IsCrossDomainAllowed { get; set; }
        public RoleViewModel()
        {

            objRoleMaster = new RoleMaster();
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
            lstRoleLayersMapping = new List<RolePermissionEntity>();
            lstRoleMaster = new List<RoleMaster>();
            pageMsg = new PageMessage();
            lstTemplatePermission = new List<LayerRightsTemplatePermission>();
            lstTemplateTicketTypePermission = new List<TicketTypeRoleMapping>();
        }
    }

    #region Role

    //public class RoleMaster_Grid
    //{

    //    public int role_id { get; set; }
    //    public string role_name { get; set; }
    //    public Boolean is_active { get; set; }
    //    public DateTime? created_on { get; set; }
    //    public string remarks { get; set; }
    //    public int line_number { get; set; }
    //    public int total { get; set; }

    //}


    public class RolePermissionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int role_id { get; set; }
       
        public int layer_id { get; set; }
        public bool add { get; set; }
        public bool edit { get; set; }
        public bool delete { get; set; }
        public bool viewonly { get; set; }
        public string network_status { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }

        [NotMapped]
        public string entity_name { get; set; }
        [NotMapped]
        public bool add_planned { get; set; }
        [NotMapped]
        public bool update_planned { get; set; }
        [NotMapped]
        public bool delete_planned { get; set; }
        [NotMapped]
        public bool view_planned { get; set; }
        [NotMapped]
        public bool add_asbuilt { get; set; }
        [NotMapped]
        public bool update_asbuilt { get; set; }
        [NotMapped]
        public bool delete_asbuilt { get; set; }
        [NotMapped]
        public bool view_asbuilt { get; set; }
        [NotMapped]
        public bool add_dormant { get; set; }
        [NotMapped]
        public bool update_dormant { get; set; }
        [NotMapped]
        public bool delete_dormant { get; set; }
        [NotMapped]
        public bool view_dormant { get; set; }
    }
    #endregion


    public class UserManagerMapping
    {
        [Key]
        public int id { get; set; }
        public int manager_id { get; set; }
        public int user_id { get; set; }
    }

   
    public class FE_Tools_Details
    {
        public int id { get; set; }
        public string user_name { get; set; }
        public string tool_name { get; set; }
        public string serial_number { get; set; }
        public string barcode { get; set; }
        public DateTime date_value { get; set; }
        public string is_accepted {  get; set; }
        [NotMapped]
        public int? created_by { get; set; }
        [NotMapped]
        public int user_id { get; set; }

      





        public string upload_type { get; set; }
        
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }


    }
    public class userFeToolMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public int tool_id { get; set; }
        [NotMapped]
        public string tool_name { get; set; }
        [NotMapped]
        public string user_name { get; set; }
        [NotMapped]
        public string fe_tool { get; set; }
        [Required(ErrorMessage = "Serial Number is required.")]
        public string serial_number { get; set; }
        
        [Required(ErrorMessage = "Barcode is required.")]
        public string barcode { get; set; }

        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public DateTime? date_value { get; set; }
        [NotMapped]
        public string date_v { get; set; }
      

      

        [NotMapped]
        public List<KeyValueDropDown> lstusername { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstFEtool { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public string action_type { get; set; }
    }
    public class FE_Tools
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }
        public int user_id { get; set; }
        public int tool_id { get; set; }
        [NotMapped]
        public string user_name { get; set; }
        [NotMapped]
        public string fe_tool { get; set; }
        [NotMapped]
        public string tool_name { get; set; }
        public string serial_number { get; set; }
        public string barcode { get; set; }
        public string certificate { get; set; }

        public string image_path { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public DateTime? date_value { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstusername { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstFEtool { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public string action_type { get; set; }


    }

    public class ViewFETools
    {
        public List<FE_Tools_Details> fetools { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewFETools()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        
    }

    public class ReportingRoleMapping
    {
        [Key]
        public int id { get; set; }
        public int role_id { get; set; }
        public int reporting_role_id { get; set; }
    }
    public class TicketTypeRoleMapping
    {
        [Key]
        public int id { get; set; }
        public int ticket_type_id { get; set; }
        public int role_id { get; set; }
        public Boolean is_create { get; set; }
        public Boolean is_edit { get; set; }
        public Boolean is_view { get; set; }
        public Boolean is_approve { get; set; }
        public string ticket_type { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
    }

    public class UserWarehouseCodeMapping
    {
        [Key]
        public int id { get; set; }
        public string warehouse_code { get; set; }
        public int user_id { get; set; }
    }

    public class UserJoCategoryMapping
    {
        [Key]
        public int id { get; set; }
        public int jo_category_id { get; set; }
        public int user_id { get; set; }
    }

    public class RoleJoCategoryMapping
    {
        [Key]
        public int id { get; set; }
        public int role_id { get; set; }
        public int jo_category_id { get; set; }

    }
}
