using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models
{
    public class TicketMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ticket_id { get; set; }
        [Required]
        public string can_id { get; set; }
        [Required]
        public string customer_name { get; set; }
        [Required]
        public string bld_rfs_type { get; set; }
        [Required]
        public string address { get; set; }
        public string contact_no { get; set; }
        public string pin_code { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        [Required]
        public DateTime? target_date { get; set; }
        public string ticket_reference { get; set; }
        public string ticket_description { get; set; }
        [Required]
        public string reference_type { get; set; }
        public string ticket_status { get; set; }
        public DateTime? modified_on { get; set; }
        public int last_step_id { get; set; }
        [NotMapped]
        public TicketStepsMaster stepsMaster { get; set; }
        [Required]
        public int ticket_type_id { get; set; }
        //[Required]
        // public string ticket_type { get; set; }
        [Required]
        public string building_code { get; set; }
        public int assigned_by { get; set; }
        [Required]
        public string assigned_to { get; set; }
        public DateTime? assigned_date { get; set; }
        public string assigned_type { get; set; }
        public string reference_ticket_id { get; set; }
        public int created_by { get; set; }
        public int ticket_status_id { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? completed_on { get; set; }
        public int completed_by { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstTicketType { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstRfsType { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstAssignedBy { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstAssignedTo { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstReferenceType { get; set; }
        //[NotMapped]
        //public PageMessage objPM { get; set; }
        [NotMapped]

        public List<userName> lstUserName { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<TicketTypeMaster> lstTicketTypeMaster { get; set; }
        public TicketMaster()
        {
            pageMsg = new PageMessage();
            lstTicketType = new List<DropDownMaster>();
            lstRfsType = new List<DropDownMaster>();
            lstAssignedBy = new List<DropDownMaster>();
            lstAssignedTo = new List<DropDownMaster>();
            lstUserName = new List<Models.userName>();
            lstReferenceType = new List<DropDownMaster>();
            lstTicketTypeMaster = new List<Models.TicketTypeMaster>();
            target_date = DateTimeHelper.Now;
        }
        [NotMapped]
        public bool IsCustomerExist { get; set; }
        [NotMapped]
        public bool IsBuildingExist { get; set; } 
        
        [NotMapped]
        public int user_role_id { get; set; }
        [NotMapped]
        public int user_id { get; set; }
    }

    public class TicketTypeMaster
    {
        public int id { get; set; }
        public string ticket_type { get; set; }

        public string description { get; set; }
        public string color_code { get; set; }
        public string icon_content { get; set; }
        public string icon_class { get; set; }
        public string module { get; set; }
        public string abbreviation { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }

        [NotMapped]
        public bool is_create { get; set; }
        [NotMapped]
        public bool is_edit { get; set; }
        [NotMapped]
        public bool is_view { get; set; }
        [NotMapped]
        public bool is_approve { get; set; }
        public TicketTypeMaster()
        {
            objPM = new PageMessage();

        }
    }
    public class ViewTicketType : CommonGridAttributes
    {
        public List<TicketTypeMaster> listTTM { get; set; }
        public ViewTicketType()
        {
            listTTM = new List<TicketTypeMaster>();
        }
    }
    public class TicketMasterViewModel
    {
        public List<TicketMasterGrid> lstTicketMaster { get; set; }

        public TicketManagerFilter objTicketManagerFilter { get; set; } 
        public List<ticketStatus> objticketstatus { get; set; }
        public TicketMaster objTicketMaster { get; set; }
        [NotMapped]

        public List<userName> lstUserName { get; set; }
        public TicketMasterViewModel() 
        {
            lstTicketMaster = new List<Models.TicketMasterGrid>();
            objTicketManagerFilter = new Models.TicketManagerFilter();
            objticketstatus = new List<ticketStatus>();
            objTicketMaster = new TicketMaster();
            lstUserName = new List<Models.userName>();
        }
    }
    public class TicketMasterGrid
    {
        //public int ticket_id { get; set; }
        //public string ticket_type { get; set; }
        //public string ticket_description { get; set; }
        //public string reference_type { get; set; } 
        //public string ticket_status { get; set; }
        //public DateTime? assigned_date { get; set; }
        //public int can_id { get; set; }
        //public string assigned_by { get; set; }
        //public string assigned_to { get; set; }
        //[NotMapped]
        //public int totalrecords { get; set; } 
        //public int ticket_id { get; set; }
        public int ticket_id { get; set; }
        public string ticket_type { get; set; }
        public string reference_type { get; set; }
        public int can_id { get; set; }
        public string customer_name { get; set; }
        public string address { get; set; }
        public string ticket_description { get; set; } 
        public string ticket_status { get; set; }
        public string building_code { get; set; }
        public string bld_rfs_type { get; set; } 
        public string assigned_to { get; set; }
        public string completed_by { get; set; }
        public DateTime? completed_on { get; set; }
        public DateTime? target_date { get; set; }
        public DateTime? assigned_date { get; set; }  
        [NotMapped]
        public int totalrecords { get; set; } 
        public string assigned_by { get; set; }
        
        // public DateTime assigned_on { get; set; }
    }

    public class TicketManagerFilter
    {
        public int ticket_id { get; set; }
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
        //public string lstTicketType { get; set; }
        //public string lstStatus { get; set; }
        public string lstAssignedTo { get; set; }
        public List<ticketType> lstTicketType { get; set; }
        public List<status> lstStatus { get; set; }
        //public List<AssignedTo> lstAssignedTo { get; set; }
        public List<TicketMasterGrid> lstTicketMaster { get; set; }
        public List<ticketStatus> objticketstatus { get; set; }
        public TicketMaster objTicketMaster { get; set; }
        [NotMapped] 
        public List<userName> lstUserName { get; set; }
        public TicketManagerFilter()
        {
            lstTicketMaster = new List<Models.TicketMasterGrid>();
            lstTicketType = new List<Models.ticketType>();
            lstStatus = new List<Models.status>();
            //lstAssignedTo = new List<Models.AssignedTo>();
            objticketstatus = new List<Models.ticketStatus>();
            objTicketMaster = new TicketMaster();
            lstUserName = new List<Models.userName>();
        }
    }
    public class ticketStatus
    {
        public string ticket_status { get; set; }
        public int ticket_count { get; set; }
        public string color_code { get; set; }
        public bool isStatusChecked { get; set; }
    }
    public class ticketType
    {
        public int id { get; set; }
        public string ticket_type { get; set; }
        public bool tickettype { get; set; }
    }
    public class status
    {
        public string ticket_status { get; set; }
    }
    public class AssignedTo
    {
        public string Assigned_to { get; set; }
    }

    public class TempTicketMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ticket_id { get; set; }  
        public string can_id { get; set; } 
        public string customer_name { get; set; } 
        public string rfs_type { get; set; } 
        public string address { get; set; }
        public string contact_no { get; set; }
        public string pin_code { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; } 
        public string target_date { get; set; }
        public string ticket_reference { get; set; }
        public string ticket_description { get; set; } 
        public string reference_type { get; set; }
        public string ticket_status { get; set; }
        public DateTime? modified_on { get; set; }
        public int last_step_id { get; set; } 
        public string ticket_type{ get; set; }  
        public string building_code { get; set; }
        public int assigned_by { get; set; } 
        public string assigned_to { get; set; }
        public DateTime assigned_date { get; set; }
        public string assigned_type { get; set; }
        public string reference_ticket_id { get; set; }
        public int created_by { get; set; }
        public int ticket_status_id { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? completed_on { get; set; }
        public int completed_by { get; set; } 
        public TempTicketMaster() 
        {
              
        }
        
        public int uploaded_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
    }

    public class TicketAttachments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int ticket_id { get; set; }
        public string org_file_name { get; set; }
        public string file_name { get; set; }
        public string file_extension { get; set; }
        public string file_location { get; set; }
        public string upload_type { get; set; }
        public int uploaded_by { get; set; }
        public DateTime? uploaded_on { get; set; }
        public int file_size { get; set; }
    }
    public class customerTicketStatus
    {
        public string ticket_id { get; set; }
        public string ticket_status { get; set; }
        public string can_id { get; set; }
        public string created_on { get; set; }
        public string assigned_to { get; set; }
        public string assigned_date { get; set; }
        public string target_date { get; set; }
        public string remarks { get; set; }
    }


    public class Customers
    {
        [Required(ErrorMessage = "CAN ID is required.")]

        public string can_id { get; set; }
        [Required(ErrorMessage = "Latitude is required.")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double latitude { get; set; }
        [Required(ErrorMessage = "Longitude is required.")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double longitude { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string address { get; set; }
    }
    public class CustomerTicketMaster
    {
        [NotMapped]
        [Required(ErrorMessage = "Connection entity type is required.")]
        public string connection_entity_type { get; set; }
        [Required(ErrorMessage = "Connection entity ID is required.")]

        [NotMapped]
        public int connection_entity_id { get; set; }
        [Required(ErrorMessage = "Reference ID is required.")]
        [NotMapped]
        public string reference_id { get; set; }
         [Required(ErrorMessage = "Ticket Source is required.")]
        [NotMapped]
        public string ticket_source { get; set; }
        public Customers customer { get; set; }
    }
}
