using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Models
{
    public class NetworkTicket : NetworkTicketRef
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ticket_id { get; set; }
        [Required]
        public int? ticket_type_id { get; set; }
        [Required]
        public string reference_type { get; set; }
        [Required]
        public int? region_id { get; set; }
        [Required]
        public int? province_id { get; set; }
        [Required]
        public int assigned_to { get; set; }=0;
        public string network_id { get; set; }
        [Required]
        public string name { get; set; }
        public DateTime? target_date { get; set; }
        //[NotMapped]
        public int? ticket_status_id { get; set; }
        [Required]
        public string for_network_type { get; set; }
        public string remarks { get; set; }
        public string reference_description { get; set; }        
        [NotMapped]
        public string ticket_status { get; set; }     
        [NotMapped]
        public string Network_Ticket_ID { get; set; }     
        [NotMapped]
        public int user_role_id { get; set; }
        public string reference_ticket_id { get; set; }
        [NotMapped]
        public string source { get; set; }
        public string project_code { get; set; }
        public string account_code { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string modeType { get; set; }
        [NotMapped]
        public Int64 radius { get; set; }
        [NotMapped]
        public string ticket_type_id_match { get; set; }
		[NotMapped]
		public string ticket_type { get; set; }
		[NotMapped]
		public string SystemID { get; set; }
		[NotMapped]
		public string entityType { get; set; }
		[NotMapped]
        public int ticket_type_roleid_match { get; set; }
        [NotMapped]
        public List<TicketTypeMaster> lstTicketTypeMaster { get; set; }

    }
    public class NetworkTicketRef
    {
        [NotMapped]
        public List<Region> lstRegion { get; set; }
        [NotMapped]
        public List<Province> lstProvince { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstReferenceType { get; set; }
        [NotMapped]
        public List<TicketTypeMaster> lstTicketTypeMaster { get; set; }
        [NotMapped]
        public List<userName> lstUserName { get; set; }

        [NotMapped]
        public NetworkTicketinput objNetwork_Ticketinput { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public NetworkTicketRef()
        {
            pageMsg = new PageMessage();
            lstUserName = new List<Models.userName>();
            lstReferenceType = new List<DropDownMaster>();
            lstTicketTypeMaster = new List<Models.TicketTypeMaster>();
            lstProvince = new List<Models.Province>();
            lstRegion = new List<Models.Region>();
            objNetwork_Ticketinput = new NetworkTicketinput();
            lstUserModule = new List<string>();
        }
    }
    public class NetworkTicketStatus
    {
        public string ticket_status { get; set; }
        public Int64 ticket_count { get; set; }
        public string color_code { get; set; }
        public string opacity { get; set; }
        
    }

    public class NetworkTicketType
    {
        public string ticket_type { get; set; }
        public string description { get; set; }
        public string color_code { get; set; }
    }
    public class NetworkTicketinput
    {
        public List<int> regionId { get; set; }
        public List<int> provinceId { get; set; }
        public string SelectedProvinceIds { get; set; }
        public string SelectedRegionIds { get; set; }
        public NetworkTicketinput()
        {
            regionId = new List<int>();
            provinceId = new List<int>();
        }
    }
    public class NetworkTicketFilter
    {
        public string searchText { get; set; }
        public string searchByText { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int customDate { get; set; }
        public int userId { get; set; }
        public string orderBy { get; set; }
        public int ticketTypeId { get; set; }
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public int ticket_id { get; set; }
        public string geom_type { get; set; }
        public string entity_action { get; set; }
        public NetworkTicket objNetworkTicket { get; set; }
        public List<NetworkTicketStatus> objticketstatus { get; set; }
        public List<dynamic> lstNetworkTicket { get; set; }
        public List<WebGridColumn> webColumns { get; set; }
        public List<dynamic> lstNWEntityHistory { get; set; }
        public List<dynamic> lstNetworkTicketData { get; set; }
        //public List<dynamic> lstNetworkTicketEntityAction { get; set; }
        public List<entityInfo> lstentityInfo { get; set; }
        [NotMapped]
        public List<TicketTypeMaster> lstTicketTypeMaster { get; set; }

        [NotMapped]
        public IList<DropDownMaster> lstRejectComment { get; set; }
        [NotMapped]
        public bool isAuthorizedToApprove { get; set; }
		[NotMapped]
		public bool isTraceEnabled { get; set; }
		[NotMapped]
		public bool isAllSplitterTraceStatus { get; set; }
        [NotMapped]
        public List<DocumentResult> lstDocumentResult { get; set; }


        public NetworkTicketFilter()
        {
            webColumns = new List<WebGridColumn>();

            objticketstatus = new List<Models.NetworkTicketStatus>();
            lstNetworkTicket = new List<dynamic>();
            objNetworkTicket = new NetworkTicket();
            lstNetworkTicketData = new List<dynamic>();
            lstNWEntityHistory = new List<dynamic>();
            //lstNetworkTicketEntityAction = new List<dynamic>();
            lstentityInfo = new List<entityInfo>();
            lstDocumentResult= new List<DocumentResult>();
        }
    }
    public class DashboardInfo
    {
        public List<NetworkTicketStatus> lstNWStatus { get; set; }
        public List<Dictionary<string, string>> lstNWDetails { get; set; }
    }
    public class NWEntityInfo
    {
        public List<NetworkTicketStatus> lstNWEntityStatus { get; set; }
        public List<Dictionary<string, string>> lstNWEntityDetails { get; set; }
        [NotMapped]
        public bool isAuthorizedToApprove { get; set; }

    }
    public class NetworkTicketList
    {
        public int ticket_id { get; set; }
        public int? ticket_type_id { get; set; }
        public string reference_type { get; set; }
        public int? region_id { get; set; }
        public int? province_id { get; set; }
        public int? assigned_to { get; set; }
        public string network_id { get; set; }
        public string name { get; set; }
        public DateTime? target_date { get; set; }
        public int? ticket_status_id { get; set; }
        public string for_network_type { get; set; }
        public string short_network_type { get; set; }
        public string remarks { get; set; }
        public string reference_description { get; set; }
        public int? created_by { get; set; }
        public string created_on { get; set; }
        public int? modified_by { get; set; }
        public string modified_on { get; set; }
        public string assigned_to_text { get; set; }
        public string created_by_text { get; set; }
        public string modified_by_text { get; set; }
        public string ticket_status { get; set; }
        public string ticket_status_color_code { get; set; }
        public string Network_Ticket_ID { get; set; }
        public string network_status { get; set; }
        public string assigned_by { get; set; }
        public string ticket_type { get; set; }
        public int totalrecords { get; set; }
        public int s_no { get; set; }
        public int user_role_id { get; set; }
        public string geom { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string network_type { get; set; }
        public string reference_ticket_id { get; set; }
        public string source { get; set; }
        public string ticket_type_color_code { get; set; }
    }
    public class NWTicket_List_Status
    {
        public List<NetworkTicketStatus> lstNWStatus { get; set; }
        public List<NetworkTicketList> lstNWDetails { get; set; }
        public List<NetworkTicketType> lstNWTypes { get; set; }

        
        public NWTicket_List_Status()
        {
            lstNWStatus = new List<NetworkTicketStatus>();
            lstNWDetails = new List<NetworkTicketList>();
            lstNWTypes = new List<NetworkTicketType>();

        }
    }
    public class GetGeometryByTicketId {
        public string geom { get; set; }
    }
    public class NWTktEntityLst
    {
        public List<NetworkTicketStatus> lstNWEntityStatus { get; set; }
        public List<NetworkTicketEntityList> lstNWEntityDetails { get; set; }
    }
    public class ticketEntityBounds
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string sp_geometry { get; set; }
        public string geom_type { get; set; }
    }
        public class NetworkTicketEntityList
    {
        public int id { get; set; }
        public string entity_type { get; set; }
        public string entity_title { get; set; }
        public string network_id { get; set; }
        public string entity_name { get; set; }
        public string geom_type { get; set; }
        public string entity_action { get; set; }
        public string status { get; set; }
        public string status_description { get; set; }
        public string status_color { get; set; }
        public string action_color_code { get; set; }
        public string status_remarks { get; set; }
        public int opacity { get; set; }
        public string display_name { get; set; }
        public bool is_revert_allowed { get; set; }
        public string network_status { get; set; }
    }
    public class NetworkTicketEntityListParam
    {
        public int ticket_id { get; set; }
        public int user_id { get; set; }
    }

    public class NWEntitySummaryReport
    {
        public List<dynamic> lstEntitySummaryReport { get; set; }

        public NWEntitySummaryReport()
            {
            lstEntitySummaryReport = new List<dynamic>();
        }

    }
    public class NetworkTicketEmailDetail
    {
        public int ticket_id { get; set; }
        public string projectname { get; set; }

        public string ticketcategory { get; set; }


    }
    public class NWEntityTicketDetails
    {
		public int ticket_id { get; set; }
		public int assigned_to { get; set; } = 0;
		public string network_id { get; set; }
        public string ticket_status { get; set; }
        public string ticket_type { get; set; }
	}

}
