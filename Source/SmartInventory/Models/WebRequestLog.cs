using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class WebRequestLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int log_id { get; set; }
        public string source { get; set; }
        public string user_name { get; set; }
        public string controller_name { get; set; }
        public string action_name { get; set; }
        public string request { get; set; }
        public string header_attribute { get; set; }
        public DateTime created_on { get; set; }
        public string transaction_id { get; set; }
        public DateTime in_date_time { get; set; }
        public DateTime out_date_time { get; set; }
        public double latency { get; set; }
        public string response { get; set; }
        public string client_ip { get; set; }
        public string server_ip { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
    }


}
