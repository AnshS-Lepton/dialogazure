

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Models.WFM
{
    public class ReqInput
    {
        public string data { get; set; }

    }
    public class InputSlotRequest
    {
        public dynamic slotrequest { get; set; }
    }
    public class InputBookSlotRequest
    {
        public dynamic bookslotrequest { get; set; }
    }

    public class InputCreateJobRequest
    {
        public dynamic createjobrequest { get; set; }
    }
    public class BookingResponce
    {
        public dynamic bookingid { get; set; }
    }
    public class SlotRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public string order_type { get; set; }

        public string service_type { get; set; }


        [NotMapped]
        public List<string> service_types { get; set; }
        [Required]
        public double? latitude { get; set; }
        [Required]
        public double? longitude { get; set; }
        [Required]
        public DateTime appointment_date { get; set; }
        public string referenceid { get; set; }
        public DateTime? created_date { get; set; }
        public string response { get; set; }
        public string remark { get; set; }
        public int managerid { get; set; }

        public string jo_category { get; set; }

        public string request { get; set; }
        public SlotRequest()
        {
            jo_category = "REGULAR";
        }
    }

    public class ManagerList
    {
        //public int UserId { get; set; }
        //public string Name { get; set; }

        public int? user_id { get; set; }
        public string user_name { get; set; }
        public string Jo_type { get; set; }
        public string service { get; set; }

        public string user_Jo_type { get; set; }
        public string user_service { get; set; }

        public string jo_category_role { get; set; }

        public string jo_category_user { get; set; }

        public string block_name { get; set; }
        public string role_name { get; set; }
        public int capacity { get; set; }
        public int? dispatchercount { get; set; }
        public string group_name { get; set; }

    }

    public class FEList
    {
        public int? UserId { get; set; }
        public string Name { get; set; }
        public int ManagerUserId { get; set; }
        public bool isRosterAvailable { get; set; }
        public string user_Jo_type { get; set; }
        public string user_service { get; set; }
        public int _start_time { get; set; }
        public int _end_time { get; set; }
        public string user_type { get; set; }
        public string role_name { get; set; }
        public int fecount { get; set; }
    }

    public class UserServiceDetail
    {
        public string service { get; set; }
        public string transection { get; set; }
        public string jo_category_name { get; set; }
        public string user_type { get; set; }
    }
    public class ServiceType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ttid { get; set; }
        public int oid { get; set; }
        public int sdid { get; set; }
        public string skid { get; set; }
        public string name { get; set; }
        public string add_service { get; set; }
        public string remove_service { get; set; }

    }

    public class EFSkill
    {
        public int UserId { get; set; }
        public string SkillId { get; set; }
    }

    public class Slot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int slotid { get; set; }
        public int from_time { get; set; }
        public int to_time { get; set; }
        public int sdid { get; set; }
    }

    public class SlotResponse
    {
        public string slotid { get; set; }
        public string temp_slotid { get; set; }
        public string from_time { get; set; }
        public string to_time { get; set; }
        public string referenceId { get; set; }
        public string appointment_date { get; set; }
        public string dispatcher_name { get; set; }
    }

    public class AppointmentDetail
    {
        public int bookingid { get; set; }
        public string order_type { get; set; }
        public string service_type { get; set; }
        public int from_time { get; set; }
        public int to_time { get; set; }
        [Required]
        public string referenceid { get; set; }
        public string appointment_date { get; set; }
    }

    public class SlotConfirmation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bookingid { get; set; }
        [Required]
        public string slotid { get; set; }
        [Required]
        public string referenceid { get; set; }
        public int flag { get; set; }
        public int managerid { get; set; }
        public int feuserid { get; set; }
        public int master_slot_id { get; set; }
        public DateTime appointment_date { get; set; }
        public DateTime created_date { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_date { get; set; }
        public int iscanceled { get; set; }
    }


    public class VMSlotConfirmation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bookingid { get; set; }
        [Required]
        public string slotid { get; set; }
        [Required]
        public string referenceid { get; set; }
        public int flag { get; set; }
        public int managerid { get; set; }
        public int feuserid { get; set; }
        public int master_slot_id { get; set; }
        public DateTime appointment_date { get; set; }
        public DateTime created_date { get; set; }
        public DateTime created_datetime { get; set; }
        public DateTime? updated_date { get; set; }
        public int iscanceled { get; set; }

        public int from_time { get; set; }
        public int to_time { get; set; }
    }

    public class CreateJob
    {
        //public int bookingid { get; set; }
        //public string referenceid { get; set; }
        [Required]
        public string joborderid { get; set; }
        //store booking id in reference id
        [Required]
        public int bookingid { get; set; }
        public string atomicid { get; set; }
        public string atomicidClose { get; set; }

        public string access_type { get; set; }
        public string package_name { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        [Required]
        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public string email { get; set; }
        [NotMapped]
        [Required]
        public List<address> address { get; set; }

        //public string installation_address { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        public string pinCode { get; set; }
        public string state_Province { get; set; }
        public string address_id { get; set; }
        public string local_convergence_point { get; set; }
        public string current_cpesn { get; set; }
        public string listextension_boxsn { get; set; }
        public string comment { get; set; }
        public string customerid { get; set; }
        public string task_category { get; set; }
        public DateTime booking_date { get; set; }
        public DateTime created_date { get; set; }
        [Required]
        public string cpe_type { get; set; }
        [Required]
        public string cpe_portno { get; set; }
        [Required]
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string facility { get; set; }
        public string site_type { get; set; }
        public string subscriber_name { get; set; }

        public string latitude { get; set; }
        public string longitude { get; set; }
        public string jo_category { get; set; }
        public string account_number { get; set; }
        public string reference_id { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }
        public string customer_category { get; set; }
        public string customer_segment { get; set; }
        public string msp_cic_Identifier { get; set; }
        public string bandwidth { get; set; }

    }

    public class address
    {
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string city { get; set; }
        public string address_type { get; set; }
        public string pincode { get; set; }
        public string stateorprovince { get; set; }
        public string addressid { get; set; }
        public string address_line4 { get; set; }
        public string address_line5 { get; set; }

    }


    public class CreateJobNew
    {
        //public int bookingid { get; set; }
        //public string referenceid { get; set; }
        [Required]
        public string joborderid { get; set; }
        //store booking id in reference id
        [Required]
        public int bookingid { get; set; }
        public string atomicid { get; set; }
        public string atomicidClose { get; set; }

        public string access_type { get; set; }
        public string package_name { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        [Required]
        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public string email { get; set; }
        [NotMapped]
        [Required]
        public List<address> address { get; set; }

        public string installation_address { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }

        public string address_line4 { get; set; }
        public string address_line5 { get; set; }
        public string address_type { get; set; }
        public string city { get; set; }
        public string pinCode { get; set; }
        public string state_Province { get; set; }
        public string address_id { get; set; }
        public string local_convergence_point { get; set; }
        public string current_cpesn { get; set; }
        public string listextension_boxsn { get; set; }
        public string comment { get; set; }
        public string customerid { get; set; }
        public string task_category { get; set; }
        public DateTime booking_date { get; set; }
        public DateTime created_date { get; set; }
        //[Required]
        public string cpe_type { get; set; }
        //[Required]
        public string cpe_portno { get; set; }
        // [Required]
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string facility { get; set; }
        public string site_type { get; set; }
        public string subscriber_name { get; set; }

        public string latitude { get; set; }
        public string longitude { get; set; }
        public string jo_category { get; set; }
        public string account_number { get; set; }
        public string reference_id { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }

        public List<Service> Services { get; set; }

        public string ip { get; set; }
        public string gw { get; set; }
        public string sm { get; set; }
        public string dns { get; set; }
        public string adns { get; set; }

        public int priority { get; set; }
        public string paymentType { get; set; }
     //   public string payment_status { get; set; }
        public decimal paymentTotal { get; set; }
        public List<Payment_Details> paymentSplitup { get; set; }
        public CreateJobNew()
        {
            priority = 5;
        }
    }

    public class CancelJobOrder
    {
        [Required]
        public string joborderid { get; set; }
    }

    //Hpsm_ticket_master
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hpsm_ticket_id { get; set; }
        public string hpsmid { get; set; }       //bookingid
        public string hpsmstatus { get; set; }
        //[XmlElement(ElementName = "AlarmOccuredTime")]
        //public string AlarmOccuredTime { get; set; }
        [XmlElement(ElementName = "opendate")]
        public string earlystart { get; set; }

        [XmlElement(ElementName = "region")]
        public string region { get; set; }

        [XmlElement(ElementName = "isresolve")]
        public string isresolve { get; set; }
        [XmlElement(ElementName = "latitude")] // customer lat
        public string latitude { get; set; }
        [XmlElement(ElementName = "longitude")] // customer long
        public string longitude { get; set; }
        //[XmlElement(ElementName = "ticketType")] //
        //public string ticketType { get; set; }
        [XmlElement(ElementName = "comment")]   //Comment
        public string comment_ { get; set; }
        [XmlElement(ElementName = "tasktype")]  // order type
        public string tasktype { get; set; }

        //[XmlElement(ElementName = "OperationCenters")]
        //public string OperationCenters { get; set; }

        [XmlElement(ElementName = "taskcategory")]   //Normal/Troubalshoot
        public string taskcategory { get; set; }
        [XmlElement(ElementName = "tasksubcategory")] // service type
        public string tasksubcategory { get; set; }
        public string producttype { get; set; }
        public string status { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public string parent_hpsmid { get; set; }
        public string customer_id { get; set; }
        //public string customer_serial_number1 { get; set; }
        //public string customer_serial_number2 { get; set; }
        //public string customer_brand { get; set; }
        //public string customer_model { get; set; }
        //public string customer_ont_id { get; set; }
        public string customer_rmn { get; set; }
        public string customer_add { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string addressline3 { get; set; }
        public string addressline4 { get; set; }
        public string addressline5 { get; set; }
        public string addresstype { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string stateorprovince { get; set; }
        public string addressid { get; set; }
        public DateTime? customer_preferred_time { get; set; }
        //public string issue_comment { get; set; }
        //public string customer_plan_speed { get; set; }
        //public string customer_plan_name { get; set; }
        //public string service_status { get; set; }
        //public string service_activation_date { get; set; }
        public string ticketstatus { get; set; }
        public string ticket_type { get; set; }
        public int ticket_source_id { get; set; }
        public string society_name { get; set; }
        public string resolution_close_id { get; set; }
        public string root_cause_id { get; set; }
        public string device_serial_number1 { get; set; }
        //public string device_serial_number2 { get; set; }
        //public string device_brand { get; set; }
        //public string device_model { get; set; }
        //public string dial_number { get; set; }
        //public string tat_delay_remark { get; set; }
        //public string customer_device_status { get; set; }
        //public string vendor_code { get; set; }
        //public string vendor_name { get; set; }
        //public string installed_by { get; set; }
        //public string installed_user_id { get; set; }
        public string email_id { get; set; }

        //new columns

        public int bookingid { get; set; }
        public string atomic_id { get; set; }
        public string access_type { get; set; }
        public string package_name { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }


        public string local_convergence_point { get; set; }
        public string secondary_contact { get; set; }
        public string current_cpesn { get; set; }
        public string listextension_boxsn { get; set; }

        public string cpe_type { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string facility { get; set; }
        public string site_type { get; set; }
        public string subscriber_name { get; set; }
        public int? stage { get; set; }
        public string cluster_id { get; set; }
        public string cluster_name { get; set; }
        public string circle_name { get; set; }
        public string remarks { get; set; }

        public string jo_category { get; set; }
        public string account_number { get; set; }
        public string reference_id { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }
        public string cpe_ref_serial { get; set; }
        public string cpe_uom { get; set; }
        public string cpe_wh { get; set; }
        public string atomicidclose { get; set; }
        public string action { get; set; }

        public string customer_category { get; set; }
        public string customer_segment { get; set; }
        public string msp_cic_identifier { get; set; }
        public string bandwidth { get; set; }

        public string ip { get; set; }
        public string gw { get; set; }
        public string sm { get; set; }
        public string dns { get; set; }
        public string adns { get; set; }
        public string service_status { get; set; }
        public int priority { get; set; }
        public string cls { get; set; }
        public string issue_comment { get; set; }
        public bool sendtoerp { get; set; }
        public string erp_response { get; set; }
        public string wfmcomment { get; set; }
        public string product_instance_id { get; set; }
        public string payment_mode { get; set; }
        public string payment_status { get; set; }
        public string ar_no { get; set; }
        public decimal total_amount { get; set; }
        public string payment_type { get; set; }
        public string task_id {get;set;}
        public string old_cpe_item_code { get; set; }
        public string is_cpe_collected { get; set; }

    }

    public class HPSM_Ticket_Master_History
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int History_Id { get; set; }
        public int Hpsm_Ticket_Id { get; set; }
        public string HPSMID { get; set; }
        public string HPSMStatus { get; set; }
        public string AlarmOccuredTime { get; set; }
        public string OpenDate { get; set; }
        public string EarlyStart { get; set; }
        public string Region { get; set; }
        public string IsResolve { get; set; }
        public string District { get; set; }
        public string TaskPriority { get; set; }
        public string DueDate { get; set; }
        public string Duration { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TicketType { get; set; }
        public string Comment_ { get; set; }
        public string TaskType { get; set; }
        public string OperationCenters { get; set; }
        public string TaskCategory { get; set; }
        public string TaskSubCategory { get; set; }
        public string PrimaryAssignedGroup { get; set; }
        public string PrimaryAssigneeName { get; set; }
        public string SecondaryAssignedGroup { get; set; }
        public string RemarksSMS { get; set; }
        public string ImpactSMS { get; set; }
        public string RCASMS { get; set; }
        public string SpecificProblems { get; set; }
        public string AffectedCI { get; set; }
        public string SiteName { get; set; }
        public string TechCIType { get; set; }
        public string CIType { get; set; }
        public string ProductType { get; set; }
        public string Cls { get; set; }
        public string Action { get; set; }
        public string Created_By { get; set; }
        public DateTime? Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public string Status { get; set; }


        //////////Wasim Date:03/11/2020
        public string customer_id { get; set; }
        public string customer_rmn { get; set; }
        public string customer_add { get; set; }
        public DateTime? customer_preferred_time { get; set; }
        public string issue_comment { get; set; }
        public string customer_plan_speed { get; set; }
        public string customer_plan_name { get; set; }
        public string service_status { get; set; }
        public string task_id { get; set; }
        public string work_item_id { get; set; }
        public string out_come_id { get; set; }
        public int route_issue_id { get; set; }
        public string resolution_close_id { get; set; }
        public string root_cause_id { get; set; }
        ///////////////////////////////
    }
  
    public class VW_HPSM_Ticket_Master_History
    {
        public int History_Id { get; set; }
        public int Hpsm_Ticket_Id { get; set; }
        public string HPSMID { get; set; }
        public string HPSMStatus { get; set; }
        public string AlarmOccuredTime { get; set; }
        public string OpenDate { get; set; }
        public string EarlyStart { get; set; }
        public string Region { get; set; }
        public string IsResolve { get; set; }
        public string District { get; set; }
        public string TaskPriority { get; set; }
        public string DueDate { get; set; }
        public string Duration { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TicketType { get; set; }
        public string Comment_ { get; set; }
        public string TaskType { get; set; }
        public string OperationCenters { get; set; }
        public string TaskCategory { get; set; }
        public string TaskSubCategory { get; set; }
        public string PrimaryAssignedGroup { get; set; }
        public string PrimaryAssigneeName { get; set; }
        public string SecondaryAssignedGroup { get; set; }
        public string RemarksSMS { get; set; }
        public string ImpactSMS { get; set; }
        public string RCASMS { get; set; }
        public string SpecificProblems { get; set; }
        public string AffectedCI { get; set; }
        public string SiteName { get; set; }
        public string TechCIType { get; set; }
        public string CIType { get; set; }
        public string ProductType { get; set; }
        public string Cls { get; set; }
        public string Action { get; set; }
        public string Created_By { get; set; }
        public DateTime? Created_On { get; set; }
        public DateTime? Modified_On { get; set; }
        public string Status { get; set; }
        public string status_date { get; set; }

        //////////Wasim Date:03/11/2020
        public string customer_id { get; set; }
        public string customer_rmn { get; set; }
        public string customer_add { get; set; }
        public DateTime? customer_preferred_time { get; set; }
        public string issue_comment { get; set; }
        public string customer_plan_speed { get; set; }
        public string customer_plan_name { get; set; }
        public string service_status { get; set; }
        public string task_id { get; set; }
        public string work_item_id { get; set; }
        public string out_come_id { get; set; }
        public int route_issue_id { get; set; }
        public string resolution_close_id { get; set; }
        public string resolution_close_description { get; set; }

        public string root_cause_id { get; set; }

        public string root_cause_description { get; set; }

        public string sub_status { get; set; }
        public int frt_id { get; set; }
        public string user_name { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }

        //Durgesh
        public string remarks { get; set; }
        public string hold_remarks { get; set; }

        public string hold_rc_code { get; set; }

        public string hold_rca_code { get; set; }
        public string hold_appointment_date { get; set; }

        public string sub_role_name { get; set; }

        public string addressline1 { get; set; }

        public string addressline2 { get; set; }
        public string addressline3 { get; set; }


        public VW_HPSM_Ticket_Master_History()
        {
            History_Id = 0;
            Hpsm_Ticket_Id = 0;
            HPSMID = "";
            HPSMStatus = "";
            AlarmOccuredTime = "";
            OpenDate = "";
            EarlyStart = "";
            Region = "";
            IsResolve = "";
            District = "";
            TaskPriority = "";
            DueDate = "";
            Duration = "";
            Latitude = "";
            Longitude = "";
            TicketType = "";
            Comment_ = "";
            TaskType = "";
            OperationCenters = "";
            TaskCategory = "";
            TaskSubCategory = "";
            PrimaryAssignedGroup = "";
            PrimaryAssigneeName = "";
            SecondaryAssignedGroup = "";
            RemarksSMS = "";
            ImpactSMS = "";
            RCASMS = "";
            SpecificProblems = "";
            AffectedCI = "";
            SiteName = "";
            TechCIType = "";
            CIType = "";
            ProductType = "";
            Cls = "";
            Action = "";
            Created_By = null;
            Created_On = null;
            Modified_On = null;
            Status = "";


            //////////Wasim Date:03/11/2020
            customer_id = "";
            customer_rmn = "";
            customer_add = "";
            customer_preferred_time = null;
            issue_comment = "";
            customer_plan_speed = "";
            customer_plan_name = "";
            service_status = "";
            task_id = "";
            work_item_id = "";
            out_come_id = "";
            route_issue_id = 0;
            resolution_close_id = "";
            root_cause_id = "";
            sub_status = "";
            frt_id = 0;
            user_name = "";
            role_id = 0;
            role_name = "";
        }
    }
    public class Task_Tracking
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int task_tracking_id { get; set; }
        public int issue_id { get; set; }
        public int frt_id { get; set; }
        public DateTime? assigned_date { get; set; }
        public string status { get; set; }
        [Required]
        public int created_by { get; set; }
        [Required]
        public int modified_by { get; set; }
        public DateTime? mobile_checkin_time { get; set; }
        public DateTime? mobile_checkout_time { get; set; }
        public string checkin_remarks { get; set; }
        public string checkout_remarks { get; set; }
        public double? actual_lat { get; set; }
        public double? actual_lng { get; set; }
        public DateTime? server_checkin_time { get; set; }
        public DateTime? server_checkout_time { get; set; }
        public int checkin_radius { get; set; }
        public DateTime modified_on { get; set; }
        public int? hpsm_ticketid { get; set; }
        public string sub_status { get; set; }
        public string remarks { get; set; }
        public double? checkout_lat { get; set; }
        public double? checkout_lng { get; set; }
        public string hold_remarks { get; set; }
        public string hold_rc_code { get; set; }
        public string hold_rca_code { get; set; }
        public DateTime? hold_appointment_date { get; set; }


    }
    public class Route_Issue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int issue_id { get; set; }
        [Required]
        public int route_assignment_id { get; set; }
        [Required]
        public int user_id { get; set; }
        [Required]
        //public string action { get; set; }
        public string status { get; set; }
        [Required]
        public double? latitude { get; set; }
        [Required]
        public double? longitude { get; set; }
        [Required]
        public DateTime mobile_time { get; set; }
        [Required]
        public int issue_type_id { get; set; }
        public string user_remark { get; set; }
        [Required]
        public int manager_id { get; set; }
        public string manager_remark { get; set; }
        public DateTime? modified_on { get; set; }
        public int? hpsm_ticketid { get; set; }
        public string sub_status { get; set; }
        public int is_reopened { get; set; }
        public string customer_id { get; set; }
        public string issue_remark { get; set; }
        public int circle_id { get; set; }

    }
    public class ViewManagerRouteIssueApprove
    {
        public int user_id { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
        public int issueId { get; set; }
        public int frtUserId { get; set; }
        public DateTime? assignedDate { get; set; }

        public int CircleCount { get; set; }
        public string issuesId { get; set; }
        public int checkinRadius { get; set; }
        public IList<User_Master> lstUserDetail { get; set; }
        public string hpsm_ticket_id { get; set; }
    }

    public class VW_Route_Issue
    {
        public int issue_id { get; set; }
        public string issue_desc { get; set; }
        public string hpsm_ticketid { get; set; }
        public string sub_status { get; set; }
        public string route_name { get; set; }
        public string status { get; set; }
        public string user_name { get; set; }
        public string mobile_time { get; set; }
        public Double latitude { get; set; }
        public Double longitude { get; set; }
        public string user_remark { get; set; }
        public string manager_remark { get; set; }
        public string modified_on { get; set; }
        public int vsf_ticketid { get; set; }
        public string customer_id { get; set; }
        public string customer_add { get; set; }
        public string created_on { get; set; }

        public int ticket_source_id { get; set; }
        public string ticket_source { get; set; }

        public string circle_name { get; set; }
        public string cluster_id { get; set; }
        public string cluster_name { get; set; }

        public string society_name { get; set; }

        public string impacttype { get; set; }
        public string networkentity { get; set; }
        public string pop_name { get; set; }
        public string tickettype { get; set; }
        public int customercount { get; set; }
        public string location { get; set; }
        public string boundary { get; set; }
        //shazia add column 
        public string hpsmid { get; set; }
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
        public string group_user { get; set; }

        public string ip { get; set; }
        public string gw { get; set; }
        public string sm { get; set; }
        public string dns { get; set; }
        public string adns { get; set; }
        public string comment { get; set; }
        public string product_instance_id { get; set; }
        public string parent_hpsmid { get; set; }
        public string payment_mode { get; set; }
        public decimal total_amount { get; set; }
        public string ar_no { get; set; }
        public string payment_status { get; set; }
        public string payment_type { get; set; }
    }

    public class StatusCount
    {
        public int count { get; set; }
        public string status { get; set; }
    }

    public class Vw_Hpsm_Ticket_Status
    {
        [Key]
        public int issue_id { get; set; }
        public int hpsm_ticket_id { get; set; }
        public string status { get; set; }
        public string sub_status { get; set; }
        public string hpsmid { get; set; }
        public string wfmcode { get; set; }
        public string wfmrole { get; set; }
        public string operatorname { get; set; }
        public DateTime datestamp { get; set; }
        public string wfmnumber { get; set; }
        public string wfmcomment { get; set; }
        public DateTime startstamp { get; set; }
        public DateTime finishtime { get; set; }
        public int? userrole { get; set; }
        // public string circle_code { get; set; }
        public string subtype { get; set; }

    }

    public class AssignedTaskDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int task_tracking_id { get; set; }
        public string jobid { get; set; }
        public int issue_id { get; set; }
        public string order_type { get; set; }
        public string order_type_code { get; set; }
        public string service_type { get; set; }
        public string service_type_code { get; set; }
        public string issue_category { get; set; }
        [Required]
        public int user_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string assigned_date { get; set; }
        public string patroller_remark { get; set; }
        public string manager_remark { get; set; }
        public string checkin_time { get; set; }
        public string checkout_time { get; set; }
        public string checkin_remarks { get; set; }
        public string checkout_remarks { get; set; }
        public string status { get; set; }
        public int? hpsm_ticketid { get; set; }


        public string task_id { get; set; }
        public string sub_status { get; set; }
        public string customer_id { get; set; }
        public string customer_rmn { get; set; }
        // public string customer_preferred_time { get; set; }
        public string customer_appointment_date { get; set; }
        public string created_on { get; set; }
        public string issue_comment { get; set; }
        public string service_status { get; set; }

        public string customer_add { get; set; }

        //public int checkin_radius { get; set; }
        //public int? circle_id { get; set; }
        //public string fibercircle { get; set; }
        //public string work_item_id { get; set; }
        //public string out_come_id { get; set; }
        //public string customer_plan_speed { get; set; }
        //public string customer_plan_name { get; set; }
        //public string customer_serial_number1 { get; set; }
        //public string customer_serial_number2 { get; set; }
        //public string customer_brand { get; set; }
        //public string customer_model { get; set; }
        //public string customer_ont_id { get; set; }
        //public string new_serial_number1 { get; set; }
        //public string new_serial_number2 { get; set; }
        //public string new_brand { get; set; }
        //public string new_model { get; set; }
        //public string dial_number { get; set; }
        //public string notifi_status { get; set; }
        //public string timelapse { get; set; }

        ////For nms ticket
        //public int? ticket_source_id { get; set; }

        public int bookingid { get; set; }
        public string atomic_id { get; set; }
        public string access_type { get; set; }
        public string package_name { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        public string local_convergence_point { get; set; }
        public string secondary_contact { get; set; }
        public string current_cpesn { get; set; }
        public string listextension_boxsn { get; set; }
        public string cpe_type { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string facility { get; set; }
        public string site_type { get; set; }
        public string subscriber_name { get; set; }
        public string color_code { get; set; }

        public string button_color { get; set; }
        public string email_id { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string addressline3 { get; set; }
        public string addressline4 { get; set; }
        public string addressline5{ get; set; }
        public string addresstype { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string stateorprovince { get; set; }
        public string addressid { get; set; }
        public string remarks { get; set; }
        public string slot_time { get; set; }

        public string jo_category { get; set; }
        public string account_number { get; set; }
        public string reference_id { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }
        public string cpe_ref_serial { get; set; }
        public string cpe_uom { get; set; }
        public string cpe_wh { get; set; }
        public string action { get; set; }

        public int master_slot_id { get; set; }

        [NotMapped]
        public double distance { get; set; }

        public string ip { get; set; }
        public string gw { get; set; }
        public string sm { get; set; }
        public string dns { get; set; }
        public string adns { get; set; }
        public string is_cpe_collected { get; set; }
        public string payment_mode { get; set; }
        public string payment_type { get; set; }
        public string payment_status { get; set; }
        public string ar_no { get; set; }
        public decimal total_amount { get; set; }
        public AssignedTaskDetail()
        {
            //checkin_radius = 2000000;
            customer_id = "N/A";
            customer_rmn = "N/A";
            // customer_preferred_time = "N/A";
            issue_comment = "N/A";
            //customer_plan_speed = "N/A";
            //customer_plan_name = "N/A";
            service_status = "N/A";
            task_id = "N/A";
            //work_item_id = "N/A";
            //out_come_id = "N/A";
            customer_add = "N/A";
            // jobid = "N/A";
            //customer_serial_number1 = "N/A";
            //customer_serial_number2 = "N/A";
            //customer_brand = "N/A";
            //customer_model = "N/A";
            //customer_ont_id = "N/A";
            //dial_number = "N/A";

            //new_serial_number1 = "";
            //new_serial_number2 = "";
            //new_brand = "";
            //new_model = "";
            //notifi_status = "N/A";
            //timelapse = "N/A";

        }
    }

    public class AssignedTaskDetailIn
    {
        public string user_id { get; set; }
        public string status { get; set; }
        public string jobid { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }

    public class AssignedTaskMaster
    { 
        public string slotid { get; set; }
    }

     public class AssignedTaskDetailTT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int task_tracking_id { get; set; }
        public string jobid { get; set; }
        public int issue_id { get; set; }
        public string order_type { get; set; }
        public string type_code { get; set; }
        public string service_type { get; set; }
        public string category_type_code { get; set; }
        public string issue_category { get; set; }
        [Required]
        public int user_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string assigned_date { get; set; }
        public string patroller_remark { get; set; }
        public string manager_remark { get; set; }
        public string checkin_time { get; set; }
        public string checkout_time { get; set; }
        public string checkin_remarks { get; set; }
        public string checkout_remarks { get; set; }
        public string status { get; set; }
        public int? hpsm_ticketid { get; set; }


        public string task_id { get; set; }
        public string sub_status { get; set; }
        public string customer_id { get; set; }
        public string customer_rmn { get; set; }
        // public string customer_preferred_time { get; set; }
        public string customer_appointment_date { get; set; }
        public string created_on { get; set; }
        public string issue_comment { get; set; }
        public string service_status { get; set; }

        public string customer_add { get; set; }
        public int bookingid { get; set; }
        public string atomic_id { get; set; }
        public string access_type { get; set; }
        public string package_name { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        public string local_convergence_point { get; set; }
        public string secondary_contact { get; set; }
        public string current_cpesn { get; set; }
        public string listextension_boxsn { get; set; }
        public string cpe_type { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string facility { get; set; }
        public string site_type { get; set; }
        public string subscriber_name { get; set; }
        public string color_code { get; set; }

        public string button_color { get; set; }
        public string email_id { get; set; }
        public string addressline1 { get; set; }
        public string addressline2 { get; set; }
        public string addressline3 { get; set; }
        public string addressline4 { get; set; }
        public string addressline5 { get; set; }
        public string addresstype { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string stateorprovince { get; set; }
        public string addressid { get; set; }
        public string remarks { get; set; }
        public string slot_time { get; set; }

        public string jo_category { get; set; }
        public string account_number { get; set; }
        public string reference_id { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }
        public string cpe_ref_serial { get; set; }
        public string cpe_uom { get; set; }
        public string cpe_wh { get; set; }
        public string action { get; set; }

        public int master_slot_id { get; set; }

        [NotMapped]
        public double distance { get; set; }

        public string customer_category { get; set; }
        public string customer_segment { get; set; }
        public string msp_cic_identifier { get; set; }
        public string Bandwidth { get; set; }
        public int ticket_source_id { get; set; }

        public string main_issue_type { get; set; }

        public string cls { get; set; }
        public string parent_hpsmid { get; set; }
        public AssignedTaskDetailTT()
        {
            customer_rmn = "N/A";
            issue_comment = "N/A";
            service_status = "N/A";
            task_id = "N/A";
            customer_add = "N/A";
        }
    }

    public class GetAttachmentDetailsIn
    {
        public string job_id { get; set; }
        public string upload_type { get; set; }
        public string screen { get; set; }
    }
    public class DeleteAttachmentsIn
    {
        public int attachmentId { get; set; }

        public string image_size { get; set; }
        public string job_id { get; set; }
        public int step_order { get; set; }
    }
    public class GetAssignedTaskDetailsIn
    {
        public int userId { get; set; }
    }
    public class nmsticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string ticketId { get; set; }

        public DateTime? createdTime { get; set; }
        public string impactType { get; set; }
        public string deviceRole { get; set; }
        public string gisId { get; set; }
        public string remarks { get; set; }
        public string inventoryName { get; set; }
        public string eventSource { get; set; }
        public string port { get; set; }
        public string trailId { get; set; }
        public string region { get; set; }
        public string rootcauseDesc { get; set; }
        public string cityDivision { get; set; }
        public string popLocation { get; set; }
        public string area { get; set; }
        public string ticketType { get; set; }

        public string probableCause { get; set; }
        public string specificProblem { get; set; }

        public DateTime? clearedTime { get; set; }
        public string ticketStatus { get; set; }
        public string ownerGroup { get; set; }
        public string assignedGroup { get; set; }
        public DateTime? etr { get; set; }
        public string previousAction { get; set; }
        public string alarmDispName { get; set; }
        public string fetchCustomers { get; set; }
        public int CustomerCount { get; set; }
        public string NetworkEntity { get; set; }

        public string province_name { get; set; }

        public string primary_cluster_id { get; set; }
        public string primary_cluster_name { get; set; }
        public string pop_id { get; set; }
        public string pop_name { get; set; }
        public string olt_id { get; set; }
        public string olt_name { get; set; }
        public string pon_port_id { get; set; }
        public string fat_id { get; set; }
        public string primary_splitter_id { get; set; }
        public string secondary_splitter_id { get; set; }
        public string society_id { get; set; }
        public string society_name { get; set; }
        public string hpsmticketstatus { get; set; }
        public string hpsmticketremark { get; set; }
        public string status_history { get; set; }
        public string Outage_address { get; set; }
        public string location { get; set; }
        public string boundary { get; set; }
        public string jsondata { get; set; }
    }
    public class ViewNMSTicketDetails
    {
        public ViewTicketFilter viewTicketFilter { get; set; }
        public List<nmsticket> TaskDetails { get; set; }
        public ViewNMSTicketDetails()
        {
            viewTicketFilter = new ViewTicketFilter();
            TaskDetails = new List<nmsticket>();
        }

    }
    public class ViewTicketFilter
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
        public int userId { get; set; }
        public int subordinateUserId { get; set; }
        public int duration { get; set; }
        public string status { get; set; }
        public string issueType { get; set; }
        public string ticketId { get; set; }
    }
    public class ViewHPSMTicketDetails
    {
        public ViewHPSMTicketDetails()
        {
            viewTicketFilter = new ViewTicketFilter();
            ticketDetails = new List<HPSM_Ticket_Master_History>();
            TaskDetails = new List<Models.WFM.Task>();
        }
        public ViewTicketFilter viewTicketFilter { get; set; }
        public List<HPSM_Ticket_Master_History> ticketDetails { get; set; }
        public List<Models.WFM.Task> TaskDetails { get; set; }
        public List<VW_HPSM_Ticket_Master_History> ticket_Details { get; set; }
    }

    public class ViewVSFTicketDetails
    {
        public int totalRecord { get; set; }
        public int ticketId { get; set; }
        public List<VSF_TICKET_HISTORY> ticketDetails { get; set; }
        public ViewVSFTicketDetails()
        {
            totalRecord = 0;
            ticketDetails = new List<VSF_TICKET_HISTORY>();
        }
    }

    public class VSF_TICKET_HISTORY
    {
        [Key]
        public int history_id { get; set; }
        public int? hpsm_ticketid { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public DateTime mobile_time { get; set; }
        public DateTime modified_on { get; set; }
        public string status { get; set; }
        public string sub_status { get; set; }
        public string assign_id { get; set; }
        public int issue_id { get; set; }
    }

    public class Issue_Resolution_Type_Master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int issue_resolution_id { get; set; }
        public string issue_resolution_code { get; set; }
        public string issue_resolution_description { get; set; }
        public int issue_type_id { get; set; }
        public int issue_sub_type_id { get; set; }
        public int issue_resolution_type_id { get; set; }
        public int issue_parent_resolution_id { get; set; }
        public string issue_resolution_close_id { get; set; }
    }
    public class Circle_Master
    {
        public Circle_Master() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int circle_id { get; set; }
        public string circle_code { get; set; }
        public string circle_description { get; set; }
    }
    public class NOTIFICATION_ALERTS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int alert_id { get; set; }
        public int seq_id { get; set; }
        public int route_assignment_id { get; set; }
        public string notification_type { get; set; }
        public DateTime last_notified_on { get; set; }
        public int tracking_id { get; set; }
    }
    public class VW_USER_MANAGER_RELATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string fcm_key { get; set; }
        public int? manager_id { get; set; }
        public string manager_fcmkey { get; set; }
        public string manager_name { get; set; }
        public string full_name { get; set; }
    }
    public class NOTIFICATION_ALERTS_HISTORY
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int history_id { get; set; }
        public int user_id { get; set; }
        public int route_assignment_id { get; set; }
        public string fcm_key { get; set; }
        public string notification_type { get; set; }
        public string message { get; set; }
        public int seq_id { get; set; }
        public int tracking_id { get; set; }
        //public DateTime last_notified_on { get; set; }

    }

    public class FCMResponse
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<FCMResult> results { get; set; }
    }
    public class FCMResult
    {
        public string message_id { get; set; }
    }

    public class tbl_wfm_jobstatus
    {
        [Key]
        public int id { get; set; }
        public string action { get; set; }
        public string status { get; set; }
        public string sub_status { get; set; }
        public bool is_active { get; set; }
    }

    public class job_status_type
    {
        public string key { get; set; }
        public string value { get; set; }

    }

    public class job_status_type_rca
    {
        public string key { get; set; }
        public string value { get; set; }

        public List<rca> rca { get; set; }

    }

    public class rca
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class RCADetailByJobId
    {
        public string Job_Id { get; set; }
        public string Remarks { get; set; }
        public string RC { get; set; }
        public string RCA { get; set; }
    }


    public class AdditionalMaterialMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string material_code { get; set; }
        public string material_name { get; set; }
        public string material_unit { get; set; }
        public string ticket_type { get; set; }
        //public string sap_material_code { get; set; }
        public bool is_active { get; set; }
        public bool is_serialized { get; set; }
    }

    public class AdditionalMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string jobid { get; set; }
        public string material_id { get; set; }
        public string details { get; set; }
        public string serial_no { get; set; }
        public string wh_code { get; set; }
        public string item_code { get; set; }
        public string quantity { get; set; }
        public string uom { get; set; }
        public int reading_from { get; set; }
        public int reading_to { get; set; }
        public string batch_no { get; set; }
    }

    public class TriggerActivateDetail
    {
        public string orderId { get; set; }
        public string portNumber { get; set; }
        public string macId { get; set; }
        public string cpeModel { get; set; }
        public string cpeMake { get; set; }
        public string serialNo { get; set; }
        public string stbSerialNo { get; set; } // 
        public string casSerialNo { get; set; }// cpe serial number
        public string facility { get; set; }
    }

    public class User_Detail
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string grantType { get; set; }
    }
    public class JoCategoryRoleMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string jo_category { get; set; }
        public string role_name { get; set; }
        public string order_type { get; set; }
        public string workflow { get; set; }

        public string jo_category_code { get; set; }
        public string order_type_code { get; set; }

    }

    public class AddRemoveservice
    {
        public string service { get; set; }
        public string action { get; set; }
    }




    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Service
    {
        [Required]
        public string facility { get; set; }
        [Required]
        public string Action { get; set; }
        [Required]
        public string atomicidActivate { get; set; }
        public string atomicidClose { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        public string cpe_type { get; set; }
        public string cpe_portno { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string listextension_boxsn { get; set; }
        public string cpe_mac_address { get; set; }
        public string cpe_item_code { get; set; }
        public string package_name { get; set; }
        public string ip { get; set; }
        public string gw { get; set; }
        public string sm { get; set; }
        public string dns { get; set; }
        public string adns { get; set; }
    }

    //public class ServiceFacilityMaster
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }     
    //    public string service_facility_name { get; set; }      
    //    public string service_facility_description { get; set; }
    //    public bool is_active { get; set; }
    //    public string service_facility_code { get; set; }

    //}

    public class JoCategoryMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string jo_category_name { get; set; }
        public string jo_category_description { get; set; }
        public bool is_active { get; set; }
        public string jo_category_code { get; set; }
        public int priority { get; set; }

    }

    public class wfm_notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string hpsmid { get; set; }

        public string notification_type { get; set; }

        public int manager_id { get; set; }
        public string user_name { get; set; }
        public int reporting_manager { get; set; }
        public int jo_manager { get; set; }
        public string message { get; set; }
        public int isread { get; set; }
        public int task_tracking_id { get; set; }
        public int frt_id { get; set; }
        public int? hpsm_ticketid { get; set; }
        public int issue_id { get; set; }
        public string email_id { get; set; }
        public DateTime? appointment_date { get; set; }
        public DateTime? created_on { get; set; }

        public wfm_notification()

        {
            hpsm_ticketid = 0;
        }


    }

    public class ViewNotificationFilter
    {
        public int roleId { get; set; }
        public int userId { get; set; }
        public int managerId { get; set; }
        public string searchText { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int pageSize { get; set; }
        public int noOfPages { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int page { get; set; }
        public int duration { get; set; }
        public string status { get; set; }
    }

    public class ViewNotification
    {
        public ViewNotification()
        {
            viewNotificationFilter = new ViewNotificationFilter();
        }

        public IList<wfm_notification> lstNotificationDetail { get; set; }
        public ViewNotificationFilter viewNotificationFilter { get; set; }
    }


    //public class JoTypeMaster
    //{
    //    [Key]
    //    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    //    public int id { get; set; }
    //    public string jo_name { get; set; }
    //    public string jo_description { get; set; }
    //    public bool is_active { get; set; }
    //    public string jo_code { get; set; }

    //}

    public class wfm_email_sms_log
    {
        public int id { get; set; }
        public string joborderid { get; set; }
        public DateTime appointment_date { get; set; }
        public int emailstatus { get; set; }

        public DateTime emaildeliverytime { get; set; }


        public string emailremark { get; set; }
        public int smsstatus { get; set; }
        public DateTime smsdeliverytime { get; set; }
        public string smsremark { get; set; }
        public string type { get; set; }
    }

    public class wfm_notification_template
    {
       
        public int id { get; set; }
        public string type { get; set; }
        public string subject { get; set; }
        public string email { get; set; }
        public string sms { get; set; }
    }

    public class CreateTTJob
    {


        public string tt_id { get; set; }
        public string tt_category { get; set; }
        public string tt_type { get; set; }
        public string tt_subtype { get; set; }
        public DateTime tt_creationdatetime { get; set; }
        public string nap_port { get; set; }
        public string node { get; set; }
        public string cpe_brand { get; set; }
        public string cpe_model { get; set; }
        public string cpe_serialno { get; set; }
        public string cpe_macaddress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public address address { get; set; }
        public string customer_category { get; set; }
        public string customer_segment { get; set; }
        public string msp_cic_Identifier { get; set; }
        public string bandwidth { get; set; }
        public string remarks { get; set; }
        public string customer_name { get; set; }
        public string account_number { get; set; }
        public string package { get; set; }

        public string primary_contact { get; set; }
        public string secondary_contact { get; set; }
        public string email { get; set; }
        public string customer_id { get; set; }
        public string circuit_id { get; set; }
        public string service_type { get; set; }
        public string product_instance_id { get; set; }
        public string cpe_item_code { get; set; }
    }

    public class wfm_tt_rc
    {
        public int id { get; set; }
        public string rc_code { get; set; }
        public string rc_description { get; set; }
    }
    public class wfm_tt_rca
    {
        public int id { get; set; }
        public string rc_code { get; set; }
        public string rca_code { get; set; }
        public string rca_description { get; set; }
    }
    public class ReqSendBackTicket
    {
        public string job_id { get; set; }
        public string remarks { get; set; }
        public string main_issue_type { get; set; }
        public string rca { get; set; }
    }

    public class SendBackTicket
    {
        public string itemID { get; set; }
        public string outcomeRefId { get; set; }
        public string remarks { get; set; }
    }

    public class SendBackTicketResponse
    {
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }

    public class CloseTicket
    {
        public string itemID { get; set; }
        public string outcomeRefId { get; set; }
        //root cause
        public string rc { get; set; }
        //root cause analysis
        public string rca { get; set; }
        public string remarks { get; set; }
    }
    public class IssueType
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class FRTCapacity
    {
        public int user_id { get; set; }
        public DateTime? assigned_date { get; set; }
        public string jobid { get; set; }
        public string status { get; set; }
    }
    public class GetUserPermissionArea
    {
        public string country_name { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string subdistrict_name { get; set; }
        public string block_name { get; set; }
    }
    public class ViewMaterialDetail
    {
        public List<ViewMaterialDetail> ViewMaterialDetails;

        public int totalRecord { get; set; }
        public int material_id { get; set; }
        public string material_name { get; set; }
        public string material_code { get; set; }
        public string jobid { get; set; }
        public string serial_no { get; set; }
        public string wh_code { get; set; }
        public string item_code { get; set; }
        public string quantity { get; set; }
        public string uom { get; set; }

       
    }

    public class Payment_Details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int payment_id { get; set; }
        public int hpsm_ticket_id { get; set; }
        public string job_id { get; set; }
        public string itemdesc { get; set; }
        public decimal itemamount { get; set; }
    }

}
