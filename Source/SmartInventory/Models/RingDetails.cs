using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RingDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string agg1 { get; set; }
        public string agg2 { get; set; }
        public int no_of_sites { get; set; }

        public double max_distance_peer { get; set; }
        public int region_id { get; set; }
        public int segment_id { get; set; }
        public int ring_id { get; set; }
        public string network_id { get; set; }

        public string topology_type { get; set; }
        public string ring_capacity { get; set; }
        public string region_name { get; set; }
        public string segment_name { get; set; }
        public string ring_name { get; set; }
        public string region { get; set; }
       
        public int totalRecords { get; set; }
        public string SearchbyRegionName { get; set; } 
        public string SearchbySegmentName { get; set; } 
        public string SearchbyRingType { get; set; } 

    }
   
   
    public class RingDetailsFiltter
    {
        public int id { get; set; }
        public List<RingDetails> lstRingDetails { get; set; }
        public RingDetails objRingDetails { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        
        public List<KeyValueDropDown> lstRegionName { get; set; }
        public List<KeyValueDropDown> lstSegmentName { get; set; }
        public List<KeyValueDropDown> lstRingType { get; set; }
        public RingDetailsFiltter()
        {
            objGridAttributes = new CommonGridAttributes();
            objGridAttributes.searchText = string.Empty;
            objGridAttributes.is_active = true;
            lstRegionName = new List<KeyValueDropDown>();
            lstSegmentName = new List<KeyValueDropDown>();
            lstRingType = new List<KeyValueDropDown>();
        }

    }


}
