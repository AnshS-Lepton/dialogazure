using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{

    public class UserTimeSheetVM
    {
        public UserTimeSheetInput userTimeSheetInput { get; set; }
        // public IList<FrequencyType> lstfreqType { get; set; }
        public IList<User_Master> lstUserDetail { get; set; }
        // public IList<VW_Route_Master> lstRoutes { get; set; }
        public User_Master usermaster { get; set; }

        public RoleMaster objRoleMaster { get; set; }

        public UserTimeSheetVM()
        {
            userTimeSheetInput = new UserTimeSheetInput();
            usermaster = new User_Master();
        }

    }

    public class UserTimeSheetInput
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_timesheet_id { get; set; }
        public int user_id { get; set; }
        public int manager_id { get; set; }

        public string start_date { get; set; }
        [Required(ErrorMessage = "Select start date")]
        public string end_date { get; set; }
        [Required(ErrorMessage = "Select start time ")]
        public string start_time { get; set; }
        [Required]
        public string end_time { get; set; }
        [Required]

        public string working_days { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime modified_on { get; set; }
        public int modified_by { get; set; }
        public bool is_active { get; set; }
        public string confirmmsg { get; set; }
        public string workingmode { get; set; }


    }

    //[Serializable]
    public class UserDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }
        public string user_name { get; set; }
        //public string password { get; set; }
        public string full_name { get; set; }
        public string email_id { get; set; }
        public int is_active { get; set; }
        public int? manager_id { get; set; }
        public string manager_name { get; set; }
        public string manager_phone { get; set; }
        public string user_img { get; set; }
        public int role_id { get; set; }
        public string role_name { get; set; }
        public string phone { get; set; }
        //public string about_user { get; set; }   
        public string createdBy { get; set; }
        public string modifiedBy { get; set; }
        public DateTime? created_on { get; set; }
        public DateTime? modified_on { get; set; }
        public int circle_Id { get; set; }
        public string circle_name { get; set; }
        // public string employee_id { get; set; }

        public int sub_role_id { get; set; }
        public string manager_role_name { get; set; }

    }


    public class User_Master
    {
        [Key]
        public int? user_id { get; set; }
        public string name { get; set; }
        public int manager_id { get; set; }
        public int role_id { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string user_email { get; set; }
        public bool is_active { get; set; }
        public int module_id { get; set; }
        public string user_img { get; set; }
        public int template_id { get; set; }
        public int group_id { get; set; }
        public bool is_deleted { get; set; }
        public string remarks { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public int S_No { get; set; }

        [NotMapped]
        public string reporting_manager { get; set; }

        [NotMapped]
        public string group_name { get; set; }
    }

    public class User_TimeSheet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_timesheet_id { get; set; }
        public int user_id { get; set; }
        // public string user_name { get; set; }
        public int manager_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string working_days { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int modified_by { get; set; }
        public bool is_active { get; set; }
        // public string  active { get; set; }



    }

    public class User_TimeSheetFilter
    {
        public string searchText { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int pageSize { get; set; }
        public int noOfPages { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get { return page == 0 ? 1 : page; } set { } }
        public string sort { get; set; }
        public string sortdir { get; set; }
        public int page { get; set; }
        public int userId { get; set; }
        public int managerId { get; set; }
        public int roleId { get; set; }
        public int duration { get; set; }
    }

    public class User_TimeSheetVW
    {

        public int user_timesheet_id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string full_name { get; set; }
        public int manager_id { get; set; }
        public string manager_name { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string working_days { get; set; }
        public string working_days_name { get; set; }
        public DateTime? created_on { get; set; }
        public int created_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int modified_by { get; set; }
        public bool is_active { get; set; }
        public string active_status { get; set; }
    }

    public class RosterVW
    {
        public int _user_id { get; set; }
        public int _manager_id { get; set; }
        public int _start_time { get; set; }
        public int _end_time { get; set; }
    }
    public class ViewUser_TimeSheet
    {
        public ViewUser_TimeSheet()
        {
            viewUserTimeSheetFilter = new User_TimeSheetFilter();
        }
        public IList<User_TimeSheetVW> lstUserTimeSheetDetails { get; set; }
        public User_TimeSheetFilter viewUserTimeSheetFilter { get; set; }
    }

}

