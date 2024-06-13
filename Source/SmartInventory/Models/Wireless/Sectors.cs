using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Admin;
using System.ComponentModel.DataAnnotations;

namespace Models
{

	public class SectorTemplateMaster : itemMaster
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


    public class SectorItemMaster : SectorTemplateMaster
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
        public SectorItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class SectorMaster : SectorTemplateMaster, IProjectSpecification, IOwnershipInfo, IInstallationInfo,IReference,IGeographicDetails
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
		public string parent_site_id { get; set; }
		public string sector_layer_id { get; set; }
		public string node_identity { get; set; }
		[Range(0, 10)]
		public double total_tilt { get; set; }
		public int sequence_id { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string remarks { get; set; }
		public int region_id { get; set; }
		public int province_id { get; set; }
		public double azimuth { get; set; }
		[Required]
		public string technology { get; set; }
		public int port_name { get; set; }
		[Required]
		public string down_link { get; set; }
		[Required]
		public string uplink { get; set; }
		public string remark { get; set; }

		public string ownership_type { get; set; }
		public string source_ref_type { get; set; }
		public string source_ref_id { get; set; }
		public string source_ref_description { get; set; }
		 
		public string status_remark { get; set; }
		public int status_updated_by { get; set; }
		public DateTime status_updated_on { get; set; }
		public string network_status { get; set; }
		[Required]
		public string frequency { get; set; }
		[Required]
		public string sector_type { get; set; }
		public string status { get; set; }
		[NotMapped]
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

		[NotMapped]
		public string region_name { get; set; }
		[NotMapped]
		public string province_name { get; set; }
		[NotMapped]
		public string geom { get; set; }
		[NotMapped]
		public string networkIdType { get; set; }
		[NotMapped]
		public PageMessage objPM { get; set; }
		[NotMapped]
		public bool is_servingdb { get; set; }
		public int? project_id { get; set; }
		public int? planning_id { get; set; }
		public int? workorder_id { get; set; }
		public int? purpose_id { get; set; }
		[NotMapped]
		public int pSystemId { get; set; }
		[NotMapped]
		public string pEntityType { get; set; }
		[NotMapped]
		public string pNetworkId { get; set; }

		public string third_party_vendor_id { get; set; }
		public string own_vendor_id { get; set; }

		[NotMapped]
		public string circuit_id { get; set; }
		[NotMapped]
		public string thirdparty_circuit_id { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> list3rdPartyVendorId { get; set; }
		[NotMapped]
		public List<KeyValueDropDown> listOwnVendorId { get; set; }
		[NotMapped]
		public List<ProjectCodeMaster> lstBindProjectCode { get; set; }
		[NotMapped]
		public List<PlanningCodeMaster> lstBindPlanningCode { get; set; }
		[NotMapped]
		public List<WorkorderCodeMaster> lstBindWorkorderCode { get; set; }
		[NotMapped]
		public List<PurposeCodeMaster> lstBindPurposeCode { get; set; }


		[NotMapped]
		public IList<DropDownMaster> lstDownlink { get; set; }

		[NotMapped]
		public IList<DropDownMaster> lstSectorType { get; set; }
		[NotMapped]
		public IList<DropDownMaster> lstFrequencyType { get; set; }

		[NotMapped]
		public IList<DropDownMaster> lstUplink { get; set; }
		public string brand_name { get; set; }
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
		public string ne_id { get; set; }
		public string prms_id { get; set; }
		public string jc_id { get; set; }
		public string mzone_id { get; set; }
		[NotMapped]
		public string partner_name { get; set; }
        [NotMapped]
        public List<string> lstUserModule { get; set; }
        public SectorMaster()
		{
			objPM = new PageMessage();
			lstBindProjectCode = new List<ProjectCodeMaster>();
			lstBindPlanningCode = new List<PlanningCodeMaster>();
			lstBindWorkorderCode = new List<WorkorderCodeMaster>();
			lstBindPurposeCode = new List<PurposeCodeMaster>();
			EntityReference = new EntityReference();
            project_id = 0;
            planning_id = 0;
            workorder_id = 0;
            purpose_id = 0;
            lstUserModule = new List<string>();
        }
	}
}
