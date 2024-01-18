using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class Fault: IReference,IGeographicDetails,ICustomCoordinate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string fault_id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        [Required]
        public string fault_type { get; set; }
        [Required]
        public string select_entity { get; set; }
        public int fault_entity_system_id { get; set; }
        public string fault_entity_type { get; set; }
        public string fault_entity_network_id { get; set; }
        [Required]
        public string fault_reason { get; set; }
        
        public string fault_ticket_type { get; set; }
        [Required]
        public string business_type { get; set; }
        public string fault_ticket_id { get; set; }
        public int parent_system_id { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
        public int sequence_id { get; set; }
        [Required]
        public string address { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
         
        public string remarks { get; set; }
        public string fault_status { get; set; }
        public string status { get; set; }
        public string network_status { get; set; } 
        public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; } 
        [NotMapped]
        public FaultStatusHistory objFaultStatusHistory { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstTicketType { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstBusinessType { get; set; }
        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string geom { get; set; }
        [NotMapped]
        public string networkIdType { get; set; } 
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
       public List<EntityDetail> lstEntitiesNearbyFault { get; set; }
        [NotMapped]
        public int user_id { get; set; }
        [NotMapped]
        public string user_name { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public bool isDirectSave { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        public int? primary_pod_system_id { get; set; }
        public int? secondary_pod_system_id { get; set; }
        public string status_remark { get; set; }
        public bool is_new_entity { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string subarea_id { get; set; }
        public string area_id { get; set; }
        public string dsa_id { get; set; }
        public string csa_id { get; set; }
        public string gis_design_id { get; set; }
        [NotMapped]
        public string geographic_id { get; set; }
        [NotMapped]
        public string region_abbreviation { get; set; }
        [NotMapped]
        public string province_abbreviation { get; set; }
        public string st_x { get; set; }
        public string st_y { get; set; }
        public string ne_id { get; set; }
        public string prms_id { get; set; }
        public string jc_id { get; set; }
        public string mzone_id { get; set; }
        [NotMapped]
        public string partner_name { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public Fault()
        {
            objPM = new PageMessage();
            entityType = "Fault";
            //lstEntitiesNearbyFault = new List<Models.EntityDetail>();
            lstUserModule = new List<string>();
        }
    }
    public class FaultStatusHistory 
    {
        public int id { get; set; }
        public int fault_system_id { get; set; }
        [Required]
        public string fault_status { get; set; }
        [Required]
        public DateTime? fault_status_updated_on { get; set; }
       // [Required]
        public string rca { get; set; }
        public string updated_by { get; set; }
        [Required]
        public string requested_by { get; set; }
        
        public string request_comment { get; set; }
        public int created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime created_on { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public IList<DropDownMaster> lstFaultStatus { get; set; } 
        public FaultStatusHistory()
        {

        }
    } 
    public class FaultStatusViewModel
    {
        
        public Fault objFault { get; set; }
        public FaultStatusHistory objFaultStatusHistory { get; set; }
        public FaultStatusViewModel()
        {
            objFault = new Models.Fault();
            objFaultStatusHistory = new Models.FaultStatusHistory();
        }
    }

    public class FaultDetails
    {
        public string fault_id { get; set; }
        public string sequence_id { get; set; } 
    }

    public class ExportFaultHistory 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int fault_system_id { get; set; }
        public string network_id { get; set; }
        public string fault_status { get; set; }
        public DateTime? fault_status_updated_on { get; set; }
        public string rca { get; set; }
        public string updated_by { get; set; }
        public string requested_by { get; set; }
        public string request_comment { get; set; }
        public string modified_by { get; set; }
        public DateTime created_on { get; set; }

    }
}
