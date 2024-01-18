using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dashboard
{
	public class DashBoardResult
	{
		public int partner_town { get; set; }
		public int partner_partner { get; set; }
		public int partner_fsa { get; set; }
		public int fsa_si_town { get; set; }
		public int fsa_si_partner { get; set; }
		public int fsa_si_fsa { get; set; }
		public int fsa_sc_town { get; set; }
		public int fsa_sc_partner { get; set; }
		public int fsa_sc_fsa { get; set; }
		public int fsa_dc_town { get; set; }
		public int fsa_dc_partner { get; set; }
		public int fsa_dc_fsa { get; set; }
		public int cwip_town { get; set; }
		public int cwip_jc { get; set; }
		public int cwip_fsa { get; set; }

		public int rwip_town { get; set; }
		public int rwip_jc { get; set; }
		public int rwip_fsa { get; set; }

		public double scope_feeder_fsa { get; set; }

		public double scope_feeder_ring1 { get; set; }

		public double scope_feeder_ring2 { get; set; }

		public double scope_distribution_fsa { get; set; }
		public double scope_distribution_ring1 { get; set; }
		public double scope_distribution_ring2 { get; set; }


		public int scope_s1_fsa { get; set; }

		public int scope_s2_fsa { get; set; }

		public int scope_csa_fsa { get; set; }

		public int scope_s1_ring1 { get; set; }

		public int scope_s2_ring1 { get; set; }

		public int scope_csa_ring1 { get; set; }

		public int scope_s1_ring2 { get; set; }

		public int scope_s2_ring2 { get; set; }

		public int scope_csa_ring2 { get; set; }

		public int cwip_fsa_ring { get; set; }
		public int cwip_ring1 { get; set; }
		public int cwip_ring2 { get; set; }


		public double completed_feeder_fsa { get; set; }

		public double completed_feeder_ring1 { get; set; }

		public double completed_feeder_ring2 { get; set; }

		public double completed_distribution_fsa { get; set; }
		public double completed_distribution_ring1 { get; set; }
		public double completed_distribution_ring2 { get; set; }


		public int completed_s1_fsa { get; set; }

		public int completed_s2_fsa { get; set; }

		public int completed_s1_ring1 { get; set; }

		public int completed_s2_ring1 { get; set; }

		public int completed_s1_ring2 { get; set; }

		public int completed_s2_ring2 { get; set; }

		public int rfs_csa_fsa { get; set; }
		public int rfs_csa_ring1 { get; set; }
		public int rfs_csa_ring2 { get; set; }

		public int rfs_s2_fsa { get; set; }
		public int rfs_s2_ring1 { get; set; }
		public int rfs_s2_ring2 { get; set; }

	}
	public class HierarchyMaster
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int serialno { get; set; }
		public string r4g_state_code { get; set; }
		public string r4g_state_name { get; set; }
		public string jc_sapplant_code { get; set; }
		public string jc_name { get; set; }
		public string town_code { get; set; }
		public string town_name { get; set; }
		public string partner_prms_id { get; set; }
		public string fsa_id { get; set; }
        public string now { get; set; }
	}
}
