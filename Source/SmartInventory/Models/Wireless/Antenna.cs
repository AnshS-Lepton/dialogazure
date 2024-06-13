using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Admin;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AntennaTemplateMaster : itemMaster
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

    public class AntennaItemMaster : AntennaTemplateMaster
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
        public AntennaItemMaster()
        {
            objPM = new PageMessage();
        }
    }
    public class AntennaMaster : AntennaTemplateMaster, IProjectSpecification, IOwnershipInfo, IReference, IGeographicDetails, IAdditionalFields
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
        [Required]
        public string antenna_type { get; set; }
        public string remarks { get; set; }
        public string antenna_sub_type { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public double minimum_frequency { get; set; }
        public double maximum_frequency { get; set; }
        public double diameter_in_meter { get; set; }
        [Range(1.5, 50)]
        public double maximum_gain { get; set; }
        public double boresight_gain { get; set; }
        [Required]
        public string user_cross_polor_pattern { get; set; }
        [Range(1.5, 50)]
        public double co_polor_vertical_maximum_gain { get; set; }
        public double co_polor_vertical_fb { get; set; }
        public double co_polor_vertical_bw { get; set; }
        public double co_polor_vertical_boresight { get; set; }
        [Range(1.5, 50)]
        public double cross_polor_vertical_maximum_gain { get; set; }
        public double cross_polor_vertical_fb { get; set; }
        public double cross_polor_vertical_bw { get; set; }
        public double cross_polor_vertical_boresight { get; set; }
        [Range(1.5, 50)]
        public double co_polor_horizontal_maximum_gain { get; set; }
        public double co_polor_horizontal_fb { get; set; }
        public double co_polor_horizontal_bw { get; set; }
        public double co_polor_horizontal_boresight { get; set; }
        [Range(1.5, 50)]
        public double cross_polor_horizontal_maximum_gain { get; set; }
        public double cross_polor_horizontal_fb { get; set; }
        public double cross_polor_horizontal_bw { get; set; }
        public double cross_polor_horizontal_boresight { get; set; }
        public string remark { get; set; }

        public string ownership_type { get; set; }
        public string source_ref_type { get; set; }
        public string source_ref_id { get; set; }
        public string source_ref_description { get; set; }
        public string status_remark { get; set; }
        public int status_updated_by { get; set; }
        public DateTime? status_updated_on { get; set; }
        public string network_status { get; set; }
        public string status { get; set; }
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
        public string model_number { get; set; }
        public string manufacturer_name { get; set; }
        public string polarization { get; set; }
        [NotMapped]
        public string thirdparty_circuit_id { get; set; }
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
        public IList<DropDownMaster> lstAntennaType { get; set; }

        [NotMapped]
        public IList<DropDownMaster> lstSubAntennaType { get; set; }

        [NotMapped]
        public IList<DropDownMaster> lstAntennaOperator { get; set; }

        [NotMapped]
        public IList<DropDownMaster> lstUsePattern { get; set; }
        [NotMapped]
        public VSATAntenna objVSATAntenna { get; set; }
        [Required]
        public double height { get; set; }
        public double azimuth { get; set; }
        public string antenna_operator { get; set; }
        public double mechanical_tilt { get; set; }
        public double electrical_tilt { get; set; }
        [Range(0, 10)]
        public double total_tilt { get; set; }
        public bool is_new_entity { get; set; }
        [NotMapped]
        public int? vsat_id { get; set; }
        [NotMapped]
        public PageMessage pageMsg { get; set; }
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
        // public string served_by_ring { get; set; }
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
        public AntennaMaster()
        {
            objPM = new PageMessage();
            lstBindProjectCode = new List<ProjectCodeMaster>();
            lstBindPlanningCode = new List<PlanningCodeMaster>();
            lstBindWorkorderCode = new List<WorkorderCodeMaster>();
            lstBindPurposeCode = new List<PurposeCodeMaster>();
            EntityReference = new EntityReference();
            objVSATAntenna = new VSATAntenna();
            planning_id = 0;
            project_id = 0;
            workorder_id = 0;
            purpose_id = 0;
            bom_sub_category = Convert.ToString(Bom_boq_category.Proposed);
            lstUserModule = new List<string>();
        }
    }

    public class VSATAntenna
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string rf_band_type { get; set; }
        public int uplink_ghz { get; set; }
        public int downlink_ghz { get; set; }

        public string satellite_type { get; set; }

        public string transponder_type { get; set; }

        public string look_angle_deg { get; set; }

        public string elevation_deg { get; set; }

        public DateTime? operational_from_date_bs { get; set; }

        public DateTime? operational_to_date_bs { get; set; }

        public string radio_model { get; set; }

        public string status { get; set; }

        public string orientation { get; set; }

        public string purpose { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        public int sequence_id { get; set; }
        [NotMapped]
        public bool is_vsat_updated { get; set; }

        public int building_system_id { get; set; }


        [NotMapped]
        public IList<DropDownMaster> VSATSetellite { get; set; }
        [NotMapped]
        public IList<DropDownMaster> VSATTranspond { get; set; }
        [NotMapped]
        public IList<DropDownMaster> VSATRFBand { get; set; }


    }

}
