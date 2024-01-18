using DataAccess.DBHelpers;
using DataAccess.WFM;
using Models.Dashboard;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
	public class DADashboard : Repository<HierarchyMaster>
	{
		
		public List<HierarchyMaster> GetHiearchyList()
		{
			try
			{
				return repo.GetAll().ToList();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public List<DashBoardResult> GetDashboardResult(string state, string jc, string town, string partner, string fsa)
		{
			try
			{
				var result = repo.ExecuteProcedure<DashBoardResult>("fn_dashboard_result", new
				{
					r4g_state_code = state,
					jc_sapplant_code = jc,
					town_code = town,
					partner_prms_id = partner,
					fsa_id = fsa
				}, true);
				return result;


			}
			catch
			{
				throw;
			}
		}
        public List<Dictionary<string, string>> GetDashboardDumpResult(string state, string jc, string town, string partner, string fsa)
        {
            try
            {
                var result = repo.ExecuteProcedure<Dictionary<string, string>>("fn_export_dashboard_dump", new
                {
                    r4g_state_code = state,
                    jc_sapplant_code = jc,
                    town_code = town,
                    partner_prms_id = partner,
                    fsa_id = fsa
                }, true);
                return result;
            }
            catch
            {
                throw;
            }
        }
    }
}

