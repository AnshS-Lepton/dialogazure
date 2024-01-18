using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Web;

namespace Models
{
    public class JobPackMaster
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string job_type { get; set; }
        [Required]
        public string job_description { get; set; }
        [Required]
        public string nims_no { get; set; }
        [Required]
        public string ngwfmt_project_id { get; set; }
        public string node1 { get; set; }
        public string node2 { get; set; }
        public string grid_ref { get; set; }
        public string highway_authority { get; set; }
        [Required]
        public double? latitude { get; set; }
        [Required]
        public double? longitude { get; set; }
        [Required]
        public string gis_address { get; set; }
        [Required]
        public string manual_address { get; set; }
        public string planner_contact_no { get; set; }
        public string planner_tel_no { get; set; }
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Invalid Email Address!")]
        public string planner_email { get; set; }
        public string delivery_contact_no { get; set; }
        public string delivery_tel_no { get; set; }
        [RegularExpression(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$", ErrorMessage = "Invalid Email Address!")]
        public string delivery_email { get; set; }
        public string remarks { get; set; }
        [Required]
        public string work_instructions { get; set; }
        public string stage1 { get; set; }
        public string stage2{ get; set; }
        public string stage3 { get; set; }
        public int? assigned_to { get; set; }
        public string status { get; set; }
        public int created_by { get; set; }       
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public DateTime? assigned_on { get; set; }
        public int? assigned_by { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public bool isParentModelType { get; set; }
        //[NotMapped]
        //public HttpPostedFileBase ImageFile { get; set; }
       
        public JobPackMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class JobAssignment
    {

        public int system_id { get; set; }       
        public string job_type { get; set; }
        public string job_description { get; set; }
        public string Job_status { get; set; }
        public string assigned_to_user { get; set; }
        public string modified_by_user { get; set; }
        public string modified_date { get; set; }
        public string manual_address { get; set; }
        public string created_date { get; set; }
        public string created_by { get; set; }
        public string assigned_date { get; set; }
        public string assigned_by { get; set; }
        public string remarks { get; set; }    
        public int completed { get; set; }
        public int totalRecords { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }
    public class JobContractUser
    {
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_email { get; set; }
        public string assigned_to { get; set; }
        public string assigned_date { get; set; }
    }
    public class JobFilterAttribute : CommonGridAttributes
    {
        public DateTime? searchFrom { get; set; }
        public DateTime? searchTo { get; set; }
        public int userid { get; set; }
        public int role_id { get; set; }
        public int JobContractUser { get; set; }
        public string JobStatus { get; set; }
    }
    public class ViewJobAssignment
    {
        public List<JobAssignment> lstJobAssignment { get; set; }
        public JobFilterAttribute objFilterAttributes { get; set; }

        public IList<DropDownMaster> ListJobstatus { get; set; }
        public IList<JobContractUser> lstJobContractUser { get; set; }
        public List<JobAssignment> AllJobAssignment { get; set; }
        public ViewJobAssignment()
        {
            objFilterAttributes = new JobFilterAttribute();
            objFilterAttributes.searchText = string.Empty;
            objFilterAttributes.searchFrom = null;
            objFilterAttributes.searchTo = null;           
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }
    public class assignJob
    {
        public int systemId { get; set; }
        [Required(ErrorMessage ="Please select user")]
        public int assignedTo { get; set; }
        public int assignedBy { get; set; }
        public string jobType { get; set; }
        public string jobDescription { get; set; }
        public IList<JobContractUser> lstJobContractUser { get; set; }
        public PageMessage objPM { get; set; }
    }
}
