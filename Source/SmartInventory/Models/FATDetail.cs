using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FATDetail
    {
        public string fdc_name { get; set; }
        public int splitter_system_id { get; set; }
        public int splitter_parent_id { get; set; }
        public int input_1_cable_system_id { get; set; }
        public int input_2_cable_system_id { get; set; }
        public string connection_ring_name { get; set; }
        public string input_1_cable_name { get; set; }
        public string input_1_cable_fiber_no { get; set; }
        public string input_1_cable_category { get; set; }
        public string input_1_total_core { get; set; }
        public int input_1_tube_number { get; set; }
        public string input_2_cable_name { get; set; }
        public int input_2_cable_fiber_no { get; set; }
        public string input_2_cable_category { get; set; }
        public string input_2_total_core { get; set; }
        public int input_2_tube_number { get; set; }
        public string remark { get; set; }
        public string is_connected { get; set; }
        public string connection_status { get; set; }
        public string splitter_type { get; set; }
        public int serving_bdb_system_id { get; set; }
        
    }

    public class FATDetailprocess : CommonGridAttributes
    {
        [NotMapped]
        public int systemId { get; set; }
        [NotMapped]
        public string fsa_design_id { get; set; }
        [NotMapped]
        public string fsa_name { get; set; }
        [NotMapped]
        public string fsa_connection_status { get; set; }
        [NotMapped]
        public string p_connection_status { get; set; }         
        [NotMapped]
        public string sortdir { get; set; }
        public List<FATDetail> lstfatdetails { get; set; }
        public int pageSize { get; set; }
        [NotMapped]     
        public string message { get; set; }
        public string BackgroundProcessStatus { get; set; }
        [NotMapped]
        public int fat_process_id { get; set; }
        [NotMapped]
        public bool bt_lock { get; set; }

    }

    public class FatProcessSummary
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int fat_process_id { get; set; }
        public int sub_area_system_id { get; set; }
        public string sub_area_name { get; set; }
        public string process_status { get; set; }
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime modified_on { get; set; }
        public DateTime process_start_time { get; set; }
        public DateTime process_end_time { get; set; }
        public string remarks { get; set; }
        public string approval_status { get; set; }        
    }

    public class FatProcessResult
    {
        public int fat_process_id { get; set; }
        public bool status { get; set; }
        public string MESSAGE { get; set; }      
    }

    public class FatProcessRunningStatus
    {
        public int sub_area_system_id { get; set; }
        public bool process_status { get; set; }
        public string process_message { get; set; }
        public DateTime ? created_on  { get; set; }
        public DateTime ? modified_on { get; set; }
        public bool bt_lock { get; set; }
    }
}