using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.TempUpload
{
    public class TempWirelineCustomer:IReference
    {
        [Key, Column("system_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }//eLOC id 
        public string customer_name { get; set; }
        //New fields added for sify
        public string customer_code { get; set; }
        public string site_id { get; set; }
        public string site_name { get; set; }
        public string site_address { get; set; }
        public string lms_id { get; set; }
        public string customer_link_id { get; set; }
        public string customer_po_id1 { get; set; }
        public string customer_po_id2 { get; set; }
        public DateTime po_date { get; set; }
        public string customer_service_address { get; set; }
        public string owner { get; set; }
        public double building_height { get; set; }
        public string building_type1 { get; set; }
        public string building_type2 { get; set; }
        public string building_owner_type { get; set; }
        public string connected_sw_ip { get; set; }
        public string connected_sw_port { get; set; }
        public string customer_subscribed_bw_in_kbps { get; set; }
        public string customer_connected_through { get; set; } //UG/OH
        //public double cable_length { get; set; }//Underground/Overhead Cable length
        public double fiber_length { get; set; }
        public string path_type { get; set; }//Primary/Secondary
        public string primary_bstn_name { get; set; }//Primary
        public string secondary_bstn_name { get; set; }//Secondary
        public DateTime commissioning_date { get; set; }
        public string vendor_name { get; set; }
        public double kml_length { get; set; }
        public int city_id { get; set; }

        //fields already exists and used in sify
        public int region_id { get; set; }
        public int province_id { get; set; }//state_id in sify
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string remarks { get; set; }
        public int? modified_by { get; set; }
        public DateTime? modified_on { get; set; }
        [NotMapped]
        public DateTime created_on { get; set; }
        public int created_by { get; set; }

        [NotMapped]
        public string province_name { get; set; }
        [NotMapped]
        public string region_name { get; set; }
        [NotMapped]
        public string city_name { get; set; }
        [NotMapped]
        public string geom { get; set; }

        //fields of spactra base currently not used=======================================================================
        public string structure_name { get; set; }
        public string address { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "PinCode length should be 6")]
        // [Required]
        public string pin_code { get; set; }
        public DateTime? activation_date { get; set; }
        public DateTime? deactivation_date { get; set; }
        public string activation_status { get; set; }
        
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int sequence_id { get; set; }

        [NotMapped]
        public string networkIdType { get; set; }
        [NotMapped]
        public int pSystemId { get; set; }
        [NotMapped]
        public string pEntityType { get; set; }
        [NotMapped]
        public PageMessage objPM { get; set; }
        [NotMapped]
        public List<DropDownMaster> lstActivationStatus { get; set; }
        [NotMapped]
        public IspEntityMapping objIspEntityMap { get; set; }
        [NotMapped]
        public int templateId { get; set; }
        [NotMapped]
        public string entityType { get; set; }
        [NotMapped]
        public string pNetworkId { get; set; }
        public string rfstype { get; set; }
        public string buildcode { get; set; }
        public int upload_id { get; set; }
        public string network_stage { get; set; }
        public string collector_ring_name { get; set; }
        public string physical_status { get; set; }
        public string fusion_status { get; set; }
        public string commercial_status { get; set; }
        public string terminate_pe_ip { get; set; }
        public string pe_interface { get; set; }
        public string bstn_sw_port { get; set; }
        public string bstn_sw_ip { get; set; }
        public string customer_connecting_port_from_bstn_sw { get; set; }
        public string fms_port_no { get; set; }

		public string primary_site_switch_ip { get; set; }
		public string primary_switch_port_no { get; set; }
		public string secondary_site_id { get; set; }
		public string secondary_site_name { get; set; }
		public string secondary_site_address { get; set; }
		public string secondary_site_switch_ip { get; set; }
		public string secondary_site_switch_port_no { get; set; }
        public string otdr_length { get; set; }
        public string po_length { get; set; }
        public string origin_from { get; set; }
        public string origin_ref_id { get; set; }
        public string origin_ref_code { get; set; }
        public string origin_ref_description { get; set; }
        public string request_ref_id { get; set; }
        public string requested_by { get; set; }
        public string request_approved_by { get; set; }
        public string network_status { get; set; }

    }
    public class TempWirelessCustomer
    {
        [Key, Column("system_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int system_id { get; set; }
        public string network_id { get; set; }
        public string customer_name { get; set; }
        public string mobile_no { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public int parent_system_id { get; set; }
        public string parent_network_id { get; set; }
        public string parent_entity_type { get; set; }
        public int upload_id { get; set; }
        public int created_by { get; set; }
        public bool is_valid { get; set; }
        public string error_msg { get; set; }
        public int batch_id { get; set; }
        public int row_order { get; set; }
        public string remarks { get; set; }

        public string address { get; set; }
    }
}
