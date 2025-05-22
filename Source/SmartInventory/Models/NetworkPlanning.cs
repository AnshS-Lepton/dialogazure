using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    public class NetworkPlanning
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int planid { get; set; }
        [Required]
        public string plan_name { get; set; }
        [Required]
        public string planning_mode { get; set; }
        public string end_point_type { get; set; }

        public int? end_point_entity { get; set; }
        public string edit_path { get; set; }

        [Required]
        [Remote("chk_end_buffer", "plan")]
        public double? end_point_buffer { get; set; }

        [Required]
        public string start_point { get; set; }

        [NotMapped]
        public double? start_point_buffer { get; set; }

        [NotMapped]
        public int? start_point_entity { get; set; }

        [Required]
        public string end_point { get; set; }
        [Required]
        public string cable_type { get; set; }
        [Required]
        [Range(double.Epsilon, double.MaxValue)]
        public double? pole_manhole_distance { get; set; }
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
        public IList<DropDownMaster> listcableType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public List<layerDetail> lstLayers { get; set; }
        [NotMapped]
        public string measured_cable_length { get; set; }

        [NotMapped]
        public string region_ids { get; set; }
        [NotMapped]
        public string province_ids { get; set; }
        public string layer_id { get; set; }
        [NotMapped]
        public int temp_plan_id  { get; set; }

        [NotMapped]
        public bool is_offset_required { get; set; }
        [NotMapped]
        public bool offset_position { get; set; }
        [NotMapped]
        public  double? offset_value  { get; set; }
        public bool is_loop_required { get; set; }
        public double loop_length { get; set; }
        [NotMapped]
        public bool is_loop_update { get; set; }
        [NotMapped]
        public double MinAutoPlanEndPointBuffer { get; set; }
        [NotMapped]
        public double MaxAutoPlanEndPointBuffer { get; set; }
        [NotMapped]
        public double MaxAutoOffsetValue { get; set; }
        [NotMapped]
        public bool is_bomboq_reqested { get; set; }
        [NotMapped]
        public double buffer { get; set; }

        public NetworkPlanning()
        {
            objPM = new PageMessage();
            is_offset_required = false;
            is_loop_required = false;
            offset_position = true;
            is_loop_update = false;
          
        }
    }

    public class EntityDirection
    {
        public string layer_type { get; set; }
        public List<layerDetail> lstLayers { get; set; }
        public bool is_offset_required { get; set; }
        public bool offset_position { get; set; }
        public double? offset_value { get; set; }
        public PageMessage objPM { get; set; }
        public EntityDirection()
        {
            objPM = new PageMessage();
            is_offset_required = false;
            offset_position = true;
            offset_value = 0;
        }
    }


    public class temp_auto_network_plan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public int plan_id { get; set; }
        public string entity_type { get; set; }
        public string entity_network_id { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string sp_geometry { get; set; }

        public int? province_id { get; set; }

        public int? region_id { get; set; }
        public int? created_by { get; set; }
        public string line_sp_geometry { get; set; }
        public string is_middle_point { get; set; }
        public double fraction { get; set; }
        public double loop_length { get; set; }
        [NotMapped]
        public int entity_count { get; set; }

    }
    public class PlanBom
    {
        public string entity_type { get; set; }
        public string length_qty { get; set; }
        public double cost_per_unit { get; set; }
        public double service_cost_per_unit { get; set; }
        public double amount { get; set; }
        public bool is_template_extis { get; set; }
        public string msg { get; set; }
        public string geometry { get; set; }

        public int temp_plan_id { get; set; }
        public double cable_length { get; set; }
    }

    public class ViewModelNetworkPlanning {
        public List<PlanBom> PlanBoms { get; set; }
        public NetworkPlanning networkPlanning { get; set; }
    }

    public class NetworkPlanningDataFilter : CommonGridAttributes
    {
        // will add more pproperties based on requirement.
    }
    public class ModelNetworkPlanningDetails
    {
        public List<NetworkPlanning> lstPlanDetails { get; set; }
        public NetworkPlanningDataFilter objPlanDataFilter { get; set; }

        public ModelNetworkPlanningDetails()
        {
            lstPlanDetails = new List<NetworkPlanning>();
            objPlanDataFilter = new NetworkPlanningDataFilter();
        }
    }
}
