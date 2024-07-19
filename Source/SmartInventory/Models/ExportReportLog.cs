using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ExportReportLog
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int sno { get; set; }
        public int user_id { get; set; }
        public DateTime? export_started_on { get; set; }
        public DateTime? export_ended_on { get; set; }
        public string file_name { get; set; }
        public string file_type { get; set; }
        public int total_entity { get; set; }
        public int planned { get; set; }
        public int asbuilt { get; set; }
        public int dormant { get; set; }
        public string applied_filter { get; set; }
        public string sp_geometry { get; set; }
        public string status { get; set; }
        public string file_location { get; set; }
        public string file_extension { get; set; }
        public string log_type { get; set; }
    }
    public class ExportReportLogVM
    {
        public List<ExportReportLogInfo> ExportLog { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public ExportReportLogVM()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    public class ExportReportLogInfo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int sno { get; set; }
        public int user_id { get; set; }
       public string user_name { get; set; }
        public DateTime? export_started_on { get; set; }
        public DateTime? export_ended_on { get; set; }
        public string file_name { get; set; }
        public string file_type { get; set; }
        public int total_entity { get; set; }
        public int planned { get; set; }
        public int asbuilt { get; set; }
        public int dormant { get; set; }
        public string applied_filter { get; set; }
        public string sp_geometry { get; set; }
        public string status { get; set; }
        public string file_location { get; set; }
        public int totalRecords { get; set; }
        public int S_No { get; set; }
        public int page_count { get; set; }
        public string file_extension { get; set; }
        public string log_type { get; set; }
    }
}
