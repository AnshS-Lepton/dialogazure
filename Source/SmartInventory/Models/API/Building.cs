using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.ISP;

namespace Models.API
{
    public class BuildingInfo
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string building_name { get; set; }
        public string building_no { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        //public string location { get; set; }
        //public string area { get; set; }
        //public string street { get; set; }
        public string address { get; set; }
        public string pin_code { get; set; }
        //public int province_id { get; set; }
        //public int region_id { get; set; }
        public int surveyarea_id { get; set; }
        public int business_pass { get; set; }
        public string building_type { get; set; }
        public int home_pass { get; set; }
        //public int total_tower { get; set; }
        public int no_of_floor { get; set; }
        //public int no_of_flat { get; set; }
        //public int no_of_occupants { get; set; }
        public string building_status { get; set; }
        //public string network_status { get; set; }
        public string status { get; set; }
        //public int db_flag { get; set; }
        //public string cluster_ref { get; set; }
        //public string pod_name { get; set; }
        //public string pod_code { get; set; }
        public string rfs_status { get; set; }
        public DateTime? rfs_date { get; set; }
        //public string customer_name { get; set; }
        //public string account_no { get; set; }
        //public DateTime? activation_date { get; set; }
        //public DateTime? deactivation_date { get; set; }
        //public string media { get; set; }
        //public string coverage_type_inside { get; set; }
        //public string requesting_customer { get; set; }
        //public string business_cluster { get; set; }
        //public string traffic_status { get; set; }
        //public string bldg_status_ring_spur { get; set; }
        //public int parent_system_id { get; set; }
        //public string parent_network_id { get; set; }
        //public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }
        public string tenancy { get; set; }
        public string category { get; set; }
        public string subcategory { get; set; }
        //public string gis_address { get; set; }
        public string rwa { get; set; }
        //public bool is_mobile { get; set; }
        public string remarks { get; set; }
        //public int created_by { get; set; }
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        //public int user_id { get; set; }
        public string road { get; set; }
        public string sub_locality { get; set; }
        public string locality { get; set; }
    }
    public class buildstructviewmodel
    {
        public BuildingMaster buildingdetail  { get; set; }
        public List<StructureMaster> structuredetails { get; set; }  
    }

    public class StructElementsViewmodel
    {
        public StructureMaster structuredetail { get; set; } 
        public List<StructureEntityView>  shaftFloors { get; set; } 
        public List<StructureElement> structElements  { get; set; }

    }
}

