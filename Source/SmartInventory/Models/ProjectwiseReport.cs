using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProjectwiseReportFilter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
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
        public string orderBy { get; set; }
        public int user_id { get; set; }
        public int systemId { get; set; } = 0;
        public int project_id { get; set; } = 0;
        public int planning_id { get; set; } = 0;
        public int workorder_id { get; set; } = 0;
        public List<ProjectwiseReportRequestLog> lstReportLogs { get; set; }
        public List<KeyValueDropDown> lstRegion { get; set; }
        public List<KeyValueDropDown> lstProvince { get; set; }
        public List<KeyValueDropDown> lstBlock { get; set; }
       
        public ProjectwiseReportFilter()
        {
            lstReportLogs = new List<ProjectwiseReportRequestLog>();
            lstRegion = new List<KeyValueDropDown>();
            lstProvince = new List<KeyValueDropDown>();
            lstBlock = new List<KeyValueDropDown>();
        }
    }

    public class ProjectwiseReportRequestLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string report_type { get; set; }
        public string file_name { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? completed_on { get; set; }
        public string status { get; set; }
        public int block_code { get; set; }
        public string filepath { get; set; }
        public string error_description { get; set; }
    }
}
