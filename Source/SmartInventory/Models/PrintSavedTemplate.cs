using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{


    public class PrintSavedTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }
        public string template_name { get; set; }
        public int user_id { get; set; }
        public string job_id { get; set; }
        public string department { get; set; }
        public string plotted_by { get; set; }
        public string team { get; set; }
        public string drawn_by { get; set; }
        public string checked_by { get; set; }
        public string rechecked_by { get; set; }
        public string approved_by { get; set; }
        public string x_document_index { get; set; }
        public string y_document_index { get; set; }
        public string phase { get; set; }
        public string plan { get; set; }
        public string prov_dir { get; set; }
        public string fdc_no { get; set; }
        public string olt { get; set; }
    }

}
