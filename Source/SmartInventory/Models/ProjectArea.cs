using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class ProjectArea:IReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        [Required]
        public string network_id { get; set; }
      
        public string projectarea_name { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public string remarks { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public string status { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstAreaRFS { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string status_remark { get; set; }
        public ProjectArea()
        {
            objPM = new PageMessage();
             
        }

    }
}
