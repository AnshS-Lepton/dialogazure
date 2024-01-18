using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Admin
{
    public class ProjectMaster : ListTemplateForDropDown
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string project_name { get; set; }
        [Required]
        public string project_code { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string planing_name { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string planing_code { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string purpose_name { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string purpose_code { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string workorder_name { get; set; }
        [Required(ErrorMessage = "Required.")]
        public string workorder_code { get; set; }

        public string network_stage { get; set; }

        public int created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public int user_id { get; set; }

         [NotMapped]
        public PageMessage pageMsg { get; set; }
         public ProjectMaster()
        {
            pageMsg = new PageMessage();
            lstBindProject = new List<KeyValueDropDown>();
            lstBindPlanning = new List<KeyValueDropDown>();
            lstBindWorkorder = new List<KeyValueDropDown>();
            lstBindPurpose = new List<KeyValueDropDown>();
             
        }


        [NotMapped]
         public ProjectCodeMaster saveProjectCodeDetail { get; set; }

        [NotMapped]
        public PlanningCodeMaster savePlanningCodeDetail { get; set; }

        [NotMapped]
        public WorkorderCodeMaster saveWorkorderCodeDetail { get; set; }


        [NotMapped]
        public PurposeCodeMaster savePurposeCodeDetail { get; set; }

    }

    public class ProjectCodeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }

       
        public string project_name { get; set; }
       
        public string project_code { get; set; }
           public int? created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        public string network_stage { get; set; }

        [NotMapped]
        public int user_id { get; set; }

         [NotMapped]
        public PageMessage pageMsg { get; set; }
         public ProjectCodeMaster()
        {
            pageMsg = new PageMessage();
          
        }

    }


    public class PlanningCodeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
       
        public string planning_name { get; set; }
       
        public string planning_code { get; set; }

        public int project_id { get; set; }

        public string network_stage { get; set; }

        public int created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public int user_id { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }
        [NotMapped]
        public string project_code { get; set; }
        [NotMapped]
        public List<int> ddlproject_ids { get; set; }
        public PlanningCodeMaster()
        {
            pageMsg = new PageMessage();
        }
    }

    public class WorkorderCodeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
      
        public string workorder_name { get; set; }
     
        public string workorder_code { get; set; }

        public int planning_id { get; set; }

        public string network_stage { get; set; }

        public int? created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }

        [NotMapped]
        public string planning_code { get; set; }

        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public List<int> ddlplanning_ids { get; set; }

        public WorkorderCodeMaster()
        {
            pageMsg = new PageMessage();
        }

    }

    public class PurposeCodeMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
      
        public string purpose_name { get; set; }
      
        public string purpose_code { get; set; }

        public int workorder_id { get; set; }

        public string network_stage { get; set; }

        public int created_by { get; set; }
        //public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }

        [NotMapped]
        public PageMessage pageMsg { get; set; }

        [NotMapped]
        public string workorder_code { get; set; }

        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public List<int> ddlworkorder_ids { get; set; }

        public PurposeCodeMaster()
        {
            pageMsg = new PageMessage();
        }

    }

 
    public class ViewProjectDetail
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public string status { get; set; }
        public string networkStage { get; set; }

    }


    public class ViewProjectDetailsList : ListTemplateForDropDown
    {
        public ViewProjectDetailsList()
        {
            viewProjectDetail = new ViewProjectDetail();
            //viewProjectDetail.searchBy = "project_name"; // Set Filter by User_Id
            viewProjectDetail.status = string.Empty;
            viewProjectDetail.searchText = string.Empty;
        }
        public IList<ViwProjectList> ProjectDetailList { get; set; }
        public ViewProjectDetail viewProjectDetail { get; set; }
    }



    public class ViwProjectList 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
       
        public string project_name { get; set; }
       
        public string project_code { get; set; }
       
        public string planing_name { get; set; }
       
        public string planing_code { get; set; }
       
        public string purpose_name { get; set; }
       
        public string purpose_code { get; set; }
       
        public string workorder_name { get; set; }
       
        public string workorder_code { get; set; }

        public string network_stage { get; set; }

        public int created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? modified_by { get; set; }
        
        public DateTime? modified_on { get; set; }
        public string created_by_text { get; set; }
        public int totalRecords { get; set; }

        public string modified_by_text { get; set; }

        public Boolean is_active { get; set; }
       
    
    }



    public class ListTemplateForDropDown
    {
        [NotMapped]
        public List<KeyValueDropDown> lstNetworkstage { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstBindSearchBy { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindProject { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindPlanning { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindWorkorder { get; set; }

        [NotMapped]
        public List<KeyValueDropDown> lstBindPurpose { get; set; }

    }

  
      
  

}
