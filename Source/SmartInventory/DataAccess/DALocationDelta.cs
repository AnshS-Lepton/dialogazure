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
	}
}
