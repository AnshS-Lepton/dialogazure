using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    public class BackBonePlanning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int plan_id { get; set; }
        [Required]
        public string plan_name { get; set; }     

        [Required]
        public string start_point { get; set; }

        [Required]
        public string end_point { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double? manhole_distance { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double? pole_distance { get; set; }
        public bool is_create_trench { get; set; }
        public bool is_create_duct { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double? cable_length { get; set; }
        public int? created_by { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public string created_on { get; set; }

        [NotMapped]
        public string geometry { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public string measured_cable_length { get; set; }

        [NotMapped]
        public string region_ids { get; set; }
        [NotMapped]
        public string province_ids { get; set; }
        public string layer_id { get; set; }
        [NotMapped]
        public int temp_plan_id { get; set; }
        public bool is_loop_required { get; set; }
        public double loop_length { get; set; }
        [NotMapped]
        public bool is_loop_update { get; set; }
        [NotMapped]
        public bool is_bomboq_reqested { get; set; }
        [NotMapped]
        public double buffer { get; set; }
        [Required]
        public string backbone_fiber { get; set; }
        [Required]
        public string sprout_fiber { get; set; }
        public string selectedSites { get; set; }
        public string backbone_geometry { get; set; }
        public string startpoint_network_id { get; set; }
        public string endpoint_network_id { get; set; }
        [Required]
        public double? threshold { get; set; }

        [NotMapped]
        public List<DropDownMaster> lstBackboneFiber { get; set; }

        [NotMapped]
        public List<DropDownMaster> lstSproutFiber { get; set; }

        public BackBonePlanning()
        {
            objPM = new PageMessage();
        }
    }


    public class siteBufferGeometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }  // For Polygon
    }
    public class BackBoneSitePlanDetails
    {
        public siteBufferGeometry buffer_geometry { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstSproutFiber { get; set; }
        public List<SitePlanList> sites { get; set; }
        public int pageSize { get; set; }
        public int totalRecord { get; set; }
        public int currentPage { get; set; }
    }
    public class SitePlanList
    {
        public string site_name { get; set; }
        public string site_id { get; set; }
        public string network_id { get; set; }
        public string geometry { get; set; }
        public string network_status { get; set; }
    }
    public class BackBoneSproutFiberDetails
    {
        public int plan_id { get; set; }
        public int system_id { get; set; }
        public string fiber_type { get; set; }
        public string intersect_line_geom { get; set; }
        public string site_geom { get; set; }
        public int created_by { get; set; }

    }

    public class BackBoneBOMOBOQResponse
    {
        public string entity_type { get; set; }
        public string cable_Length_qty { get; set; }
        public string cost_per_unit { get; set; }
        public string service_cost_per_unit { get; set; }
        public string total_cost { get; set; }
        public List<siteBufferGeometry> backbonelinegeom { get; set; }
    }
    public class SiteBufferGeometryRaw
    {
        public string geojson { get; set; }
    }
    public class siteLineGeometry
    {
        public string type { get; set; }
        public List<List<double>> coordinates { get; set; }

    }
    public class BackBoneBom
    {
        public string entity_type { get; set; }
        public string length_qty { get; set; }
        public double cost_per_unit { get; set; }
        public double service_cost_per_unit { get; set; }
        public double amount { get; set; }
        public bool is_template_extis { get; set; }
        public string msg { get; set; }
        public string geometry { get; set; }
        public double cable_length { get; set; }
    }
    public class ModelBackbonePlanningDetails
    {
        public List<BackBonePlanning> lstPlanDetails { get; set; }
        public BackBonePlanningDataFilter objPlanDataFilter { get; set; }

        public ModelBackbonePlanningDetails()
        {
            lstPlanDetails = new List<BackBonePlanning>();
            objPlanDataFilter = new BackBonePlanningDataFilter();
        }
    }
    public class BackBonePlanningDataFilter : CommonGridAttributes
    {
        // will add more pproperties based on requirement.
    }

    public class DbMessageForBackbonePlan
    {
        public bool status { get; set; }
        public string message { get; set; }

        public int v_plan_id { get; set; }
    }
    public class BackBonePlanBom
    {
        public string entity_type { get; set; }
        public string length_qty { get; set; }
        public double cost_per_unit { get; set; }
        public double service_cost_per_unit { get; set; }
        public double amount { get; set; }
    }

    public class BackbonePlanNetworkDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int plan_id { get; set; }
        public string entity_type { get; set; }
        public string entity_network_id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int created_by { get; set; }
        public int cable_id { get; set; }
        public int? entity_system_id { get; set; }
        public string cable_network_id { get; set; }
        public int duct_id { get; set; }
        public int trench_id { get; set; }
        public string trench_network_id { get; set; }
        public string duct_network_id { get; set; }
        public string fiber_type { get; set; }
        public double loop_length { get; set; }
        public bool is_loop_required { get; set; }
    }


}
