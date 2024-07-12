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
	}
}
