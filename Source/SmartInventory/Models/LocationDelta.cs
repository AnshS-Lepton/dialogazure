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
}
