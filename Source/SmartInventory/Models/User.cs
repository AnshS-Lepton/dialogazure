using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Data;
using Models.Admin;

namespace Models
{
    public class UserDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public string user_email { get; set; }
        public string mobile_number { get; set; }
        public bool isactive { get; set; }
        public int manager_id { get; set; }
        public int role_id { get; set; }
        public int module_id { get; set; }
        public string user_img { get; set; }
        public int template_id { get; set; }
        public int group_id { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string remarks { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }

        public int totalRecords { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string created_by_text { get; set; }
        public string modified_by_text { get; set; }
        public string role_name { get; set; }
        public string user_type { get; set; }
        [NotMapped]
        public int S_No { get; set; }
        [NotMapped]
        public int history_id { get; set; }

        public string application_access { get; set; }
        [NotMapped]
        public string reporting_manager { get; set; }
    }
    public class userName
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
    }


    public class UserLoginHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public int totalRecords { get; set; }
        public int S_No { get; set; }
        public string user_name { get; set; }
        public string role_name { get; set; }
        public string user_email { get; set; }
        public string client_ip { get; set; }
        public string server_ip { get; set; }
        public string application_access { get; set; }
        public string source { get; set; }
        public string browser_name { get; set; }
        public string browser_version { get; set; }
        public string login_time { get; set; }
        public string logout_time { get; set; }
        public string is_active { get; set; }
        
        public string mobile_number { get; set; }  
    }

    public class UserLoginHistoryInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int history_id { get; set; }
        public int user_id { get; set; }
        public int login_id { get; set; }
        public string client_ip { get; set; }
        public string server_ip { get; set; }
        public string source { get; set; }
        public string browser_name { get; set; }
        public string browser_version { get; set; }
        public string mac_address { get; set; }
        public DateTime? login_time { get; set; }
        public DateTime? logout_time { get; set; }
        public string access_token { get; set; }
        public string signout_type { get; set; }
    }


    [Serializable]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        [Required]
        [RegularExpression(@"^[^<>,?;:'()!~%\@#/*""\s]+$", ErrorMessage = "Only dot and underscore are allowed")]
        public string user_name { get; set; }
        [Required]
        //[StringLength(40, MinimumLength = 8, ErrorMessage = "Password should have minimum 8 characters!")]
        //[RegularExpression(@"^\.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$", ErrorMessage = "Password Not Meeting Specific Requirement!")]
        public string password { get; set; }
        [Required]
        public string name { get; set; }
        [Required]


        //[RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Invalid Email Address!")]
        [Display(Name = "Email Id")]
        public string user_email { get; set; }
        [Required]
        public int manager_id { get; set; }
        
        [Required]
        public int role_id { get; set; }
        public List<int> module_id { get; set; }
        public string user_img { get; set; }
        public int template_id { get; set; }
        public int group_id { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string remarks { get; set; }
        [Required]
        public string mobile_number { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public string application_access { get; set; }
        [Required]
        public string user_type { get; set; }
        [NotMapped]
        public string groupUser { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        // public userModuleViewModel objUserModuleMaster { get; set; }
        [NotMapped]
        public List<UserModule> lstUserModule { get; set; }
        [NotMapped]
        public List<UserModuleMapping> lstUserModuleMapping { get; set; }
        [NotMapped]
        public DataTable dtUserModuleMapping { get; set; }
        [NotMapped]
        public List<SelectListItem> lstCountry { get; set; }
        [NotMapped]
        public List<SelectListItem> lstRegion { get; set; }
        [NotMapped]
        public string selectedCountry { get; set; }
        [NotMapped]
        public string selectedRegion { get; set; }
        [NotMapped]
        public string selectedProvinces { get; set; }
        [NotMapped]
        public List<UserPermissionArea> lstUserPermissionArea { get; set; }
        [NotMapped]
        public bool IsSubmit { get; set; } = false;
        [NotMapped]
        public bool IsUserinLimit { get; set; } = false;
        [NotMapped]
        public bool IsUserWebinLimit { get; set; } = false;
        [NotMapped]
        public bool IsUserMobileinLimit { get; set; } = false;

        public bool is_admin_rights_enabled { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstUserType { get; set; }
        public int? jc_id { get; set; }
        [NotMapped]
        public List<FormInputSettings> formInputSettings { get; set; }
        public User()
        {
            pageMsg = new PageMessage();
            is_active = true;
            lstUserModuleMapping = new List<UserModuleMapping>();
            lstUserModule = new List<UserModule>();
            lstUserPermissionArea = new List<UserPermissionArea>();
            dtUserModuleMapping = new DataTable();
            lstRegion = new List<SelectListItem>();
            formInputSettings = new List<FormInputSettings>();


        }
        [NotMapped]
        public List<KeyValueDropDown> lstRole { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstRM { get; set; }
        [NotMapped]
        public string role_name { get; set; }
        [NotMapped]
        public string userImgBytes { get; set; }
        [NotMapped]
        public string session_id { get; set; }

        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstRoleServiceFacilityMapping { get; set; }
        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstRoleJobOrderMapping { get; set; }
        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstViewServiceFacilityJobOrder { get; set; }
        [NotMapped]
        public List<ViewServiceFacilityJobOrder> lstRoleJobCategoryMapping { get; set; }

        public string company_name { get; set; }

        public string warehouse_code { get; set; }

        [NotMapped]
        public string selectedSubDistrict { get; set; }
        [NotMapped]
        public string selectedBlock { get; set; }
        [NotMapped]
        public bool is_multi_manager_allowed { get; set; }
        [NotMapped]
        public string multi_manager_ids { get; set; }
        [NotMapped]
        public string multi_tool_ids { get; set; }
        public double? capacity { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstWarehouseCode { get; set; }
        [NotMapped]
        public string multi_warhouse_code { get; set; }

        [NotMapped]
        public string warehouse_codes { get; set; }

        public string pan { get; set; }
        public string prms_id { get; set; }
        public string vendor_id { get; set; }
        public bool is_all_provience_assigned { get; set; }
        
        [NotMapped]
        public List<KeyValueDropDown> lstFEtool { get; set; }

    }

    public class UserApplicationLimit
    {
        public int total { get; set; }
        public string application_access { get; set; }
    }
    public class UserPermissionArea
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public int city_id { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        [NotMapped]
        public string countryName { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int? modified_by { get; set; }
        public string subdistrict_id { get; set; }
        public string block_id { get; set; }
    }
    public class UserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int login_id { get; set; }
        [Required]
        public int user_id { get; set; }
        public string client_ip { get; set; }
        public string server_ip { get; set; }
        [Required]
        public string source { get; set; }
        public string browser_name { get; set; }
        public string browser_version { get; set; }
        public string mac_address { get; set; }
        public string os_name { get; set; }
        public string os_version { get; set; }
        public string os_type { get; set; }
        public string mobile_app_version { get; set; }
        public string session_id { get; set; }
        public int history_id { get; set; }
        public string refresh_token { get; set; }
        public DateTime? login_time { get; set; }
        public DateTime? logout_time { get; set; }
        public string signout_type { get; set; }
        //public bool isMasterLogin { get; set; }
    }
    public class ADFSInput
    {
        public string user_name { get; set; }
        public string user_email { get; set; }
        public string password { get; set; }
        public string ADFSEndPoint { get; set; }
        public string ADFSRelPartyUri { get; set; }
        public string ADFSUserNamePreFix { get; set; }
        public string ADFSAutheticationBasedOn { get; set; }

        public string application_access { get; set; }
    }

   
    public class ADFSDetail
    {
        public string tokenId { get; set; }
        public DateTime validFrom { get; set; }
        public DateTime validTo { get; set; }
        public string errorMsg { get; set; }
        public Exception ADFSException { get; set; }

    }
   
    public class ViewUserModel
    {
        public int loginId_UserId { get; set; }
        public List<UserDetail> lstUsers { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewUserModel()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;

        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        public string application_access { get; set; }
    }

    public class UserLoginHistoryVM
    {
        //implemented by priyanka
        public int loginId_UserId { get; set; }
        //implemented by priyanka end
        public List<UserLoginHistory> lstUsers { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public UserLoginHistoryVM()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    public class UserLocationTracking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tracking_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        public int login_id { get; set; }
        [Required]
        public double latitude { get; set; }
        [Required]
        public double longitude { get; set; }
        public string network_provider { get; set; }
        public string entity_type { get; set; }
        public int? entity_system_id { get; set; }
        [Required]
        public DateTime mobile_time { get; set; }
        [NotMapped]
        public DateTime server_time { get; set; }
        [NotMapped]
        public string entity_network_id { get; set; }
        [NotMapped]
        public string entity_name { get; set; }
        [NotMapped]
        public string building_status { get; set; }
        public double location_accuracy { get; set; }
    }
    public class CheckUserDetails
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }
        public bool is_active { get; set; }
        public string user_email { get; set; }
        public string user_img { get; set; }
        public bool is_admin_rights_enabled { get; set; }
        public string user_type { get; set; }
        public string prms_id { get; set; }
        public string mobile_number { get; set; }
        public string application_access { get; set; }
        public bool is_all_provience_assigned { get; set; }
    }
    public class ValidateuserLogInDetails
    {
        public int user_id { get; set; }
        public string session_id { get; set; }
    }

    public class UserApiLoginHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int historyid { get; set; }
        public int userid { get; set; }
        public string requestip { get; set; }
        public string macaddress { get; set; }
        public string browsername { get; set; }
        public string browserversion { get; set; }
        public DateTime logindate { get; set; }

        public string osname { get; set; }
        public string osversion { get; set; }
        public string refreshtoken { get; set; }
    }

    [Serializable]
    public class BulkUserUpload
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public int user_id { get; set; }

        public string user_name { get; set; }
        public string password { get; set; }
        public string user_password { get; set; }

        [Required]
        public string name { get; set; }

        public string user_email { get; set; }
        [Required]
        public int manager_id { get; set; }
        [Required]
        public int role_id { get; set; }
        public List<int> module_id { get; set; }
        public string user_img { get; set; }
        public int template_id { get; set; }
        public int group_id { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string remarks { get; set; }
        [Required]
        public string mobile_number { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public int? modified_by { get; set; }
        public string application_access { get; set; }
        [NotMapped]
        public string groupUser { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }

        [NotMapped]
        public string selectedCountry { get; set; }
        [NotMapped]
        public string selectedRegion { get; set; }
        [NotMapped]
        public string selectedProvinces { get; set; }
        [NotMapped]
        public bool IsSubmit { get; set; } = false;
        [NotMapped]
        public bool IsUserinLimit { get; set; } = false;
        [NotMapped]
        public bool IsUserWebinLimit { get; set; } = false;
        [NotMapped]
        public bool IsUserMobileinLimit { get; set; } = false;

        public bool is_admin_rights_enabled { get; set; }


        [NotMapped]
        public List<KeyValueDropDown> lstRM { get; set; }

        public string role_name { get; set; }
        [NotMapped]
        public string userImgBytes { get; set; }
        [NotMapped]
        public string session_id { get; set; }
        public string company_name { get; set; }

        public string warehouse_code { get; set; }
        public string err_message { get; set; }
        public string reporting_manager { get; set; }

        public bool isvalid { get; set; }
        public string status { get; set; }

        public bool is_all_user_report_enabled { get; set; }
        public bool is_all_provience_assigned { get; set; }

        public string user_type { get; set; }
        [NotMapped]
        public string user_type_value { get; set; }
        public string pan { get; set; }
        public string prms_id { get; set; }
        public string vendor_id { get; set; }
        public string invalid_reporting_manager { get; set; }
        [NotMapped]
        public string multi_manager_ids { get; set; }
        [NotMapped]
        public bool is_multi_manager_allowed { get; set; }

    }

    #region BulkUserUpload
    public class BulkUserUploadSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string file_name { get; set; }
        public string status { get; set; }

        public int total_record { get; set; }
        [NotMapped]
        public int failed_record { get; set; }
        [NotMapped]
        public int success_record { get; set; }
        [NotMapped]
        public string status_message { get; set; }
        [NotMapped]
        public int s_no { get; set; }

        public string err_description { get; set; }
        [NotMapped]
        public DateTime? created_date { get; set; }

        [NotMapped]
        public int grid_total_record { get; set; }

        public int created_by { get; set; }
        [NotMapped]
        public string user_name { get; set; }
    }
    public class ViewBulkUserUploadSummary
    {
        public int loginId_UserId { get; set; }

        [NotMapped]
        public string access_token { get; set; }

        public List<BulkUserUploadSummary> lstBulkUserUploadSummary { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ViewBulkUserUploadSummary()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        public int totalRecords { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        [NotMapped]
        public bool IsSubmit { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }

    }
    public class BulkUserUploadModuleMapping
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public string user_name { get; set; }
        public string module_name { get; set; }
        public string value { get; set; }
        public string module_type { get; set; }
    }
    public class BulkUserUploadWorkAreaDetail
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public string user_name { get; set; }
        public string country_name { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string sub_district_name { get; set; }
        public string block_name { get; set; }
    }
    public class BulkUserUploadLimit
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
    public class BulkUserUploadModuleMasterMapping
    {
        public string module_name { get; set; }
        public string type { get; set; }
        public bool is_active { get; set; }
    }
    #region ankit
    public class BulkUserUploadManagerMapping
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public string user_name { get; set; }
        public string manager_name { get; set; }
        public int manager_id { get; set; }
    }
    public class BulkUserUploadJoTypeMapping
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public string user_name { get; set; }
        public string jo_name { get; set; }
        public string value { get; set; }
        public string module_type { get; set; }
    }
    public class BulkUserUploadJoTypeMasterMapping
    {
        public string jo_name { get; set; }
        public string type { get; set; }
        public bool is_active { get; set; }
    }
    public class BulkUserUploadJoCategoryMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public string user_name { get; set; }
        public string category_name { get; set; }
        public string value { get; set; }
        public string module_type { get; set; }
    }
    public class BulkUserUploadJoCategoryMasterMapping
    {
        public string category_name { get; set; }
        public string type { get; set; }
        public bool is_active { get; set; }
    }

    public class BulkUserUploadServiceFacilityMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_upload_id { get; set; }
        public string user_name { get; set; }
        public string service_facility_name { get; set; }
        public string value { get; set; }
        public string module_type { get; set; }
    }
    public class BulkUserUploadServiceFacilityMasterMapping
    {
        public string service_facility_name { get; set; }
        public string type { get; set; }
        public bool is_active { get; set; }
    }
    #endregion
    #endregion

    #region OTP Authentication  

    public class APIRootResponse
    {
        public string status { get; set; }
        public string error_message { get; set; }
        public object result { get; set; }

    }

    public class OTPResponse
    {
        public bool is_OTP_generated { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_email { get; set; }
        public int otp_resend_limit_left { get; set; }
        public string message { get; set; }
        public string footer_message { get; set; }
        public int resend_otp_timer { get; set; }
        public int alert_message_timeout { get; set; }
        public bool is_locked { get; set; }
        public int locked_timer { get; set; }
        public string mobile_no { get; set; }
        public string masked_mobile_no { get; set; }
        public int OTP_length { get; set; }
        public string OTP { get; set; }

    }

    public class VerifyOTPResponse
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public bool is_verified { get; set; }
        public string message { get; set; }
        public string footer_message { get; set; }
        public bool? is_locked { get; set; }
        public int locked_timer { get; set; }
        public int alert_message_timeout { get; set; }
    }

    public class NewOTPRequest
    {
        public string user_name { get; set; }
        public int user_id { get; set; }
        public string source { get; set; }
        [System.ComponentModel.DefaultValue("mobile")]
        public string otp_channel { get; set; }
    }

    public class ValidateOTPRequest
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string OTP { get; set; }
        public string source { get; set; }
    }

    #endregion

    public class UserOTPDetails
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string otp { get; set; }
        public DateTime otp_creation_time { get; set; }
        public int otp_expiry_time_sec { get; set; }
        public int resend_count { get; set; }
        public int failed_attempt_count { get; set; }
        public bool is_locked { get; set; }
        public string lock_type { get; set; }
        public DateTime lock_start_time { get; set; }
        public DateTime lock_end_time { get; set; }
        public DateTime last_attempt_time { get; set; }
    }

    public class SendOPTDeliveryOption
    {
        public string user_name { get; set; }
        public int user_id { get; set; }
        public string user_email { get; set; }
        public string mobile_number { get; set; }
        public string otptype { get; set; }
        public string mask_user_email { get; set; }
        public string mask_mobile_number { get; set; }
        public string otp { get; set; }
        public string message { get; set; }
        public string footer_message { get; set; }
        public int resend_otp_timer { get; set; }
        public bool is_locked { get; set; }
        public int OTP_length { get; set; }
    }


    public class ADOIDAuthentication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string source { get; set; }
        public string mobile { get; set; }
        public string email_id { get; set; }

    }

    public class SMSOTPRequest
    {
        public string referenceID { get; set; }
        public string templateID { get; set; }
        public string channelID { get; set; }
        public ContractMedium contactMedium { get; set; }
        public PayloadMessage payloadMessage { get; set; }

    }

    public class ContractMedium
    {
        public Characteristics characteristics { get; set; }
    }
   
    public class PayloadMessage
    {
        public Attributes attributes { get; set; }
        public string customerID { get; set; }
        public string orderRefNo { get; set; }
        public string registeredMobileNo { get; set; }
        public string serviceID { get; set; }
    }
    public class Characteristics
    {
        public List<CommonDataList> commonDataList { get; set; }
    }
    public class Attributes
    {
        public List<CommonDataList> commonDataList { get; set; }
    }
    public class CommonDataList
    {
        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

    public class validateUser
    {
        public string user_name { get; set; }
        public string user_token { get; set; }
        public string fsa_id { get; set; }
        public string integration_source { get; set; }
        
    }

    public class UserInfo
    {
        public List<User> lstUser { get; set; }
        public string name { get; set; }
        public string user_email { get; set; }
        public string mobile_number { get; set; }
        public int user_id { get; set; }

        public UserInfo()
        {
            lstUser = new List<User>();
        }
    }
    public enum EmailEventList
    {
        UserCreation,
        UserDeletion,
        PriceChange,
        PercentUtilization70,
        PercentUtilization50,
        SurveySubmitted,
        ProjectClosure,
        AcceptanceCompleted,
        ImplementationCompleted,
        MaterialRequestSubmitted,
        ManagerReview,
        DesignReviewed,
        DesignSubmitted,
        SurveyReviewed


    }

    public class GetOTPInternally
    {

        public string user_name { get; set; }
        public string otp { get; set; }
        public string organization { get; set; }
    }

    public class UserRegionProvinceFilter
    {
        public string lng { get; set; }
        public string lat { get; set; }

        public int userId { get; set; }

        public UserRegionProvinceFilter()
        {
            userId = 0;
        }

    }

    public class UserRegionProvince
    {
        public bool is_user_permission { get; set; }
        public int regionid { get; set; }
        public int provinceid { get; set; }

        public string regionname { get; set; }

        public string provincename { get; set; }



    }
   
    public class LDAPAuthentication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string source { get; set; }
        public string mobile { get; set; }
        public string email_id { get; set; }

    }
}





