using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Feasibility
{
    public class FeasibilityViewModel
    {
        public User userDetail { get; set; }
        public List<FeasibilityCableType> lstCableTypes { get; set; }
        public FeasibilityCableType FeasibilityCableType { get; set; }
        public string color_LMC_Start { get; set; }
        public string color_LMC_End { get; set; }
        public FeasibilityInput FeasibilityInputModel { get; set; }
        public FTTHFeasibilityModel FTTHFeasibilityModel { get; set; }
        public string color_Outside { get; set; }
        public string color_inside { get; set; }
        public string color_inside_A { get; set; }
        public string color_inside_P { get; set; }
        public MainViewModel layerView { get; set; }

        //  public List<NEntityLayers> NELayers { get; set; }
        public List<NEntityDetails> NEDetails { get; set; }

        public FeasibilityViewModel()
        {
            userDetail = new User();
            lstCableTypes = new List<FeasibilityCableType>();
            FeasibilityCableType = new FeasibilityCableType();
            layerView = new MainViewModel();
            // NELayers = new List<NEntityLayers>();
            NEDetails = new List<NEntityDetails>();
        }
    }
    public class FeasibilityInputModel
    {
        [Required]
        public string customer_name { get; set; }
        [Required]
        public string customer_id { get; set; }
        [Required]
        public string feasibility_id { get; set; }
        [Required]
        public string cable_type { get; set; }
        [Required]
        public double start_lat { get; set; }
        [Required]
        public double start_lng { get; set; }
        [Required]
        public double end_lat { get; set; }
        [Required]
        public double end_lng { get; set; }
        [Required]
        public int cores_free { get; set; }
    }
    public class FeasibilityDetails
    {
        public int feasibility_id { get; set; }
        public string feasibilityName { get; set; }
        public double totalLength { get; set; }
        public double ExistingLength_P { get; set; }
        public double ExistingLength_A { get; set; }
        public double NewOutside_A_Length { get; set; }
        public double NewOutside_B_Length { get; set; }
        public double lmc_A_End_Path { get; set; }
        public double lmc_B_End_Path { get; set; }
        public double start_lat { get; set; }
        public double start_lng { get; set; }
        public double end_lat { get; set; }
        public double end_lng { get; set; }
        public bool isSelected { get; set; }
        public string CableGeom { get; set; }
        public int subPathCount { get; set; }
        public double buffer_radius_a { get; set; }
        public double buffer_radius_b { get; set; }
        public string core_level_result { get; set; }
        public string feasibility_result { get; set; }
    }
    public class FeasibilityCableTypeViewModel
    {
        public List<FeasibilityCableType> lstCableTypes { get; set; }
        public FeasibilityCableType feasibilityCable { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }

        public FeasibilityCableTypeViewModel()
        {
            feasibilityCable = new FeasibilityCableType();
            lstCableTypes = new List<FeasibilityCableType>();
            objGridAttributes = new CommonGridAttributes();
        }
    }
    public class FeasibilityDetailsViewModel
    {
        public User userDetail { get; set; }
        public List<FeasibilityCableType> lstCableTypes { get; set; }
        public List<FeasibilityDetails> lstFeasibilityDetails { get; set; }
        public FeasibilityInput objFeasibilityModel { get; set; }
        public FeasibilityDetails objFeasibilityDetails { get; set; }
        public List<FeasibiltyGeometry> lstFeasibilityGeometry { get; set; }
    }
    public class FeasibilityReportData
    {
        public double Start_Lat { get; set; }
        public double Start_Lng { get; set; }
        public double End_Lat { get; set; }
        public double End_Lng { get; set; }
        public string Start_Point_Name { get; set; }
        public string End_Point_Name { get; set; }
        public int Cores_Required { get; set; }
        public double Total_Length { get; set; }
        public double Inside_Planned_Length { get; set; }
        public double Inside_AsBuilt_Length { get; set; }
        //public double Inside_Length { get; set; }
        public double Outside_A_Length { get; set; }
        public double Outside_B_Length { get; set; }
        public double LMC_A_Length { get; set; }
        public double LMC_B_Length { get; set; }
        public double Inside_Length_Percent { get; set; }
        public string Status { get; set; }
        public string is_core_feasibile { get; set; }
        //public int cable_type_cores { get; set; }
        public List<FeasibilityInsideCableReport> insideCables { get; set; }
        public FeasibilityReportData()
        {
            insideCables = new List<FeasibilityInsideCableReport>();
        }

    }

    public class MergedExcelModel
    {
        public string History_ID { get; set; }
        public string Feasibility_ID { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_ID { get; set; }
        public string Start_Lng_lat { get; set; }
        public string End_Lng_Lat { get; set; }
        public string Start_Point_Name { get; set; }
        public string End_Point_Name { get; set; }
        public int Cores_Required { get; set; }
        public string Cable_Type { get; set; }
        public double Total_Length { get; set; }
        public double Inside_Planned_Length { get; set; }
        public double Inside_AsBuilt_Length { get; set; }
        public double Outside_A_Length { get; set; }
        public double Outside_B_Length { get; set; }
        public double LMC_A_Length { get; set; }
        public double LMC_B_Length { get; set; }
        public double Inside_Length_Percent { get; set; }
        public string Status { get; set; }
        public string is_core_feasibile { get; set; }
    }

    public class MergedExcelFTTHModel
    {
        public string History_ID { get; set; }
        public string Feasibility_ID { get; set; }
        public string Customer_Name { get; set; }
        public string Customer_ID { get; set; }
        public string Lat_Lng { get; set; }
        public string Entity_Id { get; set; }
        public string Entity_Loc { get; set; }
        public string Buffer_Radius { get; set; }
        public string path_distance { get; set; }

    }

    public class FeasibilityKMLData
    {
        public string cable_id { get; set; }
        public string cable_name { get; set; }
        public string entity_type { get; set; }
        public string entity_title { get; set; }
        public string geom { get; set; }
        public string description { get; set; }
        public string entity_name { get; set; }
        public string geom_type { get; set; }
        public double distance { get; set; }
        public string colorHex_8 { get; set; }
    }


    public class FeasibilityBOMData
    {
        public int cable_type_id { get; set; }
        public double cable_length { get; set; }
        public string cable_type { get; set; }
        public double material_unit_price { get; set; }
        public double service_unit_price { get; set; }
        public double total_material_cost { get; set; }
        public double total_service_cost { get; set; }
        // public double bom_material { get; set; }
        // public double bom_service { get; set; }
        [DefaultValue(false)]
        public bool isPastData { get; set; }
    }

    public class FeasibilityInsideCableReport
    {
        public string cable_id { get; set; }
        //public int system_id { get; set; }
        public string cable_name { get; set; }
        public string network_status { get; set; }
        public int total_cores { get; set; }
        public int used_cores { get; set; }
        public int available_cores { get; set; }
        public double cable_length { get; set; }
    }

    public class InsideCablesMergedModel
    {
        public string history_id { get; set; }
        public string cable_id { get; set; }
        public string cable_name { get; set; }
        public string network_status { get; set; }
        public int total_cores { get; set; }
        public int used_cores { get; set; }
        public int available_cores { get; set; }
        public double cable_length { get; set; }
    }

    public class PastFeasibility
    {
        public int history_id { get; set; }
        public string history_display_id { get; set; }
        public int feasibility_id { get; set; }
        public string feasibility_name { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string start_point_name { get; set; }
        public string start_lat_lng { get; set; }
        public string end_point_name { get; set; }

        public string end_lat_lng { get; set; }
        public int cores_required { get; set; }
        // public double exisiting_length { get; set; }
        public double inside_p { get; set; }
        public double inside_a { get; set; }

        public double outside_a_end { get; set; }
        public double outside_b_end { get; set; }
        public double lmc_a { get; set; }
        public double lmc_b { get; set; }
        public string created_on { get; set; }
        public string core_level_result { get; set; }
        public string feasibility_result { get; set; }
        public double buffer_radius_a { get; set; }
        public double buffer_radius_b { get; set; }
        public int cable_type_id { get; set; }
        public string cable_type { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }


    }
    public class FeasibilityKMLModel
    {
        public List<FeasibilityCableGeoms> lstFeasibilityGeom { get; set; }
        public List<FeasibilityKMLData> lstKMLDataPoints { get; set; }
        public string feasibility_name { get; set; }
        public string history_id { get; set; }
        public string start_lat_lng { get; set; }
        public string end_lat_lng { get; set; }
        public string start_Point_Name { get; set; }
        public string end_Point_Name { get; set; }

    }
    public class FeasibilityCableGeoms
    {
        public int history_id { get; set; }
        public string cable_geometry { get; set; }
        public double cable_length { get; set; }
        public string cable_type { get; set; }
        public int system_id { get; set; }
        public string cable_id { get; set; }
        public string cable_name { get; set; }
        public int total_cores { get; set; }
        public int available_cores { get; set; }
        public string network_status { get; set; }
        public double material_cost { get; set; }
        public double service_cost { get; set; }
    }
    public class PastFeasibilityViewModel
    {
        PastFeasibility objPastFeasibility { get; set; }
        public List<PastFeasibility> lstPastFeasibility { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public PastFeasibilityViewModel()
        {
            objPastFeasibility = new PastFeasibility();
            lstPastFeasibility = new List<PastFeasibility>();
            objGridAttributes = new CommonGridAttributes();
        }
    }

    // NEW CLASS ADDED

    public class NEntityLayers
    {
        public int RecId { get; set; }
        public string LayerId { get; set; }
        public string NEntityLayerId { get; set; }
        public string LayerGroup { get; set; }
        public string LayerType { get; set; }
        public string LayerName { get; set; }
        public string LayerTable { get; set; }
        public string LayerTitle { get; set; }
    }

    public class NEntityDetails
    {
        public string AssetId { get; set; }
        public string NEntityIcon { get; set; }
        public string NESystemId { get; set; }
        public string NetworkId { get; set; }
        public string NetworkType { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public double NeLat { get; set; }
        public double NeLng { get; set; }
        public int CustomerCount { get; set; }
        public int TotalNofPorts { get; set; }
        public int PortUtilization { get; set; }
        public int FaultyPort { get; set; }
        public string Utilization { get; set; }
    }

    public class FTTHFeasibilityModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int feasibility_id { get; set; }
        [Required]
        public double lat { get; set; }
        [Required]
        public double lng { get; set; }
        public int buffer_radius { get; set; }
        [Required]
        public string feasibility_name { get; set; }
        [Required]
        public string customer_id { get; set; }
        [Required]
        public string customer_name { get; set; }
        public int created_by { get; set; }
        public string [] path_geometry { get; set; }
        public string entity_id { get; set; }
        public string entity_location { get; set; }
        public string path_distance { get; set; }

    }

    public class FTTHFeasibilityDetailModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int feasibility_id { get; set; }
        public string lat_lng { get; set; }
        public string feasibility_name { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string created_by { get; set; }
        public int buffer_radius { get; set; }
        public string entity_loc { get; set; }

    }
    public class FTTHFeasibilityHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int history_id { get; set; }

        public int feasibility_id { get; set; }

        public string coordinates { get; set; }

        public string created_by { get; set; }

        public string path_distance { get; set; }

        public string entity_id { get; set; }

        public double material_cost { get; set; }
        public double service_cost { get; set; }

    }

    public class PastFeasibilityFtthViewModel
    {
        PastFeasibilityFtth objPastFeasibilityFtth { get; set; }
        public List<PastFeasibilityFtth> lstPastFeasibilityFtth { get; set; }
        public CommonGridAttributes objGridAttributes { get; set; }
        [NotMapped]
        public List<KeyValueDropDown> lstSearchBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public PastFeasibilityFtthViewModel()
        {
            objPastFeasibilityFtth = new PastFeasibilityFtth();
            lstPastFeasibilityFtth = new List<PastFeasibilityFtth>();
            objGridAttributes = new CommonGridAttributes();
        }
    }

    public class PastFeasibilityFtth {
        public int history_id { get; set; }
        public int feasibility_id { get; set; }
        public string lat_lng { get; set; }
        public string created_by { get; set; }
        public string created_on { get; set; }
        public string path_distance { get; set; }
        public string entity_id { get; set; }
        public double material_cost { get; set; }
        public double service_cost { get; set; }
        public string feasibility_name { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public int buffer_radius { get; set; }
        public string geometry { get; set; }
        public string entity_loc { get; set; }
        public string history_display_id { get; set; }
        public int totalRecords { get; set; }
    }

    public class FTTHFeasibilityReportData
    {
        public int feasibility_id { get; set; }
        public string lat_lng { get; set; }
        public string path_distance { get; set; }
        public string entity_id { get; set; }
        public string feasibility_name { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public int buffer_radius { get; set; }
    }


    public class FeasibilityKMLFTTHModel
    {
        public List<FeasibilityKMLDataFTTH> lstKMLDataPoints { get; set; }
        public int history_id { get; set; }
        public int feasibility_id { get; set; }
        public string lat_lng { get; set; }
        public string path_distance { get; set; }
        public string entity_id { get; set; }
        public string entity_loc { get; set; }
        public string feasibility_name { get; set; }
        public string customer_id { get; set; }
        public string customer_name { get; set; }
        public string description { get; set; }
        public string cable_geometry { get; set; }
        public FeasibilityKMLFTTHModel()
        {
            lstKMLDataPoints = new List<FeasibilityKMLDataFTTH>();
        }


    }
    public class FeasibilityKMLDataFTTH
    {
        public string feasibility_id { get; set; }
        public string feasibility_name { get; set; }
        public string geometry { get; set; }
        public string description { get; set; }
        public string geom_type { get; set; }
        public string feasibility_title { get; set; }
        public string path_distance { get; set; }
        public string colorHex_8 { get; set; }
    }
}
