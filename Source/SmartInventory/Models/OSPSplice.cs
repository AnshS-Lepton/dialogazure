using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{



    public class ViewOSPCPFModel
    {

        public string FiberLinkId { get; set; }
        public string equipment_id { get; set; }
        public double txtTransmit { get; set; }
        public int txtReceiving { get; set; }


        [NotMapped]
        public List<EquipementPort> lstEquipementCore { get; set; }
        public IList<DropDownMaster> EquipementCore { get; set; }
        public List<ConnectionInfo> lstConnectionInfo { get; set; }
        public List<Models.Admin.LinkBudgetMaster> lstWave_length { get; set; }
        public string SearchPort { get; set; }
        [NotMapped]
        public string entity_geomid { get; set; }
        public ConnectionInfoFilter objFilterAttributes { get; set; }
        public string schematicJsonString { get; set; }

        public bool backword_path_type { get; set; }

        public List<ConnectionInfoForOpticalLinkBudget> lstConnectionLinkBudgetData { get; set; }
        public bool isControllEnable { get; set; }

        [NotMapped]
        public List<connectedCusotmer> lstConnectedCustomer { get; set; }
        [NotMapped]
        public Dictionary<string, string> lstFiberLinkDetails { get; set; }
        public bool isOLBEnabled { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }

        public ViewOSPCPFModel()
        {
            objFilterAttributes = new ConnectionInfoFilter();
            lstConnectionInfo = new List<ConnectionInfo>();
            lstEquipementCore = new List<EquipementPort>();
            lstWave_length = new List<Models.Admin.LinkBudgetMaster>();
            objFilterAttributes.totalRecord = 0;
            lstConnectionLinkBudgetData = new List<ConnectionInfoForOpticalLinkBudget>();
            //isControllEnable = true;
            lstConnectedCustomer = new List<Models.connectedCusotmer>();
            lstFiberLinkDetails = new Dictionary<string, string>();
            lstUserModule = new List<string>();
        }

    }
    public class ConnectionInfoFilter : CommonGridAttributes
    {
        public int entityid { get; set; }
        public string entity_type { get; set; }
        public int port_no { get; set; }
        public bool isStartingPoint { get; set; }
        public ConnectionInfoFilter()
        {
            isStartingPoint = false;
        }
    }
    public class EquipementSearchResult
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string entity_type { get; set; }
        public string geom_Type { get; set; }
        public string network_status { get; set; }
        public string no_of_ports { get; set; }
        public string display_name { get; set; }
        public string node_type { get; set; }
        public string end_point_geom { get; set; }
    }
    public class EquipementSearchResultViewModel
    {
        public List<EquipementSearchResult> lstEquipementSearchResult { get; set; }
        public EquipementSearchResultViewModel()
        {
            lstEquipementSearchResult = new List<EquipementSearchResult>();
        }
    }

    public class LogicalViewEquipementSearch
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string entity_type { get; set; }
        public string geom_Type { get; set; }
        public string network_status { get; set; }
        public string no_of_ports { get; set; }
        public string display_name { get; set; }
    }
    public class LogicalViewEquipementSearchVM
    {
        public List<LogicalViewEquipementSearch> lstLogicalViewEquipementSearch { get; set; }
        public LogicalViewEquipementSearchVM()
        {
            lstLogicalViewEquipementSearch = new List<LogicalViewEquipementSearch>();
        }
    }
    public class EquipementPort
    {
        public string port_text { get; set; }
        public string endpoint { get; set; }
        public int port_value { get; set; }
    }
    public class EquipementPortViewModel
    {
        public List<EquipementPort> lstEquipementPort { get; set; }
        public EquipementPortViewModel()
        {
            lstEquipementPort = new List<Models.EquipementPort>();
        }
    }
    public class ConnectionInfoMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int connection_id { get; set; }
        public int source_system_id { get; set; }
        public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public int source_port_no { get; set; }
        public int destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public int destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public string splicing_source { get; set; }
        public bool is_cable_a_end { get; set; }
        [NotMapped]
        public bool is_source_cable_a_end { get; set; }
        [NotMapped]
        public bool is_destination_cable_a_end { get; set; }
        [NotMapped]
        public bool is_valid { get; set; }
        [NotMapped]
        public string error_msg { get; set; }
        [NotMapped]
        public int equipment_system_id { get; set; }
        [NotMapped]
        public string equipment_network_id { get; set; }
        [NotMapped]
        public string equipment_entity_type { get; set; }
        [NotMapped]
        public int equipment_tray_system_id { get; set; }
        public string source_entity_sub_type { get; set; }
        public string destination_entity_sub_type { get; set; }
        public bool is_through_connection { get; set; }
        [NotMapped]
        public bool is_virtual { get; set; }


    }
    public class SplicingInfo
    {
        public int? connection_id { get; set; }
        public int? source_system_id { get; set; }
        public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public int? source_port_no { get; set; }
        public int? destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public int? destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public int? via_system_id { get; set; }
        public string via_entity_type { get; set; }
        public string via_network_id { get; set; }
        public int? via_port_no { get; set; }
        public bool is_source_connected_to_other { get; set; }
        public string source_other_text { get; set; }
        public bool is_destination_connected_to_other { get; set; }
        public string destination_other_text { get; set; }
        public bool is_through_connection { get; set; }
    }
    public class TempConnectionInfoMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public int connection_id { get; set; }
        public int source_tray_system_id { get; set; }
        public int destination_tray_system_id { get; set; }
        public string source_system_id { get; set; }
        public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public string source_port_no { get; set; }
        public string source_port_type { get; set; }
        public string destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_port_no { get; set; }
        public string destination_port_type { get; set; }
        public bool is_customer_connected { get; set; }
        public bool is_through_connection { get; set; }
        public bool is_cable_a_end { get; set; }
        public DateTime? created_on { get; set; }
        public string created_by { get; set; }
        public string approved_by { get; set; }
        public string equipment_network_id { get; set; }
        public string equipment_entity_type { get; set; }
        public int equipment_system_id { get; set; }
        public DateTime? approved_on { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int uploaded_by { get; set; }
    }
    public class ConnectionInfo
    {
        public int connection_id { get; set; }
        public int source_system_id { get; set; }
        public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public string source_entity_title { get; set; }
        public int source_port_no { get; set; }
        public bool is_source_virtual { get; set; }
        public int destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_entity_title { get; set; }
        public int destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public bool is_destination_virtual { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public int childordering { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        public int id { get; set; }
        public bool is_backward_path { get; set; }
        public string sp_geometry { get; set; }
        public double? cable_calculated_length { get; set; }
        public double? cable_measured_length { get; set; }
        public string cable_network_status { get; set; }
        public string splitter_ratio { get; set; }
        [NotMapped]
        public bool isprocessed { get; set; }
        public string source_display_name { get; set; }
        public string destination_display_name { get; set; }
        public int source_tray_system_id { get; set; }
        public int destination_tray_system_id { get; set; }
        public string source_tray_display_name { get; set; }
        public string destination_tray_display_name { get; set; }
        public string source_port_display_name { get; set; }
        public string destination_port_display_name { get; set; }
        public double? source_cable_calculated_length { get; set; }
        public double? destination_cable_calculated_length { get; set; }
    }
    public class ConnectionInfoViewModel
    {
        public List<ConnectionInfo> lstConnectionInfo { get; set; }
        public ConnectionInfoViewModel()
        {
            lstConnectionInfo = new List<Models.ConnectionInfo>();
        }
    }
    public class ConnectionInfoReport
    {
        public int connection_id { get; set; }
        public int source_system_id { get; set; }
        // public string source_network_id { get; set; }
        public string source_display_name { get; set; }
        public string source_entity_type { get; set; }
        public string source_port_no { get; set; }
        public int destination_system_id { get; set; }
        // public string destination_network_id { get; set; }
        public string destination_display_name { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public int childordering { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        public int id { get; set; }
        public bool is_backward_path { get; set; }
        public string source_tray_display_name { get; set; }
        public string destination_tray_display_name { get; set; }
        public string source_port_display_name { get; set; }
        public string destination_port_display_name { get; set; }

    }
    public class ConnectionInfoForOpticalLink
    {
        public int connection_id { get; set; }
        public int source_system_id { get; set; }
        public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public int source_port_no { get; set; }
        public int destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public int destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public int childordering { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        public int id { get; set; }
        public bool is_backward_path { get; set; }
        public int a_end_length { get; set; }
        public int b_end_length { get; set; }

    }

    public class ConnectionInfoForOpticalLinkBudget
    {
        public int connection_id { get; set; }
        public int source_system_id { get; set; }
        public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public string source_port_no { get; set; }
        public int destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public DateTime? created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public int childordering { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        public int id { get; set; }
        public bool is_backward_path { get; set; }
        public int a_end_length { get; set; }
        public int b_end_length { get; set; }

        public double? a_end_loss { get; set; }
        public double? b_end_loss { get; set; }
        public string Connection_Type { get; set; }

        public double? splitter_loss { get; set; }

        public double? txtReceiving { get; set; }


        public List<ConnectionInfoForOpticalLinkBudget> ViewLinkBudgetDetails { get; set; }

        public ConnectionInfoForOpticalLinkBudget()
        {

            ViewLinkBudgetDetails = new List<ConnectionInfoForOpticalLinkBudget>();

        }




    }


    public class CPFElements
    {
        public int system_id { get; set; }
        public string en_type { get; set; }
        public string sp_geometry { get; set; }
        public int port_no { get; set; }
        public string network_id { get; set; }
        public Boolean backward_path { get; set; }
        public bool is_virtual_port_allowed { get; set; }
        public string entity_category { get; set; }
        public bool is_virtual { get; set; }
        public string display_name { get; set; }
        public string entity_title { get; set; }

    }

    public class CPFElementsViewModel
    {
        public List<CPFElements> lstCPFElements { get; set; }
        public CPFElementsViewModel()
        {
            lstCPFElements = new List<Models.CPFElements>();
        }
    }


    public class SchematicViewModel
    {

        public string equipment_id { get; set; }
        public IList<DropDownMaster> EquipementCore { get; set; }
        public string SearchPort { get; set; }
        public ConnectionInfoFilter objFilterAttributes { get; set; }
        public SchematicViewModel()
        {
            objFilterAttributes = new ConnectionInfoFilter();
        }
    }

    public class SplicingEntity
    {
        public string entity_type { get; set; }
        public int entity_system_id { get; set; }
        public string geom_Type { get; set; }
        public int a_system_id { get; set; }
        public string a_entity_type { get; set; }
        public int b_system_id { get; set; }
        public string b_entity_type { get; set; }
        public string common_name { get; set; }
        public string total_core_ports { get; set; }
        public string network_status { get; set; }
        public bool is_start_point { get; set; }
        public bool is_end_point { get; set; }
        public bool is_splicer { get; set; }
        public bool is_cpe_entity { get; set; }
        public bool is_isp_entity { get; set; }
        public bool isCableTPPoint { get; set; }
        public bool isSelectedEntity { get; set; }
        public bool is_middleware_entity { get; set; }
        public bool is_virtual { get; set; }
        public bool is_virtual_entity { get; set; }

        public string layer_title { get; set; }
        public string layer_abbr { get; set; }
        public string layer_display_abbr { get; set; }

    }
    public class CPESplicingEntity
    {
        public string entity_type { get; set; }
        public int entity_system_id { get; set; }
        public string geom_Type { get; set; }
        public string common_name { get; set; }
        public string total_core { get; set; }
        public string network_status { get; set; }
    }
    public class SplicingViewModelOld
    {
        public List<SplicingCables> cables { get; set; }
        public List<SplicingConnectors> Connectors { get; set; }
        public SplicingCables sourceCables { get; set; }
        public SplicingCables destinationCables { get; set; }
        public SplicingConnectors parentConnectors { get; set; }
        public List<SplicingConnectors> childConnectors { get; set; }
        public List<SplicingPortInfo> portInfo { get; set; }
        public int parentInputPort { get; set; }
        public int parentOutPort { get; set; }
        public int maxCore { get; set; }
        public int userId { get; set; }

    }
    public class SplicingViewModelOLD
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string network_id { get; set; }
        public int no_of_tube { get; set; }
        public int tube_number { get; set; }
        public string tube_coor_code { get; set; }
        public int core_port_number { get; set; }
        public string core_color_code { get; set; }
        public string core_port_status { get; set; }
        public int is_parent { get; set; }
        public string input_output { get; set; }
        public int destination_system_id { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_network_id { get; set; }
        public int destination_port_no { get; set; }
        public int via_system_id { get; set; }
        public string via_entity_type { get; set; }
        public string via_network_id { get; set; }
        public int via_port_no { get; set; }
        public bool is_connected_to_other { get; set; }
        public string other_text { get; set; }
    }
    public class SplicingViewModel
    {
        public SplicingConectionInfo sourceCable { get; set; }
        public SplicingConectionInfo destinationCable { get; set; }
        public SplicingConectionInfo connector { get; set; }
        public List<SplicingConectionInfo> splicingConnections { get; set; }
        public int userId { get; set; }
        public int total_ports { get; set; }
        public int available_ports { get; set; }
        public int splice_tray_system_id { get; set; }
        public List<portStatusMaster> listPortStatus { get; set; }
        public List<middleWarePorts> middlePortList { get; set; }
        public List<string> lstUserModule { get; set; }
        public bool isEditAllowed { get; set; }
        [NotMapped]
        public List<SpliceTrayInfo> lstSpliceTray { get; set; }
        public string connector_entity_type { get; set; }
        public bool is_middleware_entity { get; set; }
        //public string entity_name { get; set; }
        public ConnectionInfoMaster ConnInfoMaster { get; set; }
        public bool is_virtual { get; set; }
        public bool is_virtual_entity { get; set; }
        public string unauthorisedmessage { get; set; }

    }
    public class PatchingViewModel
    {

        public List<SplicingConectionInfo> splicingConnections { get; set; }
        public List<portStatusMaster> listPortStatus { get; set; }
        public int userId { get; set; }
        public ConnectionFilter filter { get; set; }
        public PatchingViewModel()
        {
            filter = new ConnectionFilter();
        }
    }
    public class SplicingConectionInfo
    {
        public int? system_id { get; set; }
        public string entity_type { get; set; }
        public string network_id { get; set; }
        public string display_name { get; set; }
        public int? no_of_tube { get; set; }
        public int? tube_number { get; set; }
        public string tube_color_code { get; set; }
        public int? core_port_number { get; set; }
        public string core_color_code { get; set; }
        public string core_port_status { get; set; }
        public int? is_parent { get; set; }
        public string input_output { get; set; }
        public int? destination_system_id { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_network_id { get; set; }
        public int? destination_tube_number { get; set; }
        public int? destination_port_no { get; set; }
        public int? via_system_id { get; set; }
        public string via_entity_type { get; set; }
        public string via_network_id { get; set; }
        public int? via_port_no { get; set; }
        public bool is_connected_to_other { get; set; }
        public string other_text { get; set; }
        public bool is_cable_a_end { get; set; }
        public string core_port_status_color { get; set; }
        public bool is_customer_connected { get; set; }
        public bool is_other_end_spliced { get; set; }
        public string other_end_text { get; set; }
        public bool is_multiconnection { get; set; }
        public int link_system_id { get; set; }
        public int connectioncount { get; set; }
        public int superparent { get; set; }
        public int parentsystem_id { get; set; }
        public string parententitytype { get; set; }
        public string parentnetworkid { get; set; }
        public string modelname { get; set; }
        public int port_statusid { get; set; }
        public string port_comment { get; set; }
        public string short_network_id { get; set; }
        public string name { get; set; }
        public string entity_sub_type { get; set; }
        //public bool is_middle_entity { get; set; }
        public int source_tray_system_id { get; set; }
        public int destination_tray_system_id { get; set; }
        public string source_tray_display_name { get; set; }
        public string destination_tray_display_name { get; set; }
        public string entity_type_title { get; set; }
        public string destination_entity_type_title { get; set; }
        public string via_entity_type_title { get; set; }
        public string source_port { get; set; }
        public string destination_port { get; set; }
        public string via_port { get; set; }
        public string tray_name { get; set; }
        public bool is_through_connection { get; set; }
        public bool is_virtual { get; set; }

        public string destination_display_name { get; set; }
        public string via_display_name { get; set; }
    }
    public class AvailablePorts
    {
        public int total_ports { get; set; }
        public int available_ports { get; set; }
    }

    public class SplicingCables
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public int total_core { get; set; }
        public int no_of_tube { get; set; }
        public int no_of_core_per_tube { get; set; }


        public int tube_number { get; set; }
        public string tube_color { get; set; }
        public string tube_color_code { get; set; }
        public int core_number { get; set; }
        public string core_color { get; set; }
        public string core_color_code { get; set; }
        public int fiber_number { get; set; }
        public bool is_connected_as_source { get; set; }
        public bool is_connected_as_destination { get; set; }

    }
    public class SplicingConnectors
    {
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string ports { get; set; }
        public string entity_type { get; set; }
        public int is_parent { get; set; }
    }
    public class SplicingPortInfo
    {
        public int parent_system_id { get; set; }
        public string parent_entity_type { get; set; }
        public string parent_network_id { get; set; }
        public string input_output { get; set; }
        public string port_status { get; set; }
        public int port_number { get; set; }
        public string port_name { get; set; }
    }
    public class SplicingInput
    {
        public int SSystem_Id { get; set; }
        public int DSystem_Id { get; set; }
        public int CenSystem_Id { get; set; }
        public string CenType { get; set; }
        public string ConnectivityType { get; set; }
    }
    public class connectionInput: Models.CommonGridAttributes
    {
        public int source_system_id { get; set; }
        public string source_entity_type { get; set; }
        public bool is_source_start_point { get; set; }
        public List<connectors> listConnector { get; set; }
        //public string connector_system_id { get; set; }
        public string connector_entity_type { get; set; }
        public int destination_system_id { get; set; }
        public string destination_entity_type { get; set; }
        public bool is_destination_start_point { get; set; }
        public string customer_ids { get; set; }
        public string splicing_type { get; set; }
        public bool is_middleware_entity { get; set; }
        public List<ExportSplicing> listSplicingReport { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public connectionInput()
        {
            listConnector = new List<connectors>();
            listSplicingReport = new List<ExportSplicing>();

        }
        public int user_id { get; set; }
    }
    public class connectors
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public bool is_virtual { get; set; }
        public bool is_virtual_entity { get; set; }
    }
    public class deleteConeectionInput
    {
        public int source_system_id { get; set; }
        public string source_entity_type { get; set; }
        public int source_port_no { get; set; }
        public int destination_system_id { get; set; }
        public string destination_entity_type { get; set; }
        public int destination_port_no { get; set; }
        public bool is_source_cable_a_end { get; set; }
        public bool is_destination_cable_a_end { get; set; }
        public int equipment_system_id { get; set; }
        public string equipment_entity_type { get; set; }
    }
    public class TubeCoreColor
    {
        public int cable_id { get; set; }
        public int tube_number { get; set; }
        public string tube_color { get; set; }
        public string tube_color_code { get; set; }
        public int core_number { get; set; }
        public string core_color { get; set; }
        public string core_color_code { get; set; }
    }

    public class LinkBudgetFilter : Models.CommonGridAttributes
    {
        public int equipmentsystemid { get; set; }
        public int equipmentPortid { get; set; }
        public int sourcesystemid { get; set; }
        public int sourceportno { get; set; }
        public string sourceentitytype { get; set; }

        public int destinationsystemid { get; set; }
        public int destinationportno { get; set; }
        public string destinationentitytype { get; set; }
        public double transmitpower { get; set; }
        public int wavelengthid { get; set; }
        public string equipmenttype { get; set; }
        public bool isUpStreamOnly { get; set; }
        public string connectionString { get; set; }
        public LinkBudgetFilter()
        {
            sourcesystemid = 0;
            sourceportno = 0;
            sourceentitytype = string.Empty;
            destinationsystemid = 0;
            destinationportno = 0;
            destinationentitytype = string.Empty;
        }
    }

    public class ViewLinkBudgetDataDetail
    {
        public int id { get; set; }
        public int connection_id { get; set; }
        public int source_system_id { get; set; }
        //public string source_network_id { get; set; }
        public string source_entity_type { get; set; }
        public int source_port_no { get; set; }
        public int destination_system_id { get; set; }
        //public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public int destination_port_no { get; set; }
        public bool is_customer_connected { get; set; }
        public DateTime created_on { get; set; }
        public int? created_by { get; set; }
        public int? approved_by { get; set; }
        public DateTime? approved_on { get; set; }
        public decimal? cable_calculated_length { get; set; }
        public decimal? cable_measured_length { get; set; }
        public string splitter_ratio { get; set; }
        public decimal? receiving_power { get; set; }
        public string loss_type { get; set; }
        public decimal? loss_value { get; set; }
        public decimal? transmit_power { get; set; }
        public bool is_source_virtual { get; set; }
        public bool is_destination_virtual { get; set; }
        public string source_display_name { get; set; }
        public string destination_display_name { get; set; }
    }
    public class portStatusMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string status { get; set; }
        public string color_code { get; set; }
        public bool is_active { get; set; }
        public bool is_manual_status { get; set; }
        public bool is_splicing_enabled { get; set; }
        [NotMapped]
        public string created_by_text { get; set; }
        public DateTime? created_on { get; set; }
        [NotMapped]
        public string modified_by_text { get; set; }
        public DateTime? modified_on { get; set; }
        public int? created_by { get; set; }
        public int? modified_by { get; set; }
        [NotMapped]
        public int totalRecords { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
    }

    public class LogicalViewVM : CommonGridAttributes
    {
        public int system_id { get; set; }
        public string entity_type { get; set; }
        public string networkId { get; set; }
        public List<portStatusMaster> listPortStatus { get; set; }
        public List<LogicalViewPortDetail> lstport { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public string connectionString { get; set; }
        public LogicalViewVM()
        {
            entity_type = string.Empty;
            lstUserModule = new List<string>();
        }
    }

    public class EntityLogicalView_EntityDetail
    {
        public string ratio { get; set; }
        public string name { get; set; }
        public string network_id { get; set; }
        public int parent_system_id { get; set; }
        public string longitude { get; set; }
        public int? parent_network_id { get; set; }
        public int system_id { get; set; }
        public string parent_entity_type { get; set; }
        public string latitude { get; set; }
        public string type { get; set; }
    }
    public class LogicalViewPortDetail
    {
        public int source_system_id { get; set; }
        public int source_port_number { get; set; }
        public string source_network_id { get; set; }
        public string source_port_io_text { get; set; }
        public string source_port_type { get; set; }
        public string source_ratio { get; set; }
        public string source_entity_category { get; set; }
        public string source_entity_type { get; set; }
        public int? connected_system_id { get; set; }
        public int? connected_port_number { get; set; }
        public string connected_network_id { get; set; }
        public string connected_port_status { get; set; }
        public string connected_entity_type { get; set; }
        public string connected_entity_category { get; set; }
        public string connected_entitytype_text { get; set; }
        public string connected_port_text { get; set; }
        public string connected_ratio { get; set; }
        public string connected_entity_name { get; set; }
        public string connected_on { get; set; }
        public string port_status_color { get; set; }
        public string port_comment { get; set; }
        public string connected_label { get; set; }
        public string source_display_name { get; set; }
        public string destination_display_name { get; set; }

    }
    public class middleWarePorts
    {
        public int system_id { get; set; }
        public string port_name { get; set; }
        public int port_number { get; set; }
    }
    public class connectedCusotmer
    {
        public string can_id { get; set; }
        public string customer_name { get; set; }
        public string customer_type { get; set; }
        public string LMC_Type { get; set; }
        public string address { get; set; }
        public string region_name { get; set; }
        public string province_name { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        [NotMapped]
        public string entity_type { get; set; }
        [NotMapped]
        public string network_id { get; set; }
        [NotMapped]
        public int core_port_number { get; set; }
        public string mobile_no { get; set; }
        public string phone_no { get; set; }
        public string email_id { get; set; }
    }

    public class ConnectionFilter
    {
        public List<DropDownMaster> listRack { get; set; }
        public int SourcePODId { get; set; }
        public int SourceRackId { get; set; }
        public int SourceEquipmentId { get; set; }
        public string SourceEquipmentNWId { get; set; }
        public int DestinationPODId { get; set; }
        public int DestinationRackId { get; set; }
        public int DestinationEquipmentId { get; set; }
        public string DestinationEquipmentNWId { get; set; }
        public int entityId { get; set; }
        public string entity_type { get; set; }
        public List<DropDownMaster> SourceEquipmentList { get; set; }
        public List<DropDownMaster> DestinationEquipmentList { get; set; }
        public int SourcePortId { get; set; }
        public int DestinationPortId { get; set; }
        public bool isInsideConnectivity { get; set; }
        public ConnectionFilter()
        {
            SourceEquipmentList = new List<DropDownMaster>();
            DestinationEquipmentList = new List<DropDownMaster>();
        }
    }

    public class MultipleConnections
    {
        public int connection_id { get; set; }
        public int destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public string destination_entity_type { get; set; }
        public int destination_port_no { get; set; }
        public int patch_cord_id { get; set; }
        public string patch_network_id { get; set; }
        public string port_network_id { get; set; }
        public string port_name { get; set; }
        public string port_type { get; set; }
        public int model_id { get; set; }
        public string rack_network_id { get; set; }

    }
    public class viewMultipleConnections
    {
        public int source_system_id { get; set; }
        public string source_entity_type { get; set; }
        public string source_network_id { get; set; }
        public int source_port_no { get; set; }
        public string portType { get; set; }
        public bool isPreviousConnection { get; set; }
        public List<MultipleConnections> connectionsList { get; set; }
        public int rackId { get; set; }
        public bool isOutConnection { get; set; }
    }
    public class UpdateCorePortStatus
    {
        public string entity_type { get; set; }
        public int entity_system_id { get; set; }
        public int portStatus { get; set; }
        public string comment { get; set; }
        public string core_port_number { get; set; }
        public int user_id { get; set; }
        public List<portStatusMaster> listPortStatus { get; set; }
        public DbMessage pageMsg { get; set; }
        public string type { get; set; }
        public string portColor { get; set; }
        public string status { get; set; }
        public bool isGridCalling { get; set; }
    }
    
    public class connectionInfoCable
    {
        public List<CablerouteInfo> lstConnectionInfo { get; set; }
    }
    public class CablerouteInfo
    {

        public int source_system_id { get; set; }
        public string source_network_id { get; set; }
        public int source_port_no { get; set; }
        public string source_entity_type { get; set; }
        public string source_entity_title { get; set; }

        public int destination_system_id { get; set; }
        public string destination_network_id { get; set; }
        public int destination_port_no { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_entity_title { get; set; }

        public int? viya_system_id { get; set; }
        public string via_network_id { get; set; }
        public string via_entity_type { get; set; }
        public int path_id { get; set; }
        public int headerCol { get; set; }
        public int rowsdataloop { get; set; }
        public int globaLoopcount { get; set; }

        public string source_tube_colour_code { get; set; }
        public string source_port_colour_code { get; set; }
        public string source_tube_name { get; set; }
        public string destination_tube_colour_code { get; set; }
        public string destination_port_colour_code { get; set; }
        public string destination_tube_name { get; set; }
        public int fms_id { get; set; }
        public int? splitter_id { get; set; }

    }
    public class connectionInfoPath
    {
        public List<ConnectionInfo> lstConnectionInfo { get; set; }
    }
    public class SchematicView
    {
        public string nodes { get; set; }
        public string edges { get; set; }
        public string legends { get; set; }
        public string cables { get; set; }
        public List<legend> lstlegend { get; set; }
        public List<CableLegend> lstCableLegend { get; set; }
        public string downstreamNodes { get; set; }
        public string downstreamEdges { get; set; }
        public string downstreamLegends { get; set; }
        public string downstreamCables { get; set; }
        public List<legend> lstdownStremaLegend { get; set; }
        public List<CableLegend> lstDownStreamCableLegend { get; set; }

    }
    public class legend
    {
        public string entity_type { get; set; }
        public string entity_title { get; set; }
        public bool upstream { get; set; }
    }
    public class checkbox
    {
        public string checkbox_entity_type { get; set; }
        public int count { get; set; }
        public string color { get; set; }
    }
    public class CableLegend
    {
        public string color_code { get; set; }
        public string text { get; set; }
        public bool upstream { get; set; }
    }
    public class VizButterFlyNetwork
    {
        public string entityType { get; set; }
        public string entityTitle { get; set; }
        public string entityDisplayText { get; set; }
        public string nodes { get; set; }
        public string edges { get; set; }
        public string legends { get; set; }
        public string checkbox { get; set; }
        public List<legend> lstlegend { get; set; }
        public List<checkbox> lstChekbox { get; set; }
    }
    public class SLDModel
    {
        public string entityType { get; set; }
        public string entityTitle { get; set; }
        public string entityDisplayText { get; set; }
        public string nodes { get; set; }
        public string edges { get; set; }
        public string nodesDownstream { get; set; }
        public string edgesDownstream { get; set; }
        public string legends { get; set; }
        public List<legend> lstlegend { get; set; }
        public string title { get; set; }
        public string cables { get; set; }
        public List<CableLegend> lstCableLegend { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public DateTime created_on { get; set; }
        [NotMapped]
        public string fiber_link_id { get; set; }
    }
    public class AddConnection
    {
        public List<DropDownMaster> cardList { get; set; }
        public int cardId { get; set; }
        public int sourcePortNo { get; set; }
        public int destinationPortNo { get; set; }
    }
    public class PortsList
    {
        public int dropdown_value { get; set; }
        public string dropdown_key { get; set; }
        public int port_status_id { get; set; }
        public bool is_multi_connection { get; set; }
    }
    public class ExportSplicingInfo
    {
        public string source_network_id { get; set; }
        public string source_type { get; set; }
        public int? source_tube_no { get; set; }
        public int? source_port_no { get; set; }
        public string source_port { get; set; }
        public string via_entity { get; set; }
        public string via_entity_id { get; set; }
        public int? via_entity_port { get; set; }
        public string via_port { get; set; }
        public string using_entity { get; set; }
        public string using_entity_id { get; set; }
        public int? using_entity_input { get; set; }
        public string using_input { get; set; }
        public int? using_entity_output { get; set; }
        public string using_output { get; set; }
        public string destination_network_id { get; set; }
        public string destination_type { get; set; }
        public int? destination_tube_no { get; set; }
        public string destination_port { get; set; }
        public int? destination_port_no { get; set; }
        public string source_tray_display_name { get; set; }
        public string destination_tray_display_name { get; set; }
        public string tray_name { get; set; }
        public bool is_through_connection { get; set; }
        public string Connectivity_Type { get; set; }

    }
    public class ExportPatchingInfo
    {
        public string source_id { get; set; }
        public string source_type { get; set; }
        public int source_port_no { get; set; }
        public string source_port_description { get; set; }
        public string via_entity { get; set; }
        public string via_entity_id { get; set; }
        public string destination_id { get; set; }
        public string destination_type { get; set; }
        public int destination_port_no { get; set; }
        public string destination_port_description { get; set; }

    }
    public class PortHistory
    {
        public int system_id { get; set; }
        public string port_status { get; set; }
        public string port_name { get; set; }
        public int port_number { get; set; }
        public string port_type { get; set; }
        public string input_output { get; set; }
        public string port_comment { get; set; }
        public DateTime? created_on { get; set; }
    }
    public class ExportSplicing
    {
        public string source { get; set; }
        public string source_entity_type { get; set; }
        public string source_tube_no { get; set; }
        public string source_port_no { get; set; }
        public string via_entity { get; set; }
        public string via_entity_type { get; set; }
        public string via_port_no { get; set; }
        public string tray_name { get; set; }
        public string using_entity { get; set; }
        public string using_entity_type { get; set; }
        public string using_port_in { get; set; }
        public string using_port_out { get; set; }
        public string destination { get; set; }
        public string destination_entity_type { get; set; }
        public string destination_tube_no { get; set; }
        public string destination_port_no { get; set; }
        public int totalRecords { get; set; }
    }
    public class NotificationUtlization
    {
        public string entityname { get; set; }
        public int system_id { get; set; }
        public string utilization_abbr { get; set; }
        public string utilized_text { get; set; }
        public int total_ports { get; set; }
        public int used_ports { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public string network_id { get; set; }
    }

    public class SplicingInputReq
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double bufferRadius { get; set; }
        public int role_id { get; set; }
        public int user_id { get; set; }
    }
    public class ViewSplicingEntity
    {
        public List<SplicingEntity> SplicingLst { get; set; }
        public ViewSplicingEntity()
        {
            SplicingLst = new List<SplicingEntity>();
        }
    }
    public class AvailabePortReq
    {
        public int systemId { get; set; }
        public string entityType { get; set; }
    }
    public class ConnectionInfoMasterInput
    {
        public List<ConnectionInfoMaster> connections { get; set; }
        public User user { get; set; }
    }
    public class bulkSplicingInput
    {
        public int systemId { get; set; }
        public string networkId { get; set; }
        public string entityType { get; set; }
        public bool isCableAend { get; set; }
        public string connectionType { get; set; }
        public int from { get; set; }
        public int to { get; set; }
    }

    public class splicingReport
    {
        [Required(ErrorMessage = "Source System ID is required")]
        public int source_system_id { get; set; }
        public string source_type { get; set; }
        [Required(ErrorMessage = " is source connected is required")]
        public bool is_source_connected { get; set; }
        [Required(ErrorMessage = " connecting entity is required")]
        public string connecting_entity { get; set; }
        [Required(ErrorMessage = " destination type is required")]
        public string destination_type { get; set; }
        [Required(ErrorMessage = " destination system id is required")]
        public int destination_system_id { get; set; }
        [Required(ErrorMessage = " is_destination_connected is required")]
        public bool is_destination_connected { get; set; }
        [Required(ErrorMessage = " exportTypeis required")]
        public string exportType { get; set; }
        [Required(ErrorMessage = " exportKey is required")]
        public string exportKey { get; set; }
        public string customer_Ids { get; set; }
    }
}


