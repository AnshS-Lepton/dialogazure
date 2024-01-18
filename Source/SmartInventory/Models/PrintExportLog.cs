using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{

    public class PrintExportLogVM
    {
        public List<PrintExportLogInfo> printLog { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        public PrintExportLogVM()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
        }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
    }

    public class PrintExportLogInfo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public int totalRecords { get; set; }
        public int S_No { get; set; }
        public string page_title { get; set; }
        public DateTime start_on { get; set; }
        public DateTime? end_on { get; set; }
        public int page_count { get; set; }
        public string file_size { get; set; }
        public string file_path { get; set; }
        public string export_progress { get; set; }
        public string export_status { get; set; }
        public string file_extension { get; set; }
        public string page_type { get; set; }
        public string error_message { get; set; }

    }


    public class PrintExportLog
    {  
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        //public int totalRecords { get; set; }
        //public int S_No { get; set; }
        public string page_title { get; set; }
        public string page_type { get; set; }
        public int page_count { get; set; }
        public string file_path { get; set; }
        public string file_extension { get; set; }
        public string file_size { get; set; }
        public DateTime start_on { get; set; }
        public DateTime? end_on { get; set; }
        public string export_request_params { get; set; }
        public string export_progress { get; set; }
        public string export_status { get; set; }
        public string error_message { get; set; }
        
    }

}
