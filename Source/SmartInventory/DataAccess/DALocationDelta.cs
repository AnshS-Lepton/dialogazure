using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DBHelpers;
using Models;

namespace DataAccess
{
	public class DALocationDelta: Repository<DropDownMaster>
	{
		public List<GetLocationDelta> getLocationDelta(string entity_type, string delta_date)
		{
			try
			{
				return repo.ExecuteProcedure<GetLocationDelta>("fn_get_location_delta_audit_details",
															  new { p_entity_name = entity_type, p_delta_date = delta_date }, true);
			}
			catch { throw; }
		}

		public List<GetSiteLocationList> getSiteLocations(string entity_type, int? page, int? page_size)
		{
			try
			{
				return repo.ExecuteProcedure<GetSiteLocationList>("fn_get_site_location",
															  new { p_page = page, p_page_size = page_size, p_entity_name = entity_type }, true);
			}
			catch { throw; }
		}
		public List<GETFAULTLOCATIONLIST> getFaultLocations(string fiber_link_id, string equipment_id, string site_code, string port_id, int? optical_distance)
		{
			try
			{
				return repo.ExecuteProcedure<GETFAULTLOCATIONLIST>("fn_api_get_fault_location_detail",
															  new { p_fiber_link_id = fiber_link_id, p_equipment_id = equipment_id, p_site_code = site_code, p_entity_port_no = Convert.ToInt32(port_id), p_distance = Convert.ToDouble(optical_distance) }, true).ToList();
			}
			catch { throw; }
		}

	}
}
