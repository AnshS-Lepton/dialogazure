using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.WFM
{
    public class chartsfilter
    {
        public string user_name { get; set; }
        public string role_name { get; set; }
        public string action_name { get; set; }
        public int jomanagerId { get; set; }
        public string JoAssignedTo { get; set; }
        public string ticketsource { get; set; }
        public string DboardStatus { get; set; }
        public string DboardService { get; set; }
        public string DboardOrderType { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
    }
    public class charts
    {
        public string[] labels { get; set; }
        public int[] series { get; set; }
        public int sum { get; set; }
    }
    public class chartsjson
    {
        public chartsjson()
        {
            Chartsfilter = new chartsfilter();
        }
        public chartsfilter Chartsfilter { get; set; }
        public IList<User_Master> lstJoManager { get; set; }
        public IList<Issue_Type_Master> lstIssueType { get; set; }
        public IList<DropDownMaster> lststatus { get; set; }
        public IList<wfm_jo_type_master> lstJoType { get; set; }
        public IList<wfm_service_facility_master> lstServiceType { get; set; }
        public IList<JoCategoryMaster> lstJoCategory { get; set; }
        public IList<TT_Customer_Category> lstCustomerCategory { get; set; }
        public IList<TT_Customer_Segment> lstCustomerSegment { get; set; }
        public IList<TT_category> lstcategory { get; set; }
        public IList<TT_Type> lsttype { get; set; }
        public IList<string> lstregion_name { get; set; }
        public IList<string> lstprovince_name { get; set; }
        public IList<string> lstsubdistrict_name { get; set; }
        public charts citywisechartdata { get; set; }
        //public charts statewisechartdata { get; set; }
        public charts issue_description { get; set; }
        public charts assgintofrtchartdata { get; set; }
        public charts jofacility { get; set; }
        public charts jostatus { get; set; }
    }
    public  class JobOrder
    {
    }

    public class ViewJobOrder
    {
        public  ViewJobOrder()
        {
            vwJobOrderFilter = new ViewJobOrderFilter();

        }

        public IList<User_Master> lstUserDetail { get; set; }
        public IList<VW_Route_Issue> lstIssueDetails { get; set; }
        public IList<Issue_Type_Master> lstIssueType { get; set; }

        public IList<TT_Customer_Category> lstCustomerCategory { get; set; }

        public IList<TT_Customer_Segment> lstCustomerSegment { get; set; }
        public IList<TT_category> lstcategory { get; set; }
        public IList<TT_Type> lsttype { get; set; }

        public IList<DropDownMaster> lststatus { get; set; }

        public ViewJobOrderFilter vwJobOrderFilter { get; set; }
        public IList<User_Master> lstGroupDetails { get; set; }
        public IList<User_Master> lstJoDetailDetails { get; set; }
        public IList<wfm_service_facility_master> lstServiceType { get; set; }
        public IList<JoCategoryMaster> lstJoCategory { get; set; }
        public string statusCount { get; set; }
        public IList<wfm_jo_type_master> lstJoType { get; set; }
        public IList<User_Master> lstRegionalHead { get; set; }

    }

    public class MainJobOrderFilter
    {
        public string searchText { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int pageSize { get; set; }
        public int noOfPages { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public string ticketSource { get; set; }
        public int page { get; set; }
        public int duration { get; set; }
        public string status { get; set; }
        public string issueType { get; set; }
        public int userId { get; set; }
        public int assignId { get; set; }
        public string assignName { get; set; }
        public int? selectedAssignType { get; set; }
       

    }

    public class ViewJobOrderFilter : MainJobOrderFilter
    {

        public int userId { get; set; }
        public int subordinateUserId { get; set; }
        public int groupmanagerId { get; set; }
        public int jomanagerId { get; set; }
        public int isLoginRole { get; set; }
        public string serviceType { get; set; }
        public string joCategory { get; set; }
        public string joType { get; set; }
        public int searchby { get; set; }
        public string customerCatagory { get; set; }
        public string customerSegment { get; set; }
        public string ttcategory { get; set; }
        public string tttype { get; set; }
        public int regionalHeadId { get; set; }

    }
    public class ViewAssignJobOrderFilter : MainJobOrderFilter
    {
        public int managerId { get; set; }
        public int issueRaisedBy { get; set; }
        public int issueAssignedTo { get; set; }
        public int groupmanagerId { get; set; }
        public int jomanagerId { get; set; }
        public int isLoginRole { get; set; }
        public string serviceType { get; set; }
        public string joCategory { get; set; }
        public string joType { get; set; }
        public int searchby { get; set; }
        public int subordinateUserId { get; set; }
        public string customerCatagory { get; set; }
        public string customerSegment { get; set; }
        public string ttcategory { get; set; }
        public string tttype { get; set; }
        public int regionalHeadId { get; set; }
    }


    public class ViewAssignJobOrder
    {
        public ViewAssignJobOrder()
        {
            viewAssignJobOrderFilter = new ViewAssignJobOrderFilter();
        }
        public IList<TT_Customer_Category> lstCustomerCategory { get; set; }
        public IList<TT_Customer_Segment> lstCustomerSegment { get; set; }
        public IList<TT_category> lstcategory { get; set; }
        public IList<TT_Type> lsttype { get; set; }
        public IList<User_Master> lstIssueRaisedByDetail { get; set; }
        public IList<User_Master> lstIssueAssignedToDetail { get; set; }
        
        public IList<VW_Assign_Job_Order> lstAssignJobOrderDetails { get; set; }
        public IList<VW_Route_Assigned> lstRouteAssignedDetails { get; set; }
        public IList<VW_Route_Scheduled> lstRouteScheduledDetails { get; set; }
        public IList<RouteAssignedReport> lstRouteAssignedReport { get; set; }
        public IList<RouteScheduledReport> lstRouteScheduledReport { get; set; }
        public IList<Issue_Type_Master> lstIssueType { get; set; }
        public IList<DropDownMaster> lststatus { get; set; }
        public IList<User_Master> lstCantractorDetails { get; set; }
        public ViewAssignJobOrderFilter viewAssignJobOrderFilter { get; set; }
        public IList<User_Master> lstGroupDetails { get; set; }
        public IList<User_Master> lstUserDetail { get; set; }
        public IList<User_Master> lstJoDetailDetails { get; set; }
        public IList<wfm_service_facility_master> lstServiceType { get; set; }
        public IList<JoCategoryMaster> lstJoCategory { get; set; }
        public string statusCount { get; set; }
        public IList<wfm_jo_type_master> lstJoType { get; set; }
        public List<User_Master> lstRegionalHead { get; set; }


    }
    public class VW_Assign_Job_Order
    {
        public string hpsmid { get; set; }
        public int issue_id { get; set; }

        public string quantity { get; set; }
        public string material_code { get; set; }
        public string material_name { get; set; }
        public string serial_no { get; set; }
        public string wh_code { get; set; }
        public string item_code { get; set; }
        public string uom { get; set; }
        public int task_tracking_id { get; set; }
        public string issue_desc { get; set; }
        public string status { get; set; }
        public string hpsm_ticketid { get; set; }
        public int vsf_ticketid { get; set; }
        public string sub_status { get; set; }
        public string patroller_user_name { get; set; }
        public string patroller_remark { get; set; }
        public string frt_user_name { get; set; }
        public string assigned_date { get; set; }
        public string manager_remark { get; set; }
        public Double latitude { get; set; }
        public Double longitude { get; set; }
        public string created_on { get; set; }
        public string modified_on { get; set; }
        public string mobile_checkin_time { get; set; }
        public string mobile_checkout_time { get; set; }
        public string checkin_remarks { get; set; }
        public string checkout_remarks { get; set; }
        public string hold_rc_code { get; set; }
        public string hold_rca_code { get; set; }
        public string hold_remarks { get; set; }
        public string hold_appointment_date { get; set; }

        //Durgesh Add column
        public string rc { get; set; }
        public string rca { get; set; }

        public string circle_name { get; set; }
        public string cluster_id { get; set; }
        public string cluster_name { get; set; }

        public string society_name { get; set; }
        public string customer_id { get; set; }
        public string customer_add { get; set; }
        public int ticket_source_id { get; set; }
        public string ticket_source { get; set; }
        //shazia add column 
        public int bookingid { get; set; }
        public string atomic_id { get; set; }
        public string access_type { get; set; }
        public string package_name { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        public string cpe_type { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string facility { get; set; }
        public string customer_rmn { get; set; }
        public string secondary_contact { get; set; }
        public string email_id { get; set; }
        public string local_convergence_point { get; set; }
        public string current_cpesn { get; set; }
        public string listextension_boxsn { get; set; }
        public string issue_comment { get; set; }
        public string site_type { get; set; }
        public string subscriber_name { get; set; }
        public string addressline1 { get; set; }
         public string addressline2 { get; set; }
        public string addressline3 { get; set; }
        public string addressline4 { get; set; }
        public string barangay_name { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string stateorprovince { get; set; }
        public string addressid { get; set; }
        public string remarks { get; set; }
        public string jo_category { get; set; }
        public string account_number { get; set; }
        public string reference_id { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }
        public string cpe_ref_serial { get; set; }
        public string cpe_uom { get; set; }
        public string cpe_wh { get; set; }
        public string appointment_date { get; set; }
        public string slot_time { get; set; }
        public string sla { get; set; }
        public string is_attachment { get; set; }

        public string customer_category { get; set; }
        public string customer_segment { get; set; }
        public string bandwidth { get; set; }
        public string tasktype { get; set; }
        public string taskcategory { get; set; }
        public string tasksubcategory { get; set; }

        public string ip { get; set; }
        public string gw { get; set; }
        public string sm { get; set; }
        public string dns { get; set; }
        public string adns { get; set; }
        public string group_user { get; set; }
        public bool sendtoerp { get; set; }
        public string erp_response { get; set; }
        public string wfmcomment { get; set; }
        public string product_instance_id { get; set; }
        public string parent_hpsmid { get; set; }
        public string payment_mode { get; set; }
        public decimal total_amount { get; set; }
        public string ar_no { get; set; }
        public string payment_status { get; set; }
        public string payment_type { get; set; }
    }
    [Serializable]
    public class ViewAssignedRouteFilter
    {
        public string searchText { get; set; }
        public int assignId { get; set; }
        public string assignName { get; set; }
        public int? selectedAssignType { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int userId { get; set; }
        public int duration { get; set; }
    }

    public class ViewAssignedRoute
    {
        public ViewAssignedRoute()
        {
            viewAssignedRouteFilter = new ViewAssignedRouteFilter();
        }

        public IList<UserDetail> lstUserDetail { get; set; }
        public IList<VW_Route_Assigned> lstRouteAssignedDetails { get; set; }
        public IList<VW_Route_Scheduled> lstRouteScheduledDetails { get; set; }
        public IList<RouteAssignedReport> lstRouteAssignedReport { get; set; }
        public IList<RouteScheduledReport> lstRouteScheduledReport { get; set; }
        public ViewAssignedRouteFilter viewAssignedRouteFilter { get; set; }
    }
    public class VW_Route_Assigned
    {
        public int route_assigned_id { get; set; }
        public int route_id { get; set; }
        public string route_name { get; set; }
        public int route_ref_id { get; set; }
        public int patroller_id { get; set; }
        public string patroller_name { get; set; }
        public int scheduled_assigment_id { get; set; }
        public string planned_start_time { get; set; }
        public string planned_end_time { get; set; }
        public string actual_start_time { get; set; }
        public string actual_end_time { get; set; }
        public string status { get; set; }
        public string created_by_name { get; set; }
        public string created_on { get; set; }
        public string modified_on { get; set; }
        public string modified_by_name { get; set; }
        public string self_check_in { get; set; }

    }
    public class VW_Route_Scheduled
    {
        public int scheduled_id { get; set; }
        public int route_id { get; set; }
        public int route_ref_id { get; set; }
        public string route_name { get; set; }
        public int patroller_id { get; set; }
        public string patroller_name { get; set; }
        public string assignment_type { get; set; }
        public string working_days { get; set; }
        public int day_of_month { get; set; }
        public string start_date { get; set; }
        public string start_time { get; set; }
        public string end_date { get; set; }
        public string end_time { get; set; }
        public string is_active { get; set; }
        public string created_by_name { get; set; }
        public string created_on { get; set; }
        public string modified_by_name { get; set; }
        public string modified_on { get; set; }
    }
    public class RouteAssignedReport
    {
        public int route_assigned_id { get; set; }
        public string route_name { get; set; }
        public string patroller_name { get; set; }
        public string planned_start_time { get; set; }
        public string planned_end_time { get; set; }
        public string actual_start_time { get; set; }
        public string actual_end_time { get; set; }
        public string status { get; set; }
        public string created_by_name { get; set; }
        public string created_on { get; set; }
        public string modified_by_name { get; set; }
        public string modified_on { get; set; }
    }
    public class RouteScheduledReport
    {
        public int scheduled_id { get; set; }
        public string route_name { get; set; }
        public string patroller_name { get; set; }
        public string assignment_type { get; set; }
        public string start_date { get; set; }
        public string start_time { get; set; }
        public string end_date { get; set; }
        public string end_time { get; set; }
        public string is_active { get; set; }
        public string created_by_name { get; set; }
        public string created_on { get; set; }
        public string modified_by_name { get; set; }
        public string modified_on { get; set; }
        public string working_days { get; set; }
        public int day_of_month { get; set; }
    }

    public class ViewIssueRouteAssignFilter
    {
        public string searchText { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string ticketSource { get; set; }
        public string status { get; set; }
        public int pageSize { get; set; }
        public int noOfPages { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int page { get; set; }
        public int managerId { get; set; }
        public int duration { get; set; }
        public int issueRaisedBy { get; set; }
        public int issueAssignedTo { get; set; }
        public string issueType { get; set; }
    }
    //public class ViewRouteAssignIssue
    //{
    //    public ViewRouteAssignIssue()
    //    {
    //        viewAssignRouteIssueFilter = new ViewIssueRouteAssignFilter();
    //    }

    //    public IList<UserDetail> lstIssueRaisedByDetail { get; set; }
    //    public IList<UserDetail> lstIssueAssignedToDetail { get; set; }
    //    public IList<Issue_Type_Master> lstIssueType { get; set; }
    //    public IList<VW_Assign_Job_Order> lstAssignIssueDetails { get; set; }
    //    public ViewIssueRouteAssignFilter viewAssignRouteIssueFilter { get; set; }
    //}
    public class Issue_Type_Master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int oid { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string name { get; set; }

        public string description2 { get; set; }

    }
    public class wfm_jo_type_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string jo_name { get; set; }
        public string jo_description { get; set; }
        public string jo_code { get; set; }
        
    }

    public class wfm_service_facility_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string service_facility_name { get; set; }
        public string service_facility_description { get; set; }
        public int slot_duration { get; set; }
        public string service_facility_code { get; set; }


    }
    public class tbl_wfm_slot_duration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sdid { get; set; }
        public int time_duration { get; set; }
        public string description { get; set; }
    }

    public class VW_ROUTE_ASSIGNED_ISSUES
    {
        public int issue_id { get; set; }
        public int task_tracking_id { get; set; }
        public string issue_desc { get; set; }
        public string status { get; set; }
        public string hpsm_ticketid { get; set; }
        public int vsf_ticketid { get; set; }
        public string sub_status { get; set; }
        public string patroller_user_name { get; set; }
        public string patroller_remark { get; set; }
        public string frt_user_name { get; set; }
        public string assigned_date { get; set; }
        public string manager_remark { get; set; }
        public Int64 latitude { get; set; }
        public Int64 longitude { get; set; }
        public string created_on { get; set; }
        public string modified_on { get; set; }
        public string mobile_checkin_time { get; set; }
        public string mobile_checkout_time { get; set; }
        public string checkin_remarks { get; set; }
        public string checkout_remarks { get; set; }
        public string hold_rc_code { get; set; }
        public string hold_rca_code { get; set; }
        public string hold_remarks { get; set; }
        public string hold_appointment_date { get; set; }

        //Durgesh Add column
        public string rc { get; set; }
        public string rca { get; set; }

        public string circle_name { get; set; }
        public string cluster_id { get; set; }
        public string cluster_name { get; set; }

        public string society_name { get; set; }
        public string customer_id { get; set; }
        public string customer_add { get; set; }
        public int ticket_source_id { get; set; }
        public string ticket_source { get; set; }
    }
 
    public class TT_Customer_Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string name { get; set; }

    }

    public class TT_Customer_Segment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string name { get; set; }

    }

    public class TT_category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string name { get; set; }

    }

    public class TT_Type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public int tt_category_id { get; set; }

    }
    public class jo_form
    {
        public string hpsmid { get; set; }
        public string assigned_name { get; set; }
        public string jo_date { get; set; }
        public string customer_name { get; set; }
        public string customer_rmn { get; set; }
        public string account_number { get; set; }
        public string address { get; set; }
        public string origin { get; set; }
        public string slot { get; set; }
        public string package { get; set; }
        public string monthlyrate { get; set; }
        public string installationfee { get; set; }
        public string checkin { get; set; }
        public string checkout { get; set; }
        public string remarks { get; set; }
        public List<material_used> material_used { get; set; }
        public bool is_satisfactory { get; set; }
        public string satisfactory_remarks { get; set; }
        public string quality_of_service { get; set; }
        public string napcode { get; set; }
        public string napport { get; set; }
        public string cpe_serial { get; set; }
        public string ref_serial { get; set; }

    }
    public class material_used
    {
        public string jobid { get; set; }
        public string serial_no { get; set; }
        public string material_name { get; set; }
        public string material_code { get; set; }
        public string material_unit { get; set; }
        public string details { get; set; }
        public string quantity { get; set; }
        public string uom { get; set; }
        public string wh_code { get; set; }
        public string item_code { get; set; }
        public string ticket_type { get; set; }
    }
    public class napdetails
    {
        public string job_id { get; set; }
        public string nap { get; set; }
        public string napport { get; set; }

    }

    public class PortManager
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string  mobile_number { get; set; }
        public string email { get; set; }
        public string region_name { get; set; }
        public string stateorprovince { get; set; }
        public bool is_active { get; set; }

    }
    public class napportnotificationdata
    {
        public string job_id { get; set; }
        public string user_id { get; set; }
        public int ticket_id { get; set; }
        public string nap { get; set; }
        public string nap_port { get; set; }
        public string assign_to { get; set; }
        public string issue { get; set; }
        public string subject_line { get; set; }
        public string email_message { get; set; }
        public string email_result { get; set; }
        public string email_response { get; set; }
        public string sms_message { get; set; }
        public string sms_result { get; set; }
        public string sms_response { get; set; }
        public string to_email { get; set; }
        public string status { get; set; }
        public string errormessage { get; set; }
        public string  to_mobile { get; set; }
    }
    public class UpdateNotificationLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int ticket_id { get; set; }
        public string notification_type { get; set; }
        public string content { get; set; }
        public string address { get; set; }
        public DateTime? created_on { get; set; }
        public string created_by { get; set; }
        public string status { get; set; }
        public string response { get; set; }
    }


    public class Payment_Collection_Details 
    {
        public string[] payment_modes { get; set; }
        public string payment_type { get; set; }
        public string payment_status { get; set; }
        public decimal total_amount { get; set; }
        public List<Payment_Details> paymentSplitup { get; set; }
        public Payment_Collection_Details() 
        {
            paymentSplitup = new List<Payment_Details>();
        }
    }
}
