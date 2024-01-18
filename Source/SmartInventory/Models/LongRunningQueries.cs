using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class LongRunningQueries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pid { get; set; }
        public string runningtime { get; set; }
        public string query { get; set; }
        public string query_start { get; set; }        
        public string state { get; set; }
        [NotMapped]
        public bool status { get; set; }

        [NotMapped]
        public int totalRecords { get; set; }
    }

    public class ViewLongRunningQueries : CommonGridAttributesLongRunningQueries
    {       
        public List<LongRunningQueries> listLongRunningQueries { get; set; }
        public LongRunningQueries objLongRunningQueriesMore { get; set; }
        public string logtype { get; set; }
        public ViewLongRunningQueries()
        {
            listLongRunningQueries = new List<LongRunningQueries>();
        }
    }

    public class CommonGridAttributesLongRunningQueries
    {
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
        public string sort { get; set; }
        public int runningTime { get; set; }
        public string orderBy { get; set; }
        public string searchText { get; set; }
        public string searchBy { get; set; }
        public Boolean is_active { get; set; }
        public string application_access { get; set; }
        public string fileType { get; set; }
        public int customDate { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int history_id { get; set; }
        public CommonGridAttributesLongRunningQueries()
        {
            sort = "";
            orderBy = "";
        }
    }

    public class TimeInterval
    {
        public string time { get; set; }
    }

}
