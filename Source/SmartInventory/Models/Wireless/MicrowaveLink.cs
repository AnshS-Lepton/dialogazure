using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Admin;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace Models
{

	[Serializable]
	public class MicrowaveLinkViewModel : itemMaster, IProjectSpecification, IOwnershipInfo, IInstallationInfo
	{

		public MicrowaveLinkMaster microwaveLinkMaster { get; set; }
		public TowerMaster towerMasterA { get; set; }
		public TowerMaster towerMasterB { get; set; }
		public AntennaMaster antennaMasterA { get; set; }
		public AntennaMaster antennaMasterB { get; set; }

		public string ownership_type { get; set; }
		public string third_party_vendor_id { get; set; }
		public string circuit_id { get; set; }
		public string thirdparty_circuit_id { get; set; }
		public string entityType { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }


		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		public int installation_id { get; set; }
		public int entity_system_id { get; set; }
		public string entity_type { get; set; }
		public string installation_number { get; set; }
		public string installation_year { get; set; }
		public string production_year { get; set; }
		public string installation_company { get; set; }
		public string installation_technician { get; set; }
		public string installation { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

	}


	public class MicrowaveLinkTemplateMaster : itemMaster
	{
		[NotMapped]
		public EntityReference EntityReference { get; set; }
		[NotMapped]
		public override int no_of_input_port { get; set; }
		[NotMapped]
		public override int no_of_output_port { get; set; }
		[NotMapped]
		public override int no_of_port { get; set; }
		[NotMapped]
		public override string unit { get; set; }
		[NotMapped]
		public override string other { get; set; }
		[NotMapped]
		public new int type { get; set; }
		[NotMapped]
		public new int brand { get; set; }
		[NotMapped]
		public new int model { get; set; }
		[NotMapped]
		public new int construction { get; set; }
		[NotMapped]
		public new int activation { get; set; }
		[NotMapped]
		public new int accessibility { get; set; }
	}


    public class MicrowaveLinkItemMaster : MicrowaveLinkTemplateMaster
    {
        //ADDITION FIELD WHICH ARE REQUIRED FOR SPLT TEMPLATE ONLY WILL BE THERE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int created_by { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public int userId { get; set; }
        public MicrowaveLinkItemMaster()
        {
            objPM = new PageMessage();
        }
    }


    public class MicrowaveLinkMaster : MicrowaveLinkTemplateMaster, IProjectSpecification, IOwnershipInfo, IInstallationInfo,IReference,IGeographicDetails,IAdditionalFields
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public string network_id { get; set; }
        [Required]
		public string network_name { get; set; }
		public int parent_system_id { get; set; }
		public string parent_network_id { get; set; }
		public string parent_entity_type { get; set; }
		public int sequence_id { get; set; }
		public int region_id { get; set; }
		public int province_id { get; set; }

		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }

        [Required]
		public string link_type { get; set; }
		public string link_name { get; set; }
		public string hierarchy_type { get; set; }
		public string service_id { get; set; }
		public string total_capacity { get; set; }
		public string free_capacity { get; set; }
		public string status { get; set; }
		public int user_id { get; set; }
		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }

		public int db_flag { get; set; }
		[NotMapped]
		public string entity_type { get; set; }

		public string installation_number { get; set; }

		public string installation_year { get; set; }

		public string production_year { get; set; }

		public string installation_company { get; set; }

		public string installation_technician { get; set; }

		public string installation { get; set; }
		public string network_status { get; set; }
		public string ownership_type { get; set; }

		public int tower_a_system_id { get; set; }
		public int tower_b_system_id { get; set; }
		public int antenna_a_system_id { get; set; }
		public int antenna_b_system_id { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }

		public string third_party_vendor_id { get; set; }
		[NotMapped]
		public string circuit_id { get; set; }
		[NotMapped]
		public string thirdparty_circuit_id { get; set; }
		public int? min_frequency_received { get; set; }
		public int? max_frequency_received { get; set; }
		public int? min_frequency_transmitted { get; set; }
		public string odu_type { get; set; }
		public int? max_frequency_transmitted { get; set; }
		public string nms_ip { get; set; }
		public string user_name { get; set; }
		public string password { get; set; }
		public string manufacturer_name { get; set; }
		public string model_number { get; set; }
		public double? license_number { get; set; }
		public int? idu_transmit_power { get; set; }
		public string modulation { get; set; }
		public int? idu_transmitted_frequency { get; set; }
		public int? idu_received_frequency { get; set; }
		public double? bandwidth { get; set; }
		public string polarization { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }
		[NotMapped]
		public TowerMaster towerMasterA { get; set; }
		[NotMapped]
		public TowerMaster towerMasterB { get; set; }
		[NotMapped]
		public AntennaMaster antennaMasterA { get; set; }
		[NotMapped]
		public AntennaMaster antennaMasterB { get; set; }

		[NotMapped]
		public IList<DropDownMaster> LstLinkType { get; set; }
        public string status_remark { get; set; }
        public bool is_new_entity { get; set; }
		public string main_link_type { get; set; }
		public string redundant_link_type { get; set; }

		public string main_link_id { get; set; }
		public string redundant_link_id { get; set; }
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
		[Required]
		public string bom_sub_category { get; set; }
		//public string served_by_ring { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstBOMSubCategory { get; set; }
		[NotMapped]
		public List<DropDownMaster> lstServedByRing { get; set; }
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public MicrowaveLinkMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			EntityReference = new EntityReference();
			LstLinkType = new List<DropDownMaster>();
            planning_id = 0;
            purpose_id = 0;
            workorder_id = 0;
            project_id = 0;
            lstUserModule = new List<string>();
        }

	}

	public class MicrowavelinkFeederSystem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public int mwlink_sys_id { get; set; }
		public string side { get; set; }
		public string feeder_type { get; set; }
		public string feeder_name { get; set; }
		public double loss { get; set; }
		public double length { get; set; }
		public double loss_tx { get; set; }
		public double loss_rx { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

	}

	public class MicrowavelinkPower
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public int mwlink_sys_id { get; set; }
		public string power_type { get; set; }
		public string side { get; set; }
		public double tx_power { get; set; }
		public double eirp { get; set; }
		public double rx_power { get; set; }
		public double rx_power_diversity { get; set; }
		public double threshold { get; set; }
		public double fade_margin { get; set; }
		public double interface_td_rain { get; set; }
		public double other_margin_rain { get; set; }
		public double flat_margin_rain { get; set; }
		public double interface_td_multipath { get; set; }
		public double other_margin_multipath { get; set; }
		public double flat_margin_multipath { get; set; }
		public double interface_td_refraction { get; set; }
		public double other_margin_refraction { get; set; }
		public double flat_margin_refraction { get; set; }
		public double tot_performance_year { get; set; }
		public double tot_performance_month { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

	}

	public class MicrowavelinkAntenna
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int system_id { get; set; }
		public int mwlink_sys_id { get; set; }
		public string side { get; set; }

		public string antenna_type { get; set; }
		public string antenna_name { get; set; }
		public string antenna_operator { get; set; }
		public string far_end_id { get; set; }
		public double height { get; set; }
		public double azimuth { get; set; }
		public double tilt { get; set; }
		public double radome_loss { get; set; }
		public double loss_tx { get; set; }
		public double loss_rx { get; set; }
		public double gain { get; set; }
		public double diameter { get; set; }
		public double antenna_loss { get; set; }
		public double coupling_loss { get; set; }
		public int created_by { get; set; }
		public DateTime created_on { get; set; }
		public int? modified_by { get; set; }
		public DateTime? modified_on { get; set; }

	}
}
