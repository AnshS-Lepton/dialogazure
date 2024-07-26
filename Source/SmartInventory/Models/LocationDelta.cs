using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class GetLocationDelta
	{
		public string entity_type { get; set; }
		public string system_id { get; set; }
		public string network_id { get; set; }
		public string site_code { get; set; }
		public string network_status { get; set; }
		public site_location site_location { get; set; }
		public string delta_type { get; set; }
		public string delta_by { get; set; }
		public string delta_on { get; set; }
	}

	public class site_location
	{
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string floor_name { get; set; }
		public string building_code { get; set; }
		public string building_name { get; set; }
		public string block_code { get; set; }
		public string block_name { get; set; }
		public string province_code { get; set; }
		public string province_name { get; set; }
		public string region_code { get; set; }
		public string region_name { get; set; }
		public string address { get; set; }
	}

	public class GetSiteLocationList
	{
		public string entity_type { get; set; }
		public string system_id { get; set; }
		public string network_id { get; set; }
		public string site_code { get; set; }
		public string network_status { get; set; }
		public site_location site_location { get; set; }
		public PaginationMetaData pagination_metadata { get; set; }
	}

	public class GetSiteLocationDetails
	{
		public string entity_type { get; set; }
		public string system_id { get; set; }
		public string network_id { get; set; }
		public string site_code { get; set; }
		public string network_status { get; set; }
		public site_location site_location { get; set; }
	}

	public class PaginationMetaData
	{
		public int total_records { get; set; }
		public int page { get; set; }
		public int page_size { get; set; }
		public int total_pages { get; set; }
		public string next_page { get; set; }
		public string previous_page { get; set; }
	}
	public class GetSiteLocation
	{
		public List<GetSiteLocationDetails> data { get; set; }
		public PaginationMetaData pagination_metadata { get; set; }
	}



	public class GETFAULTLOCATIONLIST
	{ 
	    public FAULT_LOCATION fault_Location { get; set; }
		public AFFECTED_CABLE_SEGMENT affected_Cable_Segment { get; set; }
		public string fiber_link_network_id { get; set; }
		public string fiber_link_name { get; set; }
		public string status { get; set; }
		public string error_message { get; set; }
	}
	 

	public class FAULT_LOCATION
	{
	public double? latitude { get; set; }
	public double? longitude { get; set; }
	public string block_code { get; set; }
	public string block_name { get; set; }
	public string province_code { get; set; }
	public string province_name { get; set; }
	public string region_code { get; set; }
	public string region_name { get; set; }

    }

	public class AFFECTED_CABLE_SEGMENT
	{
		public int? cable_system_id { get; set; }
		public string cable_network_id { get; set; }
		public double cable_length { get; set; }
		public string a_end_entity_type { get; set; }
		public string a_end_entity_id { get; set; }
		public double? a_end_distance { get; set; }
		public string z_end_entity_type { get; set; }
		public string z_end_entity_id { get; set; }
		public double? z_end_distance { get; set; }
	}
	public class ASSOCIATED_FIBER_LINKS
	{
		public string fiber_link_network_id { get; set; }
		public string fiber_link_name { get; set; }
	}
	public class GETFAULTLOCATION
	{
		public FAULT_LOCATION fault_location { get; set; }
		public AFFECTED_CABLE_SEGMENT affected_cable_segment { get; set; }
		public List<ASSOCIATED_FIBER_LINKS> associated_fiber_link { get; set; }
	}

}
