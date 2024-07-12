using DataAccess;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
	public class BLLocationDelta
	{
		DALocationDelta objDAMisc = new DALocationDelta();
		public List<GetLocationDelta> getLocationDelta(string entity_type, string delta_date)
		{
			return objDAMisc.getLocationDelta(entity_type, delta_date);

		}
	}
}
