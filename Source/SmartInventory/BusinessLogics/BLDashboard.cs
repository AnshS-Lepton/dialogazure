using DataAccess;
using DataAccess.WFM;
using Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics
{
	public class BLDashboard
	{
		public List<HierarchyMaster> GetHierarchyList()
		{
			return new DADashboard().GetHiearchyList();
		}

		public List<DashBoardResult> GetDashboardResult(string state, string jc, string town, string partner, string fsa)
		{
			return new DADashboard().GetDashboardResult(state, jc, town, partner, fsa);

		}
        public List<Dictionary<string, string>> GetDashboardDumpResult(string state, string jc, string town, string partner, string fsa)
        {
            return new DADashboard().GetDashboardDumpResult(state, jc, town, partner, fsa);

        }
    }
}
