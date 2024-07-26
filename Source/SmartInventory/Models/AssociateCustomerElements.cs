using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class CustomerInfo 
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string customer_name { get; set; }
        public string structure_name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string address { get; set; }
        public string pin_code { get; set; } 
        public int province_id { get; set; }
        public int region_id { get; set; }
        [NotMapped]
        public string status { get; set; }
        public string remarks { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; } 
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
        public string entityType { get; set; }
        public string buildcode { get; set; }
        [NotMapped]
        public int structure_id { get; set; }
        [NotMapped]
        public int floor_id { get; set; }
        [NotMapped]
        public bool isPortConnected { get; set; }
        [NotMapped]
        public string message { get; set; }

        public string rfstype { get; set; }
        public int? splitter_system_id { get; set; }
        public string splitter_network_id { get; set; }
        public int? splitter_port_number { get; set; }
        public string splitter_port_name { get; set; }
        public string splitter_port_status { get; set; } 
        public int? dbBox_system_id { get; set; }  
        public string DbBox_network_id { get; set; } 
        public string DbBox_entity_type { get; set; }
        public string DbBox_latitude {  get; set; }
        public string DbBox_longitude { get; set; }
        public string splitter_type { get; set; } 
        public string floor_name { get; set; }
        public string Structure_code { get; set; }
        public string route { get; set; }
        public List<WCR_Material> WCR_List { get; set; }
        public CustomerInfo()
        {
            WCR_List = new List<WCR_Material>();
        }
    }
    public class WCR_Material
    {
        public string network_id { get; set; }
        public string entity_type { get; set; }
        public int construction { get; set; }
        public int activation { get; set; }
        public int accessibility { get; set; }
        public string specification { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public string item_code { get; set; }

        public string quantity_length { get; set; }


    }
}
