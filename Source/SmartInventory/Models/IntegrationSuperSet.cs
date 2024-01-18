using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Models
{
    public class IntegrationSuperSet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string layer_name { get; set; }
        public string user_email { get; set; }
        public int plan_id { get; set; }
        public string plan_name { get; set; }
        public int sp_system_id { get; set; }
        public string sp_layer_name { get; set; }
        public string polystring { get; set; }
        public string geometry_type { get; set; }
        public int? sp_parent_system_id { get; set; }
        public string sp_parent_entity_type { get; set; }
        public int? sp_container_id { get; set; }
        public string sp_container_type { get; set; }
        public int? system_id { get; set; }
        public string network_id { get; set; }
        public string entity_name { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public int? province_id { get; set; }
        public int? region_id { get; set; }
        public int? pincode { get; set; }
        public string address { get; set; }
        public string specification { get; set; }
        public string category { get; set; }
        public string subcategory1 { get; set; }
        public string subcategory2 { get; set; }
        public string subcategory3 { get; set; }
        public string item_code { get; set; }
        public int? vendor_id { get; set; }
        public int? no_of_port { get; set; }
        public string entity_category { get; set; }
        public int? total_core { get; set; }
        public int? no_of_tube { get; set; }
        public int? no_of_core_per_tube { get; set; }
        public string status { get; set; }
        public int? created_by { get; set; }
        public DateTime created_on { get; set; }
        public string network_status { get; set; }
        public int? sequence_id { get; set; }
        public int? parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int? structure_id { get; set; }
        public int? shaft_id { get; set; }
        public int? floor_id { get; set; }
        public int? project_id { get; set; }
        public int? planning_id { get; set; }
        public int? purpose_id { get; set; }
        public int? workorder_id { get; set; }
        public string building_no { get; set; }
        public string location { get; set; }
        public string area { get; set; }
        public string street { get; set; }
        public int? surveyarea_id { get; set; }
        public int? business_pass { get; set; }
        public string building_type { get; set; }
        public int? home_pass { get; set; }
        public int? total_tower { get; set; }
        public int? no_of_floor { get; set; }
        public int? no_of_flat { get; set; }
        public int? no_of_occupants { get; set; }
        public string building_status { get; set; }
        public int? db_flag { get; set; }
        public string cluster_ref { get; set; }
        public string pod_code { get; set; }
        public string rfs_status { get; set; }
        public DateTime? rfs_date { get; set; }
        public string account_no { get; set; }
        public DateTime? activation_date { get; set; }
        public DateTime? deactivation_date { get; set; }
        public string media { get; set; }
        public string coverage_type_inside { get; set; }
        public string requesting_customer { get; set; }
        public string business_cluster { get; set; }
        public string traffic_status { get; set; }
        public string bldg_status_ring_spur { get; set; }
        public string tenancy { get; set; }
        public string gis_address { get; set; }
        public string rwa { get; set; }
        public bool? is_mobile { get; set; }
        public string remarks { get; set; }
        public string rwa_contact_no { get; set; }
        public int? no_of_shaft { get; set; }
        public int? building_id { get; set; }
        public double? height { get; set; }
        public double? length { get; set; }
        public double? width { get; set; }
        public int? no_of_units { get; set; }
        public int? copy_floor_id { get; set; }
        public int? area_id { get; set; }
        public string pole_no { get; set; }
        public string pole_height { get; set; }
        public string pole_type { get; set; }
        public bool? is_virtual { get; set; }
        public string construction_type { get; set; }
        public string splitter_ratio { get; set; }
        public string splitter_type { get; set; }
        public int? no_of_ports { get; set; }
        public int? no_of_input_port { get; set; }
        public int? no_of_output_port { get; set; }
        public string fdb_type { get; set; }
        public int? accessibility { get; set; }
        public int? type { get; set; }
        public int? brand { get; set; }
        public int? model { get; set; }
        public int? construction { get; set; }
        public int? activation { get; set; }
        public string a_location { get; set; }
        public string b_location { get; set; }
        public double? cable_measured_length { get; set; }
        public double? cable_calculated_length { get; set; }
        public string cable_type { get; set; }
        public string coreaccess { get; set; }
        public string wavelength { get; set; }
        public string optical_output_power { get; set; }
        public string frequency { get; set; }
        public string attenuation_db { get; set; }
        public string resistance_ohm { get; set; }
        public string utilization { get; set; }
        public string totalattenuationloss { get; set; }
        public string chromaticdb { get; set; }
        public string chromaticdispersion { get; set; }
        public string totalchromaticloss { get; set; }
        public int? sp_a_system_id { get; set; }
        public string sp_a_entity_type { get; set; }
        public int? sp_b_system_id { get; set; }
        public string sp_b_entity_type { get; set; }
        public int sp_building_id { get; set; }
        public int sp_tower_id { get; set; }
        public int sp_floor_id { get; set; }
        public int sp_shaft_number { get; set; }
        public int? a_system_id { get; set; }
        public string a_network_id { get; set; }
        public string a_entity_type { get; set; }
        public int? b_system_id { get; set; }
        public string b_network_id { get; set; }
        public string b_entity_type { get; set; }
        public int? duct_id { get; set; }
        public int? trench_id { get; set; }
        public string cable_category { get; set; }
        public int? loop_count { get; set; }
        public int? loop_length { get; set; }
        public int? total_loop_length { get; set; }
        public int? total_loop_count { get; set; }
        public string route_id { get; set; }
        public double? start_reading { get; set; }
        public double? end_reading { get; set; }
        public string cable_sub_category { get; set; }
        public string execution_method { get; set; }
        public double? calculated_length { get; set; }
        public double? manual_length { get; set; }
        public int? no_of_cables { get; set; }
        public double? offset_value { get; set; }
        public double? inner_dimension { get; set; }
        public double? outer_dimension { get; set; }
        public string duct_type { get; set; }
        public string color_code { get; set; }
        public double? trench_length { get; set; }
        public double? trench_width { get; set; }
        public double? trench_height { get; set; }
        public int? customer_count { get; set; }
        public string trench_type { get; set; }
        public string no_of_ducts { get; set; }
        public string mcgm_ward { get; set; }
        public string strata_type { get; set; }
        public DateTime? manufacture_year { get; set; }
        public string surface_type { get; set; }
        public bool is_processed { get; set; }
        public string no_of_ducts_created { get; set; }

        public IntegrationSuperSet()
        {
            utilization = "L";
        }
    }




    public class DisplayPlan
    {
        public int plan_id { get; set; }
        public string plan_name { get; set; }
        public bool is_processed { get; set; }
    }

    public class PlanRegionProvince
    {
        public string all_layers { get; set; }
        public double pod_lat { get; set; }
        public double pod_lng { get; set; }
        public int province_id { get; set; }
        public int region_id { get; set; }
    }

    public class UpdateTargetDetails
    {
        public int TargetRefID { get; set; }
        public string TargetRefCode { get; set; }
        public string DesignID { get; set; }
        public string ClassName { get; set; }
        public string CategoryName { get; set; }
    }
}
