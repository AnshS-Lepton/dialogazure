using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class EmailLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string subject { get; set; }
        public string entity_type { get; set; }
        public string entity_id { get; set; }
        public string email_from { get; set; }
        public string email_to { get; set; }
        public string email_to_cc { get; set; }
        public string body { get; set; }
        public string status { get; set; }
        public DateTime? sent_on { get; set; }
    }
    public class ViewEmailLog
    {
        public List<EmailLog> lstViewEmailLog { get; set; }
        public ViewEmailLog()
        {
            lstViewEmailLog = new List<EmailLog>();
        }
    }
}
