using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class TicketStepsMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int ticket_type_id { get; set; }
        public string step_name { get; set; }
        public string step_description { get; set; }
        public string icon_content { get; set; }
        public string icon_class { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public bool is_processed { get; set; }


    }
}
