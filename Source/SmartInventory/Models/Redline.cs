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

    public class RedlineFilter
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
        public int system_id { get; set; }
        public int task_id { get; set; }
        public Redline objRedline { get; set; }
        public List<Redline> lstNetworkTicket { get; set; }
        public List<dynamic> lstRedlineEntityHistory { get; set; }
        public RedlineFilter()
        {
            lstNetworkTicket = new List<Redline>();
            objRedline = new Redline();
            lstRedlineEntityHistory = new List<dynamic>();
        }
    }
    public class Redline : RedlineRef
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int task_system_id { get; set; }
        public string task_name { get; set; }
        public string task_type { get; set; }
        public string task_remarks { get; set; }
        public string task_action { get; set; }
        public int assigned_by_id { get; set; }
        public string assigned_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime? created_date { get; set; }
        public DateTime? modified_date { get; set; }
        public char action { get; set; }
        public string assigned_to { get; set; }
        public int assigned_to_id { get; set; }
        public string task_status { get; set; }
        public int status_id { get; set; }
        public string status { get; set; }
        public DateTime? created_on { get; set; }
        public string user_name { get; set; }
        public string remarks { get; set; }
        [NotMapped]
        public int totalrecords { get; set; }
    }

    public class RedlineRef
    {
        [NotMapped]
        public List<AssignedBy> AssignedBy { get; set; }
        [NotMapped]
        public List<userName> AllUsers { get; set; }
        public RedlineRef()
        {
            AllUsers = new List<Models.userName>();
            AssignedBy = new List<Models.AssignedBy>();

        }
    }


    public class RedlineMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string task_name { get; set; }

        public string task_type { get; set; }
        public string task_remarks { get; set; }
        public string task_action { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public string assigned_by { get; set; }
        public int modified_by { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public string action { get; set; }
        [NotMapped]
        public string assigned_to { get; set; }
        [NotMapped]
        public int assigned_to_id { get; set; }
        [NotMapped]
        public int status_id { get; set; }
        [NotMapped]
        public IList<userName> lstAssignedTo { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public IList<dynamic> lstAssignedUsers { get; set; }

        [NotMapped]
        public List<RedlineStatusMaster> statusDropdown { get; set; }

        public string geom { get; set; }
        public string geom_type { get; set; }

    }

    public class RedlineAssignedUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int task_id { get; set; }
        public int user_id { get; set; }
        public int status { get; set; }

        public string remarks { get; set; }
    }

    public class RedlineStatusHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int task_id { get; set; }
        public string remarks { get; set; }
        public int status { get; set; }
        public int assigned_by { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public int assigned_to { get; set; }
        public string task_name { get; set; }
        public string task_type { get; set; }

    }

    public class RedlineStatusMaster
    {
        public int status_id { get; set; }

        public string status_name { get; set; }
    }

    public class AssignedBy
    {
        public string user_name { get; set; }

        public int user_id { get; set; }
    }
}